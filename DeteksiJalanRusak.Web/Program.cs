using DeteksiJalanRusak.Web.Configurations;
using DeteksiJalanRusak.Web.Services.FileServices;
using MathNet.Numerics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.Configure<FileConfigurationOptions>(builder.Configuration.GetSection(FileConfigurationOptions.FileConfiguration));
builder.Services.AddScoped(sp => sp.GetRequiredService<IOptionsSnapshot<FileConfigurationOptions>>().Value);

builder.Services.AddScoped<IFileService, FileService>();

builder.Services.Configure<ModelConfigurationOptions>(builder.Configuration.GetSection(ModelConfigurationOptions.ModelConfigurationOption));
builder.Services.AddScoped(sp => sp.GetRequiredService<IOptionsSnapshot<ModelConfigurationOptions>>().Value);

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
