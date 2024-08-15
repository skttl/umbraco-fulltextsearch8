import { ManifestModal } from "@umbraco-cms/backoffice/extension-registry";

const reIndexNodeModal: ManifestModal = {
    type: 'modal',
    alias: 'our.umbraco.fulltextsearch.modals.reindexnode',
    name: 'Reindex node',
    js: () => import('./reindexnode.element.ts'),
}

export const manifests = [reIndexNodeModal];
