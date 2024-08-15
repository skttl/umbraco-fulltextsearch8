import { html, LitElement, property, customElement, state } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import type { UmbModalContext } from "@umbraco-cms/backoffice/modal";
import { UmbModalExtensionElement } from "@umbraco-cms/backoffice/extension-registry";
import { ReindexNodeModalData, ReindexNodeModalValue } from "./reindexnode.modaltoken.ts";
import { UUIButtonState } from "@umbraco-cms/backoffice/external/uui";

@customElement('our-umbraco-fulltext-search-reindex-node-modal')
export default class ReindexNodeDialogElement
    extends UmbElementMixin(LitElement)
    implements UmbModalExtensionElement<ReindexNodeModalData> {

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
            <uui-dialog-layout headline="Reindex ${(this.data?.unique ? `node` : `all content`)}">
                ${(this.data?.unique
                ? html`
                    <uui-button look="primary" .state=${this._withoutDescendantsState} @click=${() => this._reindex(false)}>Reindex just this node</uui-button>
                    <uui-button look="secondary" .state=${this._withDescendantsState} @click=${() => this._reindex(true)}>Reindex with descendants</uui-button>
                ` : html`
                    <uui-button look="primary" .state=${this._withDescendantsState} @click=${() => this._reindex(true)}>Reindex all nodes</uui-button>
                `)}
                <uui-button @click=${this._handleCancel}>Cancel</uui-button>
            </uui-dialog-layout>
        `;
    }
}