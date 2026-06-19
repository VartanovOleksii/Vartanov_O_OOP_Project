<script lang="ts">
	import ThemeToggle from "$lib/components/theme-toggle.svelte";
	import { PlugIcon } from "@lucide/svelte";

	let { children } = $props();
</script>

<div class="flex min-h-screen flex-col">
	<header class="flex h-16 items-center justify-between px-4 sm:px-6">
		<a
			href="/"
			class="flex items-center gap-2 font-heading text-lg font-bold tracking-tight"
		>
			<span
				class="flex size-8 items-center justify-center rounded-lg bg-primary text-primary-foreground"
			>
				<PlugIcon class="size-4" />
			</span>
			Vydelka
		</a>
		<ThemeToggle />
	</header>

	<main class="flex flex-1 items-center justify-center px-4 py-10">
		<div class="w-full max-w-md">
			{@render children()}
		</div>
	</main>
</div>
