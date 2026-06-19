<script>
    import SiteHeader from "$lib/components/site-header.svelte";
    import SiteFooter from "$lib/components/site-footer.svelte";

    let {children} = $props();
</script>

<div class="flex min-h-screen flex-col">
    <SiteHeader/>
    <main class="container mx-auto w-full flex-1 px-4 py-8">
        {@render children()}
    </main>
    <SiteFooter/>
</div>
