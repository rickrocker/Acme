var Page = {
    Load: function (page, queryString) {

        //this.logPage(page);

        switch (page.toLowerCase()) {
            case "about":
                this.loadAboutPage();
                break;
            case "support":
                this.loadSupportPage();
                break;
            case "contact":
                this.loadContactPage();
                break;
            case "home":
                this.loadHomePage();
                break;
            case "register":
                this.loadWebUIApp();
                break;
            case "signin":
                this.loadWebUIApp();
                break;
        }

    },

    loadHomePage: function () {
        Home.Render($("#divMain"));
        this.changeURL("", "Home", "/");
    },

    loadInfluencersPage: function () {
        this.changeURL("Influencers", "Influencers", "/influencers");
    },

    loadAboutPage: function () {
        About.Render($("#divMain"));
        this.changeURL("About", "About Us", "/about");
    },

    loadSupportPage: function () {
        Support.Render($("#divMain"));
        this.changeURL("Support", "Support", "/support");
    },

    loadContactPage: function () {
        Contact.Render($("#divMain"));
        this.changeURL("Contact", "Contact", "/contact");
    },

    loadWebUIApp: function () {
        window.open(WebAppUrl, "_self")
    },




    changeURL: function (html, pageTitle, urlPath) {
        if (pageTitle != null) {
            //$('#divHeaderTitle').html(TitleCase(pageTitle));
            $('#divHeaderTitle').html(pageTitle);
        }
        window.history.pushState({ "html": html, "pageTitle": pageTitle }, "", urlPath);
    },

    logPage: function (page) {
        if (page != null && page.includes(".")) {
            page = page.substring(0, page.indexOf("."));
        }
        $.ajax({
            url: WebApiUrl + '/api/Users/LogPage/' + page,
            type: 'POST',
            // dataType: 'json',
            //success: function (result) {
            //    console.log("success");
            //},
            error: function (request, message, error) {
                console.log("Error: " + error);
                LogError(error, false);
            }
        });
    }

}



