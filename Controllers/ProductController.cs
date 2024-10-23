using FinanceManagement.IRepository;
using FinanceManagement.Models;
using FinanceManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Security.Claims;

namespace FinanceManagement.Controllers
{
    public class ProductController : Controller
    {



        private readonly IProductRepository productRepository;

        private readonly ApplicationDbContext context;
        private readonly IServiceProvider serviceProvider;
        private string htmlContent;

        public ProductController(
                              IProductRepository productRepository,

                              ApplicationDbContext context, IServiceProvider serviceProvider)
        {

            this.productRepository = productRepository;

            this.context = context;
            this.serviceProvider = serviceProvider;
        }



        #region ProductIndex
        [HttpGet]
        public IActionResult ProductIndex(string search, DateTime? startDate, DateTime? endDate, int? month, int? year)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get current user's ID

            // Retrieve the company name of the logged-in user
            var user = context.Users.FirstOrDefault(u => u.Id == userId);
            var companyName = user != null ? user.CompanyName : null;

            IEnumerable<Product> products;

            if (!string.IsNullOrEmpty(companyName))
            {
                // Retrieve orders based on the company name
                products = productRepository.GetProductsFromCompanyName(companyName);

                // Apply search filter
                if (!string.IsNullOrEmpty(search))
                {
                    products = products.Where(p => p.ProductName.Contains(search)).ToList();
                }

                // Apply date range filter
                if (startDate.HasValue && endDate.HasValue)
                {
                    products = products.Where(p => p.Date >= startDate && p.Date <= endDate).ToList();
                }

                // Apply month and year filter
                if (month.HasValue && year.HasValue)
                {
                    products = products.Where(p => p.Date.Month == month.Value && p.Date.Year == year.Value).ToList();
                }

                // Sort orders by date
                products = products.OrderBy(o => o.Date).ToList();
            }
            else
            {
                // Handle the case where the user's company name is not found
                products = Enumerable.Empty<Product>();
            }

            return View(products);
        }
        #endregion

        #region Product Create
        [HttpGet]
        public IActionResult ProductCreate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ProductCreate(ProductViewModel product)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var managerId = User.IsInRole("Manager") ? userId : null; // Conditionally set ManagerId


                var Product = new Product
                {
                    UserId = userId,
                    Date = product.Date,
                    ProductName = product.ProductName,
                    SKU = product.SKU,
                    Price = product.Price,
                    DamageQuantity = product.DamageQuantity,
                    Quantity = product.Quantity,
                };


                productRepository.Add(Product);
                TempData["success"] = "Product added successfully!";
                return RedirectToAction("ProductIndex", "Product");
            }

            return View();
        }
        #endregion

        #region Product Edit
        [HttpGet]
        public IActionResult ProductEdit(int id)
        {
            var product = productRepository.GetById(id);
            return View(product);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ProductEdit(Product product)
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
                var existingProduct = productRepository.GetById(product.Id);
                if (existingProduct == null)
                {
                    // Handle the situation where the product does not exist
                    return NotFound();
                }



                // Update the product
                productRepository.Update(product);
                return RedirectToAction("ProductIndex");
            }
            else
            {
                // If the model state is not valid, return the view with validation errors
                return View(product);
            }
        }
        #endregion

        #region Product Delete
        [HttpGet]
        public IActionResult ProductDelete(int id)
        {
            // Ensure that the product exists
            var product = productRepository.GetById(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product); // Assuming you have a view to confirm the deletion
        }



        [HttpPost]

        public IActionResult ConfirmProductDelete(int id)
        {
            // Ensure that the product exists
            var product = productRepository.GetById(id);
            if (product == null)
            {
                return NotFound();
            }

            // Delete the product
            productRepository.Delete(id);

            return RedirectToAction("ProductIndex");
        }
        #endregion

        #region ExportToExcel
        public IActionResult ExportToExcel(DateTime? startDate, DateTime? endDate)
        {


            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get current user's ID
            var user = context.Users.FirstOrDefault(u => u.Id == userId);
            var companyName = user != null ? user.CompanyName : null;

            IEnumerable<Product> products = Enumerable.Empty<Product>();

            if (!string.IsNullOrEmpty(companyName))
            {
                // Retrieve products based on the company name
                products = productRepository.GetProductsFromCompanyName(companyName);

                // Apply date range filter if provided
                if (startDate.HasValue && endDate.HasValue)
                {
                    products = products.Where(p => p.Date >= startDate && p.Date <= endDate).ToList();
                }
                products = products.OrderBy(o => o.Date).ToList();
            }


            string startDateString = startDate?.ToString("dd-MM-yyyy") ?? "StartDate";
            string endDateString = endDate?.ToString("dd-MM-yyyy") ?? "EndDate";

            // Create the file name with the start and end dates
            string fileName = $"dashboard_{startDateString}_to_{endDateString}.xlsx";

            // Create an instance of the ExcelPackage
            using (var package = new ExcelPackage())
            {
                // Create a worksheet
                var worksheet = package.Workbook.Worksheets.Add("Products");

                // Set the title
                worksheet.Cells["A1"].Value = $"Products Report ({startDateString} to {endDateString})"; // Title text
                worksheet.Cells["A1:H1"].Merge = true; // Merge cells for the title
                worksheet.Cells["A1:H1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A1:H1"].Style.Font.Size = 30; // Increase the font size
                worksheet.Cells["A1:H1"].Style.Font.Bold = true; // Bold font
                worksheet.Cells["A1:H1"].Style.Fill.PatternType = ExcelFillStyle.Solid; // Solid fill
                worksheet.Cells["A1:H1"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);

                worksheet.Cells["A1:H1"].Style.Border.Top.Style = ExcelBorderStyle.Medium; // Top border
                worksheet.Cells["A1:H1"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium; // Bottom border
                worksheet.Cells["A1:H1"].Style.Border.Left.Style = ExcelBorderStyle.Medium; // Left border
                worksheet.Cells["A1:H1"].Style.Border.Right.Style = ExcelBorderStyle.Medium;


                // Apply bold formatting to the header cells
                // Apply bold formatting to the header cells
                worksheet.Cells["A3:H3"].Style.Font.Bold = true;
                worksheet.Cells["A3:H3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A3:H3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["A3:H3"].Style.Font.Size = 17; // Increase the font size
                                                               // Apply background color to the entire row
                worksheet.Cells["A3:H3"].Style.Fill.PatternType = ExcelFillStyle.Solid; // Solid fill


                worksheet.Cells["A3:H3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);

                worksheet.Cells["A3:H3"].Style.Border.Top.Style = ExcelBorderStyle.Thin; // Top border
                worksheet.Cells["A3:H3"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin; // Bottom border
                worksheet.Cells["A3:H3"].Style.Border.Left.Style = ExcelBorderStyle.Thin; // Left border
                worksheet.Cells["A3:H3"].Style.Border.Right.Style = ExcelBorderStyle.Thin; // Right border

                // Add headers
                worksheet.Cells["A3"].Value = "Date";
                worksheet.Cells["B3"].Value = "Product Name";
                worksheet.Cells["C3"].Value = "SKU";
                worksheet.Cells["D3"].Value = "Price";
                worksheet.Cells["E3"].Value = "Quantity";
                worksheet.Cells["F3"].Value = "Damage Quantity";
                worksheet.Cells["G3"].Value = "Total Quantity";
                worksheet.Cells["h3"].Value = "Total Amount";





                worksheet.Column(1).Width = 25; // Product Name column
                worksheet.Column(2).Width = 25; // New Stock In column
                worksheet.Column(3).Width = 25; // Return Stock In column
                worksheet.Column(4).Width = 25; // Shipment Return In column
                worksheet.Column(5).Width = 25; // Shipment Out column
                worksheet.Column(6).Width = 25; // Order Out column
                worksheet.Column(7).Width = 25; // Final Stock column
                worksheet.Column(8).Width = 25;
                worksheet.Column(9).Width = 25;

                worksheet.Row(2).Height = 45;
                worksheet.Row(3).Height = 45;

                // Add data to the worksheet
                int row = 4;
                foreach (var product in products)
                {

                    worksheet.Cells[$"A{row}:H{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[$"A{row}:H{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Row(row).Height = 28;
                    worksheet.Cells[$"A{row}"].Value = product.Date.ToString("dd-MM-yyyy");

                    worksheet.Cells[$"A{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Center-align Price
                    worksheet.Cells[$"B{row}"].Value = product.ProductName;
                    worksheet.Cells[$"B{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Center-align Date
                    worksheet.Cells[$"C{row}"].Value = product.SKU;
                    worksheet.Cells[$"C{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Center-align ProductName
                    worksheet.Cells[$"D{row}"].Value = product.Price;
                    worksheet.Cells[$"D{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Center-align SKU
                    worksheet.Cells[$"E{row}"].Value = product.Quantity;
                    worksheet.Cells[$"E{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Center-align Price
                    worksheet.Cells[$"F{row}"].Value = product.DamageQuantity;
                    worksheet.Cells[$"F{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[$"G{row}"].Value = product.Totalquantity;
                    worksheet.Cells[$"G{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[$"H{row}"].Value = product.Price * product.Totalquantity;
                    worksheet.Cells[$"H{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Center-align Quantity



                    worksheet.Cells[$"B{row}"].Style.Font.Bold = true;

                    worksheet.Cells[$"A{row}:H{row}"].Style.Font.Size = 14;
                    worksheet.Cells[$"B{row}"].Style.Fill.PatternType = ExcelFillStyle.Solid; // Solid fill
                    worksheet.Cells[$"B{row}"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);
                    worksheet.Cells[$"B{row}"].Style.Border.BorderAround(ExcelBorderStyle.Thin);


                    row++;
                }



                // Add a row for the total amount
                worksheet.Cells["E2"].Value = "Total Quantity";
                worksheet.Cells["E2"].Formula = $"SUM(E3:E{row - 1})"; // Assuming quantity is in column E
                worksheet.Cells["E2"].Style.Font.Bold = true;
                worksheet.Cells["E2"].Style.Font.Size = 19;
                worksheet.Cells["E2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["E2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["E2"].Style.Fill.PatternType = ExcelFillStyle.Solid; // Solid fill
                worksheet.Cells["E2"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);

                // Calculate total amount
                worksheet.Cells["H2"].Value = "Total Amount";
                worksheet.Cells["H2"].Formula = $"SUM(H3:H{row - 1})"; // Assuming total amount is in column F
                worksheet.Cells["H2"].Style.Font.Bold = true;
                worksheet.Cells["H2"].Style.Font.Size = 19;
                worksheet.Cells["H2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["H2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["H2"].Style.Fill.PatternType = ExcelFillStyle.Solid; // Solid fill
                worksheet.Cells["H2"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);

                // Auto-fit columns to make the content fit properly
                //worksheet.Cells.AutoFitColumns();

                // Return the Excel file as a downloadable file
                byte[] excelData = package.GetAsByteArray();
                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Products {startDateString}_to_{endDateString}.xlsx");
            }
        }
        #endregion

    }
}
