function Sample(item) {
    var self = this;
    self.name = ko.observable(item.name);
    self.id = ko.observable(item.id);
    self.expanded = ko.observable(false);
    self.toggle = function (item) {
        self.expanded(!self.expanded());
    };
    self.linkLabel = ko.computed(function () {
        return self.expanded() ? "collapse" : "expand";
    }, self);
}

function SampleMethod(method) {
    var self = this;
    self.Description = ko.observable(method.Description);
    self.Group = ko.observable(method.Group);
    self.Name = ko.observable(method.Name);
    self.Parameters = ko.observableArray(method.Parameters);
    self.TitleDescription = ko.observable(method.TitleDescription);
    self.Type = ko.observable(method.Type);
    self.Uri = ko.observable(method.Uri);
}


function brands(brand) {
    this.Name = ko.observable(brand.Name);
    this.Description = ko.observable(brand.Description);
}

var viewModel = function () {
    var self = this;

    //var model = jsonModel;
    var modelData = ko.utils.arrayMap(jsonModel, function (method) {
        return new SampleMethod(method);
    });
    self.methods = ko.observableArray(modelData);
    if (token === null) {
        token = "";
    }

    var isVisible = false;
    var json = [{

    }, {
    
    }]
   
    var data = ko.utils.arrayMap(json, function (item) {
        return new Sample(item);
    });
    this.showMe = ko.observable(true);
    self.items = ko.observableArray(data);

    this.notify = function (str) {
        changeVisible(document.getElementById(str));
    }

    this.tryIt = function (str) {
        article = document.getElementById(str + "_article");
        brand = document.getElementById(str + "_brand");
        orderId = document.getElementById(str + "_orderId");
        analogues = document.getElementById(str + "_analogues");
        execute = document.getElementById(str + "_execute");
        bTryit = document.getElementById(str + "_tryIt");

        changeVisible(article);
        changeVisible(brand);
        changeVisible(analogues);
        changeVisible(execute);
        changeVisible(orderId);

        if (bTryit !== null) {
            if (bTryit.innerHTML === 'Try it out') {
                bTryit.innerHTML = "Cancel";
            }
            else {
                bTryit.innerHTML = "Try it out";
            }
        }
    }

    function changeVisible(item) {
        if (item !== null) {
            if (item.style.display === 'none') {
                item.style.display = 'block'
            }
            else {
                item.style.display = 'none'
            }
        }
    }

    //Get current url
    var pathname = window.location.pathname;
    var mainUrl = '';
    if (pathname === '/') {
        mainUrl = window.location.href;
    }
    else {
        mainUrl = replaceString(pathname, '', window.location.href);
    }

    //Formating json for print
    function syntaxHighlight(json) {
        if (typeof json !== 'string') {
            json = JSON.stringify(json, undefined, 2);
        }
        json = json.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
        return json.replace(/("(\\u[a-zA-Z0-9]{4}|\\[^u]|[^\\"])*"(\s*:)?|\b(true|false|null)\b|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?)/g, function (match) {
            var cls = 'number';
            if (/^"/.test(match)) {
                if (/:$/.test(match)) {
                    cls = 'key';
                } else {
                    cls = 'string';
                }
            } else if (/true|false/.test(match)) {
                cls = 'boolean';
            } else if (/null/.test(match)) {
                cls = 'null';
            }
            return '<span class="' + cls + '">' + match + '</span>';
        });
    }

    // Generating request
    function Request(methodName, url) {
        var type = "GET";
        if (methodName === "CreateOrder")
            type = "POST"

        changeVisible(document.getElementById(methodName + "_loader"));
        var resp = $('#' + methodName + '_resp');
        var code = $('#' + methodName + '_code');
        var loader = $('#' + methodName + '_loader');
        var curl = $('#' + methodName + '_curl');
        var reqUrl = $('#' + methodName + '_request-url');
        var orders = document.getElementById('CreateOrder_orders').value;

        document.getElementById(methodName + '_divCurl').style.display = 'block';
        document.getElementById(methodName + '_divRequestUrl').style.display = 'block';

        curl.html("curl -X GET \"" + mainUrl + "" + url + "\"");
        curl.append(" -H \"accept: application/json\"");
        if (token !== null && token !== "") {
            if (methodName !== "GetPartners" && methodName !== "GetOrders") {
                curl.append(" -H \"authorization: " + token + "\"");
            }
        }
        reqUrl.html(mainUrl + url + "");

        $.ajax({
            url: url,
            method: type,
            dataType: "json",
            crossDomain: true,
            contentType: "application/json; charset=utf-8",
            data: orders,
            cache: false,
            beforeSend: function (xhr) {
                /* Authorization header */
                xhr.setRequestHeader("Authorization", token);
                xhr.setRequestHeader("X-Mobile", "false");
            },
            success: function (data) {
                var str = JSON.stringify(data, null, 2);
                var j = syntaxHighlight(str);
                resp.html(j);

                code.html("200");

                changeVisible(document.getElementById(methodName + "_loader"));

            },
            error: function (jqXHR, textStatus, errorThrown) {
                code.html(jqXHR.status);

                if (jqXHR.status === 401 || (jqXHR.status === 404)) {
                    resp.html("{\n    \"Message\": \"Authorization has been denied for this request.\"\n}");
                }
                if (jqXHR.status === 405) {
                    resp.html("{\n    \"Message\": \"Method not allowed.\"\n}");
                }
                if (jqXHR.status === 400) {
                    resp.html("{\n    \"Message\": " + jqXHR.responseJSON +"\n}");
                }
                changeVisible(document.getElementById(methodName + "_loader"));
            }
        });
    }

    function validation(input) {
        if (input.value === "") {
            input.classList.add("invalid");
            input.oninput = function () {
                input.classList.remove("invalid");
            };
            return false;
        }
        return true;
    }
    function AddEventListener(item, methodName) {
        item.addEventListener("keyup", function (event) {
            event.preventDefault();
            if (event.keyCode === 13) {
                document.getElementById(methodName + "_execute").click();
            }
        });
    }

    GetBrands = function () {
        article = document.getElementById("GetBrands_article");

        if (validation(article) === false) {
            return false;
        }

        analogues = document.getElementById("GetBrands_analogues");
        var url = "/api/Articles/" + article.value + "/Brands";
        if (analogues.value !== "") {
            url += "?analogues=" + analogues.value + "";
        }

        Request("GetBrands", url);
    }

    GetSpareParts = function () {
        article = document.getElementById("GetSpareParts_article");
        brand = document.getElementById("GetSpareParts_brand");
        var isArticle = validation(article);
        var isBrand = validation(brand);
        if (isArticle && isBrand === false) {
            return false;
        }

        analogues = document.getElementById("GetSpareParts_analogues");
        var url = "/api/Articles/" + article.value + "/Brand/" + brand.value;
        if (analogues.value !== "") {
            url += "?analogues=" + analogues.value + "";
        }

        Request("GetSpareParts", url);
    }

    GetOrders = function () {
        var url = "/api/orders";
        Request("GetOrders", url);
    }

    CreateOrder = function () {
        orderId = document.getElementById("CreateOrder_orders");
        var url = "/api/orders";
        var isOrderId = validation(orderId);
        if (isOrderId === false) {
            return false;
        }
        Request("CreateOrder", url);
    }

    GetOrder = function () {
        orderId = document.getElementById("GetOrder_orderId");
        var url = "/api/orders/" + orderId.value;
        var isOrderId = validation(orderId);
        if (isOrderId === false) {
            return false;
        }
        Request("GetOrder", url);
    }
            
    GetPartners = function () {
        var url = "/api/Partners/";
        Request("GetPartners", url);
    }


    var modelsHeader = document.getElementById("models-header");
    modelsHeader.onclick = function () {
        var allModels = document.getElementById("all-models");
        changeVisible(allModels);
    };


    //BODY
    delete orderModel.CompletedDate;
    delete orderModel.OrderDate;
    delete orderModel.OrderId;
    delete orderModel.Status;
    delete orderModel.Username;
    delete orderModel.Total;
    var odJson = JSON.stringify(orderModel, null, 2);
    document.getElementById('CreateOrder_orders').value = odJson;
}

ko.applyBindings(new viewModel());

function replaceString(oldS, newS, fullS) {
    for (var i = 0; i < fullS.length; ++i) {
        if (fullS.substring(i, i + oldS.length) === oldS) {
            fullS = fullS.substring(0, i) + newS + fullS.substring(i + oldS.length, fullS.length);
        }
    }
    return fullS;
}