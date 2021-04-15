
// detect back/forward button navigation
window.onpopstate = function (e) {
    if (e.state) {
        Page.Load(e.state.html);
        //document.getElementById("divMain").innerHTML = e.state.html;
        //document.title = e.state.pageTitle;
    }
};


var Init = {

    Start: function () {

        Header.Render($("#divHeader"));

        //which page are we at?
        var path = window.location.pathname;
        var queryString = window.location.search;
        var page;
        if (path == "/" || path.toLowerCase().includes("index")) {
            page = "home";
        } else {
            page = path.substring(path.indexOf("/") + 1)
        }

        //page is "/registerexternal" which is callback page after Google and FB auth; (important: access_token in url is EXTERNAL access token)
        if (page.toLowerCase() == "registerexternal" && window.location.href.includes("access_token")) {
            GetAccessTokenFromExternalAuth();
        }
        //page has access_token in url which is sent from Google and FB after auth; (important: access_token in url is EXTERNAL access token)
        else if (page.toLowerCase() == "home" && window.location.href.includes("access_token")) {
            accessToken = location.hash.split('access_token=')[1].split('&')[0];
            if (accessToken.length > 0) {
                localStorage.setItem("AccessToken", accessToken);
                //redirect to same page to take out access_token from URL
                window.location.href = window.location.href.substring(0, window.location.href.indexOf("#access_token"));
            }
        }
        else if (page.toLowerCase() == "resetpassword") {
            Page.Load("resetpassword");
        }
        else { //regular page  
            if (localStorage.getItem("AccessToken") != null) { //has internal access token
                AccessToken = localStorage.getItem("AccessToken");
                this.getUserProfileAndLoadPage(page, queryString);
            } else if (AccessToken == null || AccessToken == "") {  //no internal access token - prompt login (unless it's register page)
                if (page == "register") {
                    Page.Load("register");
                } else {
                    this.toggleLoginForm(false, page);
                }
            }
        }
    },

    toggleLoginForm: function (loggedIn, redirectPage) { //TODO: check for localStorage["UserProfile"] (value saved during Logout()), if exists -> show login form with user's name, image, & provider log in button (+"Not [John], click here")
        if (redirectPage == null || redirectPage == "") {
            redirectPage = "home";
        }
        if (loggedIn) {
            Page.Load(redirectPage);
        } else {
            Login.Render(redirectPage);
        }
    },

    getUserProfileAndLoadPage: function (page, queryString) {
        var parent = this;
        if (UserProfile == null || UserRole == null) {

            $.ajax({
                url: WebApiBaseUrl + '/api/Users/GetUserProfile',
                type: 'GET',
                dataType: 'json',
                headers: {
                    'Authorization': 'Bearer ' + AccessToken
                },
                success: function (result) {
                    UserProfile = result;
                    UserRole = result.Role;

                    Header.Render($("#divHeader"));
                    Nav.Render($("#divNav"));

                    Header.LoadProfile();

                    Page.Load("profile");

                },
                error: function (request, message, error) {
                    Header.Render($("#divHeader"));
                    Nav.Render($("#divNav"));
                    console.log("error: " + error);
                    LogError(error, true);
                }
            });

            //get all roles
            $.ajax({
                url: WebApiBaseUrl + '/api/Users/GetRoles',
                type: 'GET',
                dataType: 'json',
                headers: {
                    'Authorization': 'Bearer ' + AccessToken
                },
                success: function (result) {
                    UserRoles = result;
                },
                error: function (request, message, error) {
                    Header.Render($("#divHeader"));
                    Nav.Render($("#divNav"));
                    console.log("error: " + error);
                    LogError(error, true);
                }
            });
        }
    },

    userProfileIsComplete: function (result) {
        //if (result.FirstName == null || result.FirstName.trim() == "" || result.LastName == null || result.FirstName.trim() == "" || result.Email.toLowerCase().includes("temp.com") || result.Email == null || result.Email.trim() == "")
        if (result.Email.toLowerCase().includes("temp.com") || result.Email == null || result.Email.trim() == "") {
            return false;
        } else { return true; }
    }





};