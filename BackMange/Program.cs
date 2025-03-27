using BackMange.Data;
using BackMange.Models;
using Microsoft.AspNetCore.Authorization;
using HTML5.Hubs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using BackMange.Controllers.API;
using Microsoft.Extensions.Options;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<GameCaseContext>(options =>
{ options.UseSqlServer(builder.Configuration.GetConnectionString("GameCase")); });

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
}).AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders(); // 確保可以支援 Token（如密碼重置等）

builder.Services.AddRazorPages();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;  // ✅ 確保使用 HTTPS
    options.Cookie.SameSite = SameSiteMode.None;  // ✅ 避免跨域問題
    options.Cookie.IsEssential = true;
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// 加入 Google 認證相關的部分
builder.Services.AddAuthentication(options =>
{
    // 使用 Cookie 作為主要認證方式
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    // 使用 Google 作為外部登入提供者
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    //設定管理者未登入導回登入畫面
    options.LoginPath = "/ttAdmins/Login"; // ✅ 確保未登入時導向這個頁面
    options.AccessDeniedPath = "/User/AccessDenied";
})
.AddGoogle(googleOptions =>
{
    // 從 appsettings.json 讀取設定
    var clientId = builder.Configuration["Authentication:Google:ClientId"];
    var clientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

    //if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
    //{
    //    throw new InvalidOperationException("需要在 appsettings.json 中設定 Google 認證資訊");
    //}

    googleOptions.ClientId = clientId;
    googleOptions.ClientSecret = clientSecret;

    // 設定登入後的重導向路徑（選擇性）
    googleOptions.CallbackPath = "/signin-google";
    googleOptions.SaveTokens = true; //  確保這一行存在
    googleOptions.Scope.Add("openid");
    googleOptions.Scope.Add("email");

});

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();

// 設定 JSON 讓屬性保持 PascalCase
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null; // 停用 camelCase
});

//個人管理頁面驗證信
builder.Services.AddMemoryCache(); // 註冊快取
builder.Services.AddScoped<VerificationService>();
builder.Services.AddScoped<EmailService>();

//註冊通知功能
builder.Services.AddScoped<INotificationService, NotificationService>();
// SignalR 服務啟動
builder.Services.AddSignalR();

//允許跨域請求
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://127.0.0.1:5502", "http://127.0.0.1:5500", "https://4925-2401-e180-8883-aa8b-5506-1cb0-5bf4-1ef8.ngrok-free.app ") // 指定允許的前端來源
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials(); // 允許憑證
    });
});

var app = builder.Build();

//新增管理員帳戶使用
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    await SeedAdminUser.InitializeAsync(services, userManager);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseCors();//跨域啟動
app.MapHub<ChatHub>("/chatSr"); //SignalR 的進入點

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=FrontIndex}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();