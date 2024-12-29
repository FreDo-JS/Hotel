/*document.querySelector('.checkReserv').addEventListener('click', function (event) {
    event.preventDefault(); // Zapobiega domyślnej akcji przycisku (odświeżenie strony)

    // Pobieranie wartości z formularza rezerwacji
    const imie = document.querySelector('.inputImie').value;
    const nazwisko = document.querySelector('.inputNazwisko').value;
    const liczbaOsob = document.querySelector('.inputLiczbaOsob').value;
    const pokoj = document.querySelector('.inputPokoj').value;
    const dataPrzyjazdu = document.querySelector('.inputDataPrzyjazdu').value;
    const dataWyjazdu = document.querySelector('.inputDataWyjazdu').value;

    if (!imie || !nazwisko || !liczbaOsob || !pokoj || !dataPrzyjazdu || !dataWyjazdu) {
        alert("Wszystkie pola są wymagane.");
        return;
    }

    const przyjazd = new Date(dataPrzyjazdu);
    const wyjazd = new Date(dataWyjazdu);

    if (przyjazd >= wyjazd) {
        alert("Data wyjazdu musi być późniejsza niż data przyjazdu.");
        return;
    }

    // Wysłanie danych do kontrolera serwera (ServiceController)
    fetch('/Service/ZarezerwujPokoj', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            imie: imie,
            nazwisko: nazwisko,
            liczbaOsob: parseInt(liczbaOsob),
            pokoj: pokoj,
            dataPrzyjazdu: dataPrzyjazdu,
            dataWyjazdu: dataWyjazdu
        })
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                // Jeśli rezerwacja się powiodła, wyświetl kod QR
                alert("Rezerwacja zakończona pomyślnie! Sprawdź kod QR.");
                document.querySelector('.codeQR').innerHTML = `<img src="${data.kodQRUrl}" alt="Kod QR dla pokoju">`;
            } else {
                alert(`Błąd rezerwacji: ${data.message}`);
            }
        })
        .catch((error) => {
            console.error('Błąd w komunikacji z serwerem:', error);
            alert('Wystąpił błąd podczas rezerwacji. Proszę spróbować ponownie.');
        });
});


*/

document.getElementById("checkAvailabilityButton").addEventListener("click", async function () {
    const checkInDate = document.getElementById("dataPrzyjazdu").value;
    const checkOutDate = document.getElementById("dataWyjazdu").value;
    const floor = document.getElementById("pietro").value;
    const resultElement = document.getElementById("availabilityResult");
    const roomDetailsElement = document.getElementById("roomDetails");

    // Walidacja formularza
    if (!checkInDate || !checkOutDate) {
        resultElement.textContent = "Proszę wypełnić wszystkie wymagane pola.";
        resultElement.style.color = "red";
        return;
    }

    try {
        // Wysyłanie zapytania AJAX do kontrolera
        const response = await fetch("/Service/CheckAvailability", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({
                checkInDate: checkInDate,
                checkOutDate: checkOutDate,
                floor: floor,
            }),
        });

        const data = await response.json();

        if (response.ok && data.success) {
            resultElement.textContent = "Poniżej szczegóły dostępności pokoi:";
            resultElement.style.color = "green";

            // Wyświetlanie szczegółów pokoi
            roomDetailsElement.innerHTML = ""; // Wyczyść poprzednie wyniki
            data.rooms.forEach(room => {
                const roomStatus = room.status === "wolny" ? "green" : "red";
                const roomElement = document.createElement("div");
                roomElement.style.color = roomStatus;
                roomElement.innerHTML = `<strong>Pokój ${room.roomNumber}:</strong> ${room.status}`;

                if (room.status === "zajęty") {
                    roomElement.innerHTML += `<br>Szczegóły rezerwacji:`;
                    room.reservationDetails.forEach(detail => {
                        roomElement.innerHTML += `
                            <br>- Użytkownik: ${detail.userName || "Nieznany"} ${detail.userLastName || ""}
                            <br>- Termin: ${detail.checkInDate} - ${detail.checkOutDate}
                        `;
                    });
                }

                roomDetailsElement.appendChild(roomElement);
            });
        } else {
            resultElement.textContent = data.message || "Wystąpił błąd podczas sprawdzania dostępności.";
            resultElement.style.color = "red";
        }
    } catch (error) {
        console.error("Błąd podczas komunikacji z serwerem:", error);
        resultElement.textContent = "Wystąpił błąd po stronie serwera.";
        resultElement.style.color = "red";
    }
});
async function loadExistingQRCode() {
    const reservationId = document.getElementById("qrReservationId").value;

    if (!reservationId) {
        alert("Proszę podać ID rezerwacji.");
        return;
    }

    try {
        const response = await fetch(`/Service/GetQRCode?reservationId=${reservationId}`);
        const data = await response.json();

        if (data.success) {
            document.getElementById("qrCodeImage").src = data.qrCode;
        } else {
            alert(data.message || "Nie udało się załadować kodu QR.");
        }
    } catch (error) {
        console.error("Błąd podczas ładowania kodu QR:", error);
    }
}

async function generateNewQRCode() {
    const reservationId = document.getElementById("qrReservationId").value;

    if (!reservationId) {
        alert("Proszę podać ID rezerwacji.");
        return;
    }

    try {
        const response = await fetch(`/Service/GenerateNewQRCode?reservationId=${reservationId}`);
        const data = await response.json();

        if (data.success) {
            document.getElementById("qrCodeImage").src = data.qrCode;
            alert("Nowy kod QR został wygenerowany.");
        } else {
            alert(data.message || "Nie udało się wygenerować nowego kodu QR.");
        }
    } catch (error) {
        console.error("Błąd podczas generowania nowego kodu QR:", error);
    }
}
function openQRCodeInNewTab() {
    const qrCodeImage = document.getElementById("qrCodeImage").src;

    if (!qrCodeImage || qrCodeImage === "") {
        alert("Brak załadowanego kodu QR. Najpierw wczytaj lub wygeneruj kod QR.");
        return;
    }

    const newTab = window.open();
    newTab.document.write(`<img src="${qrCodeImage}" alt="Kod QR" style="max-width: 100%; height: auto;">`);
}
function downloadQRCode() {
    const qrCodeImage = document.getElementById("qrCodeImage").src;

    if (!qrCodeImage || qrCodeImage === "") {
        alert("Brak załadowanego kodu QR. Najpierw wczytaj lub wygeneruj kod QR.");
        return;
    }

    // Tworzenie elementu <a> do pobierania pliku
    const link = document.createElement("a");
    link.href = qrCodeImage;
    link.download = "QRCode.png";
    link.click();
}
document.addEventListener("DOMContentLoaded", () => {
    document.querySelector(".sendQR").addEventListener("click", sendQrCode);
});

async function sendQrCode() {
    const reservationId = document.getElementById("qrReservationId").value.trim();

    if (!reservationId) {
        alert("Proszę podać ID rezerwacji.");
        console.error("Brak wymaganych danych: reservationId.");
        return;
    }

    console.log("Przesyłane dane:", { reservationId });

    try {
        const response = await fetch(`/Service/SendQrCodeEmail`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({ reservationId: parseInt(reservationId) }),
        });

        const data = await response.json();
        console.log("Odpowiedź z serwera:", data);

        if (data.success) {
            alert("Kod QR został wysłany na e-mail.");
        } else {
            alert(data.message || "Nie udało się wysłać kodu QR.");
        }
    } catch (error) {
        console.error("Błąd podczas wysyłania kodu QR:", error);
    }
}

console.log("Przesyłane dane:", JSON.stringify({ reservationId: parseInt(reservationId), email: email }));









