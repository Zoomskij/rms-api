var app = new Vue({

    //var a = jsonModel;
    el: '#app',
    data: {
        jsonModel,
        isActive: true,
        counter: 0
    },
    methods: {
        changeVisible: function (name) {
            var item = document.getElementById(name);
            if (item !== null) {
                if (item.style.display === 'none') {
                    item.style.display = 'block'
                }
                else {
                    item.style.display = 'none'
                }
            }
        },
        tryIt: function (str) {
            this.changeVisible(str + "_article");
            this.changeVisible(str + "_brand");
            this.changeVisible(str + "_analogues");
            this.changeVisible(str + "_execute");
            this.changeVisible(str + "_orderId");

            var bTryit = document.getElementById(str + "_tryIt");
            if (bTryit !== null) {
                if (bTryit.innerHTML === 'Try it out') {
                    bTryit.innerHTML = "Cancel";
                }
                else {
                    bTryit.innerHTML = "Try it out";
                }
            }
        }
    }
          
})