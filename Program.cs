using Muhasebe.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Muhasebe.Data;
using Muhasebe.Services;
using Muhasebe.Services.Interfaces;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MuhasebeContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("connect")));


// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IDepartmanService, DepartmanService>();
builder.Services.AddScoped<IFinansService, FinansService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMasrafService, MasrafService>();
builder.Services.AddScoped<IOnayService, OnayService>();
builder.Services.AddScoped<IKullaniciService, KullaniciService>();
builder.Services.AddScoped<IDepartmanButceService, DepartmanButceService>();


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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
