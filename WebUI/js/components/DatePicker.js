var DatePicker = {

    htmlText: function (instanceID) {
        return '<select id="year' + instanceID + '" class="form-control" style="width:unset;"></select>\
            <select id="month' + instanceID + '" class="form-control" style="width:unset;"></select>\
            <select id="day' + instanceID + '" class="form-control" style="width:unset;"></select>\
        <div id="divErrorDate' + instanceID + '" class="form-error" style="display:none;">Enter a date</div>';
    },

    Render: function (jqueryObj, instanceID, minYear, maxYear) {

        jqueryObj.append(this.htmlText(instanceID));

        if (minYear == null) {
            minYear = (new Date()).getFullYear() - 100;
        }
        if (maxYear == null) {
            maxYear = (new Date()).getFullYear();
        }
        var monthNames = ["January", "February", "March", "April", "May", "June",
            "July", "August", "September", "October", "November", "December"
        ];
        var qntYears = maxYear - minYear;
        var selectYear = $("#year" + instanceID);
        var selectMonth = $("#month" + instanceID);
        var selectDay = $("#day" + instanceID);
        var currentYear = new Date().getFullYear();

        for (var y = 0; y < qntYears; y++) {
            let date = new Date(currentYear);
            var yearElem = document.createElement("option");
            yearElem.value = currentYear
            yearElem.textContent = currentYear;
            selectYear.append(yearElem);
            currentYear--;
        }

        for (var m = 0; m < 12; m++) {
            let monthNum = new Date(maxYear, m).getMonth()
            let month = monthNames[monthNum];
            var monthElem = document.createElement("option");
            monthElem.value = monthNum;
            monthElem.textContent = month;
            selectMonth.append(monthElem);
        }

        var d = new Date();
        var month = d.getMonth();
        var year = d.getFullYear();
        var day = d.getDate();

        selectYear.val(year);
        selectYear.on("change", adjustDays);
        selectMonth.val(month);
        selectMonth.on("change", adjustDays);

        adjustDays();
        selectDay.val(day);

        function adjustDays() {
            var year = selectYear.val();
            var month = parseInt(selectMonth.val()) + 1;
            selectDay.empty();

            //get the last day, so the number of days in that month
            var days = new Date(year, month, 0).getDate();

            //lets create the days of that month
            for (var d = 1; d <= days; d++) {
                var dayElem = document.createElement("option");
                dayElem.value = d;
                dayElem.textContent = d;
                selectDay.append(dayElem);
            }
        }
    },

    LoadDate: function (instanceNumber, myDate) {
        $("#year" + instanceNumber).val(myDate.getFullYear());
        $("#month" + instanceNumber).val(myDate.getMonth());
        $("#day" + instanceNumber).val(myDate.getDate());
    },

    GetSelectedDate: function (instanceNumber) {
        var year = $("#year" + instanceNumber).val();
        var month = $("#month" + instanceNumber).val();
        var day = $("#day" + instanceNumber).val();
        var myDate = new Date(year, month, day);
        return myDate;
    },

  
    GetSelectedDateMMddYYYY: function (instanceNumber) {
        var year = $("#year" + instanceNumber).val();
        var month = $("#month" + instanceNumber).val();
        var day = $("#day" + instanceNumber).val();
        var myDate = new Date(year, month, day);
        return (myDate.getMonth() + 1) + '/' + myDate.getDate() + '/' + myDate.getFullYear();
    },

    Validate: function (instanceNumber, errorMessage) {
        if (new Date(this.GetSelectedDate(instanceNumber)).getTime() == new Date(Date.now()).setHours(0,0,0,0)) {
            if (errorMessage != null) {
                $("#divErrorDate" + instanceNumber).text(errorMessage);
            }
            $("#divErrorDate" + instanceNumber).show();
            return false;
        } else {
            return true;
        }
    }

}