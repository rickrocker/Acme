

function GetAccessTokenFromExternalAuth() {
    if (location.hash) {
        if (location.hash.split('access_token=')) {
            var externalAccessToken = location.hash.split('access_token=')[1].split('&')[0];
            if (externalAccessToken) {
                IsUserRegistered(externalAccessToken);
            }
        }
    }

}

function IsUserRegistered(externalAccessToken) {
    $.ajax({
        url: WebApiBaseUrl + '/api/account/userinfo',
        method: 'GET',
        headers: {
            'content-type': 'application/JSON',
            'Authorization': 'Bearer ' + externalAccessToken
        },
        success: function (response) { //user has already registered and is in DB

            if (response.HasRegistered && response.Email != null) {
                localStorage.setItem('AccessToken', externalAccessToken);
                AccessToken = externalAccessToken;
                localStorage.setItem('UserName', response.Email);

                //TODO: Use LoadPage instead?
                window.location.href = "/home";
            }
            else { //user has not yet registered and is not in DB yet
                //does user have role?
                var userRole = sessionStorage.getItem("IntendedRole");
 
                sessionStorage.setItem("ExternalUser", "true");
                if (userRole == null || userRole == "") { //Show Modal to get role
                    sessionStorage.setItem("LoginProvider", response.LoginProvider) //LoginProvider is either "Google" or "Facebook"
                    ModalRoleAuthManager.Render($("#divMain"), externalAccessToken, response.LoginProvider);
                    ToggleModal($("#mdlRoleAuthManager"), true);
                } else {
                    SignupExternalUser(externalAccessToken, response.LoginProvider, userRole); //has role so ready to sign up
                }
            }
        },
        error: function (error) {
            //console.log("error: " + err.responseText);
            console.log("Error: " + error.responseText);
            LogError(error.responseText, true);
        }

    })
}

function SignupExternalUser(accessToken, provider, userRole) {
    if (provider == "google") { provider = "Google"; }
    if (provider == "facebook") { provider = "Facebook"; }
    $.ajax({
        //url: WebApiBaseUrl + '/api/account/RegisterExternal',
        url: WebApiBaseUrl + '/api/account/RegisterExternal/' + userRole,
        method: 'POST',
        headers: {
            'content-type': 'application/JSON',
            'Authorization': 'Bearer ' + accessToken,
            'AccessToken': accessToken
        },
        success: function (response) {
            console.log("response: " + response);
            //localStorage.setItem('AccessToken', accessToken);
            //AccessToken = location.hash.split('access_token=')[1].split('&')[0];
            //window.location.href = HomeUrl;
            var encodedRedirectUrl = encodeURIComponent(HomeUrl);
            window.location.href = WebApiBaseUrl + "/api/Account/ExternalLogin?provider=" + provider + "&response_type=token&client_id=self&redirect_uri=" + encodedRedirectUrl + "&state=pQ0B0lXxlVvI0PtJKe4WVV-1VaVspFt1pOVYtx8lfQw1";
        },
        error: function (error) {
            //console.log("error: " + err.responseText);
            console.log("Error: " + error.responseText);
            LogError(error.responseText, true);
        }

    })
}

function GetExternalLoginRedirectUrl(provider) {
    //var encodedredirectUrl = encodeURIComponent(WebUIBaseUrl);
    var encodedRedirectUrl = encodeURIComponent(RegisterRedirectUrl);
    var url = WebApiBaseUrl + "/api/Account/ExternalLogin?provider=" + provider + "&response_type=token&client_id=self&redirect_uri=" + encodedRedirectUrl + "&state=pQ0B0lXxlVvI0PtJKe4WVV-1VaVspFt1pOVYtx8lfQw1";
    return url;
}

function GetAccessTokenFromInternalAuth() {
    $.ajax({
        method: 'POST',
        url: WebApiBaseUrl + '/token',
        contentType: 'application/x-www-form-urlencoded',
        data: {
            username: $('#txtEmail').val(),
            password: $('#txtPassword').val(),
            grant_type: 'password'
        },
        success: function (response) {
            localStorage.setItem('AccessToken', response.access_token);
            AccessToken = response.access_token;
            
            //if (sessionStorage.getItem("IntendedRole") == "Influencer" || UserRole == "Influencer Invitee") {
            if (UserRole == "Influencer Invitee") {
                Page.Load("influencerApplication");
            } else {
                //TO DO: change to companyProfileWizard
                Header.ToggleButtons();
                Page.Load("home");
                //Page.Load("companyProfileWizard");
            }

        },
        error: function (error) {
            console.log("Error: " + error);
            LogError(error, true);
        }

    })

}

function LoginUsingEmail (userName, password) {
    var parent = this;
    $.ajax({
        method: 'POST',
        url: WebApiBaseUrl + '/token',
        contentType: 'application/x-www-form-urlencoded',
        data: {
            username: userName,
            password: password,
            grant_type: 'password'
        },
        success: function (response) {
            localStorage.setItem('AccessToken', response.access_token);
            AccessToken = response.access_token;
            Init.Start();
            Header.ToggleButtons();
        },
        error: function (err) {
            console.log("Error: " + err.responseJSON.error_description);
            LogError(err.responseJSON.error_description, true);
        }
    });
} 


function SignOut() {
    var tempAccessToken = AccessToken;

    AccessToken = null;
    UserRole = null;
    Campaign = null;
    localStorage.clear();
    sessionStorage.clear();

    //save userprofile in local storage for next login
    localStorage.setItem("LoggedOutUserProfile", UserProfile); //TODO: Edit so only necessary fields saved

    UserProfile = null;

    Header.ToggleButtons();

    $.ajax({
        method: 'POST',
        url: WebApiBaseUrl + '/api/Account/Logout',
        headers: {
            'Authorization': 'Bearer ' + tempAccessToken
        },

        success: function () {
            console.log("Logged out");
        },
        error: function (err) {
            LogError("Error logging out (/api/Account/Logout)", false);
        },
        complete: function (data) {
            Login.Render("home");
        }

    });
}


var ModalRoleAuthManager = {

    Render: function (jqueryObj, externalAccessToken) {
        jqueryObj.append(this.htmlText());
        this.addClickEvents(externalAccessToken);
    },

    htmlText: function () {
        return '\
            <div id="mdlRoleAuthManager" class="modal">\
                  <div class="modal-header">\
                       Are you an influencer or an advertiser?\
                </div>\
                <div class="modal-body">\
                    <div>\
                        <label class="modal-close" for="mdlRole"></label>\
                        <label class="radio-inline">\
                            <input type="radio" name="radRole" value="Influencer">Influencer\
                        </label>\
                    </div>\
                        <div>\
                            <label class="radio-inline">\
                                <input type="radio" name="radRole" value="Advertiser">Advertiser\
                        </label>\
                    </div>\
                        </div>\
                        <div class="modal-footer">\
                            <button id="btnSubmitRoleAuthManager" class="button button-blue" type="button">Submit</button>\
                        </div>\
                    </div>';
    },

    addClickEvents: function (externalAccessToken) {
        var parent = this;

        $("#btnSubmitRoleAuthManager").click(function () {
            parent.submitRole(externalAccessToken);
        });

    },

    submitRole: function (externalAccessToken) {
        var intendedRole = $("input[name=radRole]:checked").val();

        //if (intendedRole == "Influencer" || intendedRole == "Advertiser") {
        //    sessionStorage.setItem("IntendedRole", intendedRole);
        //}

        ToggleModal($("#mdlRole"), false);

        SignupExternalUser(externalAccessToken, sessionStorage.getItem("LoginProvider"), intendedRole); 

        //this.signUp();
    }
}


