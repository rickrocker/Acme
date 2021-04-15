var About = {

    Render: function (jqueryObj) {
        var parent = this;
        jqueryObj.load("/html/about.html?" + Version);
    },

}