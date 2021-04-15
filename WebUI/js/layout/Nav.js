var Nav = {

    Render: function (jqueryObj) {
        if (UserRole == "Advertiser") {
            jqueryObj.html(this.htmlTextAdvertiser());
        } else if (UserRole == "Influencer") {
            
            jqueryObj.html(this.htmlTextInfluencer());
        } else if (UserRole == "Influencer Invitee") {
            this.HideNav();
        } else if (UserRole == "Administrator") {
            jqueryObj.html(this.htmlTextAdvertiser());
        } else {
            this.HideNav();
        }

        this.addClickEvents();
    },

    htmlTextAdvertiser: function () {
        //var smallScreen = window.matchMedia("(max-width: 768px)");

        $(".nav").css("grid-template-columns", "repeat(7, 1fr)");

        return '\
    <div class="nav-item" onclick = "Page.Load(\'tasks\');">\
        <i class="nav-item-icon fa fa-tasks"></i>\
        <div class="nav-item-text">Tasks</div>\
    </div>\
    <div class="nav-item" onclick="Page.Load(\'notifications\');">\
        <i class="nav-item-icon far fa-bell"></i>\
        <div class="nav-item-text">Notifications</div>\
    </div>\
    <div class="nav-item" onclick="Page.Load(\'messages\');">\
        <i class="nav-item-icon far fa-comments"></i>\
        <div class="nav-item-text">Messages</div>\
    </div>\
    <div class="nav-item" onclick="Page.Load(\'campaigns\');">\
        <i class="nav-item-icon far fa-copy"></i>\
        <div class="nav-item-text">Campaigns</div>\
    </div>\
    <div class="nav-item" onclick="Page.Load(\'influencers\');">\
        <i class="nav-item-icon fas fa-users"></i>\
        <div class="nav-item-text">Influencers</div>\
    </div>\
    <div class="nav-item" onclick="Page.Load(\'profile\');">\
        <i class="nav-item-icon fas fa-cog"></i>\
        <div class="nav-item-text">Profile</div>\
    </div>\
    <div class="nav-item" onclick="Page.Load(\'analytics\');">\
        <i class="nav-item-icon fas fa-chart-line"></i>\
        <div class="nav-item-text">Analytics</div>\
    </div>\
    <div class="nav-item" onclick="Page.Load(\'support\');">\
        <i class="nav-item-icon fas fa-headset"></i>\
        <div class="nav-item-text">Support</div>\
    </div>';
    },

    htmlTextInfluencer: function () {
       $(".nav").css("grid-template-columns", "repeat(5, 1fr)");

        return '\
    <div class="nav-item" onclick = "Page.Load(\'tasks\');">\
        <i class="nav-item-icon fa fa-tasks"></i>\
        <div class="nav-item-text">Tasks</div>\
    </div>\
    <div class="nav-item" onclick="Page.Load(\'notifications\');">\
        <i class="nav-item-icon far fa-bell"></i>\
        <div class="nav-item-text">Notifications</div>\
    </div>\
    <div class="nav-item" onclick="Page.Load(\'messages\');">\
        <i class="nav-item-icon far fa-comments"></i>\
        <div class="nav-item-text">Messages</div>\
    </div>\
    <div class="nav-item" onclick="Page.Load(\'campaigns\');">\
        <i class="nav-item-icon far fa-copy"></i>\
        <div class="nav-item-text">Campaigns</div>\
    </div>\
    <div class="nav-item" onclick="Page.Load(\'settings\');">\
        <i class="nav-item-icon fas fa-cog"></i>\
        <div class="nav-item-text">Settings</div>\
    </div>';
    },

    addClickEvents: function () {
        $('.nav-item').on('click', function () {
            $('.nav-item').removeClass('selected');
            $('.nav-item-avatar').removeClass('selected');
            $(this).addClass('selected');
        });

    },

    HideNav: function () {
        $("#divMain").addClass("content-no-nav");
        $("#divNav").hide();
    },

    ShowNav: function () {
        $("#divMain").removeClass("content-no-nav");
        $("#divNav").show();
    }
};