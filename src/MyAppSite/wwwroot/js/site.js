// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.

const ApiUrl = 'http://localhost:50754/MyApp/',
    itemPerPage = 10,
    propertyType = {
        Hidden: 0,
        String: 1,
        Number: 2
    };

var loadingCount = 0;
function requestToServer(action, method, params, data, onSuccess) {
    $.ajax({
        url: ApiUrl + action + '?' + params,
        method: method,
        contentType: 'application/json;charset=utf-8',
        accept: 'application/json',
        data: method.toLowerCase() == 'post' ? JSON.stringify(data) : '',
        crossDomain: true,
        header: {
            'Content-Type': 'application/json'
        },
        processData: false,
        beforeSend: function () {
            showLoadingDiv();
        },
        error: function (result) {
            hideLoadingDiv();
            let errorMessage = 'Unknown error.';
            if (result && result.responseJSON) {
                if (result.responseJSON.Message) {
                    errorMessage = result.responseJSON.Message;
                }
                if (result.responseJSON.Code || result.responseJSON.Code === 0) {
                    errorMessage += ' Error code: ' + result.responseJSON.Code + '.';
                }
            }
            showMessage(errorMessage);
        },
        success: function (result) {
            if (result) {
                if (typeof onSuccess == 'function') {
                    onSuccess(result)
                }
                hideLoadingDiv();
            } else {
                hideLoadingDiv();
                showMessage(result.Message);
            }
        }
    });
}

function loadUsersPage(pageNumber) {
    requestToServer('GetUsers', 'GET', 'PageNumber=' + pageNumber, '', viewUserPage);
}

function viewUserPage(data) {
    if (data && data.properties && data.items) {
        $('#user-table').empty();
        resetActionsDiv();
        resetPaginationDiv();
        let tableHeader = '<table class="table table-striped table-hover">\n';
        tableHeader += '<thead>\n';
        tableHeader += '<tr>\n';
        tableHeader += '<th scope="col">\n';
        tableHeader += '<div title="Select all" class="custom-control custom-checkbox" style="margin-top: -6px;">\n';
        tableHeader += '<input type="checkbox" class="custom-control-input" id="check-user-row-all" onchange="selectAllCheckChange(this)">\n';
        tableHeader += '<label class="custom-control-label" for="check-user-row-all"></label>\n';
        tableHeader += '</div>\n';
        tableHeader += '</th>\n';
        tableHeader += '<th scope="col">#</th>\n';
        for (let i = 0; i < data.properties.length; i++) {
            if (data.properties[i].type != propertyType.Hidden) {
                tableHeader += '<th scope="col">' + data.properties[i].label + '</th>\n';
            }
        }

        tableHeader += '<th scope="col">Actions</th>\n';
        tableHeader += '</tr>\n</thead>\n'

        let tableBody = '<tbody>\n';

        for (var i = 0; i < data.items.length; i++) {
            tableBody += '<tr>\n';
            tableBody += '<td>';
            tableBody += '<div title="Select row" class="custom-control custom-checkbox" style="margin-top: -6px;">\n';
            tableBody += '<input type="checkbox" class="custom-control-input rowCheckbox" onchange="checkBoxCheckChange(event)" id="check-user-row' + data.items[i].uid + '">\n';
            tableBody += '<label class="custom-control-label" for="check-user-row' + data.items[i].uid + '"></label>\n';
            tableBody += '</div>\n';
            tableBody += '</td>\n';
            tableBody += '<td>' + ((data.currentPage - 1) * itemPerPage + i + 1) + '</td>\n';
            for (let j = 0; j < data.properties.length; j++) {
                if (data.properties[j].type != propertyType.Hidden) {
                    tableBody += '<td scope="row">' + data.items[i][data.properties[j].name] + '</th>\n';
                }
            }
            tableBody += '<td>';
            tableBody += '<div class="btn-group" style="margin: -3px 0;">';
            tableBody += '<button class="btn btn-info float-left" type="button" Title="Edit"><img width="24" src="images/edit.svg" alt="Edit Icon"/></button>';
            tableBody += '<button class="btn btn-danger float-left" type="button" Title="Delete"><img width="24" src="images/delete.svg" class="" alt="Delete Icon"/></button>';
            tableBody += '</div>';
            tableBody += '</td>\n';
            tableBody += '</tr>\n';
        }
        tableBody += '</table>\n';
        $('#user-table').append(tableHeader + tableBody);
    }
}

function selectAllCheckChange(sender) {
    $('#user-table input.rowCheckbox').prop('checked', sender.checked).trigger('change');
}

function checkBoxCheckChange(e) {
    let checked = $('#user-table input.rowCheckbox:Checked');
    let notChecked = $('#user-table input.rowCheckbox:not(:Checked)');
    let senderState = e.target.checked;

    $('#check-user-row-all').prop('indeterminate', false);
    if (checked.length == 0) {
        $('#check-user-row-all').prop('checked', false);
    } else if (notChecked.length == 0) {
        $('#check-user-row-all').prop('checked', true);
    } else {
        $('#check-user-row-all').prop('checked', false);
        $('#check-user-row-all').prop('indeterminate', true);
    }



    if (checked.length == 1) {
        if (senderState) {

        }
    }

    if (senderState) {
        $(e.target).parents('tr').addClass('selected');
    } else {
        $(e.target).parents('tr').removeClass('selected');
    }

}

function resetActionsDiv() {

}

function resetPaginationDiv() {

}

function showLoadingDiv() {
    loadingCount++;
    $('#loadingDiv').show();
}

function hideLoadingDiv() {
    loadingCount--;
    if (loadingCount < 1) {
        $('#loadingDiv').hide();
    }
}

function showMessage(message) {
    alert(message);
}