import type { Actions, PageServerLoad } from "./$types";
import { fail } from "@sveltejs/kit";
import { superValidate } from "sveltekit-superforms";
import { zod4 } from "sveltekit-superforms/adapters";
import { PUBLIC_API_BASE_URL } from "$env/static/public";
import { loginSchema } from "./schema";

export const load: PageServerLoad = async () => {
	return { form: await superValidate(zod4(loginSchema)) };
};

export const actions: Actions = {
	default: async (event) => {
		const form = await superValidate(event, zod4(loginSchema));
		if (!form.valid) return fail(400, { form });

		// Authenticate against EShop.API (server-to-server, so no browser CORS).
		// The JWT is returned to the client, which stores it in localStorage via
		// auth.login() — see +page.svelte.
		const res = await event.fetch(`${PUBLIC_API_BASE_URL}/auth/login`, {
			method: "POST",
			headers: { "Content-Type": "application/json" },
			body: JSON.stringify({
				username: form.data.username,
				password: form.data.password
			})
		});

		if (res.status === 401) {
			return fail(401, { form, error: "Invalid username or password." });
		}
		if (!res.ok) {
			return fail(res.status, { form, error: "Login failed. Please try again later." });
		}

		const data = await res.json(); // { token, role, domainId }
		return { form, loginResult: { ...data, username: form.data.username } };
	}
};
