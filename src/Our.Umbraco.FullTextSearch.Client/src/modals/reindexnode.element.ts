import { html, LitElement, property, customElement, state } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import type { UmbModalContext } from "@umbraco-cms/backoffice/modal";
import { UmbModalExtensionElement } from "@umbraco-cms/backoffice/extension-registry";
import { ReindexNodeModalData } from "./reindexnode.modaltoken.ts";
import { UUIButtonState } from "@umbraco-cms/backoffice/external/uui";
import FullTextSearchContext, { FULLTEXTSEARCH_CONTEXT_TOKEN } from "../context/fulltextsearch.context.ts";
import { UMB_NOTIFICATION_CONTEXT, UmbNotificationContext } from "@umbraco-cms/backoffice/notification";

@customElement('our-umbraco-fulltext-search-reindex-node-modal')
export default class ReindexNodeDialogElement
    extends UmbElementMixin(LitElement)
    implements UmbModalExtensionElement<ReindexNodeModalData> {
    
    #notificationContext?: UmbNotificationContext;
    #fullTextSearchContext?: FullTextSearchContext;

    constructor() {
        super();

        this.consumeContext(FULLTEXTSEARCH_CONTEXT_TOKEN, (fullTextSearchContext) => {
            this.#fullTextSearchContext = fullTextSearchContext;
        })

        this.consumeContext(UMB_NOTIFICATION_CONTEXT, (instance) => {
            this.#notificationContext = instance;
        });
    }

    @property({ attribute: false })
    modalContext?: UmbModalContext<ReindexNodeModalData>;

    @property({ attribute: false })
    data?: ReindexNodeModalData;

    @state()
    private _withDescendantsState: UUIButtonState;

    @state()
    private _withoutDescendantsState: UUIButtonState;

    private _handleCancel() {
        this.modalContext?.submit();
    }

    private async _reindex(includeDescendants: boolean) {
        if (!this.modalContext) return;

        this.modalContext?.submit();

        const reindexingNotification = this.#notificationContext?.stay('default', {
            data: {
                headline: this.localize.term(`fullTextSearch_reindexing`),
                message: this.localize.term(`fullTextSearch_reindexingMessage`)
            }
        });

        await this.#fullTextSearchContext?.reindex(includeDescendants, this.modalContext?.data.unique?.toString());
        
        reindexingNotification?.close();

        this.#notificationContext?.peek('positive', {
            data: {
                headline: this.localize.term(`fullTextSearch_reindexed`),
                message: this.localize.term(`fullTextSearch_reindexedMessage`)
            }
        });
    }

    override render() {
        return html`
            <uui-dialog-layout headline="${this.localize.term(this.modalContext?.data.unique ? `fullTextSearch_reindexNode` : `fullTextSearch_reindexAllNodes`)}">
                ${(this.modalContext?.data.unique
                ? html`
                    <uui-button look="primary" .state=${this._withoutDescendantsState} @click=${() => this._reindex(false)}>
                        <umb-localize key="fullTextSearch_reindexJustThisNode">
                            Reindex just this node
                        </umb-localize></uui-button>
                    <uui-button look="secondary" .state=${this._withDescendantsState} @click=${() => this._reindex(true)}>
                        <umb-localize key="fullTextSearch_reindexWithDescendants">
                            Reindex with descendants
                        </umb-localize>
                    </uui-button>
                ` : html`
                    <uui-button look="primary" .state=${this._withDescendantsState} @click=${() => this._reindex(true)}>
                        <umb-localize key="fullTextSearch_reindexAllContent">
                            Reindex all content
                        </umb-localize></uui-button>
                `)}
                <uui-button @click=${this._handleCancel}>
                    <umb-localize key="general_cancel">
                        Cancel
                    </umb-localize>
                </uui-button>
            </uui-dialog-layout>
        `;
    }
}