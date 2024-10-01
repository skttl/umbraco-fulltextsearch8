import { ManifestLocalization } from "@umbraco-cms/backoffice/extension-registry";

const localizations: ManifestLocalization[] = [
    {
        type: "localization",
        alias: "Our.Umbraco.FullTextSearch.Localizations.En",
        name: "English",
        meta: {
            culture: "en"
        },
        js: () => import("./en.ts")
    },
    {
        type: "localization",
        alias: "Our.Umbraco.FullTextSearch.Localizations.Da",
        name: "Danish",
        meta: {
            culture: "da"
        },
        js: () => import("./da.ts")
    },
    {
        type: "localization",
        alias: "Our.Umbraco.FullTextSearch.Localizations.Cy",
        name: "Welsh",
        meta: {
            culture: "cy"
        },
        js: () => import("./cy.ts")
    },
    {
        type: "localization",
        alias: "Our.Umbraco.FullTextSearch.Localizations.Fr",
        name: "French",
        meta: {
            culture: "fr"
        },
        js: () => import("./fr.ts")
    },
    {
        type: "localization",
        alias: "Our.Umbraco.FullTextSearch.Localizations.Nb",
        name: "Norwegian bokmål",
        meta: {
            culture: "nb"
        },
        js: () => import("./nb.ts")
    },
    {
        type: "localization",
        alias: "Our.Umbraco.FullTextSearch.Localizations.Nl",
        name: "Dutch",
        meta: {
            culture: "nl"
        },
        js: () => import("./nl.ts")
    }
]

export const manifests = localizations;
