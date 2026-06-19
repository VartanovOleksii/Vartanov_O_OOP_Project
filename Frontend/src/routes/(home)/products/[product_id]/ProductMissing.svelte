<script>
    import {Button} from "$lib/components/ui/button/index.ts";
    import {PackageIcon, ArrowLeftIcon} from "@lucide/svelte";

    let { productId } = $props()
</script>

<div class="mx-auto flex max-w-md flex-col items-center gap-4 py-24 text-center">
    <div class="flex size-16 items-center justify-center rounded-full bg-muted text-muted-foreground">
        <PackageIcon class="size-8"/>
    </div>
    <h1 class="text-2xl font-bold tracking-tight text-accent-foreground">Product not found</h1>
    <p class="text-muted-foreground">
        We couldn't find a product matching #{productId}.
    </p>
    <Button href="/" variant="secondary">
        <ArrowLeftIcon/>
        Back to products
    </Button>
</div>