var Footer = {

    Render: function (jqueryObj) {
        var parent = this;
        jqueryObj.load("/html/footer.html?" + Version,
            function (responseTxt, statusTxt, xhr) {

            });
    }

}