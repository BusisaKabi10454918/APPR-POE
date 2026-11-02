function showPopup(message, isSuccess) {
    const popup = document.getElementById(
        isSuccess ? "success-validation" : "error-validation"
    );

    const messageElement = document.getElementById(
        isSuccess ? "success-message" : "error-message"
    );

    const closeButton = document.getElementById(
        isSuccess ? "success-close" : "error-close"
    );

    messageElement.textContent = message;

    popup.classList.add("active");

    closeButton.onclick = function () {
        popup.classList.remove("active");

        if (isSuccess) {
            window.location.href = "/Index";
        }
    };
}