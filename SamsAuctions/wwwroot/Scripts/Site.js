function deleteAllRegularUsers() {
    var remove = confirm("Vill du ta bort alla regular users?");
    if (remove) {
        $.post("/account/DeleteAllRegularUsers", {}, (data) => {
            console.log(data.status);
        }).fail((xhr, textStatus, errorThrown) => {
            alert(xhr.responseText);
        });
    }
}