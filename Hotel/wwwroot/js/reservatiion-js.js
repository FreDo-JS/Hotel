document.querySelector('.checkReserv').addEventListener('click', function (event) {
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




document.querySelector('.Dostepnosc').addEventListener('click', function (event) {
    event.preventDefault(); // Zapobiega domyślnej akcji przycisku (odświeżenie strony)

    // Pobieranie wartości z pól formularza
    const dataPrzyjazdu = document.querySelector('.dataPrzyjazdu').value;
    const dataWyjazdu = document.querySelector('.dataWyjazdu').value;

    // Sprawdzenie, czy oba pola daty zostały wypełnione
    if (!dataPrzyjazdu || !dataWyjazdu) {
        alert('Proszę wypełnić oba pola daty.');
        return;
    }

    // Konwersja dat na obiekty JavaScript Date
    const przyjazd = new Date(dataPrzyjazdu);
    const wyjazd = new Date(dataWyjazdu);

    // Pobieranie danych o pokojach z Firestore
    db.collection('pokoje')
        .get()
        .then((querySnapshot) => {
            let wolnePokoje = [];

            querySnapshot.forEach((doc) => {
                const pokojData = doc.data();

                if (pokojData.status === 'wolny') {
                    // Pokój jest wolny
                    wolnePokoje.push(pokojData.numer);
                } else {
                    // Pokój jest zajęty - sprawdzamy, czy terminy rezerwacji nie pokrywają się
                    const dataPrzyjazduZarezerwowana = pokojData.rezerwujacy?.dataPrzyjazdu?.toDate();
                    const dataWyjazduZarezerwowana = pokojData.rezerwujacy?.dataWyjazdu?.toDate();

                    if (dataPrzyjazduZarezerwowana && dataWyjazduZarezerwowana) {
                        if (wyjazd <= dataPrzyjazduZarezerwowana || przyjazd >= dataWyjazduZarezerwowana) {
                            // Terminy się nie pokrywają - pokój jest wolny w wybranym terminie
                            wolnePokoje.push(pokojData.numer);
                        }
                    }
                }
            });

            // Wyświetlenie informacji o dostępnych pokojach
            if (wolnePokoje.length > 0) {
                document.querySelector('.accessible p').textContent = `W przedziale ${dataPrzyjazdu} - ${dataWyjazdu} są dostępne pokoje: ${wolnePokoje.join(', ')}.`;
            } else {
                document.querySelector('.accessible p').textContent = `W przedziale ${dataPrzyjazdu} - ${dataWyjazdu} nie ma dostępnych pokojów.`;
            }
        })
        .catch((error) => {
            console.error('Błąd przy sprawdzaniu dostępności:', error);
            alert('Wystąpił błąd przy sprawdzaniu dostępności. Proszę spróbować ponownie.');
        });
});

