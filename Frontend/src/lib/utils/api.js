import axios from 'axios';
import {toast} from "svelte-sonner";
import {browser} from "$app/environment";
import {goto} from "$app/navigation";
import {PUBLIC_API_BASE_URL} from "$env/static/public";
import {auth, TOKEN_KEY} from "$lib/stores/auth.svelte.ts";

// Base URL comes from the environment (PUBLIC_API_BASE_URL, e.g.
// http://localhost:5000/api). EShop.API uses JWT Bearer auth, not cookies, so
// there is no `withCredentials`.
const instance = axios.create({
    baseURL: PUBLIC_API_BASE_URL
});

// Attach the JWT (if present) to every request.
instance.interceptors.request.use((config) => {
    if (browser) {
        const token = localStorage.getItem(TOKEN_KEY);
        if (token) {
            config.headers = config.headers ?? {};
            config.headers.Authorization = `Bearer ${token}`;
        }
    }
    return config;
});

// On 401, drop the stored session and bounce to login (client only).
instance.interceptors.response.use(
    (response) => response,
    (error) => {
        if (error?.response?.status === 401 && browser) {
            auth.logout();
            goto('/auth/login');
        }
        return Promise.reject(error);
    }
);

export const api = {
    get: function (url, params = {}) {
        return instance.get(url, {params});
    },

    post: function (url, data = {}) {
        return instance.post(url, data);
    },

    put: function (url, data = {}) {
        return instance.put(url, data);
    },

    patch: function (url, data = {}) {
        return instance.patch(url, data)
    },

    delete: function (url) {
        return instance.delete(url);
    },
};

export async function get(
    url,
    payload = {},
    options = {
        callback: {callbackDelay: 0, callbackFn: null},
        toast: {show: false, errorMsg: "", successMsg: ""}
    }
) {
    return await safeApiRequest(
        () => api.get(url, payload),
        options?.callback?.callbackFn || (() => {
        }),
        options?.callback?.callbackDelay ?? 50,
        options?.toast?.show ?? false,
        options?.toast?.successMsg ?? '',
        options?.toast?.errorMsg ?? ''
    )
}

export async function post(
    url,
    payload = {},
    options = {
        callback: {callbackDelay: 0, callbackFn: null},
        toast: {show: false, errorMsg: "", successMsg: ""}
    }
) {
    return await safeApiRequest(
        () => api.post(url, payload),
        options?.callback?.callbackFn ?? (() => {
        }),
        options?.callback?.callbackDelay ?? 50,
        options?.toast?.show ?? false,
        options?.toast?.successMsg ?? '',
        options?.toast?.errorMsg ?? ''
    )
}

export async function patch(
    url,
    payload = {},
    options = {
        callback: {callbackDelay: 0, callbackFn: null},
        toast: {show: false, errorMsg: "", successMsg: ""}
    }
) {
    return await safeApiRequest(
        () => api.patch(url, payload),
        options?.callback?.callbackFn ?? (() => {
        }),
        options?.callback?.callbackDelay ?? 50,
        options?.toast?.show ?? false,
        options?.toast?.successMsg ?? '',
        options?.toast?.errorMsg ?? ''
    )
}

export async function apiDelete(
    url,
    options = {
        callback: {callbackDelay: 0, callbackFn: null},
        toast: {show: false, errorMsg: "", successMsg: ""}
    }
) {
    return await safeApiRequest(
        () => api.delete(url),
        options?.callback?.callbackFn ?? (() => {
        }),
        options?.callback?.callbackDelay ?? 50,
        options?.toast?.show ?? false,
        options?.toast?.successMsg ?? '',
        options?.toast?.errorMsg ?? ''
    )
}

async function safeApiRequest(fn, callback, callbackDelay = 0, showToast = true, successMessage, errorMessage) {
    let response = null;
    try {
        response = await fn()
        if (showToast) toast.success(successMessage)
    } catch (error) {
        console.error(`Error doing safeApiRequest, fn err:`, fn, error)
        if (showToast) toast.error(`${errorMessage}: ${error?.response?.data?.error || error?.message}`)
    }
    if (callback) {
        setTimeout(async () => {
            await callback(response)
        }, callbackDelay)
    }
    return response
}

// Used by universal `load` functions. EShop.API returns bare arrays/objects
// (no DRF-style { results, count, next }), so the parsed JSON is returned as-is.
export async function executeApiRequests(requests, _fetch) {
    let payload = {}
    try {
        // Execute all requests in parallel
        const results = await Promise.all(
            Object.entries(requests).map(async ([key, request]) => {
                const response = await apiFetch(_fetch ?? fetch, request.endpoint, 'GET', request.params)
                if (!response.ok) {
                    console.error(`Error fetching ${key}:`, response);
                    return [key, []];
                }

                const data = await response.json();
                // Update store if specified
                if (request.store) {
                    request.store.set(data);
                }

                // Return the data with its key
                return [key, data];
            })
        );

        // Convert a result array to object and update payload
        results.forEach(([key, data]) => {
            payload[key] = data;
        });

    } catch (error) {
        // Handle critical errors
        console.error('Critical error during data loading:', error);
        toast.error(`Critical error during data loading: ${error.message}`);
        payload.error = error.message;
    } finally {
        payload.loading = false;
    }

    return payload
}

export async function apiFetch(_fetch, endpoint, method, body) {
    let url = `${PUBLIC_API_BASE_URL}${endpoint}`
    const headers = {'Content-Type': 'application/json'}
    if (browser) {
        const token = localStorage.getItem(TOKEN_KEY)
        if (token) headers.Authorization = `Bearer ${token}`
    }
    if (method === 'GET' && body) {
        url += '?' + new URLSearchParams(body).toString()
        return _fetch(url, {method, headers})
    }
    if (['POST', 'PATCH', 'PUT'].includes(method)) {
        return _fetch(url, {
            method,
            headers,
            body: JSON.stringify(body)
        })
    }
    return _fetch(url, {method, headers})
}
