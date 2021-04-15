//const WebApiBaseUrl = WebAPIUrl;
//const WebApiUrl = WebAPIUrl;

// detect back/forward button navigation
window.onpopstate = function (e) {
    if (e.state) {
        Page.Load(e.state.html);
        //document.getElementById("divMain").innerHTML = e.state.html;
        //document.title = e.state.pageTitle;
    }
};

function JumpToAnchor(anchor) {
    var url = location.href;
    location.href = "#" + anchor;
    history.replaceState(null, null, url);
}

function AddBaconIpsum() {
    $.get("https://baconipsum.com/api/?type=all-meat&sentences=5&start-with-lorem=1", function (data) {
        $('#divBaconIpsum').prepend('<p>' + data + '</p>');
    });
}

function ToggleNav() {
    var x = document.getElementById("divTopNav");
    if (x.className === "topnav") {
        x.className += " responsive";
    } else {
        x.className = "topnav";
    }
}

function LogError(errorMessage, displayToUser) {
    console.log("Error: " + errorMessage);


    if (displayToUser) {
        var alertHTML = '<div id="divErrorMain" class="row">\
            <div class="col-xs-12">\
                <div id="divError" class="alert alert-danger collapse">\
                    <a id="linkClose" class="close" href="#">&times;</a>\
                    <div id="divErrorText"></div>\
                </div>\
             </div>\
          </div>';
        if ($('#cntError').length) { //this id exists in DOM
            $("#cntError").append(alertHTML);
        } else {
            //$('.navbar').insertAfter(alertHTML);
            alert(errorMessage);
        }
        $("#divErrorText").text(errorMessage);
        $('#divError').show();

        $('#linkClose').click(function () {
            $('#divError').hide();
            $('#divErrorMain').remove();
        });
    }
}

$(document).ready(function () {
    Init.Start();
});


var Init = {

    Start: function () {

        $("#spnCurrentYear").html(new Date().getFullYear());

        Header.Render($("#divHeader"));
        Footer.Render($("#divFooter"));

        //which page are we at?
        var path = window.location.pathname;
        var queryString = window.location.search;
        var page;
        if (path == "/" || path.toLowerCase().includes("index")) {
            page = "home";
        } else {
            page = path.substring(path.indexOf("/") + 1)
        }

        Page.Load(page, queryString);
            
    },

};