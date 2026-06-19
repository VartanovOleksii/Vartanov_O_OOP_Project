<script lang="ts">
	import { onMount } from "svelte";
	import { toast } from "svelte-sonner";
	import { auth } from "$lib/stores/auth.svelte.ts";
	import { api } from "$lib/utils/api.js";
	import { Button } from "$lib/components/ui/button/index.ts";
	import { Input } from "$lib/components/ui/input/index.ts";
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
	import { LockIcon, PackageIcon, PlusIcon, Trash2Icon } from "@lucide/svelte";
	import CreateProductDialog from "../components/CreateProductDialog.svelte";

	interface Product {
		id: number;
		name: string;
		description: string;
		price: number;
		quantity: number;
		sellerId: number;
		images: { id: number; imagePath: string; altText: string }[];
	}

	let createDialogOpen = $state(false);
	let products = $state<Product[]>([]);
	let statistics = $state("");
	let loading = $state(true);
	let busyId = $state<number | null>(null);
	// Per-product draft stock values (keyed by product id).
	let stockDraft = $state<Record<number, number>>({});

	async function loadData() {
		loading = true;
		try {
			const [productsRes, statsRes] = await Promise.all([
				api.get("/products"),
				api.get("/seller/statistics")
			]);
			const all: Product[] = productsRes.data ?? [];
			// The catalog endpoint returns all products; keep only ours.
			products = all.filter((p) => p.sellerId === auth.user?.domainId);
			for (const p of products) stockDraft[p.id] = p.quantity;
			statistics = statsRes.data?.statistics ?? "";
		} catch (error: any) {
			toast.error(error?.response?.data?.error ?? "Could not load your products");
		} finally {
			loading = false;
		}
	}

	function onCreated(created: Product) {
		products = [...products, created];
		stockDraft[created.id] = created.quantity;
	}

	async function removeProduct(id: number) {
		busyId = id;
		try {
			await api.delete(`/products/${id}`);
			products = products.filter((p) => p.id !== id);
			toast.success("Product deleted");
		} catch (error: any) {
			toast.error(error?.response?.data?.error ?? "Could not delete product");
		} finally {
			busyId = null;
		}
	}

	async function updateStock(p: Product) {
		const newQuantity = Number(stockDraft[p.id]);
		busyId = p.id;
		try {
			await api.put(`/products/${p.id}/stock`, { newQuantity });
			products = products.map((x) => (x.id === p.id ? { ...x, quantity: newQuantity } : x));
			toast.success("Stock updated");
		} catch (error: any) {
			toast.error(error?.response?.data?.error ?? "Could not update stock");
		} finally {
			busyId = null;
		}
	}

	onMount(() => {
		auth.init();
		if (auth.isSeller) loadData();
		else loading = false;
	});

	function formatPrice(value: number) {
		return `$${Number(value).toFixed(2)}`;
	}
</script>

<svelte:head>
	<title>My products · Vydelka</title>
</svelte:head>

<div class="mx-auto flex max-w-5xl flex-col gap-8">
	{#if !auth.isSeller}
		<div class="flex flex-col items-center gap-3 py-20 text-center">
			<div class="flex size-14 items-center justify-center rounded-full bg-muted text-muted-foreground">
				<LockIcon class="size-7" />
			</div>
			<p class="text-lg font-medium">Seller access only</p>
			<p class="max-w-sm text-sm text-muted-foreground">
				This area is for seller accounts. Sign in as a seller to manage your
				product listings.
			</p>
			<Button href="/auth/login" variant="secondary" size="sm">Go to login</Button>
		</div>
	{:else}
		<div class="flex flex-col gap-4 sm:flex-row sm:items-end sm:justify-between">
			<div class="flex flex-col gap-1">
				<h1 class="font-handwrite text-3xl font-bold tracking-tight text-accent-foreground">
					My products
				</h1>
				<p class="text-sm text-muted-foreground">
					Manage the listings you sell on Vydelka.
				</p>
			</div>
			<Button onclick={() => (createDialogOpen = true)}>
				<PlusIcon class="size-4" />
				Add product
			</Button>
		</div>

		{#if statistics}
			<Card>
				<CardHeader>
					<CardTitle>Statistics</CardTitle>
				</CardHeader>
				<CardContent>
					<pre class="whitespace-pre-wrap text-sm text-muted-foreground">{statistics}</pre>
				</CardContent>
			</Card>
		{/if}

		{#if loading}
			<div class="flex items-center justify-center py-20">
				<Spinner class="size-6" />
			</div>
		{:else if products.length === 0}
			<Card>
				<CardHeader>
					<CardTitle>No products yet</CardTitle>
					<CardDescription>Add your first product to start selling.</CardDescription>
				</CardHeader>
				<CardContent>
					<div class="flex flex-col items-center gap-3 rounded-lg border border-dashed py-14 text-center">
						<div class="flex size-12 items-center justify-center rounded-full bg-muted text-muted-foreground">
							<PackageIcon class="size-6" />
						</div>
						<Button variant="secondary" size="sm" onclick={() => (createDialogOpen = true)}>
							<PlusIcon class="size-4" />
							Add your first product
						</Button>
					</div>
				</CardContent>
			</Card>
		{:else}
			<Card>
				<CardContent class="pt-6">
					<Table>
						<TableHeader>
							<TableRow>
								<TableHead>Product</TableHead>
								<TableHead class="text-right">Price</TableHead>
								<TableHead>Stock</TableHead>
								<TableHead class="text-right">Actions</TableHead>
							</TableRow>
						</TableHeader>
						<TableBody>
							{#each products as p (p.id)}
								<TableRow>
									<TableCell class="font-medium">
										<a href="/products/{p.id}" class="hover:underline">{p.name}</a>
									</TableCell>
									<TableCell class="text-right">{formatPrice(p.price)}</TableCell>
									<TableCell>
										<div class="flex items-center gap-2">
											<Input
												type="number"
												min="0"
												bind:value={stockDraft[p.id]}
												class="h-8 w-20"
											/>
											<Button
												size="sm"
												variant="secondary"
												onclick={() => updateStock(p)}
												disabled={busyId === p.id}
											>
												Update
											</Button>
										</div>
									</TableCell>
									<TableCell class="text-right">
										<Button
											size="icon"
											variant="ghost"
											onclick={() => removeProduct(p.id)}
											disabled={busyId === p.id}
											aria-label="Delete product"
										>
											<Trash2Icon class="size-4" />
										</Button>
									</TableCell>
								</TableRow>
							{/each}
						</TableBody>
					</Table>
				</CardContent>
			</Card>
		{/if}

		<CreateProductDialog bind:open={createDialogOpen} oncreated={onCreated} />
	{/if}
</div>
