using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OfficeOpenXml;
using FinanceManagement;
using FinanceManagement.Models;
//using FinanceManagement.IRepository;
//using FinanceManagement.SqlRepository;
using FinanceManagement.Models;
using FinanceManagement;
using FinanceManagement.IRepository;
using FinanceManagement.SqlRepository;


var builder = WebApplication.CreateBuilder(args);

// Configure services
ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

// Configure middleware
Configure(app);

app.Run();

// Configure services
void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    // Add MVC services
    services.AddControllersWithViews();

    // Configure DbContext with SQL Server
    services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

    services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromDays(30); // Set session timeout
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });

    // Configure Identity
    services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        // Password policy configuration
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredUniqueChars = 0;
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
    })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

    // Add repositories
    services.AddScoped<ICashFlowRepository, SqlCashFlowRepository>();
    services.AddScoped<IExpensesRepository, SqlExpensesRepository>();
    services.AddScoped<IPaymentRepository, SqlPaymentRepository>();
    services.AddScoped<IUpadRepository, SqlUpadRepository>();
    services.AddScoped<ICompanyRepository, SqlCompanyRepository>();
    services.AddScoped<IAccountMasterRepository, SqlAccountMasterRepository>(); 
    services.AddScoped<IDashboardRepository , SqlDashboardRepository>();
    services.AddScoped<IOtherRepository, SqlOtherRepository>();
    services.AddScoped<IProductRepository, SqlProductRepository>();
    services.AddScoped<IDailyBeltUpdateRepository, SqlDailyBeltUpdateRepository>();

    // Add HttpContextAccessor
    services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


    // Set license context for ExcelPackage
    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
}

// Configure middleware
void Configure(WebApplication app)
{
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
        app.UseDeveloperExceptionPage();

    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllerRoute(
    name: "TransferToShipmentOut",
    pattern: "ProductOnHold/TransferToShipmentOut",
    defaults: new { controller = "ProductOnHold", action = "TransferToShipmentOut" });


        // Default route configuration
        endpoints.MapControllerRoute(
            name: "default",
        //pattern: "{controller=Home}/{action=Index}/{id?}");
        pattern: "{area=Identity}/{controller=Account}/{action=Register}/{id?}");
    });
}
