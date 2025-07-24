using StockNotificationWarning.Config;
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
builder.Services.AddScoped<IMetadataProvider, MetadataProvider>();
builder.Services.AddSingleton<IToastNotificationService, ToastNotificationService>();
builder.Services.AddScoped<IShopifyRequestService, ShopifyRequestService>();
builder.Services.AddScoped<IConfigDefaultsProvider, ConfigDefaultsProvider>();

builder.Services.Configure<ShopDefaults>(
    builder.Configuration.GetSection("Defaults"));
builder.Services.Configure<CronConfig>(
    builder.Configuration.GetSection("Cron"));
builder.Services.Configure<ShopifyConfig>(
    builder.Configuration.GetSection("Shopify"));

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

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
