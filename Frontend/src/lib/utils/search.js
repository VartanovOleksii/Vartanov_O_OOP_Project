/**
 * Lightweight, dependency-free fuzzy search for product listings.
 *
 * Scores subsequence matches (the query characters must appear in order, but
 * not necessarily contiguously), with bonuses for contiguous runs and for
 * matches that land at the start of a word.
 */

/**
 * Scores how well `query` fuzzily matches `text`.
 * @returns {number} A non-negative score (higher is better), or -1 if `query`
 *   is not a subsequence of `text`.
 */
function fuzzyScore(query, text) {
    if (!query) return 0;

    const q = query.toLowerCase();
    const t = (text ?? "").toLowerCase();
    if (!t) return -1;

    let qi = 0;
    let score = 0;
    let streak = 0;
    let lastMatch = -2;

    for (let ti = 0; ti < t.length && qi < q.length; ti++) {
        if (t[ti] !== q[qi]) continue;

        if (lastMatch === ti - 1) {
            // Contiguous run: reward longer streaks more heavily.
            streak += 1;
            score += 6 + streak * 3;
        } else {
            streak = 0;
            score += 1;
        }

        // Bonus for matching at the start of a word.
        const prev = t[ti - 1];
        if (ti === 0 || prev === " " || prev === "-" || prev === "_") {
            score += 4;
        }

        lastMatch = ti;
        qi += 1;
    }

    return qi === q.length ? score : -1;
}

/** Builds the searchable text for a product (name + description). */
function haystack(product) {
    return `${product.name ?? ""} ${product.description ?? ""}`;
}

/**
 * Filters and ranks products against a query.
 *
 * Every whitespace-separated token must fuzzily match the product's searchable
 * text; surviving products are sorted by descending score, with a boost when
 * the product name itself matches the full query. An empty query returns the
 * products unchanged.
 *
 * @param {Array<object>} products
 * @param {string} query
 * @returns {Array<object>}
 */
export function searchProducts(products, query) {
    const trimmed = (query ?? "").trim();
    if (!trimmed) return products;

    const tokens = trimmed.toLowerCase().split(/\s+/);
    const ranked = [];

    for (const product of products) {
        const hay = haystack(product);
        let total = 0;
        let matched = true;

        for (const token of tokens) {
            const score = fuzzyScore(token, hay);
            if (score < 0) {
                matched = false;
                break;
            }
            total += score;
        }

        if (!matched) continue;

        // Boost results whose name matches the full query.
        const nameScore = fuzzyScore(trimmed, product.name ?? "");
        if (nameScore > 0) total += nameScore * 2;

        ranked.push({ product, score: total });
    }

    ranked.sort((a, b) => b.score - a.score);
    return ranked.map((entry) => entry.product);
}
