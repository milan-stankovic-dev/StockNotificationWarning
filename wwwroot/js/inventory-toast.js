console.log("loaded inventory-toast.js");

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
            const msg = `⚠️ Low stock for "${product.ProductName}" — only ${product.Stock} left!`;
            Toast.failure(msg); 
        });

        console.log("Data fetched and processed successfully.");

    } catch (err) {
        console.error("Inventory fetch failed", err);
    }
})();