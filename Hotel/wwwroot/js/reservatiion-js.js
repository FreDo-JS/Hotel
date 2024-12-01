// Przechwytywanie danych formularza rezerwacji i zapisywanie do Firestore
const pokoje = [
    { numer: '001', pietro: 'parter', liczbaOsob: 2, status: 'wolny', rezerwujacy: null },
    { numer: '002', pietro: 'parter', liczbaOsob: 4, status: 'wolny', rezerwujacy: null },
    // Dodać pozostałe pokoje dla parteru, 1 piętra, 2 piętra
    { numer: '101', pietro: '1', liczbaOsob: 3, status: 'wolny', rezerwujacy: null },
    { numer: '102', pietro: '1', liczbaOsob: 2, status: 'wolny', rezerwujacy: null },
    // ...
    { numer: '201', pietro: '2', liczbaOsob: 4, status: 'wolny', rezerwujacy: null },
    { numer: '202', pietro: '2', liczbaOsob: 3, status: 'wolny', rezerwujacy: null },
    // Dodać wszystkie pokoje
];

pokoje.forEach((pokoj) => {
    db.collection('pokoje')
        .doc(pokoj.numer)
        .set(pokoj)
        .then(() => {
            console.log(`Dodano pokój ${pokoj.numer}`);
        })
        .catch((error) => {
            console.error(`Błąd przy dodawaniu pokoju ${pokoj.numer}:`, error);
        });
});

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

    // Sprawdzenie, czy pokój jest wolny
    db.collection('pokoje')
        .doc(pokoj)
        .get()
        .then((doc) => {
            if (doc.exists) {
                const pokojData = doc.data();

                if (pokojData.status === 'wolny') {
                    // Pokój jest wolny, więc dokonujemy rezerwacji
                    db.collection('pokoje')
                        .doc(pokoj)
                        .update({
                            status: 'zajęty',
                            rezerwujacy: {
                                imie: imie,
                                nazwisko: nazwisko,
                                liczbaOsob: parseInt(liczbaOsob),
                                dataPrzyjazdu: firebase.firestore.Timestamp.fromDate(przyjazd),
                                dataWyjazdu: firebase.firestore.Timestamp.fromDate(wyjazd),
                            }
                        })
                        .then(() => {
                            alert('Rezerwacja została zapisana pomyślnie!');
                        })
                        .catch((error) => {
                            console.error('Błąd przy zapisywaniu rezerwacji:', error);
                            alert('Wystąpił błąd przy zapisywaniu rezerwacji. Proszę spróbować ponownie.');
                        });
                } else {
                    alert('Pokój jest już zajęty. Proszę wybrać inny pokój.');
                }
            } else {
                alert('Podany pokój nie istnieje.');
            }
        })
        .catch((error) => {
            console.error('Błąd przy sprawdzaniu dostępności pokoju:', error);
            alert('Wystąpił błąd przy sprawdzaniu dostępności. Proszę spróbować ponownie.');
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

