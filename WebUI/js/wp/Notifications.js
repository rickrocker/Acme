var Notifications = {


    Render: function (jqueryObj) {
        jqueryObj.html(this.htmlText);
        this.getAllNotifications();
        //$("#divNotificationMessageThread").append(MessageSend.HtmlText());
    },

    htmlText: function () {
        return '<div id="divNotificationsAll" class="panel-body"></div>\
         <div id="divNotificationMessageThread" class="panel-body" style="display:none"></div>';
    },


    getAllNotifications: function () {
        var parent = this;
        $.ajax({
            url: WebApiBaseUrl + '/api/Notifications/0',
            type: 'GET',
            dataType: 'json',
            headers: {
                'Authorization': 'Bearer ' + AccessToken
            },
            success: function (result) {
                // $("#divNotificationsAll").append(result);
                parent.loadNotifications(result);
            },
            error: function (request, message, error) {
                console.log("Error: " + error);
                LogError(error, false);
            }
        });
    },

    loadNotifications: function (result) {
        var strHTML = "";

        if (result.mergedEntity.length == 0) {
            strHTML += "You currently have no notifications.";
            $("#divNotificationsAll").append(strHTML);
            strHTML = "";
        }

        if (result.mergedEntity.length > 0) {
            for (var i = 0; i < result.mergedEntity.length; i++) {

                var messageIndex = -1;
                var isMessage = false;
                var mySubject = result.mergedEntity[i].Subject;
                if (mySubject == "[MESSAGE]") {
                    mySubject = "[Direct Message]"
                    isMessage = true;
                    messageIndex++;
                }

                //determine who the sender is (opposite of current user)
                var curUserID = UserProfile.UserID;
                var otherPartyUserID = "";
                var otherPartyFirstName = "";
                var otherPartyLastName = "";
                if (result.mergedEntity[i].SenderUserID != null && result.mergedEntity[i].SenderUserID != curUserID) {
                    otherPartyUserID = result.mergedEntity[i].SenderUserID;
                    otherPartyFirstName = result.mergedEntity[i].SenderFirstName;
                    otherPartyLastName = result.mergedEntity[i].SenderLastName;
                } else if (result.mergedEntity[i].SenderUserID != null && result.mergedEntity[i].SenderUserID == curUserID) {
                    otherPartyUserID = result.mergedEntity[i].RecipientUserID;
                    otherPartyFirstName = result.mergedEntity[i].RecipientFirstName;
                    otherPartyLastName = result.mergedEntity[i].RecipientLastName;
                }

                strHTML += '<div class="notification-grid">';

                //strHTML += '<div>';
                strHTML += "<div class='notification-avatar' id='divAvatar" + i + "'></div>";
                strHTML += "<div class='notification-sender'>";
                strHTML += "<div class='notification-from-name'>";
                strHTML += otherPartyFirstName + " " + otherPartyLastName;
                strHTML += "</div>";
                if (result.mergedEntity[i].CompanyName) {
                    strHTML += "<div class='notification-from-company'>"
                    strHTML += result.mergedEntity[i].CompanyName;
                    strHTML += "</div>"
                }
                strHTML += FormatDateToShort(result.mergedEntity[i].DateCreated);
                strHTML += "</div>";

                strHTML += "<div class='notification-text'>";
               // strHTML += "ID: " + result.mergedEntity[i].ID;
               // strHTML += "; ContractID: " + result.mergedEntity[i].ContractID;
                strHTML += "<div class='notification-subject'><a onclick='Notifications.getRelatedNotificationsAndMessages(" + isMessage + "," + result.mergedEntity[i].ID + "); return false; '>" + mySubject + "</a></div>";
                if (result.mergedEntity[i].Abstract != null && result[i].Abstract.length > 0) {
                    strHTML += "<div><a onclick='Notifications.getRelatedNotificationsAndMessages(" + isMessage + "," + result.mergedEntity[i].ID + "); return false;'>" + result.mergedEntity[i].Abstract + "</a></div>";
                } else if (result.mergedEntity[i].Body != null && result.mergedEntity[i].Body.length > 0) {
                    strHTML += "<div><a onclick='Notifications.getRelatedNotificationsAndMessages(" + isMessage + "," + result.mergedEntity[i].ID + "); return false;'>" + result.mergedEntity[i].Body.substring(0, 75) + "...</a></div>";
                }
                strHTML += "</div>";

                if (!isMessage) { //get task for notification
                    strHTML += this.getTaskHTML(result.mergedEntity[i]);
                }
                else { //fill in tasks for messages
                    if (messageIndex > -1 && result.tasksEntity[messageIndex] != null) {
                        strHTML += this.getTaskHTML(result.tasksEntity[messageIndex]);
                    }
                }

                strHTML += '</div>';

                $("#divNotificationsAll").append(strHTML);
                strHTML = "";
                Avatar.Render($("#divAvatar" + i), curUserID, 50, null, false);
            }
        }
    },

    getTaskHTML: function (taskEntity) {
        strHTML = "";
        strHTML += "<div class='notification-task'>";

        if (taskEntity.TaskStatusName == "Assigned" && taskEntity.MinutesUntilTaskExpires > 0) {
            strHTML += "<a href='/Tasks?taskid=" + taskEntity.TaskID + "&taskname=" + taskEntity.TaskTypeName + "'>" + taskEntity.TaskTypeName + "</a><br />";
            strHTML += taskEntity.MinutesUntilTaskExpires;
        } else if (taskEntity.TaskStatusName == "Assigned" && taskEntity.MinutesUntilTaskExpires < 0) {
            strHTML += "Expired";
        } else if (taskEntity.TaskStatusName == "Completed") {
            strHTML += "Completed";
        }
        strHTML += "</div>";
        return strHTML;

    },

    getRelatedNotificationsAndMessages: function (isMessage, ID) {
        var parent = this;
        var myURL = "";
        if (isMessage) {
            myURL = WebApiBaseUrl + '/api/Notifications/RelatedNotificationsAndMessages/true/' + ID + '/0';
        } else {
            myURL = WebApiBaseUrl + '/api/Notifications/RelatedNotificationsAndMessages/false/' + ID + '/0';
        }
        $.ajax({
            url: myURL,
            type: 'GET',
            dataType: 'json',
            headers: {
                'Authorization': 'Bearer ' + AccessToken
            },
            success: function (result) {
                // parent.curParentNotificationOrMessage = result[0];
                parent.loadRelatedNotificationsAndMessages(result);
            },
            error: function (request, message, error) {
                console.log("Error: " + error);
                LogError(error, false);
            }
        });
    },

    loadRelatedNotificationsAndMessages: function (result) {
        $("#divNotificationsAll").hide();
        $("#divNotificationMessageThread").show();

        //Gather info for Send Message textarea 
        var otherPartyUserID;
        var curContractID;
        var curUserID = UserProfile.UserID;
        //get sender user id
        if (result[0].SenderUserID != null) { //it's a notification (reply back to sender)
            otherPartyUserID = result[0].SenderUserID;
        } else if (result[0].MessagesRecipientUserID != null && result[0].MessagesRecipientUserID != curUserID) { //it's a message and recipient is not current user
            otherPartyUserID = result[0].MessagesRecipientUserID;
        } else if (result[0].MessagesSenderUserID != null && result[0].MessagesSenderUserID != curUserID) { //it's a message and sender is not current user
            otherPartyUserID = result[0].MessagesSenderUserID;
        }
        //get contractID
        if (result[0].ContractID != null) {
            curContractID = result[0].ContractID;
        } else if (result[0].MessagesContractID != null) {
            curContractID = result[0].MessagesContractID;
        }

        //MessageSend.Render(curUserID, otherPartyUserID, curContractID, $("#divNotificationMessageThread"));
        MessageSend.Render($("#divNotificationMessageThread"), curUserID, otherPartyUserID, curContractID);


        var strHTML = "";
        if (result.length > 0) {
            for (var i = 0; i < result.length; i++) {
                if (result[i].NotificationID != null && result[i].NotificationID > 0) { //It's a Notification
                    strHTML += '<div>';

                    strHTML += "<div>";
                    //strHTML += "<div>NotificationID: " + result[i].NotificationID + "</div>";
                    //strHTML += "<div>ContractID: " + result[i].ContractID + "</div>";
                    strHTML += "<div>" + FormatDateToShort(result[i].NotificationsDateCreated) + "</div>";
                    strHTML += "<div class='notification-subject'>" + result[i].Subject + "</div>";
                    strHTML += "<div>" + result[i].Body + "</div>";
                    strHTML += "</div>";

                    strHTML += '</div>';

                    $("#divNotificationMessageThread").append(strHTML);
                    strHTML = "";
                } else if (result[i].MessageID != null && result[i].MessageID > 0) { //It's a message
                    if (curUserID == result[i].MessagesSenderUserID) { //written by self 
                        strHTML += "<div class='message-from-self'>";
                    } else {
                        strHTML += "<div class='message-from-other'>";
                    }

                    strHTML += "<div>";
                    strHTML += "<div id='divAvatarMessageThread" + i + "'></div>";
                    //strHTML += "<div>MessageID: " + result[i].MessageID + "</div>";
                    //strHTML += "<div>ContractID: " + result[i].MessagesContractID + "</div>";
                    strHTML += "<div>" + FormatDateToShort(result[i].MessagesDateCreated) + "</div>";
                    //strHTML += "<div>" + result[i].Subject + "</div>";
                    strHTML += "<div>" + result[i].Message + "</div>";
                    strHTML += "</div>";

                    strHTML += '</div>';

                    $("#divNotificationMessageThread").append(strHTML);
                    strHTML = "";

                    //get avatar
                    
                    //Avatar.RenderThumbnailImage(result[i].MessagesSenderUserID, $("#divAvatarMessageThread" + i), false);
                    Avatar.Render($("#divAvatarMessageThread" + i), result[i].MessagesSenderUserID, 50, null, false);
                }
            }
        }
    },



};