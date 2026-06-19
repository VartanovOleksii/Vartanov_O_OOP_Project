import {executeApiRequests} from "$lib/utils/api.js";

// The public storefront only needs the product catalog. Data is loaded here in
// +page.js (universal load) and rendered by +page.svelte.
export function load({ fetch }) {
    return executeApiRequests({
        products: {
            endpoint: '/products'
        }
    }, fetch)
}
