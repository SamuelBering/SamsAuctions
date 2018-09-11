﻿var myChart = null;

$(window).on('load', () => {
    var nav = $("#navAdminPage");
    nav.addClass("active");
});

function showStatistics(data) {

    if (myChart) {
        myChart.destroy();
    }

    var customValidation = document.getElementById("customValidation");
    customValidation.innerHTML = "";

    var ctx = document.getElementById("auctionsChart");

    if (data.points.length === 0) {


        //const context = ctx.getContext('2d');
        //context.clearRect(0, 0, ctx.width, ctx.height);

        customValidation.innerHTML = "<strong>Inga avslutade auktioner funna för denna period</strong>";
        return;
    }

    myChart = new Chart(ctx, {

        type: 'bar',
        data: {
            labels: data.points,
            datasets: [
                {
                    data: data.reservationPrices,
                    label: "Utgångspris",
                    borderColor: "blue",
                    backgroundColor: "blue",
                    fill: false
                },
                {
                    data: data.finalPrices,
                    label: "Slutpris",
                    borderColor: "green",
                    backgroundColor: "green",
                    fill: false
                },
                {
                    data: data.differences,
                    label: "skillnad: slutpris - utgångspris",
                    borderColor: "red",
                    backgroundColor: "red",
                    fill: false
                }
            ]
        }
    });
}