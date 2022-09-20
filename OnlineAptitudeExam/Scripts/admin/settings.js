(function ($) {
    'use strict';
    $(function () {
        $(".nav-settings").click(function () {
            $("#right-sidebar").toggleClass("open");
        });
        $(".settings-close").click(function () {
            $("#right-sidebar,#theme-settings").removeClass("open");
        });

        $("#settings-trigger").on("click", function () {
            $("#theme-settings").toggleClass("open");
        });


        //background constants
        var navbar_classes = "navbar-danger navbar-success navbar-warning navbar-dark navbar-light navbar-primary navbar-info navbar-pink";
        var sidebar_classes = "sidebar-light sidebar-dark";
        var $body = $("body");
        const key_sidebar_theme = "sidebar";
        const key_navbar_theme = "navbar";

        //sidebar backgrounds
        $("#sidebar-default-theme").on("click", function () {
            $body.removeClass(sidebar_classes);
            $(".sidebar-bg-options").removeClass("selected");
            $(this).addClass("selected");
            localStorage.setItem(key_sidebar_theme, "#sidebar-default-theme");
        });
        $("#sidebar-dark-theme").on("click", function () {
            $body.removeClass(sidebar_classes);
            $body.addClass("sidebar-dark");
            $(".sidebar-bg-options").removeClass("selected");
            $(this).addClass("selected");
            localStorage.setItem(key_sidebar_theme, "#sidebar-dark-theme");
        });

        $(function () {
            var theme = localStorage.getItem(key_sidebar_theme);
            theme = theme == null ? "#sidebar-default-theme" : theme;
            $body.removeClass(sidebar_classes);
            if (theme == "#sidebar-dark-theme") {
                $body.addClass("sidebar-dark");
            }
            $(".sidebar-bg-options").removeClass("selected");
            $(theme).addClass("selected");
        })

        //Navbar Backgrounds
        $(".tiles.primary").on("click", function () {
            $(".navbar").removeClass(navbar_classes);
            $(".navbar").addClass("navbar-primary");
            $(".tiles").removeClass("selected");
            $(this).addClass("selected");
            localStorage.setItem(key_navbar_theme, "primary");
        });
        $(".tiles.success").on("click", function () {
            $(".navbar").removeClass(navbar_classes);
            $(".navbar").addClass("navbar-success");
            $(".tiles").removeClass("selected");
            $(this).addClass("selected");
            localStorage.setItem(key_navbar_theme, "success");
        });
        $(".tiles.warning").on("click", function () {
            $(".navbar").removeClass(navbar_classes);
            $(".navbar").addClass("navbar-warning");
            $(".tiles").removeClass("selected");
            $(this).addClass("selected");
            localStorage.setItem(key_navbar_theme, "warning");
        });
        $(".tiles.danger").on("click", function () {
            $(".navbar").removeClass(navbar_classes);
            $(".navbar").addClass("navbar-danger");
            $(".tiles").removeClass("selected");
            $(this).addClass("selected");
            localStorage.setItem(key_navbar_theme, "danger");
        });
        $(".tiles.info").on("click", function () {
            $(".navbar").removeClass(navbar_classes);
            $(".navbar").addClass("navbar-info");
            $(".tiles").removeClass("selected");
            $(this).addClass("selected");
            localStorage.setItem(key_navbar_theme, "info");
        });
        $(".tiles.dark").on("click", function () {
            $(".navbar").removeClass(navbar_classes);
            $(".navbar").addClass("navbar-dark");
            $(".tiles").removeClass("selected");
            $(this).addClass("selected");
            localStorage.setItem(key_navbar_theme, "dark");
        });
        $(".tiles.light").on("click", function () {
            $(".navbar").removeClass(navbar_classes);
            $(".navbar").addClass("navbar-light");
            $(".tiles").removeClass("selected");
            $(this).addClass("selected");
            localStorage.setItem(key_navbar_theme, "light");
        });

        $(function () {
            var theme = localStorage.getItem(key_navbar_theme);
            theme = theme == null ? "light" : theme;
            $(".navbar").removeClass(navbar_classes);
            $(".navbar").addClass("navbar-" + theme);
            $(".tiles").removeClass("selected");
            $(".tiles." + theme).addClass("selected");
        })

        //Horizontal menu in mobile
        $('[data-toggle="horizontal-menu-toggle"]').on("click", function () {
            $(".horizontal-menu .bottom-navbar").toggleClass("header-toggled");
        });
        // Horizontal menu navigation in mobile menu on click
        var navItemClicked = $('.horizontal-menu .page-navigation >.nav-item');
        navItemClicked.on("click", function (event) {
            if (window.matchMedia('(max-width: 991px)').matches) {
                if (!($(this).hasClass('show-submenu'))) {
                    navItemClicked.removeClass('show-submenu');
                }
                $(this).toggleClass('show-submenu');
            }
        });

        $(window).scroll(function () {
            if (window.matchMedia('(min-width: 992px)').matches) {
                var header = $('.horizontal-menu');
                if ($(window).scrollTop() >= 71) {
                    $(header).addClass('fixed-on-scroll');
                } else {
                    $(header).removeClass('fixed-on-scroll');
                }
            }
        });

    });
})(jQuery);