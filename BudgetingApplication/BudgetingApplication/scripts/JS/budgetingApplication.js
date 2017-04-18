$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip();
    var GetDonutAndBarGraphDataURL = '/Home/GetDonutAndBarGraphData';
    var monthNames = ["January", "February", "March", "April", "May", "June",
  "July", "August", "September", "October", "November", "December"
    ];

    ctx = $("#chartArea")[0];
    ctx2 = $("#chart-area")[0];
    ctx1 = $("#graph-chart")[0];
    ctxPolar = $("#polar-chart")[0];

    if (ctx != null) {
        // getting data for donut and bar graph
        var config;
        $.ajax({
            async: true,
            url: GetDonutAndBarGraphDataURL,
            dataType: "json",
            success: function (json) {
                var d = new Date();
                var month = d.getMonth();
                config = {
                    type: 'doughnut',
                    data: json,
                    options: {
                        responsive: false,
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
                if (json.datasets[0].data.length == 0) {
                    window.graphContainer.innerHTML = "<center>There is no data for this month yet </center>";
                } else {
                    window.myChart = new Chart(ctx, config);
                }
            },
            error: function () {
                alert("Transactions 2 data request failed");
            }
        });
        $('#showCharts li span').on('click', function () {
            $(this).parent().siblings().find('span').show();
            $(this).hide();
            var type = $(this).attr("data-show");
            change(type);
            var text = $(this).text();
            $('#currentType').text(text);
        })

        function change(chartType) {
            var ctx = document.getElementById("chartArea").getContext("2d");

            if (window.myChart) {
                window.myChart.destroy();
            }
            var temp = $.extend(true, {}, config);
            temp.type = chartType;
            if (chartType == 'line' || chartType == 'bar') {
                var scale = {
                        yAxes: [{
                            ticks: {
                                beginAtZero: true
                            }
                        }]
                };
                temp.options.scales = scale;
            }
            if (chartType == 'line') {
                temp.data.datasets[0].fill = false;
            }
            window.myChart = new Chart(ctx, temp);
        }

    }
});

function currencyFormat(n, currency) {
    return currency + n.toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,");
}