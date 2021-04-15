var Header = {

    Render: function (jqueryObj) {
        var parent = this;
        jqueryObj.load("/html/header.html?" + Version,
            function (responseTxt, statusTxt, xhr) {

            });
    }

}