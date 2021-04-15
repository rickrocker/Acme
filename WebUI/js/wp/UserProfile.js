//Note: must include DatePicker.js script in html 
var UserProfileObj = {

    Render: function (jqueryObj) {
        var parent = this;
        jqueryObj.load(WebPartDirectory + "/html/UserProfile.html?" + Version,
            function (responseTxt, statusTxt, xhr) {
                parent.addClickEvents();
                parent.loadUserProfile();
                if (UserProfile.Role.includes("Influencer")) {
                    parent.loadUserProfile2();
                }
                $(this).prop("disabled", false);
            });
    },

    addClickEvents: function () {
        var parent = this;

        $("#btnSaveUserProfile").on("click", function (event) {
            event.preventDefault();
            $(this).prop("disabled", true);
            parent.save();
        });
    },

    loadUserProfile: function () {
        try {
            if (UserProfile != null && UserProfile != "") {

                if (UserProfile.Email.includes("temp.com")) {
                    $("#divEmail").show();
                }
                //myCountry = UserProfile.Country;
                //myState = UserProfile.State;
                //if (UserProfile.DateOfBirth != null) {
                //    var myDate = this.parseDate(UserProfile.DateOfBirth);
                //    DatePicker.LoadDate(0, myDate);
                //}
                //if (UserProfile.Gender != null && UserProfile.Gender.trim() != "") {
                //    $('input[name=rbGender][value=' + UserProfile.Gender + ']').prop('checked', true);
                //}

                $('#txtFirstName').val(UserProfile.FirstName);
                $('#txtLastName').val(UserProfile.LastName);
                $('#txtPhone').val(UserProfile.Phone);
                //$('#txtAddress1').val(UserProfile.Address1);
                //$('#txtAddress2').val(UserProfile.Address2);
                //$("#selCountry").val(UserProfile.Country).change();
                //$("#selState").val(UserProfile.State)
                //$('#txtCity').val(UserProfile.City);
                //$('#txtZip').val(UserProfile.Zip);
            }
        } catch (err) {
            console.log("error: " + err);
            LogError(err, false);

        }
    },

    loadUserProfile2: function () {


        DatePicker.Render($("#divDOB"), 0);
        CountryStateDropdowns.Render($("#selCountry"), $("#selState"));




        if (UserProfile != null && UserProfile != "") {
            myCountry = UserProfile.Country;
            myState = UserProfile.State;
            if (UserProfile.DateOfBirth != null) {
                var myDate = this.parseDate(UserProfile.DateOfBirth);
                DatePicker.LoadDate(0, myDate);
            }
            if (UserProfile.Gender != null && UserProfile.Gender.trim() != "") {
                // $('input[name=rbGender][value=' + UserProfile.Gender + ']').prop('checked', true);
                $("#selGender").val(UserProfile.Gender);
            }
            //$('#txtAddress1').val(UserProfile.Address1);
            //$('#txtAddress2').val(UserProfile.Address2);
            $("#selCountry").val(UserProfile.Country).change();
            $("#selState").val(UserProfile.State)
            $('#txtCity').val(UserProfile.City);
            $('#txtZip').val(UserProfile.Zip);
        }

        $("#divProfile2").show();
    }, 

    save: function () {
        if ($('#txtEmail').val().trim().length > 3) {
            this.saveEmail();
        }
        this.saveUserProfile();

    },

    saveEmail: function () {
        var parent = this;
        if (AccessToken != null && AccessToken != "") {
            $.ajax({
                url: WebApiBaseUrl + '/api/Users/UserEmail/',
                type: 'PUT',
                headers: {
                    'Authorization': 'Bearer ' + AccessToken
                },
                data: {
                    Email: $('#txtEmail').val(),
                },
                success: function (result) {

                },
                error: function (request, message, error) {
                    console.log("ERROR: " + error);
                    LogError(error, true);

                }
            });
        }
    },

    saveUserProfile: function () {
        var parent = this;
        if (AccessToken != null && AccessToken != "") {
            $.ajax({
                url: WebApiBaseUrl + '/api/Users/UserProfile/',
                type: 'PUT',
                headers: {
                    'Authorization': 'Bearer ' + AccessToken
                },
                data: {
                    FirstName: $('#txtFirstName').val(),
                    LastName: $('#txtLastName').val(),
                    Phone: $('#txtPhone').val(),
                    //Gender: $('input[name=rbGender]:checked', '#divProfile2').val(),
                    Gender: $("#selGender").find(":selected").val(),
                    DateOfBirth: DatePicker.GetSelectedDateMMddYYYY(0),
                    //Address1: $('#txtAddress1').val(),
                    //Address2: $('#txtAddress2').val(),
                    City: $('#txtCity').val(),
                    State: $("#selState").find(":selected").val(),
                    Country: $("#selCountry").find(":selected").val(),
                    Zip: $('#txtZip').val()
                },
                success: function (result) {
                    //sessionStorage.setItem("UserProfile", JSON.stringify(result));
                    UserProfile = result;
                    //parent.showProfile2IfProfile1Complete();
                    alert("Your information has been successfully saved.")
                    $("#btnSaveUserProfile").prop("disabled", false);

                },
                error: function (request, message, error) {
                    console.log("ERROR: " + error);
                    LogError(error, true);

                }
            });
        }
    },

    parseDate: function (input) {
        var parts = input.match(/(\d+)/g);
        return new Date(parts[0], parts[1] - 1, parts[2]); // months are 0-based
    }

}