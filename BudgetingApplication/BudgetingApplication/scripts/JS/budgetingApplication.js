$(document).ready(function () {
    var GetDonutAndBarGraphDataURL = '/Home/GetDonutAndBarGraphData';
    var GetCalendarDataURL = '/Home/GetCalendarData';
    var data1, labels;
    var monthNames = ["January", "February", "March", "April", "May", "June",
  "July", "August", "September", "October", "November", "December"
    ];

    ctx = $("#chart-area")[0];
    ctx1 = $("#graph-chart")[0];

    if (ctx != null) {

        // getting data for calendar
        $.ajax({
            url: GetCalendarDataURL,
            dataType: "json",
            success: function (data) {
                labels = data;
            },
            error: function (response) {
                alert("Transactions 1 data request failed");
            }
        });

        // getting data for donut and bar graph
        $.ajax({
            url: GetDonutAndBarGraphDataURL,
            dataType: "json",
            success: function (json) {
                var d = new Date();
                var month = d.getMonth();
                config = {
                    type: 'doughnut',
                    data: json,
                    options: {
                        responsive: true,
                        legend: {
                            position: 'bottom',
                            labels: {
                                boxWidth: 10
                            }
                        },
                        title: {
                            display: true,
                            text: monthNames[month] + '\'s Spending'
                        },
                        animation: {
                            animateScale: true,
                            animateRotate: true
                        },
                        tooltips: {
                            callbacks: {
                                title: function (tooltipItem, data) {
                                    return data.labels[tooltipItem[0].index];
                                },
                                //footer: function (tooltipItem, data) {
                                //    return "footer";
                                //},
                                label: function (tooltipItem, data) {
                                    //get the concerned dataset
                                    var dataset = data.datasets[tooltipItem.datasetIndex];
                                    //calculate the total of this data set
                                    var total = dataset.data[tooltipItem.index];


                                    return " " + currencyFormat(total, '$');
                                }
                            }
                        }
                    }
                };
                barGraphConfig = {
                    type: 'horizontalBar',
                    data: json,
                    options: {
                        responsive: true,
                        legend: {
                            display: false
                        },
                        title: {
                            display: true,
                            text: monthNames[month] + '\'s Spending'
                        },
                        animation: {
                            animateScale: true,
                            animateRotate: true
                        }, tooltips: {
                            callbacks: {
                                title: function (tooltipItem, data) {
                                    return data.labels[tooltipItem[0].index];
                                },
                                //footer: function (tooltipItem, data) {
                                //    return "footer";
                                //},
                                label: function (tooltipItem, data) {
                                    //get the concerned dataset
                                    var dataset = data.datasets[tooltipItem.datasetIndex];
                                    //calculate the total of this data set
                                    var total = dataset.data[tooltipItem.index];


                                    return " " + currencyFormat(total, '$');
                                }
                            }
                        }
                    }
                };
                window.myDoughnut = new Chart(ctx, config);
                window.myBar = new Chart(ctx1, barGraphConfig);
            },
            error: function () {
                alert("Transactions 2 data request failed");
            }
        });
    }
});
function currencyFormat(n, currency) {
    return currency + n.toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,");
}