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



