<script lang="ts">
	import { page } from "$app/state";
	import { goto } from "$app/navigation";
	import { auth } from "$lib/stores/auth.svelte.ts";
	import ThemeToggle from "$lib/components/theme-toggle.svelte";
	import { Button } from "$lib/components/ui/button/index.ts";
	import { Avatar, AvatarFallback } from "$lib/components/ui/avatar/index.ts";
	import {
		DropdownMenu,
		DropdownMenuContent,
		DropdownMenuGroup,
		DropdownMenuItem,
		DropdownMenuLabel,
		DropdownMenuSeparator,
		DropdownMenuTrigger
	} from "$lib/components/ui/dropdown-menu/index.ts";
	import { ClipboardListIcon, LogOutIcon, PackageIcon, ShoppingCartIcon, UserIcon } from "@lucide/svelte";
	import VydelkaLogo from "$lib/assets/icons/VydelkaLogo.svelte";

	// "My products" is seller-only, "Cart" is buyer-only; Home is always visible.
	const navLinks = $derived(
		[
			{ href: "/", label: "Home", show: true },
			{ href: "/cart", label: "Cart", show: auth.isBuyer },
			{ href: "/my-products", label: "My products", show: auth.isSeller }
		].filter((link) => link.show)
	);

	function isActive(href: string): boolean {
		if (href === "/") return page.url.pathname === "/";
		return page.url.pathname.startsWith(href);
	}

	function handleLogout(): void {
		auth.logout();
		goto("/");
	}
</script>

<header
	class="sticky top-0 z-40 w-full border-b border-border/60 bg-background/80 backdrop-blur supports-[backdrop-filter]:bg-background/60"
>
	<div class="container mx-auto flex h-16 items-center gap-4 px-4">
		<a
			href="/"
			class="flex items-center gap-2 font-heading font-bold tracking-tight"
		>
			<span
				class="flex size-8 items-center justify-center rounded-lg bg-primary text-primary-foreground"
			>
				<VydelkaLogo class="size-8" />
			</span>
			Vydelka
		</a>

		<nav class="hidden items-center gap-1 sm:flex">
			{#each navLinks as link (link.href)}
				<a
					href={link.href}
					class="rounded-md px-3 py-2 text-xl font-medium transition-colors hover:text-foreground font-handwrite {isActive(
						link.href
					)
						? 'text-foreground'
						: 'text-muted-foreground'}"
				>
					{link.label}
				</a>
			{/each}
		</nav>

		<div class="ml-auto flex items-center gap-1.5">
			<ThemeToggle />

			{#if auth.isAuthenticated}
				<DropdownMenu>
					<DropdownMenuTrigger
						class="ml-1 rounded-full ring-offset-background outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2"
						aria-label="Open profile menu"
					>
						<Avatar class="size-9 border">
							<AvatarFallback>{auth.initials}</AvatarFallback>
						</Avatar>
					</DropdownMenuTrigger>
					<DropdownMenuContent class="w-56" align="end">
						<DropdownMenuLabel>
							<div class="flex flex-col">
								<span class="font-medium">{auth.displayName}</span>
								<span class="text-xs font-normal text-muted-foreground">
									{auth.user?.role}
								</span>
							</div>
						</DropdownMenuLabel>
						<DropdownMenuSeparator />
						<DropdownMenuGroup>
							<DropdownMenuItem>
								{#snippet child({ props })}
									<a href="/profile" {...props}>
										<UserIcon class="size-4" />
										Profile settings
									</a>
								{/snippet}
							</DropdownMenuItem>
							{#if auth.isBuyer}
								<DropdownMenuItem>
									{#snippet child({ props })}
										<a href="/cart" {...props}>
											<ShoppingCartIcon class="size-4" />
											Cart
										</a>
									{/snippet}
								</DropdownMenuItem>
								<DropdownMenuItem>
									{#snippet child({ props })}
										<a href="/profile" {...props}>
											<ClipboardListIcon class="size-4" />
											My orders
										</a>
									{/snippet}
								</DropdownMenuItem>
							{/if}
							{#if auth.isSeller}
								<DropdownMenuItem>
									{#snippet child({ props })}
										<a href="/my-products" {...props}>
											<PackageIcon class="size-4" />
											My products
										</a>
									{/snippet}
								</DropdownMenuItem>
							{/if}
						</DropdownMenuGroup>
						<DropdownMenuSeparator />
						<DropdownMenuItem onSelect={handleLogout}>
							<LogOutIcon class="size-4" />
							Log out
						</DropdownMenuItem>
					</DropdownMenuContent>
				</DropdownMenu>
			{:else}
				<Button href="/auth/login" size="sm" class="ml-1">Login</Button>
			{/if}
		</div>
	</div>
</header>
