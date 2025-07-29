import { createApp } from '@shopify/app-bridge';
import { getSessionToken } from '@shopify/app-bridge-utils';
import { TitleBar } from '@shopify/app-bridge/actions';

export async function initApp(apiKey, host) {
    if (!host) {
        console.warn("Missing host, redirecting...");
        window.top.location.href = "/index";
        return;
    }

    const app = createApp({ apiKey, host });

    try {
        const token = await getSessionToken(app);
        sessionStorage.setItem("sessionToken", token);
        console.log("Shopify session token:", token);
    } catch (e) {
        console.error("Error getting session token:", e);
    }

    TitleBar.create(app, { title: document.title });

    return app;
}
