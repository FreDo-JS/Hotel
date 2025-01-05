// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Sprawdzanie wypelnienia pól w rejestracji
document.addEventListener("DOMContentLoaded", () => {
    const form = document.querySelector("form");
    const inputs = document.querySelectorAll(".styleInput");
    const registerButton = document.querySelector(".RegisterButton");

   
    function validateForm(event) {
        let isValid = true;

  
        inputs.forEach(input => {
            if (input.value.trim() === "") {
             
                input.style.border = "2px solid red";
                isValid = false;
            } else {
               
                input.style.border = "1px solid #ccc";
            }
        });

        if (!isValid) {
          
            event.preventDefault();
            alert("Wszystkie pola muszą być wypełnione.");
        }
    }

 
    registerButton.addEventListener("click", validateForm);
});


//Kod do zmiany karty  w adminPanelu
document.addEventListener("DOMContentLoaded", function () {
    const links = document.querySelectorAll(".adminList");
    const sections = document.querySelectorAll(".content-section");

    // Funkcja do ukrywania wszystkich sekcji
    function hideSections() {
        sections.forEach(section => {
            section.style.display = "none";
        });
    }

    // Funkcja do wyświetlania wybranej sekcji
    function showSection(sectionId) {
        console.log(`Próba wyświetlenia sekcji: ${sectionId}`); // Dodanie logowania
        hideSections(); // Ukrywamy wszystkie sekcje
        const activeSection = document.getElementById(sectionId);
        if (activeSection) {
            console.log(`Znaleziono sekcję: ${sectionId}`); // Logowanie sukcesu
            activeSection.style.display = "block"; // Pokazujemy wybraną sekcję
        } else {
            console.log(`Sekcja o id ${sectionId} nie istnieje.`); // Logowanie błędu
        }
    }

    // Obsługuje kliknięcie w lewym menu
    links.forEach(link => {
        link.addEventListener("click", function (event) {
            event.preventDefault(); // Zapobiegamy domyślnemu działaniu linku

            const sectionId = this.getAttribute("data-section");
            console.log(`Kliknięto link o data-section: ${sectionId}`); // Logowanie kliknięcia
            showSection(sectionId); // Wyświetlamy odpowiednią sekcję
        });
    });

    // Wyświetlenie sekcji "Dashboard" jako domyślnej po załadowaniu strony
    showSection("dashboard");
});
