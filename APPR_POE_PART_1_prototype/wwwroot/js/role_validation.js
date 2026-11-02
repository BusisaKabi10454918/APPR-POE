const form = document.querySelector("form");

form.addEventListener("submit", function (event) {

    const volunteer = document.getElementById("volunteer");
    const donor = document.getElementById("repeat-donor");

    if (!volunteer.checked && !donor.checked) {
        event.preventDefault();

        volunteer.setCustomValidity(
            "Please select at least one registration role."
        );

        volunteer.reportValidity();
    }
    else {
        volunteer.setCustomValidity("");
    }
});