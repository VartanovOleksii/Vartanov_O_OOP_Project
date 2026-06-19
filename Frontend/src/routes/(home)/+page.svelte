<script>
    import {Input} from "$lib/components/ui/input/index.ts";
    import {Button} from "$lib/components/ui/button/index.ts";
    import {AspectRatio} from "$lib/components/ui/aspect-ratio/index.ts";
    import {
        Card,
        CardDescription,
        CardFooter,
        CardHeader,
        CardTitle
    } from "$lib/components/ui/card/index.ts";
    import {
        Pagination,
        PaginationContent,
        PaginationEllipsis,
        PaginationItem,
        PaginationLink,
        PaginationNext,
        PaginationPrevious
    } from "$lib/components/ui/pagination/index.ts";
    import {ChevronRightIcon, PackageIcon, SearchIcon, SearchXIcon, SparklesIcon} from "@lucide/svelte";
    import {searchProducts} from "$lib/utils/search.js";

    let {data} = $props()

    let allProducts = $derived(data.products ?? [])

    let query = $state("")
    let currentPage = $state(1)
    const perPage = 8

    let filtered = $derived(searchProducts(allProducts, query))
    let pageCount = $derived(Math.max(1, Math.ceil(filtered.length / perPage)))
    let safePage = $derived(Math.min(currentPage, pageCount))
    let paged = $derived(filtered.slice((safePage - 1) * perPage, safePage * perPage))

    // Reset to the first page whenever the search query changes.
    $effect(() => {
        void query
        currentPage = 1
    })

    function formatPrice(value) {
        const amount = Number(value)
        return Number.isFinite(amount) ? `$${amount.toFixed(2)}` : `$${value ?? "\u2014"}`
    }
</script>

<div class="mx-auto flex max-w-6xl flex-col gap-8">
    <div class="flex flex-row items-center justify-between">
        <div class="flex flex-col gap-1">
            <h2 class="font-handwrite text-3xl font-bold tracking-tight">Browse products</h2>
            <p class="text-sm text-muted-foreground">
                {filtered.length}
                {filtered.length === 1 ? "product" : "products"}
                {query.trim() ? `matching “${query.trim()}”` : "available"}
            </p>
        </div>
        <div class="relative mt-2 w-full max-w-md">
            <SearchIcon
                    class="pointer-events-none absolute left-3 top-1/2 size-4 -translate-y-1/2 text-muted-foreground"/>
            <Input bind:value={query} placeholder="Search products..." class="h-11 pl-9"/>
        </div>
    </div>


    {#if paged.length === 0}
        <div class="flex flex-col items-center gap-3 py-20 text-center">
            <div class="flex size-14 items-center justify-center rounded-full bg-muted text-muted-foreground">
                <SearchXIcon class="size-7"/>
            </div>
            <p class="text-lg font-medium">No products found</p>
            <p class="text-sm text-muted-foreground">
                {query.trim() ? `Nothing matches “${query.trim()}”.` : "There are no products yet."}
            </p>
            {#if query.trim()}
                <Button variant="secondary" size="sm" onclick={() => (query = "")}>
                    Clear search
                </Button>
            {/if}
        </div>
    {:else}
        <div class="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4">
            {#each paged as product (product.id)}
                <Card class="group/tile gap-4 overflow-hidden pt-0 transition-all hover:-translate-y-0.5 hover:shadow-md">
                    <a href="/products/{product.id}" class="block">
                        <AspectRatio ratio={4 / 3} class="bg-gradient-to-br from-muted to-muted/40">
                            <div class="relative h-full w-full">
                                <!-- Fallback icon sits behind the image; if the image
                                     is missing or fails to load it stays visible. -->
                                <div class="flex h-full w-full items-center justify-center text-muted-foreground">
                                    <PackageIcon class="size-10 transition-transform group-hover/tile:scale-110"/>
                                </div>
                                {#if product.images?.[0]?.imagePath}
                                    <img
                                        src={product.images[0].imagePath}
                                        alt={product.images[0].altText || product.name}
                                        loading="lazy"
                                        class="absolute inset-0 h-full w-full object-cover transition-transform group-hover/tile:scale-105"
                                        onerror={(e) => e.currentTarget.remove()}
                                    />
                                {/if}
                            </div>
                        </AspectRatio>
                    </a>
                    <CardHeader>
                        <CardTitle class="line-clamp-1">
                            <a href="/products/{product.id}" class="hover:underline">{product.name}</a>
                        </CardTitle>
                        <CardDescription class="line-clamp-2">{product.description}</CardDescription>
                    </CardHeader>
                    <CardFooter class="mt-auto justify-between">
                        <span class="text-lg font-bold">{formatPrice(product.price)}</span>
                        <Button variant="secondary" size="sm" href="/products/{product.id}">
                            View
                            <ChevronRightIcon/>
                        </Button>
                    </CardFooter>
                </Card>
            {/each}
        </div>
    {/if}

    {#if pageCount > 1}
        <Pagination count={filtered.length} {perPage} bind:page={currentPage} siblingCount={1}>
            {#snippet children({pages, currentPage})}
                <PaginationContent>
                    <PaginationItem>
                        <PaginationPrevious/>
                    </PaginationItem>
                    {#each pages as page (page.key)}
                        {#if page.type === "ellipsis"}
                            <PaginationItem>
                                <PaginationEllipsis/>
                            </PaginationItem>
                        {:else}
                            <PaginationItem>
                                <PaginationLink {page} isActive={currentPage === page.value}>
                                    {page.value}
                                </PaginationLink>
                            </PaginationItem>
                        {/if}
                    {/each}
                    <PaginationItem>
                        <PaginationNext/>
                    </PaginationItem>
                </PaginationContent>
            {/snippet}
        </Pagination>
    {/if}
</div>