import { LitElement as f, html as d, property as x, state as m, customElement as y } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin as b } from "@umbraco-cms/backoffice/element-api";
import { c as C } from "./client.gen-B-W0Avrw.js";
import { UMB_NOTIFICATION_CONTEXT as v } from "@umbraco-cms/backoffice/notification";
import { tryExecute as S } from "@umbraco-cms/backoffice/resources";
class T {
  static postUmbracoManagementApiV5FulltextsearchIndexReindexnodes(e) {
    return ((e == null ? void 0 : e.client) ?? C).post({
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
        ...e == null ? void 0 : e.headers
      }
    });
  }
}
var g = Object.defineProperty, z = Object.getOwnPropertyDescriptor, _ = (t) => {
  throw TypeError(t);
}, u = (t, e, a, r) => {
  for (var i = r > 1 ? void 0 : r ? z(e, a) : e, l = t.length - 1, n; l >= 0; l--)
    (n = t[l]) && (i = (r ? n(e, a, i) : n(i)) || i);
  return r && i && g(e, a, i), i;
}, p = (t, e, a) => e.has(t) || _("Cannot " + a), h = (t, e, a) => (p(t, e, "read from private field"), e.get(t)), w = (t, e, a) => e.has(t) ? _("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(t) : e.set(t, a), k = (t, e, a, r) => (p(t, e, "write to private field"), e.set(t, a), a), o;
let s = class extends b(f) {
  constructor() {
    super(), w(this, o), this.consumeContext(v, (t) => {
      k(this, o, t);
    });
  }
  _handleCancel() {
    var t;
    (t = this.modalContext) == null || t.submit();
  }
  async _reindex(t) {
    var r, i, l, n, c;
    if (!this.modalContext) return;
    (r = this.modalContext) == null || r.submit();
    const e = (i = h(this, o)) == null ? void 0 : i.stay("default", {
      data: {
        headline: this.localize.term("fullTextSearch_reindexing"),
        message: this.localize.term("fullTextSearch_reindexingMessage")
      }
    }), a = (n = (l = this.modalContext) == null ? void 0 : l.data.unique) == null ? void 0 : n.toString();
    await S(this, T.postUmbracoManagementApiV5FulltextsearchIndexReindexnodes({
      body: {
        includeDescendants: t,
        nodeKey: a
      }
    })), e == null || e.close(), (c = h(this, o)) == null || c.peek("positive", {
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
o = /* @__PURE__ */ new WeakMap();
u([
  x({ attribute: !1 })
], s.prototype, "modalContext", 2);
u([
  x({ attribute: !1 })
], s.prototype, "data", 2);
u([
  m()
], s.prototype, "_withDescendantsState", 2);
u([
  m()
], s.prototype, "_withoutDescendantsState", 2);
s = u([
  y("our-umbraco-fulltext-search-reindex-node-modal")
], s);
export {
  s as default
};
//# sourceMappingURL=reindexnode.element-DK8m4Ock.js.map
