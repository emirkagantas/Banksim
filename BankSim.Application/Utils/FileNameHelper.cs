using System.Text;

namespace BankSim.Application.Utils
{

    public static class FileNameHelper
    {
        public static string MakeFileNameSafe(string name)
        {
            var dict = new Dictionary<char, char>
        {
            {'ç','c'}, {'Ç','C'},
            {'ğ','g'}, {'Ğ','G'},
            {'ı','i'}, {'I','I'},
            {'ö','o'}, {'Ö','O'},
            {'ş','s'}, {'Ş','S'},
            {'ü','u'}, {'Ü','U'}
        };
            var sb = new StringBuilder();
            foreach (var c in name)
            {
                if (dict.ContainsKey(c)) sb.Append(dict[c]);
                else if (char.IsLetterOrDigit(c) || c == '_') sb.Append(c);
                else if (char.IsWhiteSpace(c)) sb.Append('_');
            }
            return sb.ToString();
        }

        public static string BuildExportFileName(string? fullName, string extension)
        {
            var userName = MakeFileNameSafe((fullName ?? "Kullanici").Replace(" ", "_"));
            var now = DateTime.Now.ToString("yyyyMMdd_HHmm");
            return $"{userName}_HesapEkstresi_{now}.{extension}";
        }
    }

}
