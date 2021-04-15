//IMPORTANT: always start intanceNumber as 0, and then increment by 1 (when createing new instance);
//NOTE: instanceNumber corresponds with SortOrder
var CategorySubcategoryAjaxRequests;
var CategorySubcategory = {


    Render: function (jqueryObj, instanceNumber, boolLoadValuesForCurrentUser) {
        CategorySubcategoryAjaxRequests = 0;
        jqueryObj.append(this.htmlText(instanceNumber));
        this.getCategories(instanceNumber, boolLoadValuesForCurrentUser);
        this.addClickEvents(instanceNumber);
    },

    AppendTooltip: function (tooltipText) {
        var myStr = '<span class="tooltip">\
            <i class="fas fa-question-circle"></i>\
            <span class="tooltip-text">\
                <div>' + tooltipText + '</div>\
            </span>\
        </span>';
        $("#lblSelCategory").append(myStr);
    },


    htmlText: function (instanceNumber) {
        return '<div>\
            <label id="lblSelCategory" for="selCategory' + instanceNumber + '">Category</label>\
            <select id="selCategory' + instanceNumber + '" class="form-control"></select>\
        <div id="divErrorCategory'+ instanceNumber + '" class="form-error" style="display:none;">Choose a category</div>\
            </div>\
    <div id="divSubcategory' + instanceNumber + '" style="display:none">\
        <label for="selSubcategory' + instanceNumber + '">Subcategory</label>\
        <select id="selSubcategory' + instanceNumber + '" class="form-control"></select>\
    </div>';
    },

    addClickEvents: function (instanceNumber) {
        var parent = this;
        $('#selCategory' + instanceNumber).change(function () {
            parent.dropDownChangedByUser = true;
            parent.loadSubcategories(instanceNumber);
        });
        $('#selSubcategory' + instanceNumber).change(function () {
            parent.dropDownChangedByUser = true;
        });
    },

    ValidateCategory: function (instanceNumber) {
        if ($("#selCategory" + instanceNumber).val() == -1) {
            $("#divErrorCategory" + instanceNumber).show();
            return false;
        } else {
            return true;
        }
    },


    getCategories: function (instanceNumber, boolLoadValuesForCurrentUser) {
        var parent = this;
        CategorySubcategoryAjaxRequests++;
        $.ajax({
            url: WebApiBaseUrl + '/api/Categories',
            type: 'GET',
            headers: {
                'Authorization': 'Bearer ' + AccessToken
            },
            success: function (result) {
                var $dropdown = $("#selCategory" + instanceNumber);
                $('#selCategory' + instanceNumber).append($('<option />').val('-1').text('- Choose a Category -'));
                $.each(result, function () {
                    $dropdown.append($("<option />").val(this.ID).text(this.CategoryName));
                });
                parent.getSubcategories(instanceNumber, boolLoadValuesForCurrentUser);
                CategorySubcategoryAjaxRequests--;

            },
            error: function (request, message, error) {
                CategorySubcategoryAjaxRequests--;
                console.log("Error: " + error);
                LogError(error, true);
            }
        });
    },

    dropDownChangedByUser: false,
    subcategories: '',

    getSubcategories: function (instanceNumber, boolLoadValuesForCurrentUser) {
        var that = this;
        CategorySubcategoryAjaxRequests++;
        $.ajax({
            url: WebApiBaseUrl + '/api/Categories/GetSubcategories',
            type: 'GET',
            headers: {
                'Authorization': 'Bearer ' + AccessToken
            },
            success: function (result) {
                that.subcategories = result;
                if (boolLoadValuesForCurrentUser) {
                    that.loadForCurrentUser(instanceNumber);
                }
                CategorySubcategoryAjaxRequests--;
            },
            error: function (request, message, error) {
                CategorySubcategoryAjaxRequests--;
                console.log("Error: " + error);
                LogError(error, true);
            }
        });
    },

    loadSubcategories: function (instanceNumber) { //populate subcategory dropdowns
        $('#selSubcategory' + instanceNumber).empty();
        var categoryID = $('#selCategory' + instanceNumber).val();
        var hasSubCategory = false;
        var firstTime = true;
        for (var i = 0; i < this.subcategories.length; i++) {
            if (this.subcategories[i].CategoryID == categoryID) {
                if (firstTime) {
                    $('#selSubcategory' + instanceNumber).append($('<option />').val('-1').text('- Choose a Subcategory -'));
                    firstTime = false;
                }
                hasSubCategory = true;
                $('#selSubcategory' + instanceNumber).append($('<option />').val(this.subcategories[i].ID).text(this.subcategories[i].SubcategoryName));

            }

        }
        if (!hasSubCategory) {
            document.getElementById('selSubcategory' + instanceNumber).options.length = 0;
            //$('#selSubcategory' + instanceNumber).prop('options').length = 0;
            $("#divSubcategory" + instanceNumber).hide();
        } else {
            $('#divSubcategory' + instanceNumber).show();
        }
    },


    loadForCurrentUser: function (instanceNumber) {
        var that = this;
        CategorySubcategoryAjaxRequests++;
        $.ajax({
            url: WebApiBaseUrl + '/api/Categories/GetCategoriesByCurrentUser/' + instanceNumber,
            type: 'GET',
            dataType: 'json',
            headers: {
                'Authorization': 'Bearer ' + AccessToken
            },
            success: function (result) {
                if (result == "Record not found" || result == null || result.length == 0) {
                    console.log("Record not found");
                } else {
                    //console.log("success result: " + JSON.stringify(result));
                    $("#selCategory" + instanceNumber).val(result[0].CategoryID);
                    if (result[0].SubcategoryID != null) {
                        that.loadSubcategories(instanceNumber);
                        $("#divSubcategory" + instanceNumber).show();
                        $("#selSubcategory" + instanceNumber).val(result[0].SubcategoryID);
                    }
                }
                CategorySubcategoryAjaxRequests--;
            },
            error: function (request, message, error) {
                CategorySubcategoryAjaxRequests--;
                console.log("Error: " + error);
                LogError(error, true);
            }
        });
    },

    SaveForCurrentUser: function (instanceNumber) {
        if (this.dropDownChangedByUser) {
            $.ajax({
                url: WebApiBaseUrl + '/api/Categories/',
                type: 'POST',
                headers: {
                    'Authorization': 'Bearer ' + AccessToken
                },
                data: {
                    CategoryID: $('#selCategory' + instanceNumber).val(),
                    SubCategoryID: $('#selSubcategory' + instanceNumber).val(),
                    SortOrder: instanceNumber
                    //'': dataArray

                },
                success: function (result) {
                    InfluencerProfileAjaxRequests--;
                    CategorySubcategoryAjaxRequests--;
                    return result;

                },
                error: function (request, message, error) {
                    CategorySubcategoryAjaxRequests--;
                    console.log("Error: " + error);
                    LogError(error, true);
                    return ("error: " + error)
                }
            });
        } else {
            InfluencerProfileAjaxRequests--;
        }
    },

    ChangeLabels: function (categoryLabelText, subcategoryLabelText, instanceNumber) {
        var selCategory = 'selCategory' + instanceNumber;
        $('label[for="' + selCategory + '"]').text(categoryLabelText);

        var selSubcategory = 'selSubcategory' + instanceNumber;
        $('label[for="' + selSubcategory + '"]').text(subcategoryLabelText);
    },

    RemoveLabels: function (instanceNumber) {
        var selCategory = 'selCategory' + instanceNumber;
        $('label[for="' + selCategory + '"]').hide();

        var selSubcategory = 'selSubcategory' + instanceNumber;
        $('label[for="' + selSubcategory + '"]').hide();
    }

}