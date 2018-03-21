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
        if (element.style.display == "block") {
            element.style.display = "none";
        }
        else {
            element.style.display = "block";
        }
    }

    this.firstName = ko.observable("Test");

    this.fullName = ko.computed(function () {
        var data = { "article": "333310", "analogues": "1", "region": "rmsauto" };
        var ddd = "test";
        $.ajax({
            url: "/api/Articles/333310/Brands",
            type: "GET",
            contentType: "application/json",
            success: function (data) {
                return "aaaa";
            },
            error: function (xhr) {
                console.log(xhr);
            }
        });
        return ddd;
    }, this);

    this.capitalizeLastName = function () {
        var data = { "article": "333310", "analogues": "1", "region": "rmsauto" };

        $.getJSON("/api/Articles/333310/Brands", function (data) {

            var currentVal = this.firstName;        // Read the current value
         //   this.lastName(currentVal.toUpperCase()); // Write back a modified value

            var a = data;


            viewModel.firstName = ko.observable("adsf");
            // Now use this data to update your view models, 
            // and Knockout will update your UI automatically 
        })

    }
        loadBrands = function () {
            $.getJSON("/api/Articles/333310/Brands", function (data) {

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

               
              
            });
    }
};


ko.applyBindings(new viewModel());


function executeMethod() {
    var data = { "article": "333310", "analogues": "1", "region": "rmsauto" };
    $.ajax({
        url: "/api/Articles/333310/Brands",
        type: "GET",
        contentType: "application/json",
        success: function (data) {
            var b = data;
            this.firstName = "adsf";
        },
        error: function (xhr) {
            console.log(xhr);
        }
    });
}