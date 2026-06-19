import type { Actions, PageServerLoad } from "./$types";
import { fail } from "@sveltejs/kit";
import { superValidate } from "sveltekit-superforms";
import { zod4 } from "sveltekit-superforms/adapters";
import { PUBLIC_API_BASE_URL } from "$env/static/public";
import { registerSchema } from "./schema";

export const load: PageServerLoad = async () => {
	return { form: await superValidate(zod4(registerSchema)) };
};

export const actions: Actions = {
	default: async (event) => {
		const form = await superValidate(event, zod4(registerSchema));
		if (!form.valid) return fail(400, { form });

		// Backend expects the role as "Buyer" | "Seller" (capitalized string).
		const role = form.data.role === "SELLER" ? "Seller" : "Buyer";

		const registerRes = await event.fetch(`${PUBLIC_API_BASE_URL}/auth/register`, {
			method: "POST",
			headers: { "Content-Type": "application/json" },
			body: JSON.stringify({
				username: form.data.username,
				password: form.data.password,
				address: form.data.address,
				role
			})
		});

		if (!registerRes.ok) {
			// 400 invalid password/login, 409 username taken — surface the backend's
			// `error` message.
			let message = "Could not register. Please try different details.";
			try {
				const body = await registerRes.json();
				if (body?.error) message = body.error;
			} catch {
				/* ignore non-JSON bodies */
			}
			return fail(registerRes.status, { form, error: message });
		}

		// Registration succeeded — immediately log in to obtain a JWT (better UX).
		const loginRes = await event.fetch(`${PUBLIC_API_BASE_URL}/auth/login`, {
			method: "POST",
			headers: { "Content-Type": "application/json" },
			body: JSON.stringify({
				username: form.data.username,
				password: form.data.password
			})
		});

		if (!loginRes.ok) {
			// Account exists but auto-login failed — send them to the login page.
			return { form, registered: true };
		}

		const data = await loginRes.json(); // { token, role, domainId }
		return { form, loginResult: { ...data, username: form.data.username } };
	}
};
