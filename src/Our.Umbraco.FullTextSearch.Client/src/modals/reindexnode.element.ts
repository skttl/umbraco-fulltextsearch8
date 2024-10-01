import { html, LitElement, property, customElement, state } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import type { UmbModalContext } from "@umbraco-cms/backoffice/modal";
import { UmbModalExtensionElement } from "@umbraco-cms/backoffice/extension-registry";
import { ReindexNodeModalData } from "./reindexnode.modaltoken.ts";
import { UUIButtonState } from "@umbraco-cms/backoffice/external/uui";
import FullTextSearchContext, { FULLTEXTSEARCH_CONTEXT_TOKEN } from "../context/fulltextsearch.context.ts";

@customElement('our-umbraco-fulltext-search-reindex-node-modal')
export default class ReindexNodeDialogElement
    extends UmbElementMixin(LitElement)
    implements UmbModalExtensionElement<ReindexNodeModalData> {
    
    
    #fullTextSearchContext?: FullTextSearchContext;

    constructor() {
        super();

        this.consumeContext(FULLTEXTSEARCH_CONTEXT_TOKEN, (fullTextSearchContext) => {
            this.#fullTextSearchContext = fullTextSearchContext;
        })
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

    private _reindex(includeDescendants: boolean) {
        this._setButtonState(includeDescendants, 'waiting');

        this.#fullTextSearchContext?.reindex(includeDescendants, [ ]);

        console.log("reindexing", includeDescendants);
    }

    private _setButtonState(includeDescendants: boolean, state: UUIButtonState) {
        if (includeDescendants) {
            this._withDescendantsState = state;
        }
        else {
            this._withoutDescendantsState = state;
        }
    }

    override render() {
        return html`
            <uui-dialog-layout headline="${(this.data?.unique ? `#fullTextSearch_reindexNode` : `#fullTextSearch_reindexAllNodes`)}">
                ${(this.data?.unique
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