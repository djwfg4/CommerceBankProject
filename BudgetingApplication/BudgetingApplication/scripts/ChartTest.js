$(document).ready(function () {
    var GetDonutAndBarGraphDataURL
        = '/Home/GetDonutAndBarGraphData';
    var GetCalendarDataURL
        = '/Home/GetCalendarData';
    // page is now ready, initialize the calendar...

    $('#calendar').fullCalendar({
        header: {
            left: 'title',
            center: '',
            right: 'prev,next'
        }
        // put your options and callbacks here
    });

    

    var data1, labels;
    var monthNames = ["January", "February", "March", "April", "May", "June",
  "July", "August", "September", "October", "November", "December"
    ];
    //var labelIndex = ["Burger King",
    //    "Chipotle",
    //    "Doctor",
    //    "Home Depot",
    //    "HyVee",
    //    "KCPL",
    //    "StarBucks"];

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
                       position: 'right',
                   },
                   title: {
                       display: true,
                       text: monthNames[month] + '\'s Spending'
                   },
                   animation: {
                       animateScale: true,
                       animateRotate: true
                   },
                   hover: {
                       //onHover: function (event, array) {
                       //    unHighlightDays();
                       //    if (array.length != 0) {
                       //        category = array[0]._chart.config.data.labels[array[0]._index];
                       //        dates = labels[category];
                       //        //highlightDays(dates, array[0]._view.backgroundColor);
                       //    }
                       //}
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
                   },
                   hover: {
                       //onHover: function (event, array) {
                       //    unHighlightDays();
                       //    if (array.length != 0) {
                       //        category = array[0]._chart.config.data.labels[array[0]._index];
                       //        dates = labels[category];
                       //        //highlightDays(dates, array[0]._view.backgroundColor);
                       //    }
                       //}
                   }
               }
           };
           ctx = $("#chart-area")[0];
           ctx1 = $("#graph-chart")[0];
           window.myDoughnut = new Chart(ctx, config);
           window.myBar = new Chart(ctx1, barGraphConfig);
       },
       error: function () {
           alert("Transactions 2 data request failed");
       }
   });

  /*  $.getJSON("../../Data/testDataForCalendar.json", function (json) {
        labels = json;
    });*/
    $.getJSON("../../Data/testDataForDonut.json", function (json) {
        
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
        $("td[data-date='" + item + "']").addClass("state-highlight");
    });
    $(".state-highlight").css("background-color", color);
}
function unHighlightDays() {
    $(".state-highlight").removeAttr("style");
    $(".state-highlight").removeClass("state-highlight");
}