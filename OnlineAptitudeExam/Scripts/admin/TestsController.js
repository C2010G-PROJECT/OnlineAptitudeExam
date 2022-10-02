const _TestsTable = '#testsTable';
const _ACTION_Tests_Index = _PREFIX + '/Tests'
const _ACTION_Tests_GetData = _ACTION_Tests_Index + '/GetData'
const _ACTION_Tests_Create = _ACTION_Tests_Index + '/Create'
const _ACTION_Tests_Update = _ACTION_Tests_Index + '/Update'
const _ACTION_Tests_Delete = _ACTION_Tests_Index + '/Delete'
const _ACTION_Tests_ToggleStatus = _ACTION_Tests_Index + '/ToggleStatus'
const _ACTION_Tests_Detail = _ACTION_Tests_Index + '/Detail'

function TestsIndex() {
    prepareSearchBar();
    prepareTestTable();
}

function prepareSearchBar() {
    refreshSearchBar('#searchTests');
    prepareKeyboardAction('#searchTests', _TestsTable, _ACTION_Tests_Index);
}

function prepareTestTable() {
    const mMouseSeletor = '#contentPager a[href], .table-custom thead th.sortable';
    refreshTableItemSort('.table-custom')
    prepareMouseAction(mMouseSeletor, _TestsTable, _ACTION_Tests_Index);
}

function showTestsModal(element, isCreate = true) {
    // setup variable
    let mModal = $('#testsModal'),
        mIcon = mModal.find('.modal-header i'),
        mTitle = mModal.find('.modal-title'),
        mSubmit = mModal.find('.modal-submit'),
        mForm = mModal.find('form'),
        mEdtName = mModal.find('#Name')

    // setup modal
    let title = isCreate ? 'Create' : 'Update';
    mIcon.removeClass().addClass('mdi mdi-' + (isCreate ? 'playlist-plus' : 'playlist-edit'))
    mTitle.text(title + ' test')
    mSubmit.text(title)

    // setup data
    pendingFocus(mModal, mEdtName);
    clearFormElements(mForm);

    // validate rule
    mForm.validate({
        rules: {
            Name: {
                onkeyup: false,
                required: true,
                minlength: 6,
                maxlength: 50
            }
        },
        messages: {
            Name: {
                required: 'Please enter the test name',
            }
        }
    }).resetForm()


    let id = null;
    if (!isCreate) {
        let mTr = element.closest('tr');
        id = mTr.data('id');
        mEdtName.val(mTr.find('.field-name').text().trim())
    }

    // init event
    mEdtName.off('focusout').on('focusout', function () {
        $(this).val($(this).val().trim())
    })
    mSubmit.off('click').on('click', function (e) {
        if (mForm.valid()) {
            let data = {
                id: id,
                model: objectifyForm(mForm)
            }
            if (isCreate) {
                TestsCreate(mModal, data)
            } else {
                TestsUpdate(mModal, element, data)
            }
            e.preventDefault();
            e.stopPropagation();
        } else {
            mForm.find('.form-control.error')[0].focus()
        }
    })
    mModal.modal('show');
}

function showDeleteTestsModal(element) {
    showConfirm('Delete test',
        'Are you sure to delete this record?',
        'outline-danger',
        'delete-outline', () => TestsDelete(element))
}

function showDetailTest(element) {
    let id = element.closest('tr').data('id');
    let url = _ACTION_Tests_Detail + '/' + id;
    load(url, ContentBody, url)
}

function TestsCreate(mModal, data) {
    loadUrl(_ACTION_Tests_Create, data => {
        if (data.success) {
            mModal.modal('hide')
            showToast(data.message, data.msgType)
            load(_ACTION_Tests_GetData, _TestsTable, _ACTION_Tests_Index);
        } else {
            showToast(data.message, data.msgType)
        }
    }, null, 'POST', data)
}

function TestsUpdate(mModal, element, data) {
    loadUrl(_ACTION_Tests_Update, data => {
        if (data.success) {
            mModal.modal('hide')
            showToast(data.message, data.msgType)
            let tr = element.closest('tr');
            tr.find('.field-name').text(data.data)
        } else {
            showToast(data.message, data.msgType)
        }
    }, null, 'POST', data)
}

function TestsToggleStatus(element, id = -1) {
    if (id == -1) {
        id = element.closest('tr').data('id');
    }
    loadUrl(_ACTION_Tests_ToggleStatus, data => {
        if (data.success) {
            showToast(data.message, data.msgType)
        } else {
            showToast(data.message, data.msgType)
        }
        element.prop('checked', data.data.status == 1);
    }, null, 'POST', { id: id })
    return false;
}

function TestsDelete(element) {
    let id = element.closest('tr').data('id');
    loadUrl(_ACTION_Tests_Delete, data => {
        if (data.success) {
            showToast(data.message, data.msgType)
            load(_ACTION_Tests_GetData + window.location.search, _TestsTable, null);
        } else {
            showToast(data.message, data.msgType)
        }
    }, null, 'POST', { id: id })
}
