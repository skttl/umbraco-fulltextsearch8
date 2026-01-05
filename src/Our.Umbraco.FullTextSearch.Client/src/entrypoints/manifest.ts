export const manifests: Array<UmbExtensionManifest> = [
    {
        alias: "our.umbraco.fulltextsearch.entrypoint",
        name: "Our.Umbraco.FullTextSearch.EntryPoint",
        type: "backofficeEntryPoint",
        js: () => import("./entrypoint.js"),
    },
];
