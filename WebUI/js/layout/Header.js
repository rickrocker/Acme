var Header = {

    Render: function (jqueryObj) {
        jqueryObj.html(this.htmlText());
        this.addClickEvents();
        this.ToggleButtons();
        //this.loadProfile(); call from Init.js
    },

    htmlText: function () {
        return '<div id="divHeaderTitle" class="header-title"></div>\
                <div class="header-nav">\
                <a id="linkSignIn" class="header-nav-item"  style="display: none">Log in</a>\
                <a id="linkRegister" class="header-nav-item"  style="display: none">Sign up</a>\
                <a id="linkNotifications" class="button-notification" style="display: none">\
                    <i class="header-icon far fa-bell"></i><span class="badge-notification">4</span>\
                </a>\
                <button id="btnProfile" class="header-profile button-no-border dropdown" style="display: none">\
                    <svg viewBox="0 0 18 18" role="presentation" aria-hidden="true" focusable="false" style="height:8px;width:8px;display:inline-block"><path d="m16.29 4.3a1 1 0 1 1 1.41 1.42l-8 8a1 1 0 0 1 -1.41 0l-8-8a1 1 0 1 1 1.41-1.42l7.29 7.29z" fill-rule="evenodd"></path></svg>\
                    <div id="ddProfileMenu" class="dropdown-content dropdown-content-right">\
                        <a id="linkProfile">Profile</a>\
                        <a id="linkSignOut">Log out</a>\
                    </div>\
                </button >\
            </div>';
    },

    addClickEvents: function () {
        var parent = this;
        $('#linkNotifications').on('click', function () {
            //$('#linkNotifications').removeClass('selected');
            //$(this).addClass('selected');
            Page.Load("notifications");
        });

        $('#linkSignIn').on('click', function () {
            Page.Load("home");
        });

        $('#linkProfile').on('click', function () {
            //$("#ddProfileMenu").css("display", "none");
            Page.Load("profile");
        });

        $('#linkSignOut').on('click', function () {
            //$("#ddProfileMenu").css("display", "none");
            SignOut();
        });

        $('#linkRegister').on('click', function () {
            Page.Load("register");
        });



        //close profile dropdown when user clicks outside of menu
        $(window).click(function () {
            $("#ddProfileMenu").css("display", "none");
        });

        $('#btnProfile').on('click', function (event) {
            $("#ddProfileMenu").toggle();
            event.stopPropagation(); //except if they click Profile button
        });
    },

    ToggleButtons: function () {

        if (AccessToken != null && AccessToken != "") {
            if (UserRole == "Influencer Invitee") {
                $("#linkNotifications").hide();
                $("#btnProfile").hide();
                $("#linkSignIn").hide();
                $("#linkRegister").hide();
            } else {
                $("#linkNotifications").show();
                $("#btnProfile").show();
                $("#linkSignIn").hide();
                $("#linkRegister").hide();
            }

        } else { //no access token
            $("#linkSignIn").show();
            $("#linkRegister").show();
            $("#linkNotifications").hide();
            $("#btnProfile").hide();
        }
    },

    LoadProfile: function () {
        try {
            if (UserProfile != null && UserProfile != "") {
                var userProfileButton = "<img id='imgHeaderAvatar' src='" + Avatar.GetURL(UserProfile.UserID, true) + "' " + Avatar.ErrorImageTagSmall + " />";
                $("#imgHeaderAvatar").remove();
                $("#btnProfile").prepend(userProfileButton);
            }
        } catch (err) {
            console.log("Error: " + err.message);
            LogError("Error: " + err.message);
        }
    }

};