
/**
 * @param {String} selector
 * @param {String} into
 * @param {String} rootPath
 * @param {Function} callback
 * @param {String} action
 */
function prepareMouseAction(selector, into = "#contentTable", rootPath = null, callback = null, action = "click") {
    $(selector).off(action).on(action, function (e) {
        e.preventDefault();
        let url;
        if ($(this).get(0).tagName.toUpperCase() === "A") {
            url = $(this).attr("href");
        } else {
            url = $(this).data("url");
        }
        load(url, into, rootPath, data => {
            if (callback != null) {
                callback(data);
            }
            prepareMouseAction(selector, into, rootPath, callback, action);
        });
    });
}

/**
 * 
 * @param {String} selector
 * @param {String} into
 * @param {String} rootPath
 * @param {Function} callback
 * @param {String} action
 */
function prepareKeyboardAction(selector, into = "#contentTable", rootPath = null, callback = null, action = "keyup") {
    $(selector).off(action).on(action, function () {
        let url = $(this).data("url");
        let val = $(this).val();
        if (val != undefined && val.length > 0) {
            url += "?" + $(this).data("param-key") + "=" + val;
        }
        load(url, into, rootPath, callback);
    });
}

function refreshTableItemSort(tableId) {
    let tbl = $(tableId);
    let currentSort = tbl.data("sort");
    let currentOrder = tbl.data("order");

    tbl.find("thead th .sortable").each(function () {
        $(this).removeClass("asc desc");
        if ($(this).data("sort") === currentSort) {
            $(this).addClass(currentOrder);
            return false;
        }
    })
}

/**
 * 
 * @param {String} searchId
 * @param {String} varNames
 */
function refreshSearchBar(searchId, varNames) {
    $(searchId).val(getUrlParam(varNames));
}

function clearFormElements(ele) {
    $(ele).find(':input').each(function () {
        switch (this.type) {
            case 'password':
            case 'select-multiple':
            case 'select-one':
            case 'text':
            case 'textarea':
                $(this).val('');
                break;
            case 'checkbox':
            case 'radio':
                this.checked = false;
        }
    });
}

function pendingFocus(modal, element) {
    modal.on('shown.bs.modal', function () {
        element.focus();
    });
}

/**
 * 
 * @param {String} $msg
 * @param {String} $type
 * @param {String} $title
 * @param {Integer} $duration
 */
function showToast($msg, $type, $title = "", $duration = 3000) {
    $toast = {
        "title": $title != "" ? $title : ucfirst($type) + "!",
        "message": $msg,
        "type": $type,
        "duration": $duration
    };
    toast($toast);
}

/**
 * 
 * @param {String} $title
 * @param {String} $msg
 * @param {String} $submitType class of bootstrap
 * @param {Function} callback
 * @param {Boolean} autoHide
 */
function showConfirm($title, $msg, $btnType = "danger", callback = null, autoHide = true) {
    let mModal = $("#confirmDialog");
    let mSubmit = mModal.find("button[submit]")
    mModal.find(".modal-title").text($title)
    mModal.find(".modal-body").text($msg)

    mSubmit.removeClass();
    mSubmit.addClass("btn btn-" + $btnType)

    mSubmit.off("click").on("click", () => {
        if (autoHide) {
            mModal.modal("hide")
        }
        if (callback != null) {
            callback()
        }
    })
    mModal.modal("show")
}

function hideConfirm() {
    $("#confirmDialog").modal("hide");
}

// ============================= Utils ================================

function adapter_ajax($param) {
    $.ajax({
        url: $param.url,
        type: $param.type,
        data: $param.data,
        async: true,
        success: $param.callback,
    });
}

function adapter_ajax_with_file($param) {
    $.ajax({
        url: $param.url,
        type: $param.type,
        data: $param.data,
        contentType: false,
        processData: false,
        success: $param.callback,
    });
}

function load(url, into, rootPath, callback) {
    if (rootPath != null) {
        window.history.pushState(null, null, rootPath + getTailUrl(url));
    }
    loadUrl(url, function (data) {
        if (into != null) {
            $(into).html(data);
        }
        if (callback != null) {
            callback(data);
        }
    });
}

function loadUrl(url, success = null, type = "GET", data = null) {
    var $param = {
        type: type,
        url: url,
        data: data,
        callback: success,
    }
    adapter_ajax($param);
}

function getTailUrl(url) {
    let lastIndex = url.lastIndexOf("?");
    return lastIndex != -1 ? url.substring(lastIndex) : "";
}

function getTailUrlWidthoutQuestionMark(url) {
    let lastIndex = url.lastIndexOf("?");
    return lastIndex != -1 ? url.substring(lastIndex + 1) : "";
}

function getUrlParam(varNames) {
    let searchParams = new URLSearchParams(window.location.search)
    varNames = varNames.split(" ");
    for (let i = 0; i < varNames.length; i++) {
        const name = varNames[i].trim();
        if (searchParams.has(name)) {
            return searchParams.get(name);
        }
    }
    return null;
}

function ucfirst(string) {
    return string.charAt(0).toUpperCase() + string.slice(1)
}