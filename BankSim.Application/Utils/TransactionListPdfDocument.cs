using BankSim.Application.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;


namespace BankSim.Application.Utils
{

    public class TransactionListPdfDocument : IDocument
    {
        private readonly List<TransactionExportDto> _transactions;

        public TransactionListPdfDocument(List<TransactionExportDto> transactions)
        {
            _transactions = transactions;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Size(PageSizes.A4);
                page.Header().Text("Hesap Ekstresi / İşlem Listesi").Bold().FontSize(20).AlignCenter();
                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("Gönderen IBAN");
                        header.Cell().Element(CellStyle).Text("Gönderen Ad Soyad");
                        header.Cell().Element(CellStyle).Text("Alıcı IBAN");
                        header.Cell().Element(CellStyle).Text("Alıcı Ad Soyad");
                        header.Cell().Element(CellStyle).Text("Tutar");
                        header.Cell().Element(CellStyle).Text("Tarih");
                        header.Cell().Element(CellStyle).Text("Kur");
                        header.Cell().Element(CellStyle).Text("Çevrilen Tutar");
                    });

                    foreach (var t in _transactions)
                    {
                        table.Cell().Element(CellStyle).Text(t.FromIban);
                        table.Cell().Element(CellStyle).Text(t.FromFullName);
                        table.Cell().Element(CellStyle).Text(t.ToIban);
                        table.Cell().Element(CellStyle).Text(t.ToFullName);
                        table.Cell().Element(CellStyle).Text($"{t.Amount:N2}");
                        table.Cell().Element(CellStyle).Text(t.TransactionDate.ToString("yyyy-MM-dd HH:mm"));
                        table.Cell().Element(CellStyle).Text(t.ExchangeRate.HasValue ? t.ExchangeRate.Value.ToString("F4") : "");
                        table.Cell().Element(CellStyle).Text(t.ConvertedAmount.HasValue ? t.ConvertedAmount.Value.ToString("F2") : "");
                    }

                    static IContainer CellStyle(IContainer container)
                        => container.PaddingVertical(2).PaddingHorizontal(4);
                });
            });
        }
    }

}
