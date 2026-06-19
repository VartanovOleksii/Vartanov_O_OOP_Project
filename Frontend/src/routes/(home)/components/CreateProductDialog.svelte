<script>
    import {
        Dialog,
        DialogClose,
        DialogContent,
        DialogDescription,
        DialogFooter,
        DialogHeader,
        DialogTitle
    } from "$lib/components/ui/dialog/index.ts";
    import {Button, buttonVariants} from "$lib/components/ui/button/index.ts";
    import {Label} from "$lib/components/ui/label/index.ts";
    import {Input} from "$lib/components/ui/input/index.ts";
    import {Textarea} from "$lib/components/ui/textarea/index.ts";
    import {toast} from "svelte-sonner";
    import {api} from "$lib/utils/api.js";

    let {open = $bindable(), oncreated} = $props();

    let name = $state("");
    let description = $state("");
    let price = $state(0);
    let quantity = $state(0);
    let imagePath = $state("");
    let saving = $state(false);

    function reset() {
        name = "";
        description = "";
        price = 0;
        quantity = 0;
        imagePath = "";
    }

    // Turn user input into a usable web path: keep external URLs as-is, but for
    // local files convert backslashes to "/" and ensure a leading slash, e.g.
    // "products\foo.png" -> "/products/foo.png".
    function normalizeImagePath(value) {
        const v = value.trim();
        if (!v) return "";
        if (/^https?:\/\//i.test(v)) return v;
        const cleaned = v.replace(/\\/g, "/");
        return cleaned.startsWith("/") ? cleaned : `/${cleaned}`;
    }

    async function save() {
        if (name.trim().length < 3) {
            toast.error("Name must be at least 3 characters long");
            return;
        }
        if (description.trim().length < 10) {
            toast.error("Description must be at least 10 characters long");
            return;
        }
        saving = true;
        try {
            const res = await api.post("/products", {
                name: name.trim(),
                description: description.trim(),
                price: Number(price),
                quantity: Number(quantity)
            });
            let product = res.data;

            // Optionally attach an image. The backend stores the path as-is, so
            // this can be a local "/products/foo.jpg" or an external URL.
            const img = normalizeImagePath(imagePath);
            if (img) {
                try {
                    const withImage = await api.post(`/products/${product.id}/images`, {
                        imagePath: img,
                        altText: name.trim()
                    });
                    product = withImage.data; // ProductResponse including the image
                } catch (imgErr) {
                    toast.error(imgErr?.response?.data?.error ?? "Product created, but the image could not be attached");
                }
            }

            toast.success("Product created");
            oncreated?.(product);
            reset();
            open = false;
        } catch (error) {
            toast.error(error?.response?.data?.error ?? "Could not create product");
        } finally {
            saving = false;
        }
    }
</script>

<Dialog bind:open>
    <DialogContent>
        <DialogHeader>
            <DialogTitle>Add product</DialogTitle>
            <DialogDescription>Add a new product to your listings.</DialogDescription>
        </DialogHeader>

        <div class="flex flex-col gap-4">
            <div class="grid w-full items-center gap-1.5">
                <Label for="name">Product name</Label>
                <Input id="name" bind:value={name} />
                <div class="text-xs text-muted-foreground">Name must be at least 3 characters long</div>
            </div>
            <div class="grid w-full items-center gap-1.5">
                <Label for="description">Product description</Label>
                <Textarea id="description" rows={4} bind:value={description} />
                <div class="text-xs text-muted-foreground">Description must be at least 10 characters long</div>
            </div>
            <div class="grid grid-cols-2 gap-4">
                <div class="grid items-center gap-1.5">
                    <Label for="price">Price</Label>
                    <Input id="price" type="number" min="0" step="0.01" bind:value={price} />
                </div>
                <div class="grid items-center gap-1.5">
                    <Label for="quantity">Quantity</Label>
                    <Input id="quantity" type="number" min="0" bind:value={quantity} />
                </div>
            </div>
            <div class="grid w-full items-center gap-1.5">
                <Label for="imagePath">Image URL (optional)</Label>
                <Input id="imagePath" bind:value={imagePath} placeholder="/products/my-item.jpg or https://…" />
                <div class="text-xs text-muted-foreground">
                    A local path under <code>Frontend/static/products/</code> or an external image URL. Leave empty for a placeholder icon.
                </div>
            </div>
        </div>

        <DialogFooter>
            <DialogClose type="button" class={buttonVariants({ variant: "outline" })}>
                Cancel
            </DialogClose>
            <Button onclick={save} disabled={saving}>Save product</Button>
        </DialogFooter>
    </DialogContent>
</Dialog>
