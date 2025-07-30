using BankSim.Ui.Models;
namespace BankSim.Ui.Services
{
    public interface IInvoiceService
    {
        Task<List<InvoiceDto>> GetInvoicesByTcknAsync(string tckn);
        Task PayInvoiceAsync(int invoiceId, string tckn);
    }
}
