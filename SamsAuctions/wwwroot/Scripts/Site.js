﻿function deleteAllRegularUsers() {
    var remove = confirm("Vill du ta bort alla regular users?");
    if (remove) {
        $.post("/account/DeleteAllRegularUsers", {}, (data) => {
            console.log(data.status);
        }).fail((xhr, textStatus, errorThrown) => {
            alert(xhr.responseText);
        });
    }
}

//=====loading spinner
function startSpinAnimation() {
    let statusDiv = $("#loadingSpinner");
    statusDiv.removeClass("animationStop");
    statusDiv.addClass("animationStart");
}
function stopSpinAnimation() {
    setTimeout(function () {
        let statusDiv = $("#loadingSpinner");
        statusDiv.removeClass("animationStart");
        statusDiv.addClass("animationStop");
    }, 400);
}
