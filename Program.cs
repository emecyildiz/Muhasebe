using Muhasebe.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Muhasebe.Data;
using Muhasebe.Services;
using Muhasebe.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MuhasebeContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("connect")));


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login"; // Kullanýcý giriþ yapmamýþsa atýlacaðý sayfa
        options.AccessDeniedPath = "/Auth/AccessDenied"; // Yetkisi olmayan sayfaya girerse atýlacaðý sayfa
        options.Cookie.Name = "MuhasebeAuthCookie";
        options.ExpireTimeSpan = TimeSpan.FromHours(8); // 8 saatlik oturum süresi
        options.SlidingExpiration = true;
    });

builder.Services.AddScoped<IDepartmanService, DepartmanService>();
builder.Services.AddScoped<IFinansService, FinansService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMasrafService, MasrafService>();
builder.Services.AddScoped<IOnayService, OnayService>();
builder.Services.AddScoped<IKullaniciService, KullaniciService>();
builder.Services.AddScoped<IDepartmanButceService, DepartmanButceService>();
builder.Services.AddScoped<IMasrafTalebiService, MasrafTalebiService>();


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

app.UseStatusCodePagesWithReExecute("/Home/NotFound", "?statusCode={0}");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
