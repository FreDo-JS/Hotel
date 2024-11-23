// Import the functions you need from the SDKs you need
import { initializeApp } from "firebase/app";
import { getAnalytics } from "firebase/analytics";
import { getAuth, signInWithPopup, GoogleAuthProvider } from "firebase/auth";
// TODO: Add SDKs for Firebase products that you want to use
// https://firebase.google.com/docs/web/setup#available-libraries

// Your web app's Firebase configuration
// For Firebase JS SDK v7.20.0 and later, measurementId is optional
const firebaseConfig = {
  apiKey: "AIzaSyDDOcI0a2y3vNwrwvzzcqHkN0p_JHUvKbI",
  authDomain: "hotelssigma.firebaseapp.com",
  projectId: "hotelssigma",
  storageBucket: "hotelssigma.firebasestorage.app",
  messagingSenderId: "494639809349",
  appId: "1:494639809349:web:95d6b91b2fc50338fb342c",
  measurementId: "G-E0BQ9LVKTT"
};

// Initialize Firebase
const app = initializeApp(firebaseConfig);
const analytics = getAnalytics(app);

const auth = getAuth();
const provider = new GoogleAuthProvider();

// Funkcja logowania dostępna globalnie
function googleLogin() {
    auth.signInWithPopup(provider)
        .then((result) => {
            // Uzyskane dane o użytkowniku
            const user = result.user;
            console.log('Zalogowano:', user.displayName);
            // Tutaj możesz wykonać dowolne akcje, takie jak przekierowanie
        })
        .catch((error) => {
            console.error('Błąd logowania:', error);
        });
}

// Przypisanie funkcji do globalnego obiektu `window`
window.googleLogin = googleLogin;
