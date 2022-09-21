let mRootPath, mDataPath;
const mMouseSeletor = "#contentPager a[href], #contentTable table th .sortable";
const mInto = "#contentTable";

function TestsIndex(rootPath, dataPath) {
    mRootPath = rootPath;
    mDataPath = dataPath;
    let prepare = () => {
        prepareMouseAction(mMouseSeletor, mInto, mRootPath, () => {
            refreshTableItemSort("#tableTests");
        });
    }
    prepareKeyboardAction("#searchTests", mInto, mRootPath, () => {
        refreshTableItemSort("#tableTests");
        prepare();
    });
    prepare();
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
            load(mDataPath, mInto, mRootPath, () => {
                prepareMouseAction(mMouseSeletor, mInto, mRootPath, () => { refreshTableItemSort("#tableTests"); });
            });
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
            tr.find(".field-name").text(data.data.name)
        } else {
            showToast(data.message, "error")
        }
    }, "POST", data)
}

function TestsToggleStatus(element, url) {
    element.preventDefault();
    let id = element.closest("tr").data("id");
    loadUrl(url, data => {
        if (data.success) {
            showToast(data.message, "success")
        } else {
            showToast(data.message, "error")
        }
    }, "POST", { id: id })
}
