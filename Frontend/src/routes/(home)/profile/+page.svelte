<script lang="ts">
	import { onMount } from "svelte";
	import { toast } from "svelte-sonner";
	import { auth } from "$lib/stores/auth.svelte.ts";
	import { api } from "$lib/utils/api.js";
	import { Button } from "$lib/components/ui/button/index.ts";
	import { Input } from "$lib/components/ui/input/index.ts";
	import { Avatar, AvatarFallback } from "$lib/components/ui/avatar/index.ts";
	import { Badge } from "$lib/components/ui/badge/index.ts";
	import { Spinner } from "$lib/components/ui/spinner/index.ts";
	import {
		Card,
		CardContent,
		CardDescription,
		CardHeader,
		CardTitle
	} from "$lib/components/ui/card/index.ts";
	import {
		Table,
		TableBody,
		TableCell,
		TableHead,
		TableHeader,
		TableRow
	} from "$lib/components/ui/table/index.ts";
	import { UserIcon } from "@lucide/svelte";

	interface Order {
		id: number;
		productId: number;
		productName: string;
		sellerId: number;
		quantity: number;
		unitPrice: number;
		totalPrice: number;
		orderDate: string;
	}

	let orders = $state<Order[]>([]);
	let loadingOrders = $state(true);

	// Editable shipping/contact address (stored on AuthorizedUser.UserAddress).
	let address = $state("");
	let addressDraft = $state("");
	let loadingAddress = $state(true);
	let savingAddress = $state(false);
	const addressDirty = $derived(addressDraft.trim() !== address.trim());

	async function loadOrders() {
		loadingOrders = true;
		try {
			const res = await api.get("/orders");
			orders = res.data ?? [];
		} catch (error: any) {
			toast.error(error?.response?.data?.error ?? "Could not load orders");
		} finally {
			loadingOrders = false;
		}
	}

	async function loadAccount() {
		loadingAddress = true;
		try {
			const res = await api.get("/account/me");
			address = res.data?.userAddress ?? "";
			addressDraft = address;
		} catch (error: any) {
			toast.error(error?.response?.data?.error ?? "Could not load account");
		} finally {
			loadingAddress = false;
		}
	}

	async function saveAddress() {
		const next = addressDraft.trim();
		if (!next) {
			toast.error("Address cannot be empty");
			return;
		}
		savingAddress = true;
		try {
			const res = await api.put("/account/address", { newAddress: next });
			address = res.data?.userAddress ?? next;
			addressDraft = address;
			toast.success("Address updated");
		} catch (error: any) {
			toast.error(error?.response?.data?.error ?? "Could not update address");
		} finally {
			savingAddress = false;
		}
	}

	onMount(() => {
		auth.init();
		if (auth.isAuthenticated) loadAccount();
		else loadingAddress = false;
		if (auth.isBuyer) loadOrders();
		else loadingOrders = false;
	});

	function formatPrice(value: number) {
		return `$${Number(value).toFixed(2)}`;
	}

	function formatDate(iso: string) {
		const d = new Date(iso);
		return Number.isNaN(d.getTime()) ? iso : d.toLocaleString();
	}
</script>

<svelte:head>
	<title>Profile · Vydelka</title>
</svelte:head>

<div class="mx-auto flex max-w-2xl flex-col gap-6">
	<div class="flex flex-col gap-1">
		<h1 class="font-handwrite text-3xl font-bold tracking-tight text-accent-foreground">
			Profile
		</h1>
		<p class="text-sm text-muted-foreground">Your account details and order history.</p>
	</div>

	{#if !auth.isAuthenticated}
		<div class="flex flex-col items-center gap-3 py-20 text-center">
			<div class="flex size-14 items-center justify-center rounded-full bg-muted text-muted-foreground">
				<UserIcon class="size-7" />
			</div>
			<p class="text-lg font-medium">You're not signed in</p>
			<Button href="/auth/login" variant="secondary" size="sm">Go to login</Button>
		</div>
	{:else}
		<Card>
			<CardHeader>
				<div class="flex items-center gap-4">
					<Avatar class="size-14 border">
						<AvatarFallback class="text-lg">{auth.initials}</AvatarFallback>
					</Avatar>
					<div class="flex flex-col gap-1">
						<div class="flex items-center gap-2">
							<CardTitle>{auth.displayName}</CardTitle>
							<Badge variant="secondary">{auth.user?.role}</Badge>
						</div>
						<CardDescription>@{auth.user?.username}</CardDescription>
					</div>
				</div>
			</CardHeader>
			<CardContent class="flex flex-col gap-4 text-sm">
				<div class="flex justify-between border-t py-2">
					<span class="text-muted-foreground">Domain ID</span>
					<span class="font-medium">#{auth.user?.domainId}</span>
				</div>

				<div class="flex flex-col gap-2">
					<label for="address" class="font-medium text-muted-foreground">Address</label>
					{#if loadingAddress}
						<div class="flex items-center gap-2 text-muted-foreground">
							<Spinner class="size-4" /> Loading…
						</div>
					{:else}
						<div class="flex flex-col gap-2 sm:flex-row">
							<Input id="address" bind:value={addressDraft} placeholder="Your address" class="flex-1" />
							<Button onclick={saveAddress} disabled={savingAddress || !addressDirty}>
								{savingAddress ? "Saving…" : "Save"}
							</Button>
						</div>
					{/if}
				</div>
			</CardContent>
		</Card>

		{#if auth.isBuyer}
			<Card>
				<CardHeader>
					<CardTitle>Order history</CardTitle>
					<CardDescription>Your past purchases.</CardDescription>
				</CardHeader>
				<CardContent>
					{#if loadingOrders}
						<div class="flex items-center justify-center py-10">
							<Spinner class="size-6" />
						</div>
					{:else if orders.length === 0}
						<p class="py-6 text-center text-sm text-muted-foreground">
							You haven't placed any orders yet.
						</p>
					{:else}
						<Table>
							<TableHeader>
								<TableRow>
									<TableHead>Product</TableHead>
									<TableHead class="text-right">Qty</TableHead>
									<TableHead class="text-right">Total</TableHead>
									<TableHead class="text-right">Date</TableHead>
								</TableRow>
							</TableHeader>
							<TableBody>
								{#each orders as o (o.id)}
									<TableRow>
										<TableCell class="font-medium">
											<a href="/products/{o.productId}" class="hover:underline">{o.productName}</a>
										</TableCell>
										<TableCell class="text-right">{o.quantity}</TableCell>
										<TableCell class="text-right">{formatPrice(o.totalPrice)}</TableCell>
										<TableCell class="text-right">{formatDate(o.orderDate)}</TableCell>
									</TableRow>
								{/each}
							</TableBody>
						</Table>
					{/if}
				</CardContent>
			</Card>
		{/if}
	{/if}
</div>
