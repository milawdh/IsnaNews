using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Services.Services;
using System.Text;
using ServiceLayer.MiddleWares;
using ServiceLayer.Middlewares;
using Microsoft.AspNetCore.Antiforgery;
using ServiceLayer.Services.Api;
using ServiceLayer.Services.Impl;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<Core>();
builder.Services.AddScoped<IApplicationBuilder, ApplicationBuilder>();
builder.Services.AddSingleton<IAdminService, AdminService>();
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(3);
});
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
//Jwt Configurations
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();
builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddCookie("Cookies").AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
    };
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseSession();

var antiforgery = app.Services.GetRequiredService<IAntiforgery>();

app.UseAntiforgery();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
IConfiguration configuration = app.Configuration;
IWebHostEnvironment environment = app.Environment;
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//Validates LoginAttempts

app.UseJwtHandlerMiddleware();
app.UseRequestAttemtpsMiddleware("");
app.UseWhen(
    context => context.Request.Path.Value.Contains("/UserLogin"), app =>
    app.UseRequestAttemtpsMiddleware("")
);

app.Run();










