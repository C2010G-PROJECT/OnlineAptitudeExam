const _TestsTable = "#testsTable";
const _ACTION_Tests_Index = _PREFIX + "/" + "Tests"
const _ACTION_Tests_GetData = _ACTION_Tests_Index + "/GetData"
const _ACTION_Tests_Create = _ACTION_Tests_Index + "/Create"
const _ACTION_Tests_Update = _ACTION_Tests_Index + "/Update"
const _ACTION_Tests_Delete = _ACTION_Tests_Index + "/Delete"
const _ACTION_Tests_ToggleStatus = _ACTION_Tests_Index + "/ToggleStatus"
const _ACTION_Tests_Detail = _ACTION_Tests_Index + "/Detail"

function TestsIndex() {
    prepareSearchBar();
    prepareTestTable();
}

function prepareSearchBar() {
    // (filter search) are field in action Tests/GetData
    refreshSearchBar("#searchTests", "filter search");
    prepareKeyboardAction("#searchTests", _TestsTable, _ACTION_Tests_Index);
}

function prepareTestTable() {
    const mMouseSeletor = "#contentPager a[href], .table-custom thead th .sortable";
    refreshTableItemSort(".table-custom")
    prepareMouseAction(mMouseSeletor, _TestsTable, _ACTION_Tests_Index);
}

function showTestsModal(element, isCreate) {
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
        let name = mEdtName.val().trim();
        let data = {
            id: id,
            name: name
        }
        if (isCreate) {
            TestsCreate(mModal, data)
        } else {
            TestsUpdate(mModal, element, data)
        }

    })
    mModal.modal("show");
}

function showDeleteTestsModal(element) {
    showConfirm("Delete test",
        "Are you sure to delete this record?",
        "outline-danger",
        "delete-outline", () => TestsDelete(element))
}

function showDetailTest(element) {
    let id = element.closest("tr").data("id");
    let url = _ACTION_Tests_Detail + "/" + id;
    load(url, ContentBody, url, function () {

    })
}

function TestsCreate(mModal, data) {
    loadUrl(_ACTION_Tests_Create, data => {
        if (data.success) {
            mModal.modal("hide")
            showToast(data.message, "success")
            load(_ACTION_Tests_GetData, _TestsTable, _ACTION_Tests_Index);
        } else {
            showToast(data.message, "error")
        }
    }, "POST", data)
}

function TestsUpdate(mModal, element, data) {
    loadUrl(_ACTION_Tests_Update, data => {
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

function TestsToggleStatus(element) {
    let id = element.closest("tr").data("id");
    loadUrl(_ACTION_Tests_ToggleStatus, data => {
        if (data.success) {
            showToast(data.message, "success")
        } else {
            showToast(data.message, "error")
        }
        element.prop('checked', data.data.status == 1);
    }, "POST", { id: id })
    return false;
}

function TestsDelete(element) {
    let id = element.closest("tr").data("id");
    loadUrl(_ACTION_Tests_Delete, data => {
        if (data.success) {
            showToast(data.message, "success")
            load(_ACTION_Tests_GetData + window.location.search, _TestsTable, null);
        } else {
            showToast(data.message, "error")
        }
    }, "POST", { id: id })
}
