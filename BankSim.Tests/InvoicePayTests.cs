using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Invoice.API.Controllers;
using Invoice.API.Data;
using Invoice.API.Models;

public class InvoicePayTests
{
    private static InvoicesController CreateController(InvoiceDbContext db, string? bearerToken = null)
    {
        var controller = new InvoicesController(db);
        var http = new DefaultHttpContext();
        if (!string.IsNullOrWhiteSpace(bearerToken))
            http.Request.Headers["Authorization"] = $"Bearer {bearerToken}";
        controller.ControllerContext = new ControllerContext { HttpContext = http };
        return controller;
    }

    private static InvoiceDbContext CreateInMemoryDb(string dbName)
    {
        var options = new DbContextOptionsBuilder<InvoiceDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        var ctx = new InvoiceDbContext(options);
        ctx.Database.EnsureCreated();
        return ctx;
    }

    [Fact]
   public async Task Pay_Returns401_WhenMissingBearer()
    {
        using var db = CreateInMemoryDb(nameof(Pay_Returns401_WhenMissingBearer));

        // Fatura ekle ki NotFound'a düşmesin
        db.Invoices.Add(new InvoiceEntity
        {
            Id = 1,
            CustomerEmail = "c@x.com",
            CustomerTckn = "11111111111",
            Type = "ELEC",
            Amount = 100m,
            DueDate = DateTime.UtcNow.AddDays(3),
            IsPaid = false
        });
        await db.SaveChangesAsync();

        var controller = CreateController(db); 

        var req = new PayInvoiceRequestDto
        {
            InvoiceId = 1,
            CustomerTckn = "11111111111",
            PaymentMethod = "account",
            AccountId = 100
        };

        var res = await controller.Pay(req);
        var status = Assert.IsAssignableFrom<IStatusCodeActionResult>(res);
        Assert.Equal(StatusCodes.Status401Unauthorized, status.StatusCode);
    }


    [Fact]
    public async Task Pay_Returns404_WhenInvoiceNotFound()
    {
        using var db = CreateInMemoryDb(nameof(Pay_Returns404_WhenInvoiceNotFound));
        var controller = CreateController(db, bearerToken: "Bearer tkn");
        var req = new PayInvoiceRequestDto
        {
            InvoiceId = 99,
            CustomerTckn = "11111111111",
            PaymentMethod = "account",
            AccountId = 100
        };
        var res = await controller.Pay(req);
        var status = Assert.IsAssignableFrom<IStatusCodeActionResult>(res);
        Assert.Equal(StatusCodes.Status404NotFound, status.StatusCode);
    }

    [Fact]
    public async Task Pay_Returns404_WhenTcknMismatch()
    {
        using var db = CreateInMemoryDb(nameof(Pay_Returns404_WhenTcknMismatch));
        db.Invoices.Add(new InvoiceEntity
        {
            Id = 1,
            CustomerEmail = "c@x.com",
            CustomerTckn = "99999999999",
            Type = "ELEC",
            Amount = 100m,
            DueDate = DateTime.UtcNow.AddDays(3),
            IsPaid = false
        });
        await db.SaveChangesAsync();

        var controller = CreateController(db, bearerToken: "Bearer tkn");
        var req = new PayInvoiceRequestDto
        {
            InvoiceId = 1,
            CustomerTckn = "11111111111",
            PaymentMethod = "account",
            AccountId = 100
        };
        var res = await controller.Pay(req);
        var status = Assert.IsAssignableFrom<IStatusCodeActionResult>(res);
        Assert.Equal(StatusCodes.Status404NotFound, status.StatusCode);
    }

    [Fact]
    public async Task Pay_Returns400_WhenAlreadyPaid()
    {
        using var db = CreateInMemoryDb(nameof(Pay_Returns400_WhenAlreadyPaid));
        db.Invoices.Add(new InvoiceEntity
        {
            Id = 1,
            CustomerEmail = "c@x.com",
            CustomerTckn = "11111111111",
            Type = "ELEC",
            Amount = 100m,
            DueDate = DateTime.UtcNow.AddDays(3),
            IsPaid = true
        });
        await db.SaveChangesAsync();

        var controller = CreateController(db, bearerToken: "Bearer tkn");
        var req = new PayInvoiceRequestDto
        {
            InvoiceId = 1,
            CustomerTckn = "11111111111",
            PaymentMethod = "account",
            AccountId = 100
        };
        var res = await controller.Pay(req);
        var status = Assert.IsAssignableFrom<IStatusCodeActionResult>(res);
        Assert.Equal(StatusCodes.Status400BadRequest, status.StatusCode);
    }
}
