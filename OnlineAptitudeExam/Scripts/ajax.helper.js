
/**
 * @param {String} selector
 * @param {String} into
 * @param {String} rootPath
 * @param {Function} callback
 * @param {String} action
 */
function prepareMouseAction(selector, into = null, rootPath = null, callback = null, action = "click") {
    $(selector).off(action).on(action, function (e) {
        e.preventDefault();
        let url;
        if ($(this).get(0).tagName.toUpperCase() === "A") {
            url = $(this).attr("href");
        } else {
            url = $(this).data("url");
        }
        load(url, into, rootPath, callback);
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
function prepareKeyboardAction(selector, into = null, rootPath = null, callback = null, type = "GET", action = "keyup") {
    $(selector).off(action).on(action, function () {
        let url = $(this).data("url") + removeUrlParam(location.search, "page")
        let val = $(this).val()
        let paramKey = $(this).data("param-key");

        if (val != undefined && val.length > 0) {
            url = replaceUrlParam(url, paramKey, val)
        } else {
            url = removeUrlParam(url, paramKey)
        }

        load(url, into, rootPath, callback, type);
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

function pendingFocus(modal, ele) {
    modal.on('shown.bs.modal', function () {
        ele.focus();
    });
}

function clearFormElements(ele) {
    ele.find(":input").removeClass("valid error");
    ele[0].reset();
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
function showConfirm($title, $msg, $btnType = "danger", $icon = null, callback = null, autoHide = true) {
    let mModal = $("#confirmDialog");
    let mIcon = mModal.find(".modal-header i")
    let mSubmit = mModal.find("button[submit]")
    mModal.find(".modal-title").text($title)
    mModal.find(".modal-body").text($msg)

    mSubmit.removeClass();
    mSubmit.addClass("btn btn-" + $btnType)

    mIcon.removeClass();
    if ($icon != null) {
        mIcon.addClass("mdi mdi-" + $icon)
    }

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
        success: $param.success,
        error: $param.error
    });
}

function adapter_ajax_with_file($param) {
    $.ajax({
        url: $param.url,
        type: $param.type,
        data: $param.data,
        contentType: false,
        processData: false,
        success: $param.success,
        error: $param.error
    });
}

/**
 * 
 * @param {String} url
 * @param {String} into
 * @param {String} rootPath
 * @param {Function, String} callback function name with empty param
 * @param {String} type POST | GET
 * @param {Object} data
 */
function load(url, into, rootPath, callback = null, type = "GET", data = null) {
    if (url.startsWith(_PREFIX)) {
        url = _AJAX_PREFIX + url;
    }
    if (rootPath != null) {
        let realUrl = rootPath + getTailUrl(url)
        let pushData = {
            into: into,
            loadUrl: url,
            realUrl: realUrl,
            type: type,
        }
        if (typeof callback == "function" && callback.name !== "") {
            pushData.callback = callback.name;
        }
        if (history.state == null || history.state.realUrl !== realUrl) {
            history.pushState(pushData, null, realUrl);
        }
    }
    loadUrl(url, function (data) {
        if (into != null) {
            if (typeof into == 'string') {
                $(into).html(data);
            }
            if (typeof into == 'object') {
                into.html(data);
            }
        }
        if (callback != null) {
            callback(data);
        }
    }, null, type, data);
}

function loadUrl(url, success = null, error = null, type = "GET", data = null) {
    var $param = {
        type: type,
        url: url,
        data: data,
        success: success,
        error: error,
    }
    adapter_ajax($param);
}

function loadScripts(src, raw = false) {
    let contentScriptSelector = "body #contentScript"
    let contentScript = $(contentScriptSelector);
    if (contentScript.length == 0) {
        $("body").append("<div id='contentScript'></div>")
        contentScript = $(contentScriptSelector);
    }
    let content = raw ?
        "<script>" + src + "</script>" :
        "<script src='" + src + "'></script>";
    contentScript.html(content);
    contentScript.remove();
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

function replaceUrlParam(url, paramName, paramValue) {
    if (paramValue == null) {
        paramValue = '';
    }
    var pattern = new RegExp('\\b(' + paramName + '=).*?(&|#|$)');
    if (url.search(pattern) >= 0) {
        return url.replace(pattern, '$1' + paramValue + '$2');
    }
    url = url.replace(/[?#]$/, '');
    return url + (url.indexOf('?') > 0 ? '&' : '?') + paramName + '=' + paramValue;
}

function removeUrlParam(url, paramName) {
    var rtn = url.split("?")[0],
        param,
        params_arr = [],
        queryString = (url.indexOf("?") !== -1) ? url.split("?")[1] : "";
    if (queryString !== "") {
        params_arr = queryString.split("&");
        for (var i = params_arr.length - 1; i >= 0; i -= 1) {
            param = params_arr[i].split("=")[0];
            if (param === paramName) {
                params_arr.splice(i, 1);
            }
        }
        if (params_arr.length) rtn = rtn + "?" + params_arr.join("&");
    }
    return rtn;
}

function objectifyForm(formElement) {
    formArray = formElement.serializeArray();
    //serialize data function
    var returnArray = {};
    for (var i = 0; i < formArray.length; i++) {
        returnArray[formArray[i]['name']] = formArray[i]['value'];
    }
    return returnArray;
}

function ucfirst(string) {
    return string.charAt(0).toUpperCase() + string.slice(1)
}