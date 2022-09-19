// Toast function

function showToast($msg, $type, $title = "", $duration = 3000) {
    $toast = {
        "title": $title != "" ? $title : ucfirst($type) + "!",
        "message": $msg,
        "type": $type,
        "duration": $duration
    };
    toast($toast);
}

function ucfirst(string) {
    return string.charAt(0).toUpperCase() + string.slice(1)
}

function toast({ title = "", message = "", type = "info", duration = 3000 }) {
    const main = document.getElementById("ct_toast");
    if (main) {
        const toast = document.createElement("div");

        //Auto remove toast
        const autoRemoveId = setTimeout(function () {
            main.removeChild(toast);
        }, duration + 1000);

        // Remove toast when clicked
        toast.onclick = function (e) {
            if (e.target.closest(".ct_toast__close")) {
                main.removeChild(toast);
                clearTimeout(autoRemoveId);
            }
        };

        const icons = {
            success: "fas fa-check-circle",
            info: "fas fa-info-circle",
            warning: "fas fa-exclamation-circle",
            error: "fas fa-exclamation-circle",
        };
        const icon = icons[type];
        const delay = (duration / 1000).toFixed(2);

        toast.classList.add("ct_toast", `ct_toast--${type}`);
        toast.style.animation = `slideInLeft ease .3s, fadeOut linear 1s ${delay}s forwards`;

        toast.innerHTML = `
                      <div class="ct_toast__icon">
                          <i class="${icon}"></i>
                      </div>
                      <div class="ct_toast__body">
                          <h3 class="ct_toast__title">${title}</h3>
                          <p class="ct_toast__msg">${message}</p>
                      </div>
                      <div class="ct_toast__close">
                          <i class="fas fa-times"></i>
                      </div>
                  `;
        main.appendChild(toast);
    }
}
