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

builder.Services.AddCors(opts =>
{
    opts.AddPolicy("ShopifyCorsPolicy", policy =>
    {
        policy.WithOrigins("https://previous-stare.myshopify.com")
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

builder.Configuration.AddEnvironmentVariables();

var app = builder.Build();

app.UseCors("ShopifyCorsPolicy");
app.UseCookiePolicy();
app.UseSession();

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

app.Use(async (context, next) =>
{
    var host = context.Request.Query["host"].ToString();

    var allowedOrigins = new List<string>
    {
        "https://admin.shopify.com",
        "https://partners.shopify.com"
    };

    if (!string.IsNullOrEmpty(host))
    {
        allowedOrigins.Add($"https://{host}");
    }

    var cspValue = $"frame-ancestors {string.Join(" ", allowedOrigins)}";
    context.Response.Headers["Content-Security-Policy"] = cspValue;

    context.Response.Headers.Remove("X-Frame-Options");

    await next();
});

app.UseAuthorization();

app.MapRazorPages();

app.Run();
