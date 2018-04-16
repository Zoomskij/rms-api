var viewModel = function () {
    var self = this;

    if (token === null) {
        token = "";
    }

    this.notify = function (str) {
        changeVisible(document.getElementById(str));
    }

    var modelsHeader = document.getElementById("models-header");
    modelsHeader.onclick = function () {
        var allModels = document.getElementById("all-models");

        if (allModels !== null) {
            if (allModels.style.display === 'none') {
                allModels.style.display = 'block'
            }
            else {
                allModels.style.display = 'none'
            }
        }
    };

    var purchaseOrderNo = document.cookie;
    //BODY
    delete orderModel.CompletedDate;
    delete orderModel.OrderDate;
    delete orderModel.OrderId;
    delete orderModel.Status;
    delete orderModel.Username;
    delete orderModel.Total;
    var odJson = JSON.stringify(orderModel, null, 2);

    document.getElementById('CreateOrder_orders').value = odJson;
    var a = jsonModel[0].Parameters;

    for (var i = 0; i < jsonModel.length; i++) {
        var parameters = jsonModel[i].Response;
        for (var j = 0; j < Object.keys(parameters).length; j++) {
            var key = Object.keys(parameters)[j];
            var value = Object.values(parameters)[j];

        }
        var jsonResponse = JSON.stringify(jsonModel[i].Response, null, 2);
        jsonResponse = jsonResponse.replace("/Date(-62135596800000)/", "0000-00-00T00:00:00.00");
        document.getElementById(jsonModel[i].Name + '_resp').innerText = jsonResponse;
    }

}

ko.applyBindings(new viewModel());

function getCookie(name) {
    var matches = document.cookie.match(new RegExp(
        "(?:^|; )" + name.replace(/([\.$?*|{}\(\)\[\]\\\/\+^])/g, '\\$1') + "=([^;]*)"
    ));
    return matches ? decodeURIComponent(matches[1]) : undefined;
}
