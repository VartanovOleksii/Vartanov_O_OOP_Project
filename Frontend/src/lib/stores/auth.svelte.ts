/**
 * Client-side auth state backed by the real EShop.API JWT flow.
 *
 * The backend issues a JWT on POST /api/auth/login. We persist it (plus the
 * decoded role / domainId / username) in localStorage and keep an in-memory
 * runes copy for reactive UI.
 *
 * The store starts logged-out so SSR and the first client render agree (no
 * hydration mismatch). Call `auth.init()` from a client-only effect (root
 * +layout) to rehydrate from localStorage after mount.
 */
import { browser } from "$app/environment";

export type Role = "BUYER" | "SELLER";

export const TOKEN_KEY = "eshop_token";
export const USER_KEY = "eshop_user";

export interface AuthUser {
	username: string;
	role: Role;
	domainId: number;
}

/** Shape returned by POST /api/auth/login, with the username we submitted. */
export interface LoginResult {
	token: string;
	role: string; // "Buyer" | "Seller" from the backend
	domainId: number;
	username: string;
}

function normalizeRole(role: string): Role {
	return role.toUpperCase() === "SELLER" ? "SELLER" : "BUYER";
}

class AuthStore {
	user = $state<AuthUser | null>(null);

	get isAuthenticated(): boolean {
		return this.user !== null;
	}

	get isSeller(): boolean {
		return this.user?.role === "SELLER";
	}

	get isBuyer(): boolean {
		return this.user?.role === "BUYER";
	}

	/** Initials for the avatar fallback (backend exposes no profile name). */
	get initials(): string {
		return this.user?.username?.[0]?.toUpperCase() ?? "?";
	}

	/** Human-friendly name for the profile menu (only the username is known). */
	get displayName(): string {
		return this.user?.username ?? "";
	}

	get token(): string | null {
		if (!browser) return null;
		return localStorage.getItem(TOKEN_KEY);
	}

	/** Rehydrate from localStorage. Safe to call multiple times; client-only. */
	init(): void {
		if (!browser) return;
		const raw = localStorage.getItem(USER_KEY);
		const token = localStorage.getItem(TOKEN_KEY);
		if (!raw || !token) {
			this.user = null;
			return;
		}
		try {
			this.user = JSON.parse(raw) as AuthUser;
		} catch {
			this.user = null;
		}
	}

	login(result: LoginResult): void {
		const user: AuthUser = {
			username: result.username,
			role: normalizeRole(result.role),
			domainId: result.domainId
		};
		this.user = user;
		if (browser) {
			localStorage.setItem(TOKEN_KEY, result.token);
			localStorage.setItem(USER_KEY, JSON.stringify(user));
		}
	}

	logout(): void {
		this.user = null;
		if (browser) {
			localStorage.removeItem(TOKEN_KEY);
			localStorage.removeItem(USER_KEY);
		}
	}
}

export const auth = new AuthStore();
