using Microsoft.EntityFrameworkCore;
using StockNotificationWarning.Config;
using StockNotificationWarning.Db;
using StockNotificationWarning.Defaults;
using StockNotificationWarning.Services;
using StockNotificationWarning.Services.Abstraction;

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services
    .AddScoped<ICronConfigProvider, CronConfigProvider>();
builder.Services.AddScoped<IInventoryMonitorService, InventoryMonitorService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IToastNotificationService, ToastNotificationService>();
builder.Services.AddScoped<IShopifyRequestService, ShopifyRequestService>();
builder.Services.AddScoped<IConfigDefaultsProvider, ConfigDefaultsProvider>();
builder.Services.AddSingleton<IShopifyCredentialStore, ShopifyCredentialStore>();
builder.Services.AddScoped<IMetafieldExtensionService, MetafieldExtensionService>();
builder.Services.AddScoped<IShopTokenProvider, ShopTokenProvider>();
builder.Services.AddScoped<IMetaobjectExtensionService, MetaobjectExtensionService>();
builder.Services.AddScoped<IShopifyScopeService, ShopifyScopeService>();
builder.Services.AddScoped<IShopifyVendorService, ShopifyVendorService>();  
builder.Services.AddScoped<ICustomProductService, CustomProductService>();
builder.Services.AddScoped<IProductDetailsService, ProductDetailsService>();  

builder.Services.AddControllers();

builder.Services.AddRazorPages().AddRazorPagesOptions(opts =>
{
    opts.Conventions.AddAreaPageRoute("Admin", "/Index", "admin");
});

builder.Services.Configure<ShopDefaults>(
    builder.Configuration.GetSection("Defaults"));
builder.Services.Configure<CronConfig>(
    builder.Configuration.GetSection("Cron"));
builder.Services.Configure<ShopifyConfig>(
    builder.Configuration.GetSection("Shopify"));

builder.Services.AddDbContext<ApplicationDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(opts =>
{
    opts.AddPolicy("ShopifyCorsPolicy", policy =>
    {
        policy.WithOrigins("https://previous-stare.myshopify.com")
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

builder.Configuration.AddEnvironmentVariables();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    await RuntimeSeeder.SeedAsync(context);
}

app.UseRouting();
app.UseCors("ShopifyCorsPolicy");

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();


app.Use(async (context, next) =>
{
    context.Response.Headers.Add("Content-Security-Policy", "frame-ancestors https://admin.shopify.com https://*.myshopify.com;");
    await next();
});

app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.Run();
