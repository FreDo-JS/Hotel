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
    console.log("Funkcja googleLogin wywołana");
    auth.signInWithPopup(provider)
        .then((result) => {
            // Uzyskane dane o użytkowniku
            const user = result.user;
            console.log('Zalogowano:', user.displayName);
        })
        .catch((error) => {
            console.error('Błąd logowania:', error);
        });
};
