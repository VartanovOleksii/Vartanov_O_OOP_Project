<script>
    import {Button} from "$lib/components/ui/button/index.ts";
    import {Separator} from "$lib/components/ui/separator/index.ts";
    import {AspectRatio} from "$lib/components/ui/aspect-ratio/index.ts";
    import {Input} from "$lib/components/ui/input/index.ts";
    import {
        Card,
        CardHeader
    } from "$lib/components/ui/card/index.ts";
    import {
        Breadcrumb,
        BreadcrumbItem,
        BreadcrumbLink,
        BreadcrumbList,
        BreadcrumbPage,
        BreadcrumbSeparator
    } from "$lib/components/ui/breadcrumb/index.ts";
    import {
        Tabs,
        TabsContent,
        TabsList,
        TabsTrigger
    } from "$lib/components/ui/tabs/index.ts";
    import {
        PackageIcon,
        RotateCcwIcon,
        ShieldCheckIcon,
        ShoppingCartIcon,
        StoreIcon,
        TruckIcon
    } from "@lucide/svelte";
    import {toast} from "svelte-sonner";
    import {goto} from "$app/navigation";
    import {auth} from "$lib/stores/auth.svelte.ts";
    import {api} from "$lib/utils/api.js";
    import ProductMissing from "./ProductMissing.svelte";

    let {data} = $props()

    let productId = $derived(data.product_id)
    let product = $derived(data.product)

    // The backend exposes no seller profile (name/email) — only a sellerId.
    let images = $derived(product?.images ?? [])
    let primaryImage = $derived(images[0] ?? null)

    let price = $derived(Number(product?.price))
    let priceLabel = $derived(
        Number.isFinite(price) ? `$${price.toFixed(2)}` : `$${product?.price ?? "—"}`
    )
    let inStock = $derived((product?.quantity ?? 0) > 0)

    let quantity = $state(1)
    let adding = $state(false)

    async function addToCart() {
        if (!auth.isBuyer) {
            goto("/auth/login")
            return
        }
        adding = true
        try {
            await api.post("/cart", {productId: product.id, quantity: Number(quantity)})
            toast.success("Added to cart")
        } catch (error) {
            toast.error(error?.response?.data?.error ?? "Could not add to cart")
        } finally {
            adding = false
        }
    }
</script>

{#if !product?.id}
    <ProductMissing {productId}/>
{:else}
    <div class="mx-auto flex max-w-6xl flex-col gap-8">
        <Breadcrumb>
            <BreadcrumbList>
                <BreadcrumbItem>
                    <BreadcrumbLink href="/">Home</BreadcrumbLink>
                </BreadcrumbItem>
                <BreadcrumbSeparator/>
                <BreadcrumbItem>
                    <BreadcrumbLink href="/">Products</BreadcrumbLink>
                </BreadcrumbItem>
                <BreadcrumbSeparator/>
                <BreadcrumbItem>
                    <BreadcrumbPage>{product.name}</BreadcrumbPage>
                </BreadcrumbItem>
            </BreadcrumbList>
        </Breadcrumb>

        <div class="grid grid-cols-1 gap-8 lg:grid-cols-2">
            <!-- Gallery -->
            <div class="flex flex-col gap-4">
                <AspectRatio ratio={1} class="overflow-hidden rounded-xl border bg-muted">
                    {#if primaryImage}
                        <img
                            src={primaryImage.imagePath}
                            alt={primaryImage.altText ?? product.name}
                            class="h-full w-full object-cover"
                        />
                    {:else}
                        <div class="flex h-full w-full items-center justify-center text-muted-foreground">
                            <PackageIcon class="size-20"/>
                        </div>
                    {/if}
                </AspectRatio>
                {#if images.length > 1}
                    <div class="grid grid-cols-4 gap-3">
                        {#each images.slice(0, 4) as image (image.id)}
                            <AspectRatio ratio={1} class="overflow-hidden rounded-lg border bg-muted">
                                <img
                                    src={image.imagePath}
                                    alt={image.altText ?? product.name}
                                    class="h-full w-full object-cover"
                                />
                            </AspectRatio>
                        {/each}
                    </div>
                {/if}
            </div>

            <!-- Summary -->
            <div class="flex flex-col gap-5">
                <div class="flex flex-col gap-2">
                    <h1 class="font-heading text-3xl font-bold tracking-tight text-accent-foreground">{product.name}</h1>
                    <p class="text-sm text-muted-foreground">
                        {inStock ? `${product.quantity} in stock` : "Out of stock"}
                    </p>
                </div>

                <div class="text-4xl font-bold text-accent-foreground">{priceLabel}</div>

                <p class="text-muted-foreground">{product.description}</p>

                <Separator/>

                <div class="flex flex-col gap-3 sm:flex-row sm:items-end">
                    {#if auth.isBuyer}
                        <div class="flex flex-col gap-1">
                            <label for="qty" class="text-xs text-muted-foreground">Quantity</label>
                            <Input
                                id="qty"
                                type="number"
                                min="1"
                                max={product.quantity}
                                bind:value={quantity}
                                class="w-24"
                            />
                        </div>
                        <Button size="lg" class="flex-1" onclick={addToCart} disabled={adding || !inStock}>
                            <ShoppingCartIcon/>
                            {inStock ? "Add to cart" : "Out of stock"}
                        </Button>
                    {:else if !auth.isAuthenticated}
                        <Button size="lg" class="flex-1" href="/auth/login">
                            <ShoppingCartIcon/>
                            Sign in to buy
                        </Button>
                    {:else}
                        <Button size="lg" class="flex-1" disabled>
                            <ShoppingCartIcon/>
                            Sellers cannot purchase
                        </Button>
                    {/if}
                </div>

                <div class="grid grid-cols-3 gap-3 text-center text-xs text-muted-foreground">
                    <div class="flex flex-col items-center gap-1">
                        <TruckIcon class="size-5"/>
                        Free shipping
                    </div>
                    <div class="flex flex-col items-center gap-1">
                        <RotateCcwIcon class="size-5"/>
                        30-day returns
                    </div>
                    <div class="flex flex-col items-center gap-1">
                        <ShieldCheckIcon class="size-5"/>
                        Secure checkout
                    </div>
                </div>

                <Card>
                    <CardHeader>
                        <div class="flex items-center gap-3">
                            <div class="flex size-11 items-center justify-center rounded-full bg-muted text-muted-foreground">
                                <StoreIcon class="size-5"/>
                            </div>
                            <div class="flex flex-col">
                                <span class="font-semibold">Seller #{product.sellerId}</span>
                                <span class="text-sm text-muted-foreground">
                                    <!-- TODO: backend doesn't expose seller profile (name/contact) yet -->
                                    Sold by seller {product.sellerId}
                                </span>
                            </div>
                        </div>
                    </CardHeader>
                </Card>
            </div>
        </div>

        <!-- Details -->
        <Tabs value="description" class="w-full">
            <TabsList>
                <TabsTrigger value="description">Description</TabsTrigger>
                <TabsTrigger value="specifications">Specifications</TabsTrigger>
            </TabsList>
            <TabsContent value="description" class="pt-4 text-muted-foreground">
                <p>{product.description}</p>
            </TabsContent>
            <TabsContent value="specifications" class="pt-4">
                <div class="divide-y text-sm">
                    <div class="flex justify-between py-2">
                        <span class="text-muted-foreground">Product ID</span>
                        <span class="font-medium">#{product.id}</span>
                    </div>
                    <div class="flex justify-between py-2">
                        <span class="text-muted-foreground">Price</span>
                        <span class="font-medium">{priceLabel}</span>
                    </div>
                    <div class="flex justify-between py-2">
                        <span class="text-muted-foreground">In stock</span>
                        <span class="font-medium">{product.quantity}</span>
                    </div>
                    <div class="flex justify-between py-2">
                        <span class="text-muted-foreground">Seller</span>
                        <span class="font-medium">#{product.sellerId}</span>
                    </div>
                </div>
            </TabsContent>
        </Tabs>
    </div>
{/if}
