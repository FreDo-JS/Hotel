// firebase-config.js (wersja Firebase 8.x)

// Sprawdź, czy skrypt się ładuje
console.log("Plik firebase-config.js załadowany");

// Konfiguracja Firebase
const firebaseConfig = {
    apiKey: "AIzaSyDDOcI0a2y3vNwrwvzzcqHkN0p_JHUvKbI",
    authDomain: "hotelssigma.firebaseapp.com",
    projectId: "hotelssigma",
    storageBucket: "hotelssigma.firebasestorage.app",
    messagingSenderId: "494639809349",
    appId: "1:494639809349:web:95d6b91b2fc50338fb342c",
    measurementId: "G-E0BQ9LVKTT"
};

// Inicjalizacja Firebase
firebase.initializeApp(firebaseConfig);

// Inicjalizacja Analytics (jeśli jest wymagane)
firebase.analytics();

// Konfiguracja uwierzytelniania Google
const auth = firebase.auth();
const provider = new firebase.auth.GoogleAuthProvider();

// Funkcja logowania dostępna globalnie
window.googleLogin = function () {
    auth.signInWithPopup(provider)
        .then((result) => {
            const user = result.user;
            console.log('Zalogowano:', user.displayName);

            // Pobierz token identyfikacyjny użytkownika
            user.getIdToken().then((idToken) => {
                // Wyślij token do serwera, aby utworzyć sesję
                fetch('/Account/GoogleLogin', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({ token: idToken })
                })
                    .then(response => response.json())
                    .then(data => {
                        if (data.success) {
                            // Przekierowanie na stronę główną po pomyślnym utworzeniu sesji
                            window.location.href = '/Account/UserProfile';
                        } else {
                            console.error('Błąd logowania na serwerze:', data.message);
                        }
                    })
                    .catch((error) => {
                        console.error('Błąd w komunikacji z serwerem:', error);
                    });
            });

        })
        .catch((error) => {
            console.error('Błąd logowania:', error);
        });
};
