// Przechwytywanie danych formularza rezerwacji i zapisywanie do Firestore
document.querySelector('.checkReserv').addEventListener('click', function (event) {
    event.preventDefault(); // Zatrzymuje domyślne działanie przycisku, aby nie odświeżał strony

    // Pobieranie wartości z formularza rezerwacji
    const imie = document.querySelector('.inputImie').value;
    const nazwisko = document.querySelector('.inputNazwisko').value;
    const liczbaOsob = document.querySelector('.inputLiczbaOsob').value;
    const pokoj = document.querySelector('.inputPokoj').value;

    // Sprawdzenie, czy wszystkie wymagane pola są wypełnione
    if (!imie || !nazwisko || !liczbaOsob || !pokoj) {
        alert("Wszystkie pola są wymagane.");
        return;
    }

    // Tworzenie obiektu rezerwacji
    const reservation = {
        imie: imie,
        nazwisko: nazwisko,
        liczbaOsob: parseInt(liczbaOsob),
        pokoj: parseInt(pokoj),
        dataRezerwacji: firebase.firestore.Timestamp.now() // Data dodania rezerwacji
    };

    // Dodanie dokumentu do kolekcji 'rezerwacje' w Firestore
    db.collection('rezerwacje')
        .add(reservation)
        .then(() => {
            alert('Rezerwacja została zapisana pomyślnie!');
        })
        .catch((error) => {
            console.error('Błąd przy zapisywaniu rezerwacji:', error);
            alert('Wystąpił błąd przy zapisywaniu rezerwacji. Proszę spróbować ponownie.');
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

    // Pobieranie rezerwacji z Firestore w określonym przedziale czasowym
    db.collection('rezerwacje')
        .where('dataRezerwacji', '>=', firebase.firestore.Timestamp.fromDate(przyjazd))
        .where('dataRezerwacji', '<=', firebase.firestore.Timestamp.fromDate(wyjazd))
        .get()
        .then((querySnapshot) => {
            // Obliczenie liczby dostępnych pokoi
            const iloscRezerwacji = querySnapshot.size;
            const wolnePokoje = 50 - iloscRezerwacji; // Zakładamy, że hotel ma 50 pokoi

            // Wyświetlenie informacji o dostępnych pokojach
            document.querySelector('.accessible p').textContent = `W przedziale ${dataPrzyjazdu} - ${dataWyjazdu} jest dostępnych ${wolnePokoje} pokoi.`;
        })
        .catch((error) => {
            console.error('Błąd przy sprawdzaniu dostępności:', error);
            alert('Wystąpił błąd przy sprawdzaniu dostępności. Proszę spróbować ponownie.');
        });
});

