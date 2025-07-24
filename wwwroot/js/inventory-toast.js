console.log("loaded inventory-toast.js");
(async () => {
    const handle = window.location.pathname.split("/products")[1];
    if (!handle) { return; }

    try {

        const res = await fetch(`https://stocknotificationwarning.onrender.com/api/inventory-check`);
        const data = res.json();

        data.forEach(product => {
            const msg = `⚠️ Low stock for "${product.ProductName}" — only ${product.Stock} left!`;
            Toast.failure(msg);
        });

        if (data) {
            console.log("Data fetched successfully!");
        }

    } catch (err) {
        console.error("Inventory fetch failed", err.error);
    }
})