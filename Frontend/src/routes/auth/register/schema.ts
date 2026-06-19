import { z } from "zod";

export const ROLES = ["BUYER", "SELLER"] as const;

// Mirrors the EShop.API register payload (POST /api/auth/register):
//   { username, password, address, role }  (role -> "Buyer" | "Seller")
// Backend password rules (CustomPasswordValidator): >= 8 chars, not all digits.
export const registerSchema = z
	.object({
		username: z.string().min(3, "At least 3 characters"),
		address: z.string().min(1, "Address is required"),
		role: z.enum(ROLES).default("BUYER"),
		password: z
			.string()
			.min(8, "At least 8 characters")
			.refine((p) => !/^\d+$/.test(p), "Password cannot be only digits"),
		confirmPassword: z.string().min(1, "Confirm your password")
	})
	.refine((data) => data.password === data.confirmPassword, {
		message: "Passwords do not match",
		path: ["confirmPassword"]
	});

export type RegisterSchema = typeof registerSchema;
