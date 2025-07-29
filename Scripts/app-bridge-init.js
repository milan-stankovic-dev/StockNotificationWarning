import { createApp } from "@shopify/app-bridge";
import { getSessionToken } from "@shopify/app-bridge-utils";
import { TitleBar } from "@shopify/app-bridge/actions";

const urlParams = new URLSearchParams(window.location.search);
const apiKey = document.body.dataset.apiKey;
const host = urlParams.get("host");

const app = createApp({ apiKey, host });

getSessionToken(app).then(token => {
    sessionStorage.setItem("sessionToken", token);
    console.log("Session token:", token);
}).catch(console.error);

TitleBar.create(app, { title: document.title });
