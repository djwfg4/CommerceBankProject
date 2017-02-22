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
var config = {
    type: 'doughnut',
    data: {
        datasets: [{
            data: [
              randomScalingFactor(),
              randomScalingFactor(),
              randomScalingFactor(),
              randomScalingFactor(),
              randomScalingFactor(),
            ],
            backgroundColor: [
              window.chartColors.red,
              window.chartColors.orange,
              window.chartColors.yellow,
              window.chartColors.green,
              window.chartColors.blue,
            ],
            label: 'Dataset 1'
        }],
        labels: [
          "Red",
          "Orange",
          "Yellow",
          "Green",
          "Blue"
        ]
    },
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
        }
    }
};
var ctx;
var chart;
window.onload = function () {
    ctx = document.getElementById("chart-area").getContext("2d");
    window.myDoughnut = new Chart(ctx, config);
    chart = new Chart(ctx, config);
};

document.getElementById("chart-area").onclick = function (evt) {
    var activePoints = chart.getElementsAtEvent(evt);

    if (activePoints.length > 0) {
        //get the internal index of slice in pie chart
        var clickedElementindex = activePoints[0]["_index"];

        //get specific label by index 
        var label = chart.data.labels[clickedElementindex];

        //get value by index      
        var value = chart.data.datasets[0].data[clickedElementindex];

        div = document.getElementById("data-show");
        txt = document.createTextNode(label + " - " + value);
        div.innerText = txt.textContent;

        /* other stuff that requires slice's label and value */
    }
}

/* Button code */
document.getElementById('randomizeData').addEventListener('click', function () {
    config.data.datasets.forEach(function (dataset) {
        dataset.data = dataset.data.map(function () {
            return randomScalingFactor();
        });
    });
    chart.update();
});
var colorNames = Object.keys(window.chartColors);
document.getElementById('addDataset').addEventListener('click', function () {
    var newDataset = {
        backgroundColor: [],
        data: [],
        label: 'New dataset ' + config.data.datasets.length,
    };
    for (var index = 0; index < config.data.labels.length; ++index) {
        newDataset.data.push(randomScalingFactor());
        var colorName = colorNames[index % colorNames.length];;
        var newColor = window.chartColors[colorName];
        newDataset.backgroundColor.push(newColor);
    }
    config.data.datasets.push(newDataset);
    chart.update();
});
document.getElementById('addData').addEventListener('click', function () {
    if (config.data.datasets.length > 0) {
        config.data.labels.push('data #' + config.data.labels.length);
        var colorName = colorNames[config.data.datasets[0].data.length % colorNames.length];;
        var newColor = window.chartColors[colorName];
        config.data.datasets.forEach(function (dataset) {
            dataset.data.push(randomScalingFactor());
            dataset.backgroundColor.push(newColor);
        });
        chart.update();
    }
});
document.getElementById('removeDataset').addEventListener('click', function () {
    config.data.datasets.splice(0, 1);
    chart.update();
});
document.getElementById('removeData').addEventListener('click', function () {
    config.data.labels.splice(-1, 1); // remove the label first
    config.data.datasets.forEach(function (dataset) {
        dataset.data.pop();
        dataset.backgroundColor.pop();
    });
    chart.update();
});
