using BankSim.Application.DTOs;
using QuestPDF.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSim.Application.Utils
{
    public static class PdfExportHelper
    {
        public static byte[] ExportTransactions(List<TransactionExportDto> exportList)
        {
            try
            {
                if (exportList == null)
                    exportList = new List<TransactionExportDto>();

                var document = new TransactionListPdfDocument(exportList);
                return document.GeneratePdf();
            }
            catch (Exception ex)
            {
                Console.WriteLine("PDF Oluşturma Hatası: " + ex.ToString());
                throw;
            }
        }


    }
}
