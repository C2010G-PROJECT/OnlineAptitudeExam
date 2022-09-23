function SignOut() {
    showConfirm("Sign out", "Are you sure you want to sign out?", "danger", "logout-variant", () => {
        loadUrl("/Admin/Auth/LogOut", () => {
            window.location.href = "/Admin/Auth"
        }, "POST");
    });
}