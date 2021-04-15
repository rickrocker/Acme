var Home = {

    Render: function (jqueryObj) {
        var parent = this;
        jqueryObj.load("/html/home.html?" + Version,
            function (responseTxt, statusTxt, xhr) {

            });
    }

}