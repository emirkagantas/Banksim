using Invoice.API.Data;
using Invoice.API.Models;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using BankSim.NotificationService;



namespace Invoice.API.Controllers
{

    [ApiController]
    [Route("invoices")]
    public class InvoicesController : ControllerBase
    {
        private readonly InvoiceDbContext _db;

        public InvoicesController(InvoiceDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var invoices = await _db.Invoices.ToListAsync();
            return Ok(invoices);
        }

       
        [HttpGet("customer/{tckn}")]
        public async Task<IActionResult> GetByCustomer(string tckn)
        {
            var invoices = await _db.Invoices
                .Where(x => x.CustomerTckn == tckn)
                .ToListAsync();
            return Ok(invoices);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] InvoiceEntity invoice)
        {
            _db.Invoices.Add(invoice);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAll), new { id = invoice.Id }, invoice);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] InvoiceEntity updated)
        {
            var invoice = await _db.Invoices.FindAsync(id);
            if (invoice == null)
                return NotFound();

            invoice.Type = updated.Type;
            invoice.Amount = updated.Amount;
            invoice.DueDate = updated.DueDate;
            invoice.IsPaid = updated.IsPaid;
            invoice.CustomerTckn = updated.CustomerTckn;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var invoice = await _db.Invoices.FindAsync(id);
            if (invoice == null)
                return NotFound();

            _db.Invoices.Remove(invoice);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("pay")]
        public async Task<IActionResult> Pay([FromBody] PayInvoiceRequestEntity request)
        {
         
            var invoice = await _db.Invoices.FindAsync(request.InvoiceId);
            if (invoice == null || invoice.CustomerTckn != request.CustomerTckn)
                return NotFound();

          
            if (invoice.IsPaid)
                return BadRequest("Bu fatura zaten ödenmiş.");

        
            invoice.IsPaid = true;
            await _db.SaveChangesAsync();
            var emailToCustomer = new EmailMessageDto { 
                To = invoice.CustomerEmail, 
                Subject = "Fatura Ödeme Bildirimi",
                Body = $"{invoice.Type} faturanız ({invoice.Amount} TL) başarıyla ödendi. Teşekkür ederiz!"
            };

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "transaction.mail",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var json = JsonSerializer.Serialize(emailToCustomer);
            var body = Encoding.UTF8.GetBytes(json);
            channel.BasicPublish(exchange: "",
                                 routingKey: "transaction.mail",
                                 basicProperties: null,
                                 body: body);

            

            return Ok(invoice);
        }

    }
}
