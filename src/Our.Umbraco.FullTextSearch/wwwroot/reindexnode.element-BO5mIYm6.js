import { LitElement as f, html as d, property as x, state as m, customElement as y } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin as C } from "@umbraco-cms/backoffice/element-api";
import { c as b } from "./client.gen-BACO-MNf.js";
import { UMB_NOTIFICATION_CONTEXT as v } from "@umbraco-cms/backoffice/notification";
import { tryExecute as S } from "@umbraco-cms/backoffice/resources";
class T {
  static postFulltextsearchIndexReindexnodes(e) {
    return (e.client ?? b).post({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/management/api/v5/fulltextsearch/index/reindexnodes",
      ...e,
      headers: {
        "Content-Type": "application/json",
        ...e.headers
      }
    });
  }
}
var g = Object.defineProperty, z = Object.getOwnPropertyDescriptor, p = (t) => {
  throw TypeError(t);
}, u = (t, e, a, n) => {
  for (var i = n > 1 ? void 0 : n ? z(e, a) : e, l = t.length - 1, r; l >= 0; l--)
    (r = t[l]) && (i = (n ? r(e, a, i) : r(i)) || i);
  return n && i && g(e, a, i), i;
}, _ = (t, e, a) => e.has(t) || p("Cannot " + a), h = (t, e, a) => (_(t, e, "read from private field"), e.get(t)), w = (t, e, a) => e.has(t) ? p("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(t) : e.set(t, a), k = (t, e, a, n) => (_(t, e, "write to private field"), e.set(t, a), a), s;
let o = class extends C(f) {
  constructor() {
    super(), w(this, s), this.consumeContext(v, (t) => {
      k(this, s, t);
    });
  }
  _handleCancel() {
    var t;
    (t = this.modalContext) == null || t.submit();
  }
  async _reindex(t) {
    var n, i, l, r, c;
    if (!this.modalContext) return;
    (n = this.modalContext) == null || n.submit();
    const e = (i = h(this, s)) == null ? void 0 : i.stay("default", {
      data: {
        headline: this.localize.term("fullTextSearch_reindexing"),
        message: this.localize.term("fullTextSearch_reindexingMessage")
      }
    }), a = (r = (l = this.modalContext) == null ? void 0 : l.data.unique) == null ? void 0 : r.toString();
    await S(this, T.postFulltextsearchIndexReindexnodes({
      body: {
        includeDescendants: t,
        nodeKey: a
      }
    })), e == null || e.close(), (c = h(this, s)) == null || c.peek("positive", {
      data: {
        headline: this.localize.term("fullTextSearch_reindexed"),
        message: this.localize.term("fullTextSearch_reindexedMessage")
      }
    });
  }
  render() {
    var t, e;
    return d`
            <uui-dialog-layout headline="${this.localize.term((t = this.modalContext) != null && t.data.unique ? "fullTextSearch_reindexNode" : "fullTextSearch_reindexAllNodes")}">
                ${(e = this.modalContext) != null && e.data.unique ? d`
                    <uui-button look="primary" .state=${this._withoutDescendantsState} @click=${() => this._reindex(!1)}>
                        <umb-localize key="fullTextSearch_reindexJustThisNode">
                            Reindex just this node
                        </umb-localize></uui-button>
                    <uui-button look="secondary" .state=${this._withDescendantsState} @click=${() => this._reindex(!0)}>
                        <umb-localize key="fullTextSearch_reindexWithDescendants">
                            Reindex with descendants
                        </umb-localize>
                    </uui-button>
                ` : d`
                    <uui-button look="primary" .state=${this._withDescendantsState} @click=${() => this._reindex(!0)}>
                        <umb-localize key="fullTextSearch_reindexAllContent">
                            Reindex all content
                        </umb-localize></uui-button>
                `}
                <uui-button @click=${this._handleCancel}>
                    <umb-localize key="general_cancel">
                        Cancel
                    </umb-localize>
                </uui-button>
            </uui-dialog-layout>
        `;
  }
};
s = /* @__PURE__ */ new WeakMap();
u([
  x({ attribute: !1 })
], o.prototype, "modalContext", 2);
u([
  x({ attribute: !1 })
], o.prototype, "data", 2);
u([
  m()
], o.prototype, "_withDescendantsState", 2);
u([
  m()
], o.prototype, "_withoutDescendantsState", 2);
o = u([
  y("our-umbraco-fulltext-search-reindex-node-modal")
], o);
export {
  o as default
};
//# sourceMappingURL=reindexnode.element-BO5mIYm6.js.map
