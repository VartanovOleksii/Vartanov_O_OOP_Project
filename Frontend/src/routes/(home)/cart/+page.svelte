<script lang="ts">
	import { onMount } from "svelte";
	import { toast } from "svelte-sonner";
	import { auth } from "$lib/stores/auth.svelte.ts";
	import { api } from "$lib/utils/api.js";
	import { Button } from "$lib/components/ui/button/index.ts";
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
	import { LockIcon, ShoppingCartIcon, Trash2Icon } from "@lucide/svelte";

	interface CartItem {
		id: number;
		productId: number;
		productName: string;
		price: number;
		cartQuantity: number;
	}

	let items = $state<CartItem[]>([]);
	let loading = $state(true);
	let busyId = $state<number | null>(null);
	let buyingAll = $state(false);

	const total = $derived(
		items.reduce((sum, i) => sum + i.price * i.cartQuantity, 0)
	);

	async function loadCart() {
		loading = true;
		try {
			const res = await api.get("/cart");
			items = res.data ?? [];
		} catch (error: any) {
			toast.error(error?.response?.data?.error ?? "Could not load cart");
		} finally {
			loading = false;
		}
	}

	async function removeItem(id: number) {
		busyId = id;
		try {
			await api.delete(`/cart/${id}`);
			items = items.filter((i) => i.id !== id);
			toast.success("Removed from cart");
		} catch (error: any) {
			toast.error(error?.response?.data?.error ?? "Could not remove item");
		} finally {
			busyId = null;
		}
	}

	async function order(item: CartItem) {
		busyId = item.id;
		try {
			await api.post("/orders", { productId: item.productId, quantity: item.cartQuantity });
			// Order placed — clear the line from the cart.
			await api.delete(`/cart/${item.id}`);
			items = items.filter((i) => i.id !== item.id);
			toast.success("Order placed");
		} catch (error: any) {
			toast.error(error?.response?.data?.error ?? "Could not place order");
		} finally {
			busyId = null;
		}
	}

	async function buyAll() {
		buyingAll = true;
		let ok = 0;
		let failed = 0;
		// Iterate over a snapshot; place an order per item and drop the ones that
		// succeed. Items that fail (e.g. out of stock) stay in the cart.
		for (const item of [...items]) {
			try {
				await api.post("/orders", { productId: item.productId, quantity: item.cartQuantity });
				await api.delete(`/cart/${item.id}`);
				items = items.filter((i) => i.id !== item.id);
				ok++;
			} catch {
				failed++;
			}
		}
		buyingAll = false;
		if (ok > 0) toast.success(`Ordered ${ok} item${ok === 1 ? "" : "s"}`);
		if (failed > 0) toast.error(`${failed} item${failed === 1 ? "" : "s"} could not be ordered`);
	}

	onMount(() => {
		// auth.init() is idempotent; call it here so this page doesn't depend on
		// the root layout's onMount ordering.
		auth.init();
		if (auth.isBuyer) loadCart();
		else loading = false;
	});

	function formatPrice(value: number) {
		return `$${Number(value).toFixed(2)}`;
	}
</script>

<svelte:head>
	<title>Cart · Vydelka</title>
</svelte:head>

<div class="mx-auto flex max-w-4xl flex-col gap-6">
	<div class="flex flex-col gap-1">
		<h1 class="font-handwrite text-3xl font-bold tracking-tight text-accent-foreground">
			Your cart
		</h1>
		<p class="text-sm text-muted-foreground">Review the items in your cart.</p>
	</div>

	{#if !auth.isBuyer}
		<div class="flex flex-col items-center gap-3 py-20 text-center">
			<div class="flex size-14 items-center justify-center rounded-full bg-muted text-muted-foreground">
				<LockIcon class="size-7" />
			</div>
			<p class="text-lg font-medium">Buyer access only</p>
			<p class="max-w-sm text-sm text-muted-foreground">
				Sign in as a buyer to use your cart.
			</p>
			<Button href="/auth/login" variant="secondary" size="sm">Go to login</Button>
		</div>
	{:else if loading}
		<div class="flex items-center justify-center py-20">
			<Spinner class="size-6" />
		</div>
	{:else if items.length === 0}
		<div class="flex flex-col items-center gap-3 py-20 text-center">
			<div class="flex size-14 items-center justify-center rounded-full bg-muted text-muted-foreground">
				<ShoppingCartIcon class="size-7" />
			</div>
			<p class="text-lg font-medium">Your cart is empty</p>
			<Button href="/" variant="secondary" size="sm">Browse products</Button>
		</div>
	{:else}
		<Card>
			<CardHeader>
				<CardTitle>Items</CardTitle>
				<CardDescription>{items.length} {items.length === 1 ? "item" : "items"} in your cart.</CardDescription>
			</CardHeader>
			<CardContent>
				<Table>
					<TableHeader>
						<TableRow>
							<TableHead>Product</TableHead>
							<TableHead class="text-right">Price</TableHead>
							<TableHead class="text-right">Qty</TableHead>
							<TableHead class="text-right">Subtotal</TableHead>
							<TableHead class="text-right">Actions</TableHead>
						</TableRow>
					</TableHeader>
					<TableBody>
						{#each items as item (item.id)}
							<TableRow>
								<TableCell class="font-medium">
									<a href="/products/{item.productId}" class="hover:underline">{item.productName}</a>
								</TableCell>
								<TableCell class="text-right">{formatPrice(item.price)}</TableCell>
								<TableCell class="text-right">{item.cartQuantity}</TableCell>
								<TableCell class="text-right">{formatPrice(item.price * item.cartQuantity)}</TableCell>
								<TableCell class="text-right">
									<div class="flex justify-end gap-2">
										<Button
											size="sm"
											onclick={() => order(item)}
											disabled={busyId === item.id || buyingAll}
										>
											Buy
										</Button>
										<Button
											size="icon"
											variant="ghost"
											onclick={() => removeItem(item.id)}
											disabled={busyId === item.id || buyingAll}
											aria-label="Remove item"
										>
											<Trash2Icon class="size-4" />
										</Button>
									</div>
								</TableCell>
							</TableRow>
						{/each}
					</TableBody>
				</Table>

				<div class="mt-4 flex items-center justify-between gap-4 border-t pt-4">
					<div class="flex items-center gap-3">
						<span class="text-sm text-muted-foreground">Total</span>
						<span class="text-lg font-bold">{formatPrice(total)}</span>
					</div>
					<Button onclick={buyAll} disabled={buyingAll || busyId !== null || items.length === 0}>
						{buyingAll ? "Buying…" : "Buy All"}
					</Button>
				</div>
			</CardContent>
		</Card>
	{/if}
</div>
