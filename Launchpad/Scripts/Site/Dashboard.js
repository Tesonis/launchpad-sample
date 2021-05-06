
 //Chart1
    
    //Summary Pie Chart - All brands, all accounts total

    var total = summarytotalsku();
        var config = {
        type: 'pie',
            data: {
        datasets: [{
        data: [
        $(".summarydata").eq(0).text(),
        $(".summarydata").eq(1).text(),
        $(".summarydata").eq(2).text(),
        $(".summarydata").eq(3).text(),
        $(".summarydata").eq(4).text(),
        $(".summarydata").eq(5).text()
    ],
    backgroundColor: [
        "#000000",
        "#CACFD2",
        "#85C1E9",
        "#3498DB",
        "#A9DFBF",//green
        "#F1948A",//red
    ]
}],
labels: [
    'Discussion Required',
    'Ready to Present',
    'Initiated',
    'Pending',
    "Accepted",
    "Rejected"
]
},
            options: {
        responsive: true
}
};

        window.onload = function () {

    };

        //SKU Timeline
        var barChartDataSKU = {
            labels: ['Amazon', 'Walmart', 'Costco', 'Loblaws/Nofrills','Loblaws/Fortinos', 'Sobeys', 'Metro'],
            datasets: [{
            label: 'Discussion Required',
        backgroundColor: "#000000",
        data: [
            $(".sku-status0").eq(0).text(),
            $(".sku-status0").eq(1).text(),
            $(".sku-status0").eq(2).text(),
            $(".sku-status0").eq(3).text(),
            $(".sku-status0").eq(4).text(),
            $(".sku-status0").eq(5).text(),
            $(".sku-status0").eq(6).text()
        ]
            }, {
            label: 'Ready to Present',
        backgroundColor: "#ffc107",
        data: [
            $(".sku-status2").eq(0).text(),
            $(".sku-status2").eq(1).text(),
            $(".sku-status2").eq(2).text(),
            $(".sku-status2").eq(3).text(),
            $(".sku-status2").eq(4).text(),
            $(".sku-status2").eq(5).text(),
            $(".sku-status2").eq(6).text()
        ]
            }, {
            label: 'Initiated',
        backgroundColor: "#85C1E9",
        data: [
            $(".sku-status3").eq(0).text(),
            $(".sku-status3").eq(1).text(),
            $(".sku-status3").eq(2).text(),
            $(".sku-status3").eq(3).text(),
            $(".sku-status3").eq(4).text(),
            $(".sku-status3").eq(5).text(),
            $(".sku-status3").eq(6).text()
        ]
            }, {
            label: 'Pending',
        backgroundColor: "#3498DB",
        data: [
            $(".sku-status4").eq(0).text(),
            $(".sku-status4").eq(1).text(),
            $(".sku-status4").eq(2).text(),
            $(".sku-status4").eq(3).text(),
            $(".sku-status4").eq(4).text(),
            $(".sku-status4").eq(5).text(),
            $(".sku-status4").eq(6).text()
        ]
        }],
            options: {
            responsive: true
    }

};


        //Group by Account Chart - 1 selected brand = 1 instance
        var barChartData = {

            labels: ['Walmart', 'Costco West', 'Metro Ontario', 'Loblaw - NATIONAL', 'Sobeys', 'Metro'],
            datasets: [{
            label: 'Ready to Present',
        backgroundColor: "#CACFD2",
        data: [
            $(".acc-status1").eq(0).text(),
            $(".acc-status1").eq(1).text(),
            $(".acc-status1").eq(2).text(),
            $(".acc-status1").eq(3).text(),
            $(".acc-status1").eq(4).text(),
            $(".acc-status1").eq(5).text()
        ]
            }, {
            label: 'Initiated',
        backgroundColor: "#85C1E9",
        data: [
            $(".acc-status2").eq(0).text(),
            $(".acc-status2").eq(1).text(),
            $(".acc-status2").eq(2).text(),
            $(".acc-status2").eq(3).text(),
            $(".acc-status2").eq(4).text(),
            $(".acc-status2").eq(5).text()
        ]
            }, {
            label: 'Pending',
        backgroundColor: "#3498DB",
        data: [
            $(".acc-status3").eq(0).text(),
            $(".acc-status3").eq(1).text(),
            $(".acc-status3").eq(2).text(),
            $(".acc-status3").eq(3).text(),
            $(".acc-status3").eq(4).text(),
            $(".acc-status3").eq(5).text()
        ]
        }],
            options: {
            responsive: true
    }

};
function summarytotalsku() {
    var sum = 0;
    $('.summarydata').each(function () {
        sum += parseFloat($(this).text());
    });
    return sum;
}