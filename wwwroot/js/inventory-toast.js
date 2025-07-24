(async () => {
    const handle = window.location.pathname.split("/products")[1];
    if (!handle) { return; }

    const res = await fetch(`https://stocknotificationwarning.onrender.com/api/inventory-check?handle=${handle}`);
    const data = res.json();

    if (data) {
        console.log("Data fetched successfully!");
    }
})