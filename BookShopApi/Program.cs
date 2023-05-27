using BookShop.Application;
using BookShop.Domain;
using BookShop.Infrastructure;
using BookShop.Infrastructure.Repositories;
using BookShopApi.Middleware;
using BookShopApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(Log.Logger);

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // укзывает, будет ли валидироваться издатель при валидации токена
            ValidateIssuer = true,
            // строка, представляющая издателя
            ValidIssuer = AuthOptions.ISSUER,

            // будет ли валидироваться потребитель токена
            ValidateAudience = true,
            // установка потребителя токена
            ValidAudience = AuthOptions.AUDIENCE,
            // будет ли валидироваться время существования
            ValidateLifetime = true,

            // установка ключа безопасности
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
            // валидация ключа безопасности
            ValidateIssuerSigningKey = true,
        };
    });
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



try
{
    Log.Information("Adding services");

    builder.Configuration.AddJsonFile("marketsettings.json");
    builder.Services.Configure<PriceSettings>(builder.Configuration);

    builder.Services.AddHealthChecks();
    //builder.Services.AddHostedService<MyHealthCheck>();

    builder.Services.AddDbContext<ApplicationContext>();

    builder.Services.AddTransient<ICategoryRepository, CategoryRepository>();
    builder.Services.AddTransient<IBookRepository, BookRepository>();
    builder.Services.AddTransient<IAuthorRepository, AuthorRepository>();
    builder.Services.AddTransient<IBookGenreRepository, BookGenreRepository>();
    builder.Services.AddTransient<IBookAuthorRepository, BookAuthorRepository>();
    builder.Services.AddTransient<IPriceRepository, PriceRepository>();
    builder.Services.AddTransient<IUserRepository, UserRepository>();

}
catch
{
    Log.Error("Fail to adding services");
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<RequestLoggingMiddleware>();

app.UseAuthorization();
app.UseAuthorization();

app.MapControllers();

app.Run();
