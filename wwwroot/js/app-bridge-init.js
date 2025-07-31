import { createApp } from '@shopify/app-bridge';
import { getSessionToken as getSessionTokenOrig } from '@shopify/app-bridge-utils';
import { TitleBar } from '@shopify/app-bridge/actions';

export async function initApp(apiKey, host) {
    if (!host) {
        console.warn("Missing host, redirecting...");
        window.top.location.href = "/index";
        return;
    }

    const app = createApp({ apiKey, host, forceRedirect: true });

    try {
        const token = await getSessionTokenOrig(app);
        //sessionStorage.setItem("sessionToken", token);
        console.log("Shopify session token:", token);
    } catch (e) {
        console.error("Error getting session token:", e);
    }

    TitleBar.create(app, { title: document.title });

    return app;
}


export function getSessionToken(app) {
    return getSessionTokenOrig(app);
}