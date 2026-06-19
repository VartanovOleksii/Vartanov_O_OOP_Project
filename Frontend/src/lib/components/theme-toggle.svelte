<script lang="ts">
	import { toggleMode } from "mode-watcher";
	import { Button } from "$lib/components/ui/button/index.ts";
	import { MoonIcon, SunIcon } from "@lucide/svelte";
</script>

<Button
	onclick={toggleMode}
	variant="ghost"
	size="icon"
	class="relative"
	aria-label="Toggle light / dark theme"
>
	<SunIcon
		class="size-5 scale-100 rotate-0 transition-all dark:scale-0 dark:-rotate-90"
	/>
	<MoonIcon
		class="absolute size-5 scale-0 rotate-90 transition-all dark:scale-100 dark:rotate-0"
	/>
	<span class="sr-only">Toggle theme</span>
</Button>
