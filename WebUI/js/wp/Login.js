var Login = {

    htmlText: function () {
        return '\
        <div id="mdlLogin" class="modal login-modal">\
             <div class="modal-body">\
                  <div class="form-group form-group-md-width">\
                        <div>\
                            <label for="txtEmail">Email</label>\
                            <input id = "txtEmail" type ="email" placeholder="Email" required />\
                        </div>\
                        <div>\
                            <label for= "txtPassword" >Password</label >\
                            <input id = "txtPassword" type="password"  placeholder="Password" required />\
                            <a id="btnForgotPassword" class="login-forgot-password">Forgot Password</a>\
                        </div>\
            </div>\
                <div><button id="btnlLoginEmail" type="button" class="button button-blue-outline button-login-email">Log in</button></div>\
            </div>\
            <div class="modal-footer login-footer-text">New?\
                <a onclick="Page.Load(\'register\')"> Sign up</a>\
            </div >\
        </div>';
    },

    Render: function (redirectPage) {
        if (redirectPage) {
            this.redirectPage = redirectPage;
        } else {
            redirectPage = "home";
        }

        this.disableButtons(false);

        $("#divMain").html(this.htmlText());
        this.addClickEvents();

        ToggleModal($("#mdlLogin"), true);
    },


    redirectPage: "",

    addClickEvents: function () {
        var parent = this;

        //$("#btnLoginFacebook").on("click", function () {
        //    event.preventDefault();
        //    parent.disableButtons(true);
        //    window.location.href = GetExternalLoginRedirectUrl("Facebook");
        //});

        //$("#btnLoginGoogle").on("click", function () {
        //    event.preventDefault();
        //    parent.disableButtons(true);
        //    window.location.href = GetExternalLoginRedirectUrl("Google");
        //});

        $("#btnForgotPassword").on("click", function () {
            event.preventDefault();
            parent.disableButtons(true);
            if ($('#txtEmail').val().trim() == "") {
                alert("Email address empty. Please enter your email address to retrieve password.")
            } else {
                
                $.ajax({
                    method: 'GET',
                    url: WebApiBaseUrl + '/api/PasswordReset/ResetByEmail/' + $('#txtEmail').val() + "/",
                    success: function () {
                        alert("Please check " + $('#txtEmail').val() + " to reset your password");
                        parent.disableButtons(false);
                    },
                    error: function (err) {
                        console.log("error: " + err.statusText);
                        //LogErrorMessage("Error", true);
                        alert("Error: " + err.statusText);
                        parent.disableButtons(false);
                    }

                });
            }
        });

        $("#btnlLoginEmail").on("click", function (event) {
            parent.disableButtons(true);
            ToggleModal($("#mdlLogin"), false);
            LoginUsingEmail($('#txtEmail').val(), $('#txtPassword').val());
        });

    },

    disableButtons: function (disable) {
        $('#btnlLoginEmail').prop('disabled', disable);
        $('#btnLoginFacebook').prop('disabled', disable);
        $('#btnLoginGoogle').prop('disabled', disable);
        $('#btnForgotPassword').prop('disabled', disable);
    }

    

};