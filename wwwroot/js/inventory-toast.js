console.log("loaded inventory-toast.js");

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

(async () => {
    const handle = window.location.pathname.split("/products")[1];
    if (!handle) {
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

        console.log("Data fetched and processed successfully.");

    } catch (err) {
        console.error("Inventory fetch failed", err);
    }
})();