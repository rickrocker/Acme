var AvatarUpload = {

    Render: function (jqueryObj) {
        jqueryObj.html(this.htmlText());
        this.addChangeEvents(jqueryObj);

        $("#imgAvatarPreview").attr("src", Avatar.GetURL(UserProfile.UserID, true));
    },

    htmlText: function () {
        return '\
        <form id="frmAvatar">\
          <div class="form-group">\
             <div>\
                <label for="uploadAvatar">Profile Picture</label>\
                <img id="imgAvatarPreview"' + Avatar.ErrorImageTagSmall +' />\
                <input type="file" title=" " class="form-control-file" id="uploadAvatar" accept="image/jpeg,image/gif,image/png">\
             </div>\
          </div>\
        </form>';
    },

    uploadAvatar: function () {
        var formData = new FormData();
        var files = $("#uploadAvatar").get(0).files;

        // Add the uploaded image content to the form data collection  
        if (files.length > 0) { // check if user selected a file
            formData.append("UploadedImage", files[0]);
            $.ajax({
                url: WebApiBaseUrl + '/api/Users/Avatar',
                type: 'POST',
                dataType: 'json',
                contentType: false,
                processData: false,
                headers: {
                    'Authorization': 'Bearer ' + AccessToken
                },
                data: formData,
                success: function (result) {
                    //console.log("successfully upload profile image. result: " + result);
                    try {
                        //var src = $("#imgHeaderAvatar").attr("src") + "?time=" + new Date().getTime();
                        var src = Avatar.GetURL(UserProfile.UserID, true) + "?time=" + new Date().getTime();
                        $("#imgAvatarPreview").attr("src", src);
                        $("#imgAvatarPreview").removeAttr("onerror");
                        $("#imgHeaderAvatar").attr("src", src);
                        $("#imgHeaderAvatar").removeAttr("onerror");
                    } catch (err) {
                        console.log("error: " + err);
                        LogError(err);
                    }
                },
                error: function (request, message, error) {
                    // loadInfluencerInviteInfo();
                    console.log("Error: " + error);
                    LogError(error, true);
                }
            });
        }
    },

    addChangeEvents: function (jqueryObj) {
        var parent = this;
        function readURL(input, jqueryObj) {
            if (input.files && input.files[0]) {
                var reader = new FileReader();

                reader.onload = function (e) {
                    //jqueryObj.attr('style', 'width:100px');
                    jqueryObj.attr('src', e.target.result);
                }

                reader.readAsDataURL(input.files[0]);
            }
        }
        $("#uploadAvatar").change(function () {
            readURL(this, jqueryObj);
            $("#cntAvatar").remove();
            parent.uploadAvatar();
            //jqueryObj.append("<img src='" + myUrl + "' />");
            //if the next element is an image (the current avatar) then remove it


        });
    },

    Save: function () {
        this.uploadAvatar();
    }
}