import { UmbEntityUnique } from "@umbraco-cms/backoffice/entity";
import { UmbModalToken } from "@umbraco-cms/backoffice/modal";

export type ReindexNodeModalData = {
    unique: UmbEntityUnique;
}

export const REINDEX_NODE_MODAL_TOKEN = new UmbModalToken<ReindexNodeModalData>('our.umbraco.fulltextsearch.modals.reindexnode', {
    modal: {
        type: 'dialog',
        size: 'small'
    }
});