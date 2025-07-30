using Invoice.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Invoice.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<InvoiceDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("InvoiceConnection")));
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BankSim.API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\""
    });
});
builder.Services.AddControllers();



var app = builder.Build();


app.UseMiddleware<InternalApiKeyMiddleware>();

app.UseHttpsRedirection();


app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();




app.Run();






