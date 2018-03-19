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

var viewModel = function () {
    var self = this;
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
    console.log(self.items());

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

};


ko.applyBindings(new viewModel());