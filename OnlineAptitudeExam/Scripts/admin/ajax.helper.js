 
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
        prepareAction(url, into, rootPath, data => {
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
        prepareAction(url, into, rootPath, callback);
    });
}

function prepareAction(url,into, rootPath, callback) {
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

function loadUrl(url, success = null) {
    var $param = {
        type: "GET",
        url: url,
        data: null,
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