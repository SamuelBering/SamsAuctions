function showStatistics(data) {

    var ctx = document.getElementById("auctionsChart");
    var myChart = new Chart(ctx, {

        type: 'bar',
        data: {
            labels: data.points,
            datasets: [
                {
                    data: data.reservationPrices,
                    label: "Utgångspris",
                    borderColor: "blue",
                    backgroundColor:"blue",
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
                },
                //{
                //    data: asia,
                //    label: "Asia",
                //    borderColor: "#3e95cd",
                //    fill: false
                //},
                //{
                //    data: europe,
                //    label: "Europe",
                //    borderColor: "#3e95cd",
                //    fill: false
                //},
                //{
                //    data: latinAmerica,
                //    label: "Latin America",
                //    borderColor: "#3e95cd",
                //    fill: false
                //},
                //{
                //    data: northAmerica,
                //    label: "North America",
                //    borderColor: "#3e95cd",
                //    fill: false
                //}
            ]
        }
    });
}