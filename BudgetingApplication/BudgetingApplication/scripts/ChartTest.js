
$(document).ready(function () {

    // page is now ready, initialize the calendar...

    $('#calendar').fullCalendar({
        header: {
            left: 'title',
            center: '',
            right: 'prev,next'
        }
        // put your options and callbacks here
    })
    var data1, labels;
    var labelIndex = ["Burger King",
        "Chipotle",
        "Doctor",
        "Home Depot",
        "HyVee",
        "KCPL",
        "StarBucks"];
    $.getJSON("../../Data/testDataForCalendar.json", function(json){
        labels = json;
    });
    $.getJSON("../../Data/testDataForDonut.json", function (json) {
        config = {
            type: 'doughnut',
            data: json,
            options: {
                responsive: true,
                legend: {
                    position: 'right',
                },
                title: {
                    display: true,
                    text: 'Chart.js Doughnut Chart Example'
                },
                animation: {
                    animateScale: true,
                    animateRotate: true
                },
                hover: {
                    onHover: function (event, array) {
                        unHighlightDays();
                        if(array.length != 0){
                            category = labelIndex[array[0]._index];
                            dates = labels[category];
                            highlightDays(dates, array[0]._view.backgroundColor);
                        } 
                    }
                }
            }
        };
    ctx = document.getElementById("chart-area").getContext("2d");
    window.myDoughnut = new Chart(ctx, config);
    });
});

window.chartColors = {
    red: 'rgb(255, 99, 132)',
    orange: 'rgb(255, 159, 64)',
    yellow: 'rgb(255, 205, 86)',
    green: 'rgb(75, 192, 192)',
    blue: 'rgb(54, 162, 235)',
    purple: 'rgb(153, 102, 255)',
    grey: 'rgb(231,233,237)'
};

window.randomScalingFactor = function () {
    return (Math.random() > 0.5 ? 1.0 : -1.0) * Math.round(Math.random() * 100);
}
var randomScalingFactor = function () {
    return Math.round(Math.random() * 100);
};


function highlightDays(dateArray, color) {
    $.each(dateArray, function (index, item) {
       // formattedDate = moment(item).format('YYYY-MM-DD');
        $("td[data-date='"+item+"']").addClass("state-highlight");
    });
    $(".state-highlight").css("background-color", color);
}
function unHighlightDays() {
    $(".state-highlight").removeAttr("style");
    $(".state-highlight").removeClass("state-highlight");
}
