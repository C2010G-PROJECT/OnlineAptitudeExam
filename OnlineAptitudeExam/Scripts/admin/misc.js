var ChartColor = ["#5D62B4", "#54C3BE", "#EF726F", "#F9C446", "rgb(93.0, 98.0, 180.0)", "#21B7EC", "#04BCCC"];
var primaryColor = getComputedStyle(document.body).getPropertyValue('--primary');
var secondaryColor = getComputedStyle(document.body).getPropertyValue('--secondary');
var successColor = getComputedStyle(document.body).getPropertyValue('--success');
var warningColor = getComputedStyle(document.body).getPropertyValue('--warning');
var dangerColor = getComputedStyle(document.body).getPropertyValue('--danger');
var infoColor = getComputedStyle(document.body).getPropertyValue('--info');
var darkColor = getComputedStyle(document.body).getPropertyValue('--dark');
var lightColor = getComputedStyle(document.body).getPropertyValue('--light');

var body = $('body');
var sidebar = $('.sidebar');

function addActiveClasss() {
    let current = location.pathname.toLowerCase()
        .replace(_AJAX_PREFIX.toLowerCase(), "")
        .replace(_PREFIX.toLowerCase(), "")
        .split("/")[1];
    $("#sidebar .nav-item.active").each(function () {
        $(this).removeClass("active")
    })
    $("#sidebar .nav-item a[href]").each(function () {
        addActiveClass($(this), current);
    })
    $('.horizontal-menu .nav li > a[href]').each(function () {
        addActiveClass($(this), current);
    })
}

$("#sidebar .nav li a[href], .navbar .navbar-brand").off("click").on("click", function (e) {
    e.preventDefault();
    let url = $(this).attr("href");
    $(ContentBody).html('<div class="loader" />')
    setTimeout(function () { load(url, ContentBody, url, addActiveClasss, "GET") }, 50)
});

addActiveClasss();

//Add active class to nav-link based on url dynamically
//Active class can be hard coded directly in html file also as required
function addActiveClass(element, current) {
    if (current === "") {
        //for root url
        if (element.attr('href').toLowerCase().indexOf("index.html") !== -1) {
            element.closest(".nav-item").addClass('active');
            if (element.parents('.sub-menu').length) {
                element.closest('.collapse').addClass('show');
                element.addClass('active');
            }
        }
    } else {
        //for other url
        if (element.attr('href').toLowerCase().indexOf(current) !== -1) {
            element.closest(".nav-item").addClass('active');
            if (element.parents('.sub-menu').length) {
                element.closest('.collapse').addClass('show');
                element.addClass('active');
            }
            if (element.parents('.submenu-item').length) {
                element.addClass('active');
            }
        }
    }
}

//Close other submenu in sidebar on opening any
sidebar.on('show.bs.collapse', '.collapse', function () {
    sidebar.find('.collapse.show').collapse('hide');
});

$('[data-toggle="minimize"]').on("click", function () {
    if ((body.hasClass('sidebar-toggle-display')) || (body.hasClass('sidebar-absolute'))) {
        body.toggleClass('sidebar-hidden');
    } else {
        body.toggleClass('sidebar-icon-only');
    }
});

// off canvas
$('[data-toggle="offcanvas"]').on("click", function () {
    $('.sidebar-offcanvas').toggleClass('active')
});

// hover collapse
$(document).on('mouseenter mouseleave', '.sidebar .nav-item', function (ev) {
    var body = $('body');
    var sidebarIconOnly = body.hasClass("sidebar-icon-only");
    var sidebarFixed = body.hasClass("sidebar-fixed");
    if (!('ontouchstart' in document.documentElement)) {
        if (sidebarIconOnly) {
            if (sidebarFixed) {
                if (ev.type === 'mouseenter') {
                    body.removeClass('sidebar-icon-only');
                }
            } else {
                var $menuItem = $(this);
                var $menuTitle = $menuItem.find('.menu-title');
                if (ev.type === 'mouseenter') {
                    $menuItem.addClass('hover-open')
                    var position = $menuItem[0].getBoundingClientRect();
                    $menuTitle.css('top', position.top);
                } else {
                    $menuItem.removeClass('hover-open')
                }
            }
        }
    }
});

$('.aside-toggler').click(function () {
    $('.chat-list-wrapper').toggleClass('slide')
});

// load if can or reload when state change
$(window).off('popstate').on('popstate', function (e) {
    const state = e.originalEvent.state;
    // console.log(state)
    if (state !== null) {
        let url = state.loadUrl;
        let into = $(state.into);
        let callback = state.callback;
        // console.log(into)
        if (into.length == 0) {
            // try to find content body
            into = $(ContentBody);
            if (into.length == 0) {
                window.location.reload();
                return;
            }
            url = _AJAX_PREFIX + state.realUrl;
            callback = addActiveClasss;
        }
        load(url, null, null, (data) => {
            $(".modal").modal("hide");
            into.html(data);
            if (typeof callback == "string") {
                window[callback]();
            } else if (typeof callback == "function") {
                callback();
            }
        }, state.type);
    }
});
