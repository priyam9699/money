using FinanceManagement.IRepository;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Drawing;
using System.Security.Claims;
using FinanceManagement.Models;
using FinanceManagement.ViewModels;
using Humanizer;

namespace FinanceManagement.Controllers
{
    public class DailyBeltUpdateController : Controller
    {
        private readonly IDailyBeltUpdateRepository dailyBeltUpdateRepository;
        private readonly IProductRepository productRepository;
        private readonly ApplicationDbContext context;

        public DailyBeltUpdateController(
                              IDailyBeltUpdateRepository dailyBeltUpdateRepository,
                              IProductRepository productRepository,

                              ApplicationDbContext context)
        {
            this.dailyBeltUpdateRepository = dailyBeltUpdateRepository;
            this.productRepository = productRepository;
            this.context = context;
        }

        #region DailyBeltUpdateIndex
        [HttpGet]
        public IActionResult DailyBeltUpdateIndex(string search, DateTime? startDate, DateTime? endDate, int? month, int? year)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get current user's ID

            // Retrieve the company name of the logged-in user
            var user = context.Users.FirstOrDefault(u => u.Id == userId);
            var companyName = user != null ? user.CompanyName : null;

            IEnumerable<DailyBeltUpdate> dailybelt;

            if (!string.IsNullOrEmpty(companyName))
            {
                // Retrieve orders based on the company name
                dailybelt = dailyBeltUpdateRepository.GetDailyBeltUpdateFromCompanyName(companyName);

                // Apply search filter
                if (!string.IsNullOrEmpty(search))
                {
                    dailybelt = dailybelt.Where(p => p.ProductName.Contains(search)).ToList();
                }

                // Apply date range filter
                if (startDate.HasValue && endDate.HasValue)
                {
                    dailybelt = dailybelt.Where(p => p.Date >= startDate && p.Date <= endDate).ToList();
                }

                // Apply month and year filter
                if (month.HasValue && year.HasValue)
                {
                    dailybelt = dailybelt.Where(p => p.Date.Month == month.Value && p.Date.Year == year.Value).ToList();
                }

                // Sort orders by date
                dailybelt = dailybelt.OrderBy(o => o.Date).ToList();
            }
            else
            {
                // Handle the case where the user's company name is not found
                dailybelt = Enumerable.Empty<DailyBeltUpdate>();
            }

            return View(dailybelt);
        }
        #endregion


        #region DailyBeltUpdateCreate
        [HttpGet]
        public IActionResult DailyBeltUpdateCreate()
        {
            if (!User.Identity.IsAuthenticated)
            {
                // Redirect to the login page or handle unauthorized access
                return RedirectToAction("Login", "Account"); // Adjust to your login route
            }

            // Retrieve the user ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Retrieve the company name associated with the user ID
            var user = context.Users.FirstOrDefault(u => u.Id == userId);
            var companyName = user?.CompanyName;

            if (string.IsNullOrEmpty(companyName))
            {
                // Handle the situation where the company name is not found
                return View("Error");
            }

            var product = productRepository.GetProductsFromCompanyName(companyName);
            if (product != null)
            {
                var model = new DailyBeltUpdateVM { Products = product };
                return View(model);
            }
            return View();
        }

        [HttpPost]
        public IActionResult DailyBeltUpdateCreate(DailyBeltUpdateVM dailyBeltUpdateVM)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var product = productRepository.GetById(dailyBeltUpdateVM.SelectedProductId);

                var dailybelt = new DailyBeltUpdate
                {
                    UserId = userId,
                    Date = dailyBeltUpdateVM.Date,
                    ProductId = dailyBeltUpdateVM.SelectedProductId,
                    ProductName = product.ProductName,
                    ProductSKU = product.SKU,
                    Quantity = dailyBeltUpdateVM.Quantity,
                    
                };

                dailyBeltUpdateRepository.Add(dailybelt);
                return RedirectToAction("DailyBeltUpdateIndex", "DailyBeltUpdate");
            }

            return View();
        }
        #endregion


        #region DailyBeltUpdate Edit
        [HttpGet]
        public IActionResult DailyBeltUpdateEdit(int id)
        {

            var dailyBelt = dailyBeltUpdateRepository.GetById(id);
            return View(dailyBelt);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DailyBeltUpdateEdit(DailyBeltUpdate dailyBeltUpdate)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get current user's ID
            var user = context.Users.FirstOrDefault(u => u.Id == userId);
            var companyName = user != null ? user.CompanyName : null;

            if (ModelState.IsValid)
            {
                if (userId == null)
                {
                    // If user ID is not found, handle the situation accordingly
                    return Unauthorized();
                }

                // Check if the product exists
                var existingOrder = dailyBeltUpdateRepository.GetById(dailyBeltUpdate.Id);
                if (existingOrder == null)
                {
                    // Handle the situation where the product does not exist
                    return NotFound();
                }



                // Update the product
                dailyBeltUpdateRepository.Update(dailyBeltUpdate);
                return RedirectToAction("DailyBeltUpdateIndex");
            }
            else
            {
                // If the model state is not valid, return the view with validation errors
                return View(dailyBeltUpdate);
            }
        }

        #endregion


        #region DailyBeltUpdateDelete
        [HttpGet]
        public IActionResult DailyBeltUpdateDelete(int id)
        {
            // Ensure that the product exists
            var dailybelt = dailyBeltUpdateRepository.GetById(id);
            if (dailybelt == null)
            {
                return NotFound();
            }

            return View(dailybelt); // Assuming you have a view to confirm the deletion
        }



        [HttpPost]

        public IActionResult ConfirmDailyBeltUpdateDelete(int id)
        {
            // Ensure that the product exists
            var dailybelt = dailyBeltUpdateRepository.GetById(id);
            if (dailybelt == null)
            {
                return NotFound();
            }

            // Delete the product
            dailyBeltUpdateRepository.Delete(id);

            return RedirectToAction("DailyBeltUpdateIndex");
        }
        #endregion


        private double GetProductPriceById(int productId)
        {
            var product = productRepository.GetById(productId); // Fetch the product by ID
            if (product != null)
            {
                return product.Price; // Return the product's price
            }
            return 0; // Return 0 or handle as needed if product not found
        }


        #region Export Excel
        public IActionResult ExportToExcel(DateTime? startDate, DateTime? endDate)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get current user's ID
            var user = context.Users.FirstOrDefault(u => u.Id == userId);
            var companyName = user != null ? user.CompanyName : null;

            IEnumerable<DailyBeltUpdate> dailyBeltUpdate = Enumerable.Empty<DailyBeltUpdate>();

            if (!string.IsNullOrEmpty(companyName))
            {
                // Retrieve products based on the company name
                dailyBeltUpdate = dailyBeltUpdateRepository.GetDailyBeltUpdateFromCompanyName(companyName);

                // Apply date range filter if provided
                if (startDate.HasValue && endDate.HasValue)
                {
                    dailyBeltUpdate = dailyBeltUpdate.Where(p => p.Date >= startDate && p.Date <= endDate).ToList();
                }
                dailyBeltUpdate = dailyBeltUpdate.OrderBy(o => o.Date).ToList();
            }

            string startDateString = startDate?.ToString("dd-MM-yyyy") ?? "StartDate";
            string endDateString = endDate?.ToString("dd-MM-yyyy") ?? "EndDate";



            // Create the file name with the start and end dates
            string fileName = $"Orders{startDateString}_to_{endDateString}.xlsx";

            var RemainingAmountLastMonth = dailyBeltUpdateRepository.GetLastPendingAmount(companyName, startDate, endDate);

            // Group data by date
            var groupedData = dailyBeltUpdate
                .GroupBy(p => p.Date)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Create a list of distinct products
            var productNames = dailyBeltUpdate
                .Select(p => p.ProductName)
                .Distinct()
                .ToList();

            // Create an instance of the ExcelPackage
            using (var package = new ExcelPackage())
            {
                // Create a worksheet
                var worksheet = package.Workbook.Worksheets.Add("DailyBeltUpdate");

                // Set the title
                worksheet.Cells["A1"].Value = $"Daily Belt Report ({startDateString} to {endDateString})";
                worksheet.Cells["A1:L1"].Merge = true; // Merge cells for the title
                worksheet.Cells["A1:L1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A1:L1"].Style.Font.Size = 28; // Increase the font size
                worksheet.Cells["A1:L1"].Style.Font.Bold = true; // Bold font
                worksheet.Cells["A1:L1"].Style.Fill.PatternType = ExcelFillStyle.Solid; // Solid fill
                worksheet.Cells["A1:L1"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);

                worksheet.Cells["A1:L1"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                worksheet.Cells["A1:L1"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                worksheet.Cells["A1:L1"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                worksheet.Cells["A1:L1"].Style.Border.Right.Style = ExcelBorderStyle.Medium;


                // Headers
                worksheet.Cells["A3"].Value = "Date"; // Date column
                worksheet.Cells["A3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                worksheet.Cells["A3"].Style.Font.Size = 17;
                worksheet.Cells["A3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["A3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A3"].Style.Font.Bold = true;
                worksheet.Cells["A3"].Style.Border.Top.Style = ExcelBorderStyle.Thin; // Top border
                worksheet.Cells["A3"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin; // Bottom border
                worksheet.Cells["A3"].Style.Border.Left.Style = ExcelBorderStyle.Thin; // Left border
                worksheet.Cells["A3"].Style.Border.Right.Style = ExcelBorderStyle.Thin; // Right border


                for (int i = 0; i < productNames.Count; i++)
                {
                    worksheet.Cells[3, i + 2].Value = productNames[i]; // Create columns for each product name
                    worksheet.Cells[3, i + 2].Style.Fill.PatternType = ExcelFillStyle.Solid; // Solid fill
                    worksheet.Cells[3, i + 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                    worksheet.Cells[3, i + 2].Style.Font.Size = 17;
                    worksheet.Cells[3, i + 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[3, i + 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[3, i + 2].Style.Font.Bold = true;
                    worksheet.Cells[3, i + 2].Style.Border.Top.Style = ExcelBorderStyle.Thin; // Top border
                    worksheet.Cells[3, i + 2].Style.Border.Bottom.Style = ExcelBorderStyle.Thin; // Bottom border
                    worksheet.Cells[3, i + 2].Style.Border.Left.Style = ExcelBorderStyle.Thin; // Left border
                    worksheet.Cells[3, i + 2].Style.Border.Right.Style = ExcelBorderStyle.Thin; // Right border

                }

                worksheet.Cells[3, productNames.Count + 3].Value = "Paid Date";
                worksheet.Cells[3, productNames.Count + 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[3, productNames.Count + 3].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                worksheet.Cells[3, productNames.Count + 3].Style.Font.Size = 17;
                worksheet.Cells[3, productNames.Count + 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[3, productNames.Count + 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[3, productNames.Count + 3].Style.Font.Bold = true;
                worksheet.Cells[3, productNames.Count + 3].Style.Border.Top.Style = ExcelBorderStyle.Thin; // Top border
                worksheet.Cells[3, productNames.Count + 3].Style.Border.Bottom.Style = ExcelBorderStyle.Thin; // Bottom border
                worksheet.Cells[3, productNames.Count + 3].Style.Border.Left.Style = ExcelBorderStyle.Thin; // Left border
                worksheet.Cells[3, productNames.Count + 3].Style.Border.Right.Style = ExcelBorderStyle.Thin; // Right border


                worksheet.Cells[3, productNames.Count + 4].Value = "Paid Amount";
                worksheet.Cells[3, productNames.Count + 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[3, productNames.Count + 4].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                worksheet.Cells[3, productNames.Count + 4].Style.Font.Size = 17;
                worksheet.Cells[3, productNames.Count + 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[3, productNames.Count + 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[3, productNames.Count + 4].Style.Font.Bold = true;
                worksheet.Cells[3, productNames.Count + 4].Style.Border.Top.Style = ExcelBorderStyle.Thin; // Top border
                worksheet.Cells[3, productNames.Count + 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thin; // Bottom border
                worksheet.Cells[3, productNames.Count + 4].Style.Border.Left.Style = ExcelBorderStyle.Thin; // Left border
                worksheet.Cells[3, productNames.Count + 4].Style.Border.Right.Style = ExcelBorderStyle.Thin; // Right border


                worksheet.Cells[3, productNames.Count + 6].Value = "Product Name"; // Paid Amount header
                worksheet.Cells[3, productNames.Count + 6].Style.Font.Size = 17;
                worksheet.Cells[3, productNames.Count + 6].Style.Fill.PatternType = ExcelFillStyle.Solid; // Solid fill
                worksheet.Cells[3, productNames.Count + 6].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                worksheet.Cells[3, productNames.Count + 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[3, productNames.Count + 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[3, productNames.Count + 6].Style.Font.Bold = true;
                worksheet.Cells[3, productNames.Count + 6].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[3, productNames.Count + 6].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[3, productNames.Count + 6].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[3, productNames.Count + 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;


                worksheet.Cells[3, productNames.Count + 7].Value = "Total Quantity"; // Paid Amount header
                worksheet.Cells[3, productNames.Count + 7].Style.Font.Size = 17;
                worksheet.Cells[3, productNames.Count + 7].Style.Fill.PatternType = ExcelFillStyle.Solid; // Solid fill
                worksheet.Cells[3, productNames.Count + 7].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                worksheet.Cells[3, productNames.Count + 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[3, productNames.Count + 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[3, productNames.Count + 7].Style.Font.Bold = true;
                worksheet.Cells[3, productNames.Count + 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[3, productNames.Count + 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[3, productNames.Count + 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[3, productNames.Count + 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                worksheet.Cells[3, productNames.Count + 8].Value = "Price"; // Price
                worksheet.Cells[3, productNames.Count + 8].Style.Font.Size = 17;
                worksheet.Cells[3, productNames.Count + 8].Style.Fill.PatternType = ExcelFillStyle.Solid; // Solid fill
                worksheet.Cells[3, productNames.Count + 8].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                worksheet.Cells[3, productNames.Count + 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[3, productNames.Count + 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[3, productNames.Count + 8].Style.Font.Bold = true;
                worksheet.Cells[3, productNames.Count + 8].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[3, productNames.Count + 8].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[3, productNames.Count + 8].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[3, productNames.Count + 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;


                worksheet.Cells[3, productNames.Count + 9].Value = "Total Amount"; // Paid Amount header
                worksheet.Cells[3, productNames.Count + 9].Style.Font.Size = 17;
                worksheet.Cells[3, productNames.Count + 9].Style.Fill.PatternType = ExcelFillStyle.Solid; // Solid fill
                worksheet.Cells[3, productNames.Count + 9].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                worksheet.Cells[3, productNames.Count + 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[3, productNames.Count + 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[3, productNames.Count + 9].Style.Font.Bold = true;
                worksheet.Cells[3, productNames.Count + 9].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[3, productNames.Count + 9].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[3, productNames.Count + 9].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[3, productNames.Count + 9].Style.Border.Right.Style = ExcelBorderStyle.Thin;


                worksheet.Row(1).Height = 40;
                worksheet.Row(3).Height = 40;

                // Set column widths
                worksheet.Column(1).Width = 25; // Date column width
                for (int i = 0; i < productNames.Count; i++)
                {
                    worksheet.Column(i + 2).Width = 30; // Product columns width
                }

                worksheet.Column(productNames.Count + 3).Width = 20; // PaidAmount column width
                worksheet.Column(productNames.Count + 4).Width = 25; // Date column width
                worksheet.Column(productNames.Count + 6).Width = 30;
                worksheet.Column(productNames.Count + 7).Width = 25;
                worksheet.Column(productNames.Count + 8).Width = 25;
                worksheet.Column(productNames.Count + 9).Width = 25;


                int row = 4;
                
                // Add data to the worksheet
                
                foreach (var dateGroup in groupedData)
                {
                    worksheet.Cells[$"A{row}"].Value = dateGroup.Key.ToString("dd-MM-yyyy"); // Set date
                    worksheet.Cells[$"A{row}"].Style.Font.Size = 14;
                    worksheet.Cells[$"A{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[$"A{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[$"A{row}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[$"A{row}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[$"A{row}"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Row(row).Height = 26;


                    // Fill product quantities for the specific date
                    foreach (var productUpdate in dateGroup.Value)
                    {
                        var productIndex = productNames.IndexOf(productUpdate.ProductName) + 2; // Find the corresponding column for the product
                        worksheet.Cells[row, productIndex].Value = productUpdate.Quantity; // Set the quantity
                        worksheet.Cells[row, productIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Center-align quantity
                        worksheet.Cells[row, productIndex].Style.Font.Size = 14;
                        worksheet.Cells[row, productIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        

                    }
                    row++;

                    int newRow = 4;
                    // Fetch all AccountMaster entries that meet the criteria
                    var accountMasterEntries = context.AccountMaster
    .Where(a => a.Category == "Payments" && a.Description == "Belt Payment" &&
                (!startDate.HasValue || a.Date >= startDate.Value) &&
                (!endDate.HasValue || a.Date <= endDate.Value))
    .Select(a => new { a.PaidAmount, a.Date })
    .ToList();

                    // Add PaidAmount and PaidDate for all relevant entries (not filtered by date)
                    foreach (var paidEntry in accountMasterEntries)
                    {

                        worksheet.Cells[newRow, productNames.Count + 3].Value = paidEntry.Date.ToString("dd-MM-yyyy"); // Paid Date
                        worksheet.Cells[newRow, productNames.Count + 3].Style.Font.Size = 14;
                        worksheet.Cells[newRow, productNames.Count + 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[newRow, productNames.Count + 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells[newRow, productNames.Count + 3].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[newRow, productNames.Count + 3].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                        worksheet.Cells[newRow, productNames.Count + 4].Value = paidEntry.PaidAmount;
                        worksheet.Cells[newRow, productNames.Count + 4].Style.Font.Size = 14;
                        worksheet.Cells[newRow, productNames.Count + 4].Style.Numberformat.Format = "₹ #,##0.00";
                        worksheet.Cells[newRow, productNames.Count + 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[newRow, productNames.Count + 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells[newRow, productNames.Count + 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[newRow, productNames.Count + 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                        worksheet.Row(newRow).Height = 26;
                        newRow++;
                    }


                    int rows = 4; // Starting row for the product names

                    // Create a dictionary to store product prices
                    var productPrices = new Dictionary<string, decimal>();

                    // Assuming dailyBeltUpdate has ProductID, ProductName, and Quantity properties
                    foreach (var dailyUpdate in dailyBeltUpdate)
                    {
                        string productName = dailyUpdate.ProductName;

                        // If the price is not already fetched, get it from the database or repository
                        if (!productPrices.ContainsKey(productName))
                        {
                            double price = GetProductPriceById(dailyUpdate.ProductId); // Replace with your method to fetch the price
                            productPrices[productName] = (decimal)price;
                        }
                    }

                    // Group by ProductName and sum up the quantities for each product
                    var productDetails = dailyBeltUpdate
                        .GroupBy(d => d.ProductName)
                        .ToDictionary(
                            g => g.Key,
                            g => new
                            {
                                TotalQuantity = g.Sum(d => d.Quantity),
                                Price = productPrices[g.Key] // Get price for the product from the dictionary
                            });

                    // Now, populate the worksheet
                    decimal grandTotalAmount = 0; // To hold the total amount for all products
                    foreach (var product in productDetails)
                    {
                        string productName = product.Key;
                        int totalQuantity = product.Value.TotalQuantity;
                        decimal productPrice = product.Value.Price;

                        // Calculate the total amount
                        decimal totalAmount = totalQuantity * productPrice;
                        grandTotalAmount += totalAmount; // Add to grand total

                        // Set product name in column 9
                        worksheet.Cells[rows, productNames.Count + 6].Value = productName;
                        worksheet.Cells[rows, productNames.Count + 6].Style.Fill.PatternType = ExcelFillStyle.Solid; // Solid fill
                        worksheet.Cells[rows, productNames.Count + 6].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Khaki);

                        // Set total quantity in column 10
                        worksheet.Cells[rows, productNames.Count + 7].Value = totalQuantity;

                        // Set price in column 11
                        worksheet.Cells[rows, productNames.Count + 8].Value = productPrice;

                        // Set total amount in column 12
                        worksheet.Cells[rows, productNames.Count + 9].Value = totalAmount;

                        // Formatting the total amount
                        worksheet.Cells[rows, productNames.Count + 9].Style.Font.Size = 14;
                        worksheet.Cells[rows, productNames.Count + 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rows, productNames.Count + 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells[rows, productNames.Count + 9].Style.Numberformat.Format = "₹ #,##0.00";

                        // Optionally format the cells
                        worksheet.Cells[rows, productNames.Count + 6].Style.Font.Size = 14;
                        worksheet.Cells[rows, productNames.Count + 6].Style.Font.Bold = true;
                        worksheet.Cells[rows, productNames.Count + 7].Style.Font.Size = 14;
                        worksheet.Cells[rows, productNames.Count + 8].Style.Font.Size = 14;

                        worksheet.Cells[rows, productNames.Count + 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rows, productNames.Count + 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells[rows, productNames.Count + 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rows, productNames.Count + 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells[rows, productNames.Count + 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rows, productNames.Count + 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        // Add borders
                        worksheet.Cells[rows, productNames.Count + 6].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[rows, productNames.Count + 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[rows, productNames.Count + 6].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                        worksheet.Cells[rows, productNames.Count + 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                        worksheet.Cells[rows, productNames.Count + 8].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[rows, productNames.Count + 9].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[rows, productNames.Count + 9].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        rows++; // Move to the next row for the next product
                    }

                    // Now add the summary rows
                    worksheet.Cells[rows , productNames.Count + 8].Value = "Total Bill Amount";
                    worksheet.Cells[rows, productNames.Count + 8].Style.Font.Size = 14;
                    worksheet.Cells[rows, productNames.Count + 8].Style.Font.Bold = true;
                    worksheet.Row(rows).Height = 30;
                    worksheet.Cells[rows, productNames.Count + 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[rows, productNames.Count + 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[rows, productNames.Count + 8].Style.Fill.PatternType = ExcelFillStyle.Solid; // Solid fill
                    worksheet.Cells[rows, productNames.Count + 8].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);
                    worksheet.Cells[rows, productNames.Count + 8].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[rows, productNames.Count + 8].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[rows, productNames.Count + 9].Value = grandTotalAmount;
                    worksheet.Cells[rows, productNames.Count + 9].Style.Font.Size = 14;
                    worksheet.Cells[rows, productNames.Count + 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[rows, productNames.Count + 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[rows, productNames.Count + 9].Style.Numberformat.Format = "₹ #,##0.00";
                    worksheet.Cells[rows, productNames.Count + 9].Style.Fill.PatternType = ExcelFillStyle.Solid; // Solid fill
                    worksheet.Cells[rows, productNames.Count + 9].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightCyan);
                    worksheet.Cells[rows, productNames.Count + 9].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[rows, productNames.Count + 9].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    rows++; // Move to the next row for the next summary


                    var lastPendingAmount = RemainingAmountLastMonth;



                    var accountMasterEntry = context.AccountMaster
    .Where(a => a.Category == "Payments" && a.Description == "Belt Payment" &&
                (!startDate.HasValue || a.Date >= startDate.Value) &&
                (!endDate.HasValue || a.Date <= endDate.Value))
    .Select(a => new { a.PaidAmount, a.Date })
    .ToList();

                    // Calculate the total Paid Amount from the fetched entries
                    decimal paidAmount = accountMasterEntries.Sum(entry => entry.PaidAmount);
                    decimal? remainingAmount = grandTotalAmount + lastPendingAmount - paidAmount; // Calculate remaining amount

                    // Add Last Pending Amount row
                    worksheet.Cells[rows, productNames.Count + 8].Value = "Last Pending Amount";
                    worksheet.Cells[rows, productNames.Count + 8].Style.Font.Size = 14;
                    worksheet.Cells[rows, productNames.Count + 8].Style.Font.Bold = true;
                    worksheet.Row(rows).Height = 30;
                    worksheet.Cells[rows, productNames.Count + 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[rows, productNames.Count + 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[rows, productNames.Count + 8].Style.Fill.PatternType = ExcelFillStyle.Solid; // Solid fill
                    worksheet.Cells[rows, productNames.Count + 8].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);
                    worksheet.Cells[rows, productNames.Count + 8].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[rows, productNames.Count + 8].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[rows, productNames.Count + 9].Value = lastPendingAmount;
                    worksheet.Cells[rows, productNames.Count + 9].Style.Font.Size = 14;
                    worksheet.Cells[rows, productNames.Count + 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[rows, productNames.Count + 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[rows, productNames.Count + 9].Style.Numberformat.Format = "₹ #,##0.00";
                    worksheet.Cells[rows, productNames.Count + 9].Style.Fill.PatternType = ExcelFillStyle.Solid; // Solid fill
                    worksheet.Cells[rows, productNames.Count + 9].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightCyan);
                    worksheet.Cells[rows, productNames.Count + 9].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[rows, productNames.Count + 9].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    rows++; // Move to the next row for the next summary

                    // Add Paid Amount row
                    worksheet.Cells[rows, productNames.Count + 8].Value = "Paid Amount";
                    worksheet.Cells[rows, productNames.Count + 8].Style.Font.Size = 14;
                    worksheet.Cells[rows, productNames.Count + 8].Style.Font.Bold = true;
                    worksheet.Row(rows).Height = 30;
                    worksheet.Cells[rows, productNames.Count + 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[rows, productNames.Count + 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[rows, productNames.Count + 8].Style.Fill.PatternType = ExcelFillStyle.Solid; // Solid fill
                    worksheet.Cells[rows, productNames.Count + 8].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);
                    worksheet.Cells[rows, productNames.Count + 8].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[rows, productNames.Count + 8].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[rows, productNames.Count + 9].Value = paidAmount;
                    worksheet.Cells[rows, productNames.Count + 9].Style.Font.Size = 14;
                    worksheet.Cells[rows, productNames.Count + 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[rows, productNames.Count + 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[rows, productNames.Count + 9].Style.Numberformat.Format = "₹ #,##0.00";
                    worksheet.Cells[rows, productNames.Count + 9].Style.Fill.PatternType = ExcelFillStyle.Solid; // Solid fill
                    worksheet.Cells[rows, productNames.Count + 9].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightCyan);
                    worksheet.Cells[rows, productNames.Count + 9].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[rows, productNames.Count + 9].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    rows++; // Move to the next row for the next summary

                    // Add Remaining Amount row
                    worksheet.Cells[rows, productNames.Count + 8].Value = "Remaining Amount";
                    worksheet.Cells[rows, productNames.Count + 8].Style.Font.Size = 14;
                    worksheet.Cells[rows, productNames.Count + 8].Style.Font.Bold = true;
                    worksheet.Row(rows).Height = 30;
                    worksheet.Cells[rows, productNames.Count + 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[rows, productNames.Count + 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[rows, productNames.Count + 8].Style.Fill.PatternType = ExcelFillStyle.Solid; // Solid fill
                    worksheet.Cells[rows, productNames.Count + 8].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);
                    worksheet.Cells[rows, productNames.Count + 8].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[rows, productNames.Count + 8].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[rows, productNames.Count + 9].Value = remainingAmount;
                    worksheet.Cells[rows, productNames.Count + 9].Style.Font.Size = 14;
                    worksheet.Cells[rows, productNames.Count + 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[rows, productNames.Count + 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[rows, productNames.Count + 9].Style.Numberformat.Format = "₹ #,##0.00";
                    worksheet.Cells[rows, productNames.Count + 9].Style.Fill.PatternType = ExcelFillStyle.Solid; // Solid fill
                    worksheet.Cells[rows, productNames.Count + 9].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightCyan);
                    worksheet.Cells[rows, productNames.Count + 9].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[rows, productNames.Count + 9].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                }


                // Return the Excel file as a downloadable file
                byte[] excelData = package.GetAsByteArray();
                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"DAILY BELT SHEET {startDateString}_to_{endDateString}.xlsx");
            }
        }
        #endregion

    }
}
