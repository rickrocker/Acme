var ModalRoleAuthManager = {

    Render: function (jqueryObj) {
        jqueryObj.append(this.htmlText());
        this.addClickEvents();
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
                            <button id="btnSubmitRole" class="button button-blue" type="button">Submit</button>\
                        </div>\
                    </div>';
    },

    addClickEvents: function () {
        var parent = this;

        $("#btnSubmitRole").click(function () {
            parent.SubmitRole();
        });

    },

    SubmitRole: function () {
        var intendedRole = $("input[name=radRole]:checked").val();

        if (intendedRole == "Influencer" || intendedRole == "Advertiser") {
            sessionStorage.setItem("IntendedRole", intendedRole);
        }

        ToggleModal($("#mdlRole"), false);

        //this.signUp();
    }
}