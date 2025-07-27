using System.Net.Http;
using System.Threading.Tasks;

namespace BankSim.Ui.Services
{
    public interface IApiService
    {
        Task<HttpResponseMessage> GetAsync(string endpoint);
        Task<HttpResponseMessage> PostAsync<T>(string endpoint, T data);
        Task<string?> GetErrorMessageAsync(HttpResponseMessage response);
    }
}
