
import { defineConfig } from "vite";

export default defineConfig({
    build: {
        minify: true,
        lib: {
            entry: "src/index.ts", // your web component source file
            formats: ["es"],
            fileName: 'assets'
        },
        outDir: "../Our.Umbraco.FullTextSearch/wwwroot", // all compiled files will be placed here
        emptyOutDir: true,
        sourcemap: true,
        rollupOptions: {
            external: [/^@umbraco/], // ignore the Umbraco Backoffice package in the build
            output: {
                manualChunks: undefined,
                inlineDynamicImports: true,
                chunkFileNames: `[name]-[hash].js`,
            }
        },
    },
    mode: 'production'
});