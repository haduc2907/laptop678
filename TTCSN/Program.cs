using Microsoft.AspNetCore.Authentication.Cookies;
using TTCSN.Infrastructure.Sql;
using TTCSN.Services;
using TTCSN.Usecase.AdminSide;
using TTCSN.Usecase.AdminSide.Report;
using TTCSN.Usecase.AdminSide.Review;
using TTCSN.Usecase.UserSide;

namespace TTCSN
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Register repositories
            builder.Services.AddSingleton<IUserControllerRepository, SqlUserControllerRepository>();
            builder.Services.AddSingleton<ICategoryController, SqlCategoryControllerRepository>();
            builder.Services.AddSingleton<IProductController, SqlProductControllerRepository>();
            builder.Services.AddSingleton<IOrderController, SqlOrderControllerRepository>();
            builder.Services.AddSingleton<IOrderDetailController, SqlOrderDetailControllerRepository>();
            builder.Services.AddSingleton<IReviewController, SqlReviewControllerRepository>();
            builder.Services.AddSingleton<IReportController, SqlReportControllerRepository>();
            builder.Services.AddScoped<ReportControllerRepository>();
            builder.Services.AddScoped<ReviewControllerRepository>();
            builder.Services.AddScoped<OrderDetailControllerRepository>();
            builder.Services.AddScoped<OrderControllerRepository>();
            builder.Services.AddScoped<UserControllerRepository>();
            builder.Services.AddScoped<CategoryControllerRepository>();
            builder.Services.AddScoped<ProductControllerRepository>();

            // Add HttpContextAccessor for accessing session in services

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<CartService>();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, config =>
                {
                    config.Cookie.Name = "UserLoginCookie";
                    config.LoginPath = "/Account/Login";
                    config.AccessDeniedPath = "/Home/Index";
                    config.LogoutPath = "/Account/Logout";
                });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
