import { UmbControllerHostElement } from "@umbraco-cms/backoffice/controller-api";
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
        
        const modal = this.#modalManagerContext?.open(this, REINDEX_NODE_MODAL_TOKEN, {
            data: {
                unique: this.args.unique
            }
        });

        await modal?.onSubmit();
    }

};

export default ReindexNodeAction;