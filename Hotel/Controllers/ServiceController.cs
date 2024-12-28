using Microsoft.AspNetCore.Mvc;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using QRCoder;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Hotel.Controllers
{
    public class ServiceController : Controller
    {

        [HttpGet]
        public IActionResult Index()
        {
            return View(); // Dodaje widok Index.cshtml dla tego kontrolera
        }

        [HttpPost]
        public async Task<IActionResult> ZarezerwujPokoj([FromBody] RezerwacjaDto rezerwacja)
        {
            if (string.IsNullOrEmpty(rezerwacja.Imie) || string.IsNullOrEmpty(rezerwacja.Nazwisko) || rezerwacja.LiczbaOsob <= 0 || string.IsNullOrEmpty(rezerwacja.Pokoj) || rezerwacja.DataPrzyjazdu >= rezerwacja.DataWyjazdu)
            {
                return Json(new { success = false, message = "Wszystkie pola są wymagane, a data wyjazdu musi być późniejsza niż data przyjazdu." });
            }

            // Generowanie unikalnego identyfikatora dla pokoju
            string unikalnyKod = Guid.NewGuid().ToString();

            // Generowanie kodu QR przy użyciu QRCoder
            string kodQRUrl;
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(unikalnyKod, QRCodeGenerator.ECCLevel.Q);
                BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);
                byte[] qrCodeAsBitmapByteArr = qrCode.GetGraphic(20);
                kodQRUrl = "data:image/png;base64," + Convert.ToBase64String(qrCodeAsBitmapByteArr);
            }

            // Dodawanie rezerwacji do Firestore
            try
            {
                FirestoreDb db = FirestoreDb.Create("hotelssigma");
                var document = db.Collection("pokoje").Document(rezerwacja.Pokoj);
                var docSnapshot = await document.GetSnapshotAsync();

                if (docSnapshot.Exists)
                {
                    var pokojData = docSnapshot.ToDictionary();

                    // Sprawdzenie, czy pokój jest wolny
                    if (pokojData.ContainsKey("status") && pokojData["status"].ToString() == "wolny")
                    {
                        // Aktualizacja dokumentu pokoju
                        var rezerwujacy = new
                        {
                            imie = rezerwacja.Imie,
                            nazwisko = rezerwacja.Nazwisko,
                            liczbaOsob = rezerwacja.LiczbaOsob,
                            dataPrzyjazdu = rezerwacja.DataPrzyjazdu,
                            dataWyjazdu = rezerwacja.DataWyjazdu
                        };

                        var updateData = new
                        {
                            status = "zajęty",
                            rezerwujacy = rezerwujacy,
                            kodQR = unikalnyKod,
                            kodQRUrl = kodQRUrl
                        };

                        await document.SetAsync(updateData);

                        return Json(new { success = true, kodQRUrl = kodQRUrl });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Pokój jest już zajęty." });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Podany pokój nie istnieje." });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Błąd przy zapisywaniu rezerwacji: " + ex.ToString()); // Logowanie pełnych informacji o wyjątku
                return Json(new { success = false, message = "Wystąpił błąd przy zapisywaniu rezerwacji: " + ex.Message });
            }

        }
    }

    // DTO dla rezerwacji
    public class RezerwacjaDto
    {
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public int LiczbaOsob { get; set; }
        public string Pokoj { get; set; }
        public DateTime DataPrzyjazdu { get; set; }
        public DateTime DataWyjazdu { get; set; }
    }
}
