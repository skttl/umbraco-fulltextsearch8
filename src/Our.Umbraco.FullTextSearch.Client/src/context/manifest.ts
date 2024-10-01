import { ManifestGlobalContext } from "@umbraco-cms/backoffice/extension-registry";

const contexts: Array<ManifestGlobalContext> = [
    {
        type: 'globalContext',
        alias: 'our.umbraco.fulltextsearch.context',
        name: 'Full Text Search context',
        js: () => import('./fulltextsearch.context.ts')
    }
]

export const manifests = [...contexts];