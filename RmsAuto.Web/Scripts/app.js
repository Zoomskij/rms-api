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

        Get_Brands = function () {
            document.getElementById("loader").style.display = "block";

            var pathname = window.location.pathname; 
            var mainUrl = replaceString(pathname, '', window.location.href);

            article = document.getElementById('article').value;
            analogues = document.getElementById('analogues').value;
            var url = "/api/Articles/" + article + "/Brands";
            if (analogues !== "") {
                url += "?analogues=" + analogues + "";
            }
            $.getJSON(url, function (data) {

                $('#resp').html("[\n");

                for (var i = 0, j = data.length; i < j; i++) {
                    var brand = data[i];
                    $('#resp').append("  {\n");
                    $('#resp').append("    \"Name\":"); 
                    $('#resp').append(" \"" + brand.Name + "\"");

                    $('#resp').append("\n");
                    $('#resp').append("    \"Description\":");
                    $('#resp').append(" \"" + brand.Description + "\"");
                    $('#resp').append("\n  },\n");

                }
                $('#resp').append("]");


                $('#curl').html("curl -X GET \"" + mainUrl + "" + url + "");

                $('#request-url').html(mainUrl + url + "");
                document.getElementById("loader").style.display = "none";
              
            });
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