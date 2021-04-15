var Contact = {

    Render: function (jqueryObj) {
        var parent = this;
        jqueryObj.load("/html/contact.html?" + Version,
            function (responseTxt, statusTxt, xhr) {
                parent.addClickEvents();
            });
    },

    addClickEvents: function () {
        var parent = this;

        $("#btnSubmitContactUs").click(function () {
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
                message: "CONTACT FORM: "  + $('#txtMessage').val()
            },
            success: function (result) {
                alert("Your message has been sent. We will get back to you shortly.");
                Page.Load("home");
            },
            error: function (error) {
                LogError("Error: " + error, true);
                $("#btnSubmitContactUs").prop("disabled", false);
            }
        });
    }

}