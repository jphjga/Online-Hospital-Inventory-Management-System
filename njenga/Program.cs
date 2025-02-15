using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Njenga.Data;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add session services
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Adjust timeout as needed
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add database context with MySQL (Using Pomelo)
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseMySql("server=localhost;database=jpjga;user=root;password=",
        new MySqlServerVersion(new Version(8, 0, 30))));

builder.Services.AddRazorPages();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();
var app = builder.Build();
app.UseStaticFiles();

// Enable session middleware
app.UseSession();
app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();
app.Run();
