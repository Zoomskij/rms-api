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
        }
    }
          
})