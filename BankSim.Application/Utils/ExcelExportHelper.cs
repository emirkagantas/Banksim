using BankSim.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;
using System.Collections.Generic;
using System.IO;

namespace BankSim.Application.Utils
{
    public static class ExcelExportHelper
    {
        public static byte[] ExportTransactions(List<TransactionExportDto> exportList)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Transaction History");

            worksheet.Cell(1, 1).Value = "Gönderen IBAN";
            worksheet.Cell(1, 2).Value = "Gönderen Ad Soyad";
            worksheet.Cell(1, 3).Value = "Alıcı IBAN";
            worksheet.Cell(1, 4).Value = "Alıcı Ad Soyad";
            worksheet.Cell(1, 5).Value = "Tutar";
            worksheet.Cell(1, 6).Value = "Tarih";
            worksheet.Cell(1, 7).Value = "Kur";
            worksheet.Cell(1, 8).Value = "Çevrilen Tutar";
            int row = 2;
            foreach (var t in exportList)
            {
                worksheet.Cell(row, 1).Value = t.FromIban;
                worksheet.Cell(row, 2).Value = t.FromFullName;
                worksheet.Cell(row, 3).Value = t.ToIban;
                worksheet.Cell(row, 4).Value = t.ToFullName;
                worksheet.Cell(row, 5).Value = t.Amount;
                worksheet.Cell(row, 6).Value = t.TransactionDate.ToString("yyyy-MM-dd HH:mm");
                worksheet.Cell(row, 7).Value = t.ExchangeRate.HasValue ? t.ExchangeRate.Value.ToString("F4") : "";
                worksheet.Cell(row, 8).Value = t.ConvertedAmount.HasValue ? t.ConvertedAmount.Value.ToString("F2") : "";
                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }


}

