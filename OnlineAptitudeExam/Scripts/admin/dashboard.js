
function DashboardIndex() {
    /*--------------------------------------------------- card 1----------------------------------------------------- */

    var labels = ['January', 'February', 'March', 'April', 'May', 'June']

    var data = {
        labels: labels,
        datasets: [
            {
                label: 'Access',
                backgroundColor: 'blue',
                borderColor: 'blue',
                data: [10, 27, 56, 34, 24, 53],
                tension: 0.4,
            },
            {
                label: 'Suber',
                backgroundColor: 'red',
                borderColor: 'red',
                data: [0, 34, 32, 23, 2, 82],
                tension: 0.4,
            },
            {
                label: 'Unsub',
                backgroundColor: 'yellow',
                borderColor: 'yellow',
                data: [0, 2, 6, 3, 2, 0],
                tension: 0.4,
            },
        ],
    }
    var config = {
        type: 'line',
        data: data,
    }

    var canvas = document.getElementById('scoreChart')
    var chart = new Chart(canvas, config)

    /*--------------------------------------------------- card 2----------------------------------------------------- */


    var userData = {
        labels: ["January", "February", "March", "April", "May", "June"],
        datasets: [
            {
                label: 'All',
                backgroundColor: 'blue',
                fillColor: 'blue',
                strokeColor: 'blue',
                data: [100, 100, 95, 90, 100, 98]
            },
            {
                label: 'Pass',
                backgroundColor: 'green',
                fillColor: 'green',
                strokeColor: 'green',
                data: [70, 75, 90, 85, 60, 67]
            }
        ]
    }
    var configUserChart = {
        type: 'bar',
        data: userData,
    }
    // get bar chart canvas
    var userChart = document.getElementById("user-chart").getContext("2d");
    // draw bar chart
    var user_chart = new Chart(userChart, configUserChart)
}