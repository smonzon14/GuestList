using System.Threading.Tasks;

namespace App3.Services
{
    public interface IQrScanningService
    {
        Task<string> ScanAsync();
    }
}
