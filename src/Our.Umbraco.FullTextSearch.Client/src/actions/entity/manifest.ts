import { UMB_DOCUMENT_ENTITY_TYPE, UMB_DOCUMENT_DETAIL_REPOSITORY_ALIAS, UMB_DOCUMENT_ROOT_ENTITY_TYPE } from "@umbraco-cms/backoffice/document";
import { ManifestEntityAction } from "@umbraco-cms/backoffice/extension-registry";

const entityAction: ManifestEntityAction = {
    type: 'entityAction',
    kind: 'default',
    alias: 'our.umbraco.fulltextsearch.reindex.action',
    name: 'ReindexNode',
    weight: -100,
    forEntityTypes: [UMB_DOCUMENT_ROOT_ENTITY_TYPE, UMB_DOCUMENT_ENTITY_TYPE],
    api: () => import('./reindex.action.ts'),
    elementName: 'our-umbraco-fulltext-search-actions-entity-reindexnode',
    meta: {
        icon: 'icon-alarm-clock',
        label: '#fulltextsearch_reindex',
        repositoryAlias: UMB_DOCUMENT_DETAIL_REPOSITORY_ALIAS,
    }
}

export const manifests = [entityAction];
