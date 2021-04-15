var MessageSend = {

    Render: function (jqueryObj, senderUserID, recipientUserID, contractID) {
        jqueryObj.append(this.htmlText);
        this.addClickEvents();
        this.senderUserID = senderUserID;
        this.recipientUserID = recipientUserID;
        this.contractID = contractID;
    },

    htmlText: function () {
        return '<div id="divSendMessage" class="send-message-grid">\
            <textarea id="txtMessage" class="send-message-input"  maxlength="2000"></textarea>\
            <button id="btnSendMessage" type="button" class="button send-message-button">Send</button>\
            <div id="divSentMessages" class="send-message-messages"><div>\
            </div>';
    },
    senderUserID: "",
    recipientUserID: "",
    contractID: "",

    addClickEvents: function () {
        var parent = this;

        $("#btnSendMessage").unbind('click');
        $("#btnSendMessage").click(function () {
            $("#btnSendMessage").prop("disabled", true);
            parent.sendMessage();
            return false;
        });
    },

    sendMessage: function () {
        var parent = this;
        if (this.senderUserID != null || this.recipientUserID != null || this.contractID != null) {
            $.ajax({
                url: WebApiBaseUrl + '/api/Messages/Send',
                type: 'POST',
                dataType: 'json',
                headers: {
                    'Authorization': 'Bearer ' + AccessToken
                },
                data: {
                    SenderUserID: parent.senderUserID,
                    RecipientUserID: parent.recipientUserID,
                    Message1: $('#txtMessage').val(),
                    ContractID: parent.contractID
                },
                success: function (result) {
                    parent.insertMessage(result);
                    $("#txtMessage").val("");
                    $("#btnSendMessage").prop("disabled", false);
                },
                error: function (request, message, error) {
                    console.log("Error: " + error);
                    LogError(error, true);
                }
            });
        } else {
            console.log("Error sending message");
            LogError("Error sending message", true);
        }
    },

    insertMessage: function (result) {
        var strHTML = "";
        strHTML += "<div class='message-from-self'>";
        //strHTML += "<div class='col-sm-12'>";
        strHTML += "<div id='divAvatarInsertedMessage'></div>";
        //strHTML += "<div>MessageID: " + result.ID + "</div>";
        //strHTML += "<div>ContractID: " + result.ContractID + "</div>";
        strHTML += "<div>" + result.DateCreated + "</div>";
        //strHTML += "<div>" + result.Subject + "</div>";
        strHTML += "<div>" + result.Message1 + "</div>";
        strHTML += "</div>";
        //strHTML += '</div>';

        $("#divSentMessages").prepend(strHTML);
        Avatar.Render($("#divAvatarInsertedMessage"), result.SenderUserID, 50, null, false);
    }
    
};