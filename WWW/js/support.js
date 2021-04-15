var Support = {

    Render: function (jqueryObj) {
        var parent = this;
        jqueryObj.load("/html/support.html?" + Version,
            function (responseTxt, statusTxt, xhr) {
                parent.addClickEvents();
            });
    },

    addClickEvents: function () {
        var parent = this;

        $("#btnSubmitSupportRequest").click(function () {
            $(this).prop("disabled", true);
            parent.submitForm();
        });
    },

    submitForm: function () {
        var parent = this;
        $.ajax({
            method: 'PUT',
            url: WebApiBaseUrl + '/api/ContactUs',
            data: {
                //dataType: "json",
                email: $('#txtEmail').val(),
                name: $('#txtName').val(),
                phone: $('#txtPhone').val(),
                message: "SUPPORT FORM: " + $('#txtMessage').val()
            },
            success: function (result) {
                alert("Your support request has been submitted. We will respond very shortly.");
                Page.Load("home");
            },
            error: function (error) {
                LogError("Error: " + error, true);
                $("#btnSubmitSupportRequest").prop("disabled", false);
            }
        });
    }

}