import { UmbControllerHostElement } from "@umbraco-cms/backoffice/controller-api";
//import { UmbDocumentItemRepository } from "@umbraco-cms/backoffice/document";
import { UmbEntityActionArgs, UmbEntityActionBase } from "@umbraco-cms/backoffice/entity-action";
import { UMB_MODAL_MANAGER_CONTEXT } from '@umbraco-cms/backoffice/modal';
import { REINDEX_NODE_MODAL_TOKEN } from "../../modals/reindexnode.modaltoken";

export class ReindexNodeAction extends UmbEntityActionBase<never> {
    #modalManagerContext?: typeof UMB_MODAL_MANAGER_CONTEXT.TYPE;


    constructor(host: UmbControllerHostElement, args: UmbEntityActionArgs<never>) {
        super(host, args)

        this.consumeContext(UMB_MODAL_MANAGER_CONTEXT, (instance) => {
            this.#modalManagerContext = instance;
        });
    }

    async execute() {
        const modalContext = this.#modalManagerContext?.open(this, REINDEX_NODE_MODAL_TOKEN, {
            data: {
                unique: this.args.unique
            }
        });

        await modalContext?.onSubmit().then(() => {
            console.log("ok")
        }).catch(() => {
            console.log("no")
        });
    }

};

export default ReindexNodeAction;