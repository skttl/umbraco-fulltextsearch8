import { LitElement as S, html as c, property as p, state as f, customElement as v } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin as y } from "@umbraco-cms/backoffice/element-api";
import { FULLTEXTSEARCH_CONTEXT_TOKEN as b } from "./fulltextsearch.context-BP_Dxq3u.js";
import { UMB_NOTIFICATION_CONTEXT as w } from "@umbraco-cms/backoffice/notification";
var z = Object.defineProperty, g = Object.getOwnPropertyDescriptor, C = (e) => {
  throw TypeError(e);
}, u = (e, t, a, l) => {
  for (var i = l > 1 ? void 0 : l ? g(t, a) : t, n = e.length - 1, o; n >= 0; n--)
    (o = e[n]) && (i = (l ? o(t, a, i) : o(i)) || i);
  return l && i && z(t, a, i), i;
}, T = (e, t, a) => t.has(e) || C("Cannot " + a), h = (e, t, a) => (T(e, t, "read from private field"), t.get(e)), _ = (e, t, a) => t.has(e) ? C("Cannot add the same private member more than once") : t instanceof WeakSet ? t.add(e) : t.set(e, a), m = (e, t, a, l) => (T(e, t, "write to private field"), t.set(e, a), a), s, d;
let r = class extends y(S) {
  constructor() {
    super(), _(this, s), _(this, d), this.consumeContext(b, (e) => {
      m(this, d, e);
    }), this.consumeContext(w, (e) => {
      m(this, s, e);
    });
  }
  _handleCancel() {
    var e;
    (e = this.modalContext) == null || e.submit();
  }
  async _reindex(e) {
    var a, l, i, n, o, x;
    if (!this.modalContext) return;
    (a = this.modalContext) == null || a.submit();
    const t = (l = h(this, s)) == null ? void 0 : l.stay("default", {
      data: {
        headline: this.localize.term("fullTextSearch_reindexing"),
        message: this.localize.term("fullTextSearch_reindexingMessage")
      }
    });
    await ((o = h(this, d)) == null ? void 0 : o.reindex(e, (n = (i = this.modalContext) == null ? void 0 : i.data.unique) == null ? void 0 : n.toString())), t == null || t.close(), (x = h(this, s)) == null || x.peek("positive", {
      data: {
        headline: this.localize.term("fullTextSearch_reindexed"),
        message: this.localize.term("fullTextSearch_reindexedMessage")
      }
    });
  }
  render() {
    var e, t;
    return c`
            <uui-dialog-layout headline="${this.localize.term((e = this.modalContext) != null && e.data.unique ? "fullTextSearch_reindexNode" : "fullTextSearch_reindexAllNodes")}">
                ${(t = this.modalContext) != null && t.data.unique ? c`
                    <uui-button look="primary" .state=${this._withoutDescendantsState} @click=${() => this._reindex(!1)}>
                        <umb-localize key="fullTextSearch_reindexJustThisNode">
                            Reindex just this node
                        </umb-localize></uui-button>
                    <uui-button look="secondary" .state=${this._withDescendantsState} @click=${() => this._reindex(!0)}>
                        <umb-localize key="fullTextSearch_reindexWithDescendants">
                            Reindex with descendants
                        </umb-localize>
                    </uui-button>
                ` : c`
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
d = /* @__PURE__ */ new WeakMap();
u([
  p({ attribute: !1 })
], r.prototype, "modalContext", 2);
u([
  p({ attribute: !1 })
], r.prototype, "data", 2);
u([
  f()
], r.prototype, "_withDescendantsState", 2);
u([
  f()
], r.prototype, "_withoutDescendantsState", 2);
r = u([
  v("our-umbraco-fulltext-search-reindex-node-modal")
], r);
export {
  r as default
};
//# sourceMappingURL=reindexnode.element-DMQBLkN4.js.map
