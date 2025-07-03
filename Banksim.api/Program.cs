using BankSim.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using BankSim.Application.Services;
using BankSim.Domain.Interfaces;
using BankSim.Infrastructure.Repositories;
using BankSim.Application.Mapping;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BankSimDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("BankSimConnection"))
);

builder.Services.AddControllers();

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddAutoMapper(typeof(CustomerProfile));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
