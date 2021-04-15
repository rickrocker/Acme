//get query string params
var UrlParams;
(window.onpopstate = function () {
    var match,
        pl = /\+/g,  // Regex for replacing addition symbol with a space
        search = /([^&=]+)=?([^&]*)/g,
        decode = function (s) { return decodeURIComponent(s.replace(pl, " ")); },
        //query = window.location.search.substring(1).toLowerCase();
        query = window.location.search.substring(1);

    UrlParams = {};
    while (match = search.exec(query))
        UrlParams[decode(match[1])] = decode(match[2]);
})();

function JumpToAnchor(anchor) {
    var url = location.href;
    location.href = "#" + anchor;
    history.replaceState(null, null, url);
}


function LogErrorMessage(errorMessage) { //deprecated! use LogError instead
    LogError(errorMessage, true);
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

function ClearEror() {
    $('#divError').hide();
    $('#divErrorMain').remove();
}

function LogSuccess(message, displayToUser) {
    console.log("Error: " + message);


    if (displayToUser) {
        var alertHTML = '<div id="divSuccessMain" class="row">\
            <div class="col-xs-12">\
                <div id="divSuccess" class="alert alert-success collapse">\
                    <a id="linkClose" class="close" href="#">&times;</a>\
                    <div id="divSuccessText"></div>\
                </div>\
             </div>\
          </div>';
        if ($('#cntSuccess').length) { //this id exists in DOM
            $("#cntSuccess").append(alertHTML);
        } else {
            //$('.navbar').insertAfter(alertHTML);
            alert(message);
        }
        $("#divSuccessText").text(message);
        $('#divSuccess').show();

        $('#linkClose').click(function () {
            $('#divSuccess').hide();
            $('#divSuccessMain').remove();
        });
    }
}

var myModal;
function ToggleModal(jqueryModalObj, showModal) {
    myModal = jqueryModalObj;
    if (showModal) {
        $("#divMain").prepend('<div id="divModalBkg" class="modal-bg modal-bg-show" onclick="ToggleModal(myModal, false)"></div>');
        jqueryModalObj.prepend('<span class="modal-close-button" onclick="ToggleModal(myModal, false)">&times;</span>');

        //center modal horizontally 
        jqueryModalObj.addClass("modal-show");
        var myWidth = Math.round(jqueryModalObj.outerWidth());
        var halfMyWidth = Math.round(myWidth / 2);

        //jqueryModalObj.css('left', 'calc(0vw)');
        jqueryModalObj.css('left', 'calc((100vw - ' + myWidth + 'px)/2)');
        jqueryModalObj.css('top', '70px');
        jqueryModalObj.addClass("modal-show");
    } else {
        $("#divModalBkg").remove();
        jqueryModalObj.removeClass("modal-show");
        $(".modal-close-button").remove();
    }
}

function GetAge(birthday) { // birthday is a date
    var ageDifMs = Date.now() - birthday.getTime();
    var ageDate = new Date(ageDifMs);
    return Math.abs(ageDate.getUTCFullYear() - 1970);
}

function GetLocalDateTime(utcDateTime) {
    return new Date(utcDateTime);
}

function FormatDateToLong(date) {
    var myDate = new Date(date);
    var monthNames = [
        "January", "February", "March",
        "April", "May", "June", "July",
        "August", "September", "October",
        "November", "December"
    ];

    var day = myDate.getDate();
    var monthIndex = myDate.getMonth();
    var year = myDate.getFullYear();

    return monthNames[monthIndex] + ' ' + day + ', ' + year;
}

function FormatDateToShort(date) {
    var myDate = new Date(date);
    var monthNames = [
        "Jan", "Feb", "Mar",
        "Apr", "May", "Jun", "Jul",
        "Aug", "Sep", "Oct",
        "Nov", "Dec"
    ];

    var day = myDate.getDate();
    var monthIndex = myDate.getMonth();
    var year = myDate.getFullYear();

    return monthNames[monthIndex] + ' ' + day + ', ' + year;
}


function MinutesToString(minutes) {
    var value = minutes;

    var units = {
        "day": 24 * 60,
        "hour": 60,
        "min": 1
    }

    var result = []

    for (var name in units) {
        var p = Math.floor(value / units[name]);
        if (p == 1) result.push(" " + p + " " + name);
        if (p >= 2) result.push(" " + p + " " + name + "s");
        value %= units[name]
    }

    return result;
}

function NumberWithCommas(x) {
    return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}


function TitleCase(str) { //sets first letter of each word to Capital letter
    str = str.toLowerCase().split(' ');
    for (var i = 0; i < str.length; i++) {
        str[i] = str[i].charAt(0).toUpperCase() + str[i].slice(1);
    }
    return str.join(' ');
}

function GetQueryStringValue(queryString, variable) {
    //var query = window.location.search.substring(1);
    var query = queryString.substring(1);
    var vars = query.split('&');
    for (var i = 0; i < vars.length; i++) {
        var pair = vars[i].split('=');
        if (decodeURIComponent(pair[0]) == variable) {
            return decodeURIComponent(pair[1]);
        }
    }
    console.log('Querystring variable %s not found', variable);
}


function OpenTabContent(evt, name) {
    var i, tabcontent, tablinks;
    tabcontent = document.getElementsByClassName("tabcontent");
    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = "none";
    }
    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active", "");
    }
    document.getElementById(name).style.display = "block";
    if (evt != null) {
        evt.currentTarget.className += " active";
    }
}

function EventFire(jqueryElement, eventType) { //used to mimic mouse click [ usage e.g.: EventFire(document.getElementById('myButton'), 'click'); ]
    try {
        if (jqueryElement.fireEvent) {
            jqueryElement.fireEvent('on' + eventType);
        } else {
            var evObj = document.createEvent('Events');
            evObj.initEvent(eventType, true, false);
            jqueryElement.dispatchEvent(evObj);
        }
    } catch (ex) {
        console.log("error: " + ex);
        LogError(ex, false);
    }
}

function GetFriendlyFirstLastName(firstName, lastName) {
    var strName = "";
    if (firstName != null && firstName != undefined && firstName.trim().toLowerCase() != "null") {
        strName += firstName.trim();
    }
    if (lastName != null && lastName != undefined && lastName.trim().toLowerCase() != "null") {
        if (strName.length > 0) {
            strName += " ";
        }
        strName += lastName.trim();
    }
    return strName;
}

function DataTablesEditorModalFix() {
    //bring z-index of modals forward so they don't pop up behind current content
    $(".DTED_Lightbox_Background").css('z-index', 170);
    $(".DTED_Lightbox_Wrapper").css('z-index', 171);
}


//"includes" Prototype for IE
String.prototype.includes = function (str) {
    var returnValue = false;

    if (this.indexOf(str) !== -1) {
        returnValue = true;
    }

    return returnValue;
}
