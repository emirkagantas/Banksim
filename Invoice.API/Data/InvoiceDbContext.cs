using Microsoft.EntityFrameworkCore;
using Invoice.API.Models;



namespace Invoice.API.Data

{
    public class InvoiceDbContext : DbContext

    {
        public InvoiceDbContext(DbContextOptions<InvoiceDbContext> options) : base(options) { }

        public DbSet<InvoiceEntity> Invoices => Set<InvoiceEntity>();

    }
}
