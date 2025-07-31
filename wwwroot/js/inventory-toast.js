console.log("inventory-toast.js loading...");

(async () => {
    if (typeof Toastify === "undefined") {
        console.log("Toastify not found. Loading...");

        await new Promise((resolve, reject) => {
            const script = document.createElement("script");
            script.src = "https://cdn.jsdelivr.net/npm/toastify-js";
            script.onload = resolve;
            script.onerror = reject;
            document.head.appendChild(script);
        });

        const css = document.createElement("link");
        css.rel = "stylesheet";
        css.href = "https://cdn.jsdelivr.net/npm/toastify-js/src/toastify.min.css";
        document.head.appendChild(css);

        console.log("Toastify loaded.");
    }

    class ToastService {
        constructor(options = {}) {
            this.defaultOptions = {
                duration: 3000,
                close: true,
                gravity: "top",
                position: "right",
                stopOnFocus: true,
                ...options
            };
        }

        show(message, type = "success") {
            if (!message) return;

            const bgColor = this.getColor(type);

            Toastify({
                text: message,
                backgroundColor: bgColor,
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
                default: return "gray";
            }
        }
    }

    window.Toast = new ToastService();

    const isAdminPanel = window.location.href.include("admin");
    if (!isAdminPanel) {
        console.log("Not a product page, skipping inventory check.");
        return;
    }

    try {
        const res = await fetch(`https://stocknotificationwarning.onrender.com/api/inventorycheck`);
        if (!res.ok) {
            console.error(`Inventory check failed: ${res.status} ${res.statusText}`);
            return;
        }

        const data = await res.json();
        if (!Array.isArray(data)) {
            console.error("Unexpected data format:", data);
            return;
        }

        data.forEach(product => {
            const msg = `⚠️ Low stock for "${product.productName}" — only ${product.stock} left!`;
            Toast.failure(msg);
        });

        console.log("Low stock check completed.");
    } catch (err) {
        console.error("Inventory fetch failed", err);
    }
})();