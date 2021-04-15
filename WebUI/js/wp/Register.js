var Register = {


    Render: function (jqueryObj) {
        var parent = this;
        jqueryObj.load(WebPartDirectory + "/html/register.html?v=" + Version,
            function (responseTxt, statusTxt, xhr) {
                parent.addClickEvents();
                ToggleModal($("#mdlRegisterEmail"), true);
            });
    },

    addClickEvents: function () {
        var parent = this;


        $("#btnRegister").click(function () {
            $('#btnRegister').prop('disabled', true);
            parent.registerUsingEmail();
        });

    },


    signUp: function () {
            //ToggleModal($("#mdlRegister"), true);
            ToggleModal($("#mdlRegisterEmail"), true);
    },

    isTermsChecked: function () {
        //check if Terms is checked
        if ($('#cbxTerms').prop('checked')) {
            return true;
        } else {
            alert("You must agree to the Terms of Service to sign up.");
            return false;
        }
    },

    registerUsingEmail: function () {
        if (this.isTermsChecked()) {
            var parent = this;
            $.ajax({
                method: 'POST',
                url: WebApiBaseUrl + '/api/Account/Register',
                data: {
                    email: $('#txtEmail').val(),
                    password: $('#txtPassword').val(),
                    confirmPassword: $('#txtPassword').val(),
                    firstName: $('#txtFirstName').val(),
                    lastName: $('#txtLastName').val(),
                    role: "User"
                },
                success: function (result) {
                    UserProfile = result;
                    UserRole = result.Role;
                    UserID = result.UserID;
                    ToggleModal($("#mdlRegisterEmail"), false);
                    GetAccessTokenFromInternalAuth();
                },
                error: function (error) {
                    var errorTitle = error.responseJSON.Message;
                    var errorMessage = error.responseText;
                    console.log("Error: " + errorMessage);
                    LogError(errorMessage, true);
                    $('#btnRegister').prop('disabled', false);
                }
            })
        }
    }

   
};