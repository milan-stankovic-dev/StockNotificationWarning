class ToastService {
    constructor(options = {}) {
        this.defaultOptions = {
            duration: 3000,
            close: true,
            gravity: "top",
            position: "right",
            backgroundColor: "green",
            stopOnFocus: true,
            ...options
        };
    }

    show(message, type = "success") {
        if (!message) { return; }

        const bgColor = this.getColor(type);

        Toastify({
            text: message,
            bgColor,
            ...this.defaultOptions
        }).showToast();
    }

    success(message) {
        this.show(message, "success");
    }

    failure(message) {
        this.show(message, "failure");
    }

    getColor(type) {
        switch (type) {
            case "success": return "green";
            case "failure": return "red";
            default: return "grey";
        }
    }

}

window.Toast = new ToastService();