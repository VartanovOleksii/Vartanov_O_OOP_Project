<script lang="ts">
    import {superForm} from "sveltekit-superforms";
    import {zod4Client} from "sveltekit-superforms/adapters";
    import {goto} from "$app/navigation";
    import {auth} from "$lib/stores/auth.svelte.ts";
    import {loginSchema} from "./schema";
    import * as Form from "$lib/components/ui/form/index.ts";
    import {Input} from "$lib/components/ui/input/index.ts";
    import {Button} from "$lib/components/ui/button/index.ts";
    import {Alert, AlertDescription} from "$lib/components/ui/alert/index.ts";
    import {
        Card,
        CardContent,
        CardDescription,
        CardFooter,
        CardHeader,
        CardTitle
    } from "$lib/components/ui/card/index.ts";

    let {data} = $props();

    let errorMsg = $state("");

    const form = superForm(data.form, {
        validators: zod4Client(loginSchema),
        // Handle navigation ourselves so superForm's post-submit applyAction /
        // invalidateAll doesn't race the redirect below.
        applyAction: false,
        invalidateAll: false,
        onUpdate({result}) {
            errorMsg = "";
            if (result.type === "failure" && result.data?.error) {
                errorMsg = result.data.error;
            }
            if (result.type === "success" && result.data?.loginResult) {
                auth.login(result.data.loginResult);
                goto("/", {invalidateAll: true});
            }
        }
    });
    const {form: formData, enhance, submitting} = form;
</script>

<svelte:head>
    <title>Sign in · Vydelka</title>
</svelte:head>

<Card>
    <CardHeader>
        <CardTitle class="text-xl font-bold">Sign in</CardTitle>
        <CardDescription>
            First time here?
            <a href="/auth/register" class="font-medium text-foreground underline-offset-4 hover:underline">
                Create an account
            </a>
        </CardDescription>
    </CardHeader>
    <form method="POST" use:enhance class="contents">
        <CardContent class="flex flex-col gap-4">
            {#if errorMsg}
                <Alert variant="destructive">
                    <AlertDescription>{errorMsg}</AlertDescription>
                </Alert>
            {/if}

            <Form.Field {form} name="username">
                <Form.Control>
                    {#snippet children({props})}
                        <Form.Label>Username</Form.Label>
                        <Input {...props} bind:value={$formData.username} placeholder="alice"
                               autocomplete="username"/>
                    {/snippet}
                </Form.Control>
                <Form.FieldErrors/>
            </Form.Field>

            <Form.Field {form} name="password">
                <Form.Control>
                    {#snippet children({props})}
                        <Form.Label>Password</Form.Label>
                        <Input {...props} type="password" bind:value={$formData.password}
                               autocomplete="current-password"/>
                    {/snippet}
                </Form.Control>
                <Form.FieldErrors/>
            </Form.Field>
        </CardContent>
        <CardFooter class="flex flex-col gap-2">
            <Button type="submit" class="w-full" disabled={$submitting}>Sign in</Button>
        </CardFooter>
    </form>
</Card>
