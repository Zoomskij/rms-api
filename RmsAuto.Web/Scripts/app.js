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
    //self.d = ko.observable(item.name);
    //self.id = ko.observable(item.id);
    //self.expanded = ko.observable(false);
    //self.toggle = function (item) {
    //    self.expanded(!self.expanded());
    //};
    //self.linkLabel = ko.computed(function () {
    //    return self.expanded() ? "collapse" : "expand";
    //}, self);
}


function brand(brand) {
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
        "name": "bruce",
        "id": 1
    }, {
        "name": "greg",
        "id": 2
    }]
   
    var data = ko.utils.arrayMap(json, function (item) {
        return new Sample(item);
    });
    this.showMe = ko.observable(true);
    self.items = ko.observableArray(data);

    this.notify = function (str) {
        console.log(str)
        element = document.getElementById(str);
        if (element.style.display === "block") {
            element.style.display = "none";
        }
        else {
            element.style.display = "block";
        }
    }

    this.firstName = ko.observable("Test");


        GetBrands = function () {
            document.getElementById("GetBrands_loader").style.display = "block";

            var pathname = window.location.pathname; 
            var mainUrl = replaceString(pathname, '', window.location.href);

            article = document.getElementById('GetBrands_article').value;
            analogues = document.getElementById('GetBrands_analogues').value;
            var url = "/api/Articles/" + article + "/Brands";
            if (analogues !== "") {
                url += "?analogues=" + analogues + "";
            }

            $.ajax({
                url: url,
                method: "GET",
                dataType: "json",
                crossDomain: true,
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(data),
                cache: false,
                beforeSend: function (xhr) {
                    /* Authorization header */
                    xhr.setRequestHeader("Authorization", token);
                    xhr.setRequestHeader("X-Mobile", "false");
                },
                success: function (data) {
                    $('#GetBrands_resp').html("[\n");

                    for (var i = 0, j = data.length; i < j; i++) {
                        var brand = data[i];
                        $('#GetBrands_resp').append("  {\n");
                        $('#GetBrands_resp').append("    \"Name\":"); 
                        $('#GetBrands_resp').append(" \"" + brand.Name + "\"");

                        $('#GetBrands_resp').append("\n");
                        $('#GetBrands_resp').append("    \"Description\":");
                        $('#GetBrands_resp').append(" \"" + brand.Description + "\"");

                        $('#GetBrands_resp').append("\n  },\n");

                    }
                    $('#GetBrands_resp').append("]");


                    $('#GetBrands_curl').html("curl -X GET \"" + mainUrl + "" + url + "");

                    $('#GetBrands_request-url').html(mainUrl + url + "");
                    document.getElementById("GetBrands_loader").style.display = "none";

                },
                error: function (jqXHR, textStatus, errorThrown) {
                    if (errorThrown === "Unauthorized") {
                        $('#resp').html("{\n    \"Message\": \"Authorization has been denied for this request.\"\n}");
                    }
                    document.getElementById("GetBrands_loader").style.display = "none";
                }
            });

            //////////////////////////////////////

            GetSpareParts = function () {
                document.getElementById("GetSpareParts_loader").style.display = "block";

                var pathname = window.location.pathname;
                var mainUrl = replaceString(pathname, '', window.location.href);

                article = document.getElementById('GetSpareParts_article').value;
                brand = document.getElementById('GetSpareParts_brand').value;
                analogues = document.getElementById('GetSpareParts_analogues').value;
                var url = "/api/Articles/" + article + "/Brand/" + brand;
                if (analogues !== "") {
                    url += "?analogues=" + analogues + "";
                }

                $.ajax({
                    url: url,
                    method: "GET",
                    dataType: "json",
                    crossDomain: true,
                    contentType: "application/json; charset=utf-8",
                    data: JSON.stringify(data),
                    cache: false,
                    beforeSend: function (xhr) {
                        /* Authorization header */
                        xhr.setRequestHeader("Authorization", token);
                        xhr.setRequestHeader("X-Mobile", "false");
                    },
                    success: function (data) {
                        $('#GetSpareParts_resp').html("[\n");

                        for (var i = 0, j = data.length; i < j; i++) {
                            var sparePart = data[i];

                            $('#GetSpareParts_resp').append("  {\n");
                            $('#GetSpareParts_resp').append("    \"Article\":");
                            $('#GetSpareParts_resp').append(" \"" + sparePart.Article + "\"");

                            $('#GetSpareParts_resp').append("\n");
                            $('#GetSpareParts_resp').append("    \"Brand\":");
                            $('#GetSpareParts_resp').append(" \"" + sparePart.Brand + "\"");

                            $('#GetSpareParts_resp').append("\n");
                            $('#GetSpareParts_resp').append("    \"Count\":");
                            $('#GetSpareParts_resp').append(" \"" + sparePart.Count + "\"");

                            $('#GetSpareParts_resp').append("\n");
                            $('#GetSpareParts_resp').append("    \"DeliveryDaysMax\":");
                            $('#GetSpareParts_resp').append(" \"" + sparePart.DeliveryDaysMax + "\"");

                            $('#GetSpareParts_resp').append("\n");
                            $('#GetSpareParts_resp').append("    \"DeliveryDaysMin\":");
                            $('#GetSpareParts_resp').append(" \"" + sparePart.DeliveryDaysMin + "\"");

                            $('#GetSpareParts_resp').append("\n");
                            $('#GetSpareParts_resp').append("    \"DeliveryQuality\":");
                            $('#GetSpareParts_resp').append(" \"" + sparePart.DeliveryQuality + "\"");

                            $('#GetSpareParts_resp').append("\n");
                            $('#GetSpareParts_resp').append("    \"MinOrderQty\":");
                            $('#GetSpareParts_resp').append(" \"" + sparePart.MinOrderQty + "\"");

                            $('#GetSpareParts_resp').append("\n");
                            $('#GetSpareParts_resp').append("    \"Name\":");
                            $('#GetSpareParts_resp').append(" \"" + sparePart.Name + "\"");

                            $('#GetSpareParts_resp').append("\n");
                            $('#GetSpareParts_resp').append("    \"Price\":");
                            $('#GetSpareParts_resp').append(" \"" + sparePart.Price + "\"");

                            $('#GetSpareParts_resp').append("\n");
                            $('#GetSpareParts_resp').append("    \"SupplierID\":");
                            $('#GetSpareParts_resp').append(" \"" + sparePart.SupplierID + "\"");

                            $('#GetSpareParts_resp').append("\n");
                            $('#GetSpareParts_resp').append("    \"Type\":");
                            $('#GetSpareParts_resp').append(" \"" + sparePart.Type + "\"");

                            $('#GetSpareParts_resp').append("\n  },\n");




                            //$('#resp').append("\n");
                            //$('#resp').append("    \"Description\":");
                            //$('#resp').append(" \"" + brand.Description + "\"");
                            //$('#resp').append("\n  },\n");

                        }
                        $('#GetSpareParts_resp').append("]");


                        $('#GetSpareParts_curl').html("curl -X GET \"" + mainUrl + "" + url + "");

                        $('#GetSpareParts_request-url').html(mainUrl + url + "");
                        document.getElementById("GetSpareParts_loader").style.display = "none";

                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        if (errorThrown === "Unauthorized") {
                            $('#resp').html("{\n    \"Message\": \"Authorization has been denied for this request.\"\n}");
                        }
                        document.getElementById("GetSpareParts_loader").style.display = "none";
                    }
                });
            }


            //$.getJSON(url, function (data) {

            //    $('#resp').html("[\n");

            //    for (var i = 0, j = data.length; i < j; i++) {
            //        var brand = data[i];
            //        $('#resp').append("  {\n");
            //        $('#resp').append("    \"Name\":"); 
            //        $('#resp').append(" \"" + brand.Name + "\"");

            //        $('#resp').append("\n");
            //        $('#resp').append("    \"Description\":");
            //        $('#resp').append(" \"" + brand.Description + "\"");
            //        $('#resp').append("\n  },\n");

            //    }
            //    $('#resp').append("]");


            //    $('#curl').html("curl -X GET \"" + mainUrl + "" + url + "");

            //    $('#request-url').html(mainUrl + url + "");
            //    document.getElementById("loader").style.display = "none";
              

            //});


    }


};

ko.applyBindings(new viewModel());

function replaceString(oldS, newS, fullS) {
    for (var i = 0; i < fullS.length; ++i) {
        if (fullS.substring(i, i + oldS.length) === oldS) {
            fullS = fullS.substring(0, i) + newS + fullS.substring(i + oldS.length, fullS.length);
        }
    }
    return fullS;
}