using DataAcess;
using DataAcess.Entity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using ShoesRepository;
using ShoesRepository.GenreicRepo;
using shoesServices;
using ShoesShared.MailServics;
using StackExchange.Redis;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var _GetConnectionString = builder.Configuration.GetConnectionString("DefaultConnectionString");
builder.Services.AddDbContext<ShoesEcommerceContext>(options => options.UseSqlServer(_GetConnectionString));
// Add services to the container.
builder.Services.AddTransient<IAccountServices, AccountServices>();
builder.Services.AddTransient<IAccountRepository, AccountRepository>();
builder.Services.AddTransient(typeof(IGenericRepository<>), typeof(GenricRepository<>));
builder.Services.AddScoped<IMailServices, MailServices>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
//Connection for redis
builder.Services.AddScoped<IConnectionMultiplexer>(c =>
{
    var configuration = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("Redis"), true);
return ConnectionMultiplexer.Connect(configuration);

});
//builder.Services.AddStackExchangeRedisCache(options =>
//{
//    options.Configuration = builder.Configuration.GetConnectionString("Redis");
//    options.InstanceName = "GamesCatalog_";
//});
builder.Services.AddTransient<IBasketRepository, BasketRepository>();

// For Identity  
builder.Services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<ShoesEcommerceContext>()
                .AddDefaultTokenProviders();

//for cross origin 
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder
            .WithOrigins("http://localhost:5173") // Your React app's domain
            .AllowAnyMethod()
            .AllowAnyHeader());
});

//for jwt auth
builder.Services.AddAuthentication(option=>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});


builder.Services.Configure<DataProtectionTokenProviderOptions>(opt =>
   opt.TokenLifespan = TimeSpan.FromMinutes(10));

//Add logger configuration
builder.Host.UseSerilog((context, configuraton) =>
configuraton.ReadFrom.Configuration(context.Configuration).Enrich.FromLogContext());
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowSpecificOrigin");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
