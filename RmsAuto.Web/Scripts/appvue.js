'use strict';

// import VuePreload from './vue-preload.js'

//Vue.use(VuePreload)

//Vue.use(VuePreload, {
//    showProgress: true,
//    onStart() {
//        console.log('start')
//    },
//    onEnd() {
//        console.log('end')
//    }
//})


var app = new Vue({
    el: '#app',
    mounted: function () {
        this.loaded()
    },
    data: {
        jsonModel,
        isArticleGroup: false,
        isOrderGroup: false,
        isPartnerGroup: false,
        lastGroup: "",
        loading: false
    },
    methods: {
        ready: function () {
            this.loading = true;
            // GET request
            this.$http({ url: '/', method: 'GET' }).then(function (response) {
                // success callback
                this.loading = false;
            }, function (response) {
                // error callback
            });
        },

        IsAccess: function (allowAnonymous, methodPermissions) {
            if (allowAnonymous === true) return true;
            for (var i = 0; i < methodPermissions.length; i++) {
                for (var j = 0; j < userPermissions.length; j++) {
                    var a = methodPermissions[i].ID;
                    if (methodPermissions[i].ID == userPermissions[j]) {
                        return true;
                    }  
                }
            }
            return false;
        },

        loaded: function () {
            this.isArticleGroup = false,
            this.isOrderGroup = false,
            this.isPartnerGroup = false
        },

        ShowModel: function (str) {
            this.changeVisible(str);
            this.IsCollapsed(str);
        },
        IsCollapsed: function (str) {
            var item = document.getElementById(str + "_arrow");
            var toggles = document.getElementById(str + "_toggles");
            if (item.classList.contains("rotate")) {
                item.classList.remove("rotate")
                toggles.style.display = 'initial';
            } else {
                item.classList.add("rotate");
                toggles.style.display = 'none';
            }
        },

        changeVisible: function (str) {
            var item = document.getElementById(str);
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
            this.changeVisible(str + "_OrderHead");

            var bTryit = document.getElementById(str + "_tryIt");
            if (bTryit !== null) {
                if (bTryit.innerHTML === 'Try it out') {
                    bTryit.innerHTML = "Cancel";
                    bTryit.classList.add('cancel');
                }
                else {
                    bTryit.innerHTML = "Try it out";
                    bTryit.classList.remove('cancel');
                }
            }
        },

        syntaxHighlight: function (json) {
            if(typeof json !== 'string') {
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
        },

        replaceString: function (oldS, newS, fullS) {
            for(var i = 0; i<fullS.length; ++i) {
                if (fullS.substring(i, i + oldS.length) === oldS) {
                    fullS = fullS.substring(0, i) + newS + fullS.substring(i + oldS.length, fullS.length);
                }
            }
            return fullS;
        },

        escapeSpecialChars: function (str) {
            return str.replace(/\\n/g, "")
                .replace(/\\'/g, "\\'")
                .replace(/\\"/g, '\\"')
                .replace(/\\&/g, "\\&")
                .replace(/\\r/g, "\\r")
                .replace(/\\t/g, " ")
                .replace(/\\b/g, "\\b")
                .replace(/\\f/g, "\\f");
        },

        // Generating request
        request: function (methodName, methodType, url) {
            var mainUrl = window.location.origin;

            var data = null;
            this.changeVisible(methodName + "_loader");
            var resp = $('#' + methodName + '_resp');
            var code = $('#' + methodName + '_code');
            var loader = $('#' + methodName + '_loader');
            var curl = $('#' + methodName + '_curl');
            var reqUrl = $('#' + methodName + '_request-url');


            document.getElementById(methodName + '_divCurl').style.display = 'block';
            document.getElementById(methodName + '_divRequestUrl').style.display = 'block';

            curl.html("curl -X " + methodType + " \"" + mainUrl + "" + url + "\"");
            curl.append(" -H \"accept: application/json\"");
            if(token !== "") {
                if (methodName !== "GetPartners" && methodName !== "GetOrders") {
                    curl.append(" -H \"authorization: " + token + "\"");
                }
            }
            if (methodType === "POST") {
                data = document.getElementById('CreateOrder_OrderHead').value;
                var str = JSON.stringify(data, null, 0);
                var myEscapedJSONString = this.escapeSpecialChars(str);
                var trimStr = myEscapedJSONString.replace(/\s+/g, ' ').trim();

                curl.append(" -d " + trimStr + "");
            }


            reqUrl.html(mainUrl + url + "");

            $.ajax({
                url: url,
                method: methodType,
                crossDomain: true,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: data,
                cache: false,
                beforeSend: function (xhr) {
                    if (token !== "") {
                        /* Authorization header */
                        xhr.setRequestHeader("Authorization", token);
                        xhr.setRequestHeader("X-Mobile", "false");
                    }
                },
                success: function (data) {
                    if (methodName === "CreateOrder") {
                        delete data.Reaction;
                        delete data.Username;
                        delete data.OrderDate;
                        delete data.CompletedDate;
                        delete data.OrderName;
                    }

                    var str = JSON.stringify(data, null, 2);
                    //var j = this.syntaxHighlight(str);
                    if (typeof str !== 'string') {
                        str = JSON.stringify(str, undefined, 2);
                    }

                    resp.html(str);

                    code.html("200");

                    document.getElementById(methodName + "_loader").style.display = 'none';
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    code.html(jqXHR.status);

                    if (jqXHR.status === 401) {
                        resp.html("{\n    \"Message\": \"Authorization has been denied for this request.\"\n}");
                    }

                    if (jqXHR.status === 404 && jqXHR.responseJSON === "Error Not Found") {
                        resp.html("{\n    \"Message\": \"" + jqXHR.responseJSON + "\"\n}");
                    }

                    if (jqXHR.status === 404 && jqXHR.statusText === "Not Found" && jqXHR.responseJSON !== "Error Not Found") {
                        resp.html("{\n    \"Message\": \"" + "Access Denied" + "\"\n}");
                    }

                    if (jqXHR.status === 405) {
                        resp.html("{\n    \"Message\": \"Method not allowed.\"\n}");
                    }
                    if (jqXHR.status === 400) {
                        resp.html("{\n    \"Message\": \"" + jqXHR.responseJSON.MessageDetail + "\"\n}");
                    }
                    if (jqXHR.status === 500) {
                        resp.html("{\n    \"Message\":  \"Internal Server Error or Unauthorized Error\"\n}");
                    }

                    document.getElementById(methodName + "_loader").style.display = 'none';
                }
            });
        },

        validation: function (input) {
            if(input.value === "") {
                input.classList.add("invalid");
                input.oninput = function () {
                    input.classList.remove("invalid");
                };
                return false;
            }
            return true;
        },

        select: function (event) {
            if (event === "GetBrands") { this.GetBrands(); }
            if (event === "GetSpareParts") { this.GetSpareParts(); }
            if (event === "GetOrders") { this.GetOrders(); }
            if (event === "CreateOrder") { this.CreateOrder(); }
            if (event === "GetOrder") { this.GetOrder(); }
            if (event === "GetPartners") { this.GetPartners(); }
        },

        GetBrands: function () {
            var article = document.getElementById("GetBrands_article");

            if (this.validation(article) === false) {
                return false;
            }

            var analogues = document.getElementById("GetBrands_analogues");
            var url = "/api/Articles/" + article.value + "/Brands";
            if (analogues.value !== "") {
                url += "?analogues=" + analogues.value + "";
            }

            this.request("GetBrands", "GET", url);
        },

        GetSpareParts: function () {
            var article = document.getElementById("GetSpareParts_article");
            var brand = document.getElementById("GetSpareParts_brand");
            var isArticle = this.validation(article);
            var isBrand = this.validation(brand);
            if (isArticle && isBrand === false) {
                return false;
            }

            var analogues = document.getElementById("GetSpareParts_analogues");
            var url = "/api/Articles/" + article.value + "/Brand/" + brand.value;
            if (analogues.value !== "") {
                url += "?analogues=" + analogues.value + "";
            }

            this.request("GetSpareParts", "GET", url);
        },

        GetOrders: function () {
            var url = "/api/orders";
            this.request("GetOrders", "GET", url);
        },

        CreateOrder: function () {
            var orderId = document.getElementById("CreateOrder_OrderHead");
            var url = "/api/orders";
            var isOrderId = this.validation(orderId);
            if (isOrderId === false) {
                return false;
            }
            this.request("CreateOrder", "POST", url);
        },

        GetOrder: function () {
            var orderId = document.getElementById("GetOrder_orderId");
            var url = "/api/orders/" + orderId.value;
            var isOrderId = this.validation(orderId);
            if (isOrderId === false) {
                return false;
            }
            this.request("GetOrder", "GET", url);
        },

        GetPartners: function () {
            var url = "/api/Partners/";
            this.request("GetPartners", "GET", url);
        }
    }
          
})