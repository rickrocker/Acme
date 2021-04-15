var Avatar = {

    //defaultImageUrl: AvatarUrl + "/0_Profile.png",
    //defaultSmallImageUrl: AvatarUrl + "/0_sm_Profile.png",

    ErrorImageTagLarge: " onerror=\"this.onerror=null;this.src='" + AvatarUrl + "0_Profile.png';\"",
    ErrorImageTagSmall: " onerror=\"this.onerror=null;this.src='" + AvatarUrl + "0_sm_Profile.png';\"",

    Render: function (jqueryObj, userId, width, height, isSquare) {
        var myWidthHTML = "";
        var myHeightHTML = "";

        if (width != null) {
            myWidthHTML = "width: " + width + "px;";
        }
        if (height != null) {
            myHeightHTML = "height: " + height + "px;";
        }
        

        if (width <= 100) {
            if (isSquare) {
                jqueryObj.append("<img src='" + AvatarUrl + userId + "/" + userId + ".png" + "' style='" + myWidthHTML + myHeightHTML + "'" + this.ErrorImageTagSmall + " />");
            } else {
                jqueryObj.append("<img src='" + AvatarUrl + userId + "/" + userId + ".png" + "' style='" + myWidthHTML + myHeightHTML + " border-radius: 50%;'" + this.ErrorImageTagSmall + " />");
            }
        } else {
            if (isSquare) {
                jqueryObj.append("<img src='" + AvatarUrl + userId + "/" + "sm_" + userId + ".png" + "' style='" + myWidthHTML + myHeightHTML + "'" + this.ErrorImageTagLarge + " />");
            } else {
                jqueryObj.append("<img src='" + AvatarUrl + userId + "/" + "sm_" + userId + ".png" + "' style='" + myWidthHTML + myHeightHTML + " border-radius: 50%;'" + this.ErrorImageTagLarge + " />");
            }
        }
    },

    GetURL: function (userId, isSmall) {
        if (isSmall) {
            return AvatarUrl  + userId + "/" + "sm_" + userId + ".png";
        } else {
            return AvatarUrl + userId + "/" + userId + ".png";
        }
    }

};