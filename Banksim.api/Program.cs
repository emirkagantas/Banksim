using BankSim.API.Middleware; // <-- ExceptionMiddleware'� ekliyoruz
using BankSim.Application.Mapping;
using BankSim.Application.Services;
using BankSim.Application.Validation;
using BankSim.Domain.Interfaces;
using BankSim.Infrastructure.Persistence;
using BankSim.Infrastructure.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BankSimDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("BankSimConnection"))
);

builder.Services.AddControllers();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateCustomerValidator>();

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

// *** Exception Middleware buraya eklendi ***
app.UseMiddleware<ExceptionMiddleware>();
// *******************************************

app.UseAuthorization();

app.MapControllers();

app.Run();
