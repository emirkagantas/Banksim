using BankSim.NotificationService;
using Invoice.API.Data;
using Invoice.API.Models;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;



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
        public async Task<IActionResult> Pay([FromBody] PayInvoiceRequestDto request)
        {
           
            var invoice = await _db.Invoices.FindAsync(request.InvoiceId);
            if (invoice == null || invoice.CustomerTckn != request.CustomerTckn)
                return NotFound();

            if (invoice.IsPaid)
                return BadRequest("Bu fatura zaten ödenmiş.");

         
            var bearer = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(bearer) || !bearer.StartsWith("Bearer "))
                return Unauthorized("Bearer token eksik.");

            var token = bearer.Replace("Bearer ", "").Trim();

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

         
            var accResp = await httpClient.GetAsync($"https://localhost:7291/api/account/{request.AccountId}");
            if (!accResp.IsSuccessStatusCode)
                return BadRequest("Hesap bilgisi alınamadı.");

            var accJson = await accResp.Content.ReadAsStringAsync();
            var account = JsonSerializer.Deserialize<AccountDto>(accJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (account == null || (account.Currency ?? account.Currency) != "TL")
                return BadRequest("Sadece TL hesabı ile ödeme yapılabilir.");

            if (account.Balance < invoice.Amount)
                return BadRequest("Bakiyeniz yetersiz.");

          
            var deductBody = JsonSerializer.Serialize(new { AccountId = request.AccountId, Amount = invoice.Amount });
            var deductContent = new StringContent(deductBody, Encoding.UTF8, "application/json");
            var deductResp = await httpClient.PostAsync("https://localhost:7291/api/account/deduct", deductContent);

            if (!deductResp.IsSuccessStatusCode)
                return BadRequest("Bakiye düşme işlemi başarısız.");

           
            invoice.IsPaid = true;
            await _db.SaveChangesAsync();

            var emailToCustomer = new EmailMessageDto
            {
                To = invoice.CustomerEmail,
                Subject = "Fatura Ödeme Bildirimi",
                Body = $"{invoice.Type} faturanız ({invoice.Amount} TL) başarıyla ödendi. Teşekkür ederiz!"
            };

            var factory = new RabbitMQ.Client.ConnectionFactory() { HostName = "localhost" };
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
