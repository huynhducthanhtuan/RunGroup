function openModal(modalId) {
    var modal = document.getElementById(modalId);
    modal.style.display = "block";
}

function closeModal(modalId) {
    var modal = document.getElementById(modalId);
    modal.style.display = "none";
}

function triggerClickButton(buttonId) {
    var submitButton = document.getElementById(buttonId);
    submitButton.click();
}