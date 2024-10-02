using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.OpenApi.Models;
using ProjectAllocatorSystemAPI.Data;
using ProjectAllocatorSystemAPI.Service.Contract;
using ProjectAllocatorSystemAPI.Service.Implementation;
using ProjectAllocatorSystemAPI.Data.Contract;
using ProjectAllocatorSystemAPI.Data.Implementation;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(policy =>
{
    policy.AddPolicy("AllowClientApplicationCore", builder =>
    {
        builder.WithOrigins("http://localhost:4200","http://localhost:5011")
        .AllowAnyHeader()
               .AllowAnyMethod();
    });
});


// Add services to the container.

builder.Services.AddControllers();
// Database connection
builder.Services.AddDbContextPool<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("mydb"));
});
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IAllocatorService, AllocatorService>();
builder.Services.AddScoped<IAllocatorRepository, AllocatorRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IAppDbContext>(provider => (IAppDbContext)provider.GetService(typeof(AppDbContext)));
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

//Register Dependency
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IManagerRepository, ManagerRepository>();
builder.Services.AddScoped<IManagerService, ManagerService>();

builder.Services.AddScoped<IAppDbContext>(provider => (IAppDbContext)provider.GetService(typeof(AppDbContext)));
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard authorization heading required using bearer scheme",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    c.OperationFilter<SecurityRequirementsOperationFilter>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseCors("AllowClientApplicationCore");
app.UseAuthorization();

app.MapControllers();

app.Run();
