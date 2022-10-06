const _ReportTable = '#reportTable';
const _ACTION_Report_Index = _PREFIX + '/Report'
const _ACTION_Report_GetData = _ACTION_Report_Index + '/GetData'
const _ACTION_Report_Create = _ACTION_Report_Index + '/Create'
const _ACTION_Report_Update = _ACTION_Report_Index + '/Update'
const _ACTION_Report_Delete = _ACTION_Report_Index + '/Delete'
const _ACTION_Report_ToggleStatus = _ACTION_Report_Index + '/ToggleStatus'
const _ACTION_Report_Detail = _ACTION_Report_Index + '/Detail'

function ReportIndex(isReload = false) {
    prepareDatePicker();
    prepareReportSearchBar(isReload);
    prepareReportTable();
}

function prepareDatePicker() {
    let mSDate = $('#startDate');
    let mEDate = $('#endDate');

    let date = new Date();
    let defMin = '1900-01-01';
    let defMax = date.getFullYear() + '-' + zezoFirstIfNeed(date.getMonth() + 1) + '-' + zezoFirstIfNeed(date.getDate());

    let setupData = function (first, second, param_key, attr, defVal) {
        let searchParams = new URLSearchParams(location.search)
        if (searchParams.has(param_key)) {
            first.val(new Date(Number.parseInt(searchParams.get(param_key))).toISOString().split("T")[0])
            second.attr(attr, first.val())
        } else {
            first.val('')
            second.attr(attr, defVal)
        }
    }

    mSDate.attr('min', defMin);
    mEDate.attr('max', defMax);

    setupData(mSDate, mEDate, 'd_start', 'min', defMin)
    setupData(mEDate, mSDate, 'd_end', 'max', defMax)

    if (new Date(mSDate.attr('max')).getTime() > new Date(mEDate.attr('max')).getTime()) {
        mSDate.attr('max', mEDate.attr('max'));
    }

    let onDateChange = function () {
        let s_val = mSDate.val();
        let e_val = mEDate.val();
        let searchParams = new URLSearchParams(location.search)
        searchParams.delete('page')
        searchParams.delete('d_start')
        searchParams.delete('d_end')
        if (s_val.length != 0) {
            searchParams.set('d_start', new Date(s_val).getTime())
        }
        if (e_val.length != 0) {
            searchParams.set('d_end', new Date(e_val).getTime() + (23 * 59 * 59 * 1000))
        }
        load(_ACTION_Report_GetData + '?' + searchParams.toString(), _ReportTable, _ACTION_Report_Index)
    }

    $('#btnW').off('click').on('click', function () {
        set_current_week(mSDate, mEDate)
        onDateChange();
    })
    $('#btnM').off('click').on('click', function () {
        set_current_month(mSDate, mEDate)
        onDateChange();
    })
    $('#btnQ').off('click').on('click', function () {
        set_current_quarter(mSDate, mEDate)
        onDateChange();
    })

    $(':input[type=date]').off('click').on('click', function () { $(this).blur() });

    mSDate.off('change').on('change', function () {
        let startDate = new Date(mSDate.val());
        let endDate = new Date(mEDate.val());
        if (startDate.getTime() > endDate.getTime()) {
            mSDate.val(mEDate.val());
        }
        mEDate.attr('min', mSDate.val())
        onDateChange();
    })
    mEDate.off('change').on('change', function () {
        let startDate = new Date(mSDate.val());
        let endDate = new Date(mEDate.val());
        if (startDate.getTime() > endDate.getTime()) {
            mEDate.val(mSDate.val());
        }
        mSDate.attr('max', mEDate.val())
        onDateChange();
    })

}

function prepareReportSearchBar(isReload = false) {
    if (isReload) {
        refreshSearchBar('#searchTestsReport');
    }
    prepareKeyboardAction('#searchTestsReport', _ReportTable, _ACTION_Report_Index);
}

function prepareReportTable() {
    const mMouseSeletor = '#contentPager a[href], .table-custom thead th.sortable';
    refreshTableItemSort('.table-custom')
    prepareMouseAction(mMouseSeletor, _ReportTable, _ACTION_Report_Index);
}

function zezoFirstIfNeed(num) {
    if (num < 10) {
        return '0' + num;
    }
    return num;
}

function set_current_week(from, to) {
    var date = new Date();
    var minusDay = date.getDay() - 1;
    minusDay = minusDay == -1 ? 6 : minusDay;
    var startDate = new Date(date.getFullYear(), date.getMonth(), date.getDate() - minusDay + 1);
    var start = startDate.toISOString().split("T")[0];
    var end = new Date(startDate.setDate(startDate.getDate() + 6)).toISOString().split("T")[0];
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

function toggleReportTable(element) {
    let mTable = element.closest('table');
    let mTr = element.closest('tr').next();
    let mIcon = element.find('i');

    if (!mIcon.hasClass('active')) {
        mTable.find('.btn-toggle').find('i').each(function () {
            if ($(this).hasClass('mdi-minus')) {
                let mParent = $(this).parent();
                mParent.removeClass('btn-outline-youtube')
                mParent.addClass('btn-outline-behance')
                $(this).removeClass('mdi-minus active').addClass('mdi-plus')
                $(this).closest('tr').next().hide(100);
            }
        })
    }
     
    mTr.toggle(200, function () {
        element.removeClass('btn-outline-youtube btn-outline-behance')
        mIcon.removeClass();
        if (mTr.is(':visible')) {
            element.addClass('btn-outline-youtube')
            mIcon.addClass('mdi mdi-minus active')
        } else {
            element.addClass('btn-outline-behance')
            mIcon.addClass('mdi mdi-plus')
        }
    });
}

function showDetailReport(element) {
    let id = element.closest('tr').data('id');
    let url = _ACTION_Report_Detail + '/' + id;
    load(url, ContentBody, url, "addActiveClasss")
}
