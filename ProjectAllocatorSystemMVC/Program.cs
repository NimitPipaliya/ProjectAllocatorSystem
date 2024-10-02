using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ProjectAllocatorSystemMVC;
using ProjectAllocatorSystemMVC.Implementation;
using ProjectAllocatorSystemMVC.Infrastructure;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
// Configure JWET authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Token"])),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
        };
    });
// Register Dependency
builder.Services.AddScoped<IHttpClientService, HttpClientService>();
builder.Services.AddScoped<IJwtTokenHandler, JwtTokenHandler>();
builder.Services.AddHttpClient();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
// Add the JwetTokenMiddleware
app.UseMiddleware<JwtTokenMiddleware>();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
