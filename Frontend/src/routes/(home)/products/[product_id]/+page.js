import {executeApiRequests} from "$lib/utils/api.js";

export const load = async ({params, fetch}) => {
    const product_id = params.product_id
    const response = await executeApiRequests({
        product: {
            endpoint: `/products/${product_id}`
        }
    }, fetch)

    return {
        product_id,
        ...response
    }
}
