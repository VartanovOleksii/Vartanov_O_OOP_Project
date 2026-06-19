import {defineConfig} from 'vitest/config';
import {playwright} from '@vitest/browser-playwright';
import adapter from '@sveltejs/adapter-auto';
import {sveltekit} from '@sveltejs/kit/vite';
import tailwindcss from "@tailwindcss/vite";

export default defineConfig({
    plugins: [
        sveltekit({
            compilerOptions: {
                // Force runes mode for the project, except for libraries. Can be removed in svelte 6.
                runes: ({filename}) => filename.split(/[/\\]/).includes('node_modules') ? undefined : true
            },

            // adapter-auto only supports some environments, see https://svelte.dev/docs/kit/adapter-auto for a list.
            // If your environment is not supported, or you settled on a specific environment, switch out the adapter.
            // See https://svelte.dev/docs/kit/adapters for more information about adapters.
            adapter: adapter()
        }),
        tailwindcss()
    ],
    server: {
        watch: {
            // Don't watch the static assets folder. Image files dropped here can
            // be briefly locked by other apps (e.g. an image viewer), which would
            // otherwise crash Vite's file watcher (EBUSY). Static files are still
            // served regardless of watching.
            ignored: ['**/static/**']
        }
    },
    test: {
        expect: {requireAssertions: true},
        projects: [
            {
                extends: './vite.config.js',
                test: {
                    name: 'client',
                    browser: {
                        enabled: true,
                        provider: playwright(),
                        instances: [{browser: 'chromium', headless: true}]
                    },
                    include: ['src/**/*.svelte.{test,spec}.{js,ts}'],
                    exclude: ['src/lib/server/**']
                }
            },

            {
                extends: './vite.config.js',
                test: {
                    name: 'server',
                    environment: 'node',
                    include: ['src/**/*.{test,spec}.{js,ts}'],
                    exclude: ['src/**/*.svelte.{test,spec}.{js,ts}']
                }
            }
        ]
    }
});
