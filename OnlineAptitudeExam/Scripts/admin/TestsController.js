let mRootPath, mDataPath;
const mMouseSeletor = "#contentPager a[href], #contentTable table th .sortable";
const mInto = "#contentTable";

let prepareTable = () => {
    refreshTableItemSort("#tableTests");
    $(mInto).find(".form-switch .form-check-input").each(function () {
        $(this).off("click").on("click", e => {
            e.preventDefault();
            TestsToggleStatus($(this), $(this).data("url"));
        })
    })
}

let prepare = () => {
    prepareMouseAction(mMouseSeletor, mInto, mRootPath, prepareTable);
}

function TestsIndex(rootPath, dataPath) {
    mRootPath = rootPath;
    mDataPath = dataPath;
    
    prepareKeyboardAction("#searchTests", mInto, mRootPath, () => {
        prepare();
        prepareTable();
    });
    prepare();
    prepareTable();
    // (filter search) are field in action Tests/GetData
    refreshSearchBar("#searchTests", "filter search");
}

function showTestsModal(element, isCreate, url) {
    // setup variable
    let mModal = $("#testsModal"),
        mEdtName = mModal.find("#Name"),
        mTitle = mModal.find(".modal-title"),
        mSubmit = mModal.find(".modal-submit")

    // setup data
    clearFormElements(mModal);
    pendingFocus(mModal, mEdtName);
    let s = isCreate ? "Create" : "Update";
    mTitle.text(s)
    mSubmit.text(s)

    let id = null;
    if (!isCreate) {
        let mTr = element.closest("tr");
        id = mTr.data("id");
        mEdtName.val(mTr.find(".field-name").text().trim())
    }

    mSubmit.off("click").on("click", function () {
        let name = mEdtName.val();
        let data = {
            id: id,
            name: name
        }
        if (isCreate) {
            TestsCreate(mModal, url, data)
        } else {
            TestsUpdate(mModal, element, url, data)
        }

    })
    mModal.modal("show");
}

function TestsCreate(mModal, url, data) {
    loadUrl(url, data => {
        if (data.success) {
            mModal.modal("hide")
            showToast(data.message, "success")
            load(mDataPath, mInto, mRootPath, prepare);
        } else {
            showToast(data.message, "error")
        }
    }, "POST", data)
}

function TestsUpdate(mModal, element, url, data) {
    loadUrl(url, data => {
        if (data.success) {
            mModal.modal("hide")
            showToast(data.message, "success")
            let tr = element.closest("tr");
            tr.find(".field-name").text(data.data)
        } else {
            showToast(data.message, "error")
        }
    }, "POST", data)
}

function TestsToggleStatus(element, url) {
    let id = element.closest("tr").data("id");
    loadUrl(url, data => {
        if (data.success) {
            showToast(data.message, "success")
            element.prop('checked', data.data.status == 1);
        } else {
            showToast(data.message, "error")
        }
    }, "POST", { id: id })
}

function showDeleteTestsModal(element, url) {
    showConfirm("Delete test",
        "Are you sure to delete this record?",
        "outline-danger", () => TestsDelete(element, url))
}

function TestsDelete(element, url) {
    let id = element.closest("tr").data("id");
    loadUrl(url, data => {
        if (data.success) {
            showToast(data.message, "success")
            load(mDataPath + window.location.search, mInto, null, prepare);
        } else {
            showToast(data.message, "error")
        }
    }, "POST", { id: id })
}