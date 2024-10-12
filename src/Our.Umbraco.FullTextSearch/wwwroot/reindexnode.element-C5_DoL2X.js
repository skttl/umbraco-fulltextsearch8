import { LitElement as T, html as c, property as m, state as p, customElement as v } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin as y } from "@umbraco-cms/backoffice/element-api";
import { FULLTEXTSEARCH_CONTEXT_TOKEN as S } from "./fulltextsearch.context-BLn8Y4qb.js";
import { UMB_NOTIFICATION_CONTEXT as b } from "@umbraco-cms/backoffice/notification";
var w = Object.defineProperty, z = Object.getOwnPropertyDescriptor, f = (e) => {
  throw TypeError(e);
}, u = (e, t, a, l) => {
  for (var i = l > 1 ? void 0 : l ? z(t, a) : t, n = e.length - 1, o; n >= 0; n--)
    (o = e[n]) && (i = (l ? o(t, a, i) : o(i)) || i);
  return l && i && w(t, a, i), i;
}, C = (e, t, a) => t.has(e) || f("Cannot " + a), h = (e, t, a) => (C(e, t, "read from private field"), t.get(e)), x = (e, t, a) => t.has(e) ? f("Cannot add the same private member more than once") : t instanceof WeakSet ? t.add(e) : t.set(e, a), _ = (e, t, a, l) => (C(e, t, "write to private field"), t.set(e, a), a), s, d;
let r = class extends y(T) {
  constructor() {
    super(), x(this, s), x(this, d), this.consumeContext(S, (e) => {
      _(this, d, e);
    }), this.consumeContext(b, (e) => {
      _(this, s, e);
    });
  }
  _handleCancel() {
    var e;
    (e = this.modalContext) == null || e.submit();
  }
  async _reindex(e) {
    var a, l, i, n, o;
    if (!this.modalContext) return;
    (a = this.modalContext) == null || a.submit();
    const t = (l = h(this, s)) == null ? void 0 : l.stay("default", {
      data: {
        headline: this.localize.term("fullTextSearch_reindexing"),
        message: this.localize.term("fullTextSearch_reindexingMessage")
      }
    });
    await ((n = h(this, d)) == null ? void 0 : n.reindex(e, [Number((i = this.modalContext) == null ? void 0 : i.data.unique) || 0])), t == null || t.close(), (o = h(this, s)) == null || o.peek("positive", {
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
  m({ attribute: !1 })
], r.prototype, "modalContext", 2);
u([
  m({ attribute: !1 })
], r.prototype, "data", 2);
u([
  p()
], r.prototype, "_withDescendantsState", 2);
u([
  p()
], r.prototype, "_withoutDescendantsState", 2);
r = u([
  v("our-umbraco-fulltext-search-reindex-node-modal")
], r);
export {
  r as default
};
//# sourceMappingURL=reindexnode.element-C5_DoL2X.js.map
