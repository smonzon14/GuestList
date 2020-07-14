using App3.Services;
using System.Threading.Tasks;
using Xamarin.Forms;
using ZXing.Mobile;

[assembly: Dependency(typeof(App3.iOS.Services.QrScanningService))]
namespace App3.iOS.Services
{
    public class QrScanningService : IQrScanningService
    {
        public async Task<string> ScanAsync()
        {
            var optionsDefault = new MobileBarcodeScanningOptions();
            var scanner = new MobileBarcodeScanner
            {

                TopText = "Scan",
                BottomText = "Align QR code in box to scan"
            };
            var scanResult = await scanner.Scan(optionsDefault);
            if (scanResult == null) return null;
            return scanResult.Text;
        }
    }
}