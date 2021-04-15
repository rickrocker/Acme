var Page = {
    Load: function (page, queryString) {
        //console.log("load page '" + page + "'");
        this.logPage(page);
        if (AccessToken != null && AccessToken != "") {
            switch (page.toLowerCase()) {
                case "home":
                    Nav.ShowNav();
                    this.loadHomePage();
                    break;
                case "tasks":
                    Nav.ShowNav();
                    this.loadTasksPage();
                    break;
                case "profile":
                    Nav.ShowNav();
                    this.loadProfilePage();
                    break;
                case "notifications":
                    Nav.ShowNav();
                    this.loadNotificationsPage();
                    break;
                case "campaigns":
                    Nav.ShowNav();
                    this.loadCampaignsPage();
                    break;
                case "campaignnew":
                    Nav.ShowNav();
                    this.loadCampaignNewPage();
                    break;
                case "campaigndetails":
                    Nav.ShowNav();
                    this.loadCampaigDetailsPage(queryString);
                    break;
                case "campaigninfluencers":
                    Nav.ShowNav();
                    this.loadCampaignInfluencersPage();
                    break;
                case "campaignaddinfluencers":
                    Nav.ShowNav();
                    this.loadCampaignAddInfluencersPage();
                    break;
                case "influencerapplication":
                    Nav.HideNav();
                    this.loadInfluencerProfileWizard();
                    break;
                case "register":
                    Register.Render($("#divMain"));
                    break;
                case "resetpassword":
                    this.loadResetPassword();
                    break;

            }
        } else { //no accessToken -> either register, reset password or login
            switch (page.toLowerCase()) {
                case "register":
                    Register.Render($("#divMain"));
                    break;
                case "resetpassword":
                    this.loadResetPassword();
                    break;
                case "home":
                    this.changeURL("", "Home", "/");
                    Login.Render(page);
                    break;
                default:
                    Login.Render(page);
                    break;
            }

        }
    },

    loadHomePage: function () {
        this.changeURL("", "Home", "/");
    },

    loadProfilePage: function () {
        //$("#divMain").empty();
        //User Profile card
        $("#divMain").html("<div id='divCardUserProfile' class='card card-2-col-layout'>\
            <div class='card-title'>My Info</div>\
            <div id='divCardAvatarUpload' class='card-col-2'></div>\
            <div id='divCardUserProfilePlaceholder' class='card-col-1'></div >\
              </div>");
        AvatarUpload.Render($("#divCardAvatarUpload"));
        UserProfileObj.Render($("#divCardUserProfilePlaceholder"));


        //Influencer Profile Card
        if (UserProfile.Role == "Influencer" || UserProfile.Role == "Influencer Invitee") {
            $("#divMain").append("<div id='divCardInfluencerProfile' class='card'>\
                <div class='card-title'>My Profile</div>\
                <div id='divInfluencerProfilePlaceHolder'></div>\
              </div>");
            InfluencerProfile.Render($("#divInfluencerProfilePlaceHolder"));

            //Advertiser Card
        } else if (UserProfile.Role == "Advertiser") {
            $("#divMain").append("<div id='divCardCompanyProfile' class='card'>\
                <div class='card-title'>Company Profile</div>\
                <div id='divCardAdvertiserPlaceHolder'></div>\
              </div>");
            CompanyProfile.Render($("#divCardAdvertiserPlaceHolder"));
        }


        this.changeURL("Profile", "Profile", "/profile");

    },

    loadNotificationsPage: function () {
        
    },

    loadTasksPage: function () {
        $("#divMain").html("<div id='divCardTasks' class='card'>\
            <div id='divCardTitle' class='card-title'>Tasks</div>\
            <div id='divCardTasksPlaceHolder'></div>\
            </div>");
        Tasks.Render($("#divCardTasksPlaceHolder"));
        this.changeURL("Tasks", "Tasks", "/tasks");
    },

    loadCampaignsPage: function () {
        $("#divMain").html("<div id='divCardCampaigns' class='card'>\
            <div class='card-title'>Campaigns</div>\
            <div id='divCardCampaignsPlaceHolder'></div>\
            </div>");
        Campaigns.Render($("#divCardCampaignsPlaceHolder"));
        this.changeURL("Campaigns", "Campaigns", "/campaigns");
    },

    loadCampaignNewPage: function () {
        $("#divMain").html("<div id='divCardCampaignNew' class='card'>\
            <div class='card-title'>New Campaign</div>\
            <div id='divCardCampaignNewPlaceHolder'></div>\
            </div>");
        CampaignNew.Render($("#divCardCampaignNewPlaceHolder"));
        this.changeURL("CampaignNew", "New Campaign", "/campaignNew");
    },

    loadCampaigDetailsPage: function (queryString) {
        var campaignID = GetQueryStringValue(queryString, "id");
        $("#divMain").html("\
            <div id='divCardCampaignInfluencers' class='card'>\
                <div id='divCampaignInflunecersTitle' class='card-title'>Campaign Influencers</div>\
                <div id='divCardCampaignInfluencersPlaceHolder'></div>\
                <!--<div id='divCardCampaignSearchInfluencersPlaceHolder'></div>-->\
            </div>\
            <div id='divCardCampaignDetails' class='card'>\
                <div id='divCampaignTitle' class='card-title'>Campaign Details</div>\
                <div id='divCardCampaignDetailsPlaceHolder'></div>\
            </div>");
        CampaignDetails.Render($("#divCardCampaignDetailsPlaceHolder"), campaignID);
        CampaignInfluencers.Render($("#divCardCampaignInfluencersPlaceHolder"), campaignID);
        //SearchInfluencersByKeyword.Render($("#divCardCampaignSearchInfluencersPlaceHolder"), campaignID);
        this.changeURL("CampaignDetails", "Campaign Details", "/campaignDetails" + queryString);
    },

    loadCampaignAddInfluencersPage: function () {
        //$("#divMain").prepend("<div id='divCampaignAddInfluencersPlaceHolder'></div>");
        $("#divMain").html("<div id='divCardCampaignAddInfluencers' class='card'>\
            <div class='card-title'>Add Influencers</div>\
            <div id='divCampaignAddInfluencersPlaceHolder'></div>");

        CampaignAddInfluencers.Render($("#divCampaignAddInfluencersPlaceHolder"));

        this.changeURL("CampaignAddInfluencers", "Add Influencers", "/campaignAddInfluencers");

    },

    loadNotificationsPage: function () {
        $("#divMain").html("<div id='divCardNotifications' class='card'>\
            <div class='card-title'>Notifications</div>\
            <div id='divCardNotificationsPlaceHolder'></div>");

        Notifications.Render($("#divCardNotificationsPlaceHolder"));

        this.changeURL("Notifications", "Notifications", "/notifications");
    },

    loadInfluencerProfileWizard: function () {
        $("#divMain").html("<div id='divCardInfluencerProfileWizard' class='card'>\
            <div class='card-title'>Apply to join Influcaster</div>\
            <div id='divCardInfluencerProfileWizardPlaceHolder'></div>");

        //hide login buttons
        $(".header-nav").hide();

        InfluencerProfileWizard.Render($("#divCardInfluencerProfileWizardPlaceHolder"));

        this.changeURL("InfluencerApplication", "Influencer Registration", "/influencerapplication");
    },

    loadResetPassword: function () {
        $("#divMain").html("<div id='divResetPassword' class='card'>\
            <div class='card-title'>Reset Password</div>\
            <div id='divCardResetPasswordPlaceHolder'></div>");

        ResetPassword.Render($("#divCardResetPasswordPlaceHolder"));

        //this.changeURL("ResetPassword", "Reset Password", "/resetpassword");
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
            url: WebApiBaseUrl + '/api/Users/LogPage/' + page,
            type: 'POST',
           // dataType: 'json',
            headers: {
                'Authorization': 'Bearer ' + AccessToken
            },
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



