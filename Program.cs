using Microsoft.EntityFrameworkCore;
using ProvaPub.Helpers;
using ProvaPub.Repository;
using ProvaPub.Services;
using ProvaPub.Services.Interfaces;
using ProvaPub.Services.Payment;
using ProvaPub.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IDateTimeProvider, SystemDateTimeProvider>();

builder.Services.AddScoped<IPurchaseRule, PurchaseRules>();
builder.Services.AddScoped<IPurchaseRule, OnePurchasePerMonthRule>();
builder.Services.AddScoped<IPurchaseRule, FirstPurchaseMaxValueRule>();
builder.Services.AddScoped<IPurchaseRule, BusinessHoursRule>();

builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<PaymentStrategyFactory>();
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IRandomService, RandomService>();


builder.Services.AddDbContext<TestDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("ctx")));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
