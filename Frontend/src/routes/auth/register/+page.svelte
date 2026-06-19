<script lang="ts">
	import { superForm } from "sveltekit-superforms";
	import { zod4Client } from "sveltekit-superforms/adapters";
	import { goto } from "$app/navigation";
	import { auth } from "$lib/stores/auth.svelte.ts";
	import { registerSchema } from "./schema";
	import * as Form from "$lib/components/ui/form/index.ts";
	import { Input } from "$lib/components/ui/input/index.ts";
	import { Button } from "$lib/components/ui/button/index.ts";
	import { RadioGroup, RadioGroupItem } from "$lib/components/ui/radio-group/index.ts";
	import { Alert, AlertDescription } from "$lib/components/ui/alert/index.ts";
	import {
		Card,
		CardContent,
		CardDescription,
		CardFooter,
		CardHeader,
		CardTitle
	} from "$lib/components/ui/card/index.ts";
	import { ShoppingBagIcon, StoreIcon } from "@lucide/svelte";

	let { data } = $props();

	let errorMsg = $state("");

	const form = superForm(data.form, {
		validators: zod4Client(registerSchema),
		// Handle navigation ourselves so superForm's post-submit applyAction /
		// invalidateAll doesn't race the redirect below.
		applyAction: false,
		invalidateAll: false,
		onUpdate({ result }) {
			errorMsg = "";
			if (result.type === "failure" && result.data?.error) {
				errorMsg = result.data.error;
			}
			if (result.type === "success") {
				if (result.data?.loginResult) {
					auth.login(result.data.loginResult);
					goto("/", { invalidateAll: true });
				} else if (result.data?.registered) {
					goto("/auth/login");
				}
			}
		}
	});
	const { form: formData, enhance, submitting } = form;

	const roleOptions = [
		{
			value: "BUYER",
			label: "Buyer",
			description: "Browse and purchase products.",
			icon: ShoppingBagIcon
		},
		{
			value: "SELLER",
			label: "Seller",
			description: "List and sell your own products.",
			icon: StoreIcon
		}
	] as const;
</script>

<svelte:head>
	<title>Create an account · Vydelka</title>
</svelte:head>

<Card>
	<CardHeader>
		<CardTitle class="text-xl font-bold">Create an account</CardTitle>
		<CardDescription>
			Already have one?
			<a href="/auth/login" class="font-medium text-foreground underline-offset-4 hover:underline">
				Sign in
			</a>
		</CardDescription>
	</CardHeader>
	<form method="POST" use:enhance class="contents">
		<CardContent class="flex flex-col gap-4">
			{#if errorMsg}
				<Alert variant="destructive">
					<AlertDescription>{errorMsg}</AlertDescription>
				</Alert>
			{/if}

			<Form.Field {form} name="username">
				<Form.Control>
					{#snippet children({ props })}
						<Form.Label>Username</Form.Label>
						<Input {...props} bind:value={$formData.username} placeholder="alice" autocomplete="username" />
					{/snippet}
				</Form.Control>
				<Form.FieldErrors />
			</Form.Field>

			<Form.Field {form} name="address">
				<Form.Control>
					{#snippet children({ props })}
						<Form.Label>Address</Form.Label>
						<Input
							{...props}
							bind:value={$formData.address}
							placeholder="123 Main St, Kyiv"
							autocomplete="street-address"
						/>
					{/snippet}
				</Form.Control>
				<Form.FieldErrors />
			</Form.Field>

			<Form.Fieldset {form} name="role" class="flex flex-col gap-2">
				<Form.Legend>Account type</Form.Legend>
				<RadioGroup name="role" bind:value={$formData.role} class="grid grid-cols-2 gap-3">
					{#each roleOptions as option (option.value)}
						<Form.Control>
							{#snippet children({ props })}
								<Form.Label
									class="flex cursor-pointer flex-col gap-1 rounded-lg border p-4 font-normal transition-colors hover:bg-accent has-[[data-state=checked]]:border-primary has-[[data-state=checked]]:bg-accent"
								>
									<div class="flex items-center justify-between">
										<option.icon class="size-5" />
										<RadioGroupItem value={option.value} {...props} />
									</div>
									<span class="font-medium">{option.label}</span>
									<span class="text-xs text-muted-foreground">{option.description}</span>
								</Form.Label>
							{/snippet}
						</Form.Control>
					{/each}
				</RadioGroup>
				<Form.FieldErrors />
			</Form.Fieldset>

			<div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
				<Form.Field {form} name="password">
					<Form.Control>
						{#snippet children({ props })}
							<Form.Label>Password</Form.Label>
							<Input {...props} type="password" bind:value={$formData.password} autocomplete="new-password" />
						{/snippet}
					</Form.Control>
					<Form.FieldErrors />
				</Form.Field>

				<Form.Field {form} name="confirmPassword">
					<Form.Control>
						{#snippet children({ props })}
							<Form.Label>Confirm password</Form.Label>
							<Input {...props} type="password" bind:value={$formData.confirmPassword} autocomplete="new-password" />
						{/snippet}
					</Form.Control>
					<Form.FieldErrors />
				</Form.Field>
			</div>
		</CardContent>
		<CardFooter class="flex flex-col gap-2">
			<Button type="submit" class="w-full" disabled={$submitting}>Create account</Button>
		</CardFooter>
	</form>
</Card>
