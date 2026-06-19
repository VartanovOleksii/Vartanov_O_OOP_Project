<script>
    import "../app.css";
    import {ModeWatcher} from "mode-watcher";
    import {onMount} from "svelte";
    import {Toaster} from "$lib/components/ui/sonner/index.ts";
    import {auth} from "$lib/stores/auth.svelte.ts";

    let {children} = $props();

    // Rehydrate auth from localStorage once we're on the client (avoids SSR
    // hydration mismatch — the store renders logged-out on the server).
    onMount(() => auth.init());
</script>

<svelte:head>
    <link rel="apple-touch-icon" sizes="180x180" href="/favicon/apple-touch-icon.png">
    <link rel="icon" type="image/png" sizes="32x32" href="/favicon/favicon-32x32.png">
    <link rel="icon" type="image/png" sizes="16x16" href="/favicon/favicon-16x16.png">
    <link rel="manifest" href="/favicon/site.webmanifest">
</svelte:head>

<ModeWatcher/>
<Toaster/>

<div class="bg-background min-h-screen">
    {@render children()}
</div>
