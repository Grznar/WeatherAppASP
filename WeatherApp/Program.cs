using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WeatherApp.Application.Common.Interfaces;
using WeatherApp.Application.Services;
using WeatherApp.Domain.Entities;
using WeatherApp.Infrastructure.Data;
using WeatherApp.Infrastructure.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer
(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
builder.Services.AddHttpClient();
builder.Services.AddHttpClient<IWeatherService, WeatherService>();
builder.Services.AddTransient<RecaptchaService>();
builder.Services.AddHttpClient<GeocodingService>();
builder.Services.ConfigureApplicationCookie(option =>
{
    option.AccessDeniedPath = "/Shared/Error";
    option.LoginPath = "/Auth/Login";

});
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 6;

}
);
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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
