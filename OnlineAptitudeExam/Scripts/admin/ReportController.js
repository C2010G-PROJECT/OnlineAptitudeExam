const _ReportTable = '#reportTable';
const _ACTION_Report_Index = _PREFIX + '/Report'
const _ACTION_Report_GetData = _ACTION_Report_Index + '/GetData'
const _ACTION_Report_Create = _ACTION_Report_Index + '/Create'
const _ACTION_Report_Update = _ACTION_Report_Index + '/Update'
const _ACTION_Report_Delete = _ACTION_Report_Index + '/Delete'
const _ACTION_Report_ToggleStatus = _ACTION_Report_Index + '/ToggleStatus'
const _ACTION_Report_Detail = _ACTION_Report_Index + '/Detail'

function ReportIndex() {
    prepareDatePicker();
    prepareSearchBar();
    prepareReportTable();
}

function prepareDatePicker() {
    let mSDate = $('#startDate');
    let mEDate = $('#endDate');

    $('#btnW').off('click').on('click', function () { set_current_week(mSDate, mEDate) })
    $('#btnM').off('click').on('click', function () { set_current_month(mSDate, mEDate) })
    $('#btnQ').off('click').on('click', function () { set_current_quarter(mSDate, mEDate) })

    mSDate.off('change').on('change', function () {
        
    })
    mEDate.off('change').on('change', function () {
        console.log('change')
    })

}

function prepareSearchBar() {
    refreshSearchBar('#searchTestsReport');
    prepareKeyboardAction('#searchTestsReport', _ReportTable, _ACTION_Report_Index);
}

function prepareReportTable() {
    const mMouseSeletor = '#contentPager a[href], .table-custom thead th.sortable';
    refreshTableItemSort('.table-custom')
    prepareMouseAction(mMouseSeletor, _ReportTable, _ACTION_Report_Index);
}


function set_current_week(from, to) {
    var date = new Date();
    var first = date.getDate() - 5;
    var last = date.getDate();

    var start = new Date(date.getFullYear(), date.getMonth(), first).toISOString().split("T")[0];
    var end = new Date(date.setDate(last)).toISOString().split("T")[0];
    //var date = new Date();
    //var minusDay = date.getDay() - 1;
    //minusDay = minusDay == -1 ? 6 : minusDay;

    //console.log(date.getDay())
    //var start = new Date(date.getFullYear(), date.getMonth(), date.getDate() - date.getUTCDay()).toISOString().split("T")[0];
    //var end = new Date(date.setDate(date.getDate() + 7)).toISOString().split("T")[0];

    from.val(start); to.val(end);
}

function set_current_month(from, to) {
    var date = new Date();
    var first = new Date(date.getFullYear(), date.getMonth(), 2);
    var last = new Date(date.getFullYear(), date.getMonth() + 1, 1);
    var start = first.toISOString().split("T")[0];
    var end = last.toISOString().split("T")[0];
    from.val(start); to.val(end);
}

function set_current_quarter(from, to) {
    var date = new Date();
    var quarter = Math.floor(date.getMonth() / 3);
    var firstDate = new Date(date.getFullYear(), quarter * 3, 2);
    var lastDate = new Date(firstDate.getFullYear(), firstDate.getMonth() + 3, 1);
    var start = firstDate.toISOString().split("T")[0];
    var end = lastDate.toISOString().split("T")[0];
    from.val(start); to.val(end);
}
