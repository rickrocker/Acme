var ResetPassword = {

    htmlText: function () {
        return '<div class="form-group form-group-md-width">\
            <div>\
                <label for="txtNewPassword1">New Password</label>\
                <input type="password" id="txtNewPassword1" />\
            </div>\
            <div>\
                <label for="txtNewPassword2">Confirm Password</label>\
             <input type="password" id="txtNewPassword2" />\
            </div>\
            <div>\
                <button id="btnResetPassword" class="button button-blue">Reset Password</button>\
            </div></div>';

    },


    Render: function (jqueryObj) {
        var resetID = UrlParams["resetID"];
        if (resetID != null) {
            jqueryObj.html(this.htmlText());
            this.addClickEvents();
        } else {
            //send error message to user (Invalid ID, check email link)
            alert("Error: Invalid Request. Check your email and click the link provided to reset password.");
        }
    },

    addClickEvents: function () {
        var parent = this;

        $("#btnResetPassword").on("click", function () {
            event.preventDefault();
            $("#btnResetPassword").prop("disabled", true);
            parent.ResetPassword();
        });
    },

   
  

    ResetPassword: function () {
        ClearEror();
        if ($('#txtNewPassword1').val() == $('#txtNewPassword2').val()) {
            $.ajax({
                method: 'POST',
                url: WebApiBaseUrl + '/api/Account/ResetPassword',
                data: {
                    PasswordRequestID: UrlParams["resetID"],
                    NewPassword: $('#txtNewPassword1').val(),
                    ConfirmPassword: $('#txtNewPassword2').val()
                },
                success: function () {
                    console.log("success");
                    LogSuccess("Your password has been successfully changed. Please login again to continue.", true);
                    Page.Load("home");
                },
                //error: function (err) {
                //    console.log("error: " + err.statusText);
                //    LogError(err.statusText);
                //},
                error: function (err) {

                    if (err.status === 400) {
                        var errorText = "";
                        try {
                            errorText += err.responseJSON.ModelState["model.NewPassword"][0] + " ";
                        } catch (e) {
                            //do nothing
                        }

                        try {
                            errorText += err.responseJSON.ModelState["model.ConfirmPassword"][0] + " ";
                        } catch (e) {
                            //do nothing
                        }

                        try {
                            errorText += err.responseJSON.Message + " ";
                        } catch (e) {
                            //do nothing
                        }

                        try {
                            errorText += err.responseJSON.ModelState[""][0] + " ";
                        } catch (e) {
                            //do nothing
                        }


                        // err += err.responseJSON.ModelState["model.NewPassword"][0] + ". " + err.responseJSON.ModelState["model.ConfirmPassword"][0];
                        console.log("Error: " + errorText);
                        //alert(error);
                        LogError(errorText, true);
                    } else {
                        console.log("error: " + err.statusText);
                        LogError(err.statusText, true);
                    }

                    $('#btnResetPassword').prop('disabled', false);

                }

            });
        } else {
            //passwords don't match
            console.log("error: passwords don't match");
            LogError("Passwords don't match. Please try again.", true);
            $('#btnResetPassword').prop('disabled', false);
        }
    }

   
}