<script lang="ts">
	import { auth } from "$lib/stores/auth.svelte.ts";
	import { ExternalLinkIcon, PlugIcon } from "@lucide/svelte";

	const BACKEND_GITHUB_URL = "https://github.com/VartanovOleksii/Vartanov_OOP_Project";

	// Real auth is wired up now — show the current role as a read-only indicator
	// (no more dev role switcher).
	const currentRole = $derived(auth.user?.role ?? "GUEST");
	const roleLabels: Record<string, string> = {
		GUEST: "Guest",
		BUYER: "Buyer",
		SELLER: "Seller"
	};
</script>

<footer class="mt-auto border-t border-border/60 bg-muted/30">
	<div class="container mx-auto flex flex-col gap-8 px-4 py-10">
		<div class="grid gap-8 sm:grid-cols-3">
			<div class="flex flex-col gap-2">
				<div class="flex items-center gap-2 font-heading font-semibold">
					<PlugIcon class="size-4" />
					Vydelka
				</div>
				<p class="text-sm text-muted-foreground">
					Vydelka because "like Rozetka".
					Marketplace PoC implemented via ASP.NET backend + SvelteKit frontend
				</p>
			</div>

			<div class="flex flex-col gap-2">
				<h3 class="text-sm font-semibold">Special thanks</h3>
				<p class="text-sm text-muted-foreground">
					@ntainy, ANTHROP\C. Praise be to the Omnissiah
				</p>
			</div>

			<div class="flex flex-col gap-2">
				<h3 class="text-sm font-semibold">Project</h3>
				<a
					href={BACKEND_GITHUB_URL}
					target="_blank"
					rel="noreferrer noopener"
					class="inline-flex w-fit items-center gap-2 text-sm text-muted-foreground transition-colors hover:text-foreground"
				>
					<ExternalLinkIcon class="size-4" />
					Backend source on GitHub
				</a>
			</div>
		</div>

		<div
			class="flex flex-col gap-3 border-t border-border/60 pt-6 text-xs text-muted-foreground sm:flex-row sm:items-center sm:justify-between"
		>
			<span>© {new Date().getFullYear()} Oleksii Vartanov</span>

			<!-- Read-only indicator of the signed-in role. -->
			<span class="font-medium">Signed in as: {roleLabels[currentRole]}</span>
		</div>
	</div>
</footer>
