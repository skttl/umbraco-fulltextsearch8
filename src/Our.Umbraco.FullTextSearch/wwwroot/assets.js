var c = (e) => {
  throw TypeError(e);
};
var m = (e, t, o) => t.has(e) || c("Cannot " + o);
var _ = (e, t, o) => (m(e, t, "read from private field"), o ? o.call(e) : t.get(e)), h = (e, t, o) => t.has(e) ? c("Cannot add the same private member more than once") : t instanceof WeakSet ? t.add(e) : t.set(e, o), x = (e, t, o, n) => (m(e, t, "write to private field"), n ? n.call(e, o) : t.set(e, o), o);
import { UMB_AUTH_CONTEXT as y } from "@umbraco-cms/backoffice/auth";
import { UMB_DOCUMENT_ROOT_ENTITY_TYPE as E, UMB_DOCUMENT_ENTITY_TYPE as T, UMB_DOCUMENT_DETAIL_REPOSITORY_ALIAS as O } from "@umbraco-cms/backoffice/document";
import { UmbEntityActionBase as g } from "@umbraco-cms/backoffice/entity-action";
import { UmbModalToken as M, UMB_MODAL_MANAGER_CONTEXT as D } from "@umbraco-cms/backoffice/modal";
import { LitElement as N, html as d, property as f, state as b, customElement as S } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin as w } from "@umbraco-cms/backoffice/element-api";
const A = {
  type: "entityAction",
  kind: "default",
  alias: "our.umbraco.fulltextsearch.reindex.action",
  name: "ReindexNode",
  weight: -100,
  forEntityTypes: [E, T],
  api: () => Promise.resolve().then(() => v),
  elementName: "our-umbraco-fulltext-search-actions-entity-reindexnode",
  meta: {
    icon: "icon-alarm-clock",
    label: "#fulltextsearch_reindex",
    repositoryAlias: O
  }
}, C = [A], R = {
  type: "modal",
  alias: "our.umbraco.fulltextsearch.modals.reindexnode",
  name: "Reindex node",
  js: () => Promise.resolve().then(() => B)
}, U = [R], G = (e, t) => {
  t.registerMany([
    ...C,
    ...U
  ]), e.consumeContext(y, (o) => {
  });
}, P = new M("our.umbraco.fulltextsearch.modals.reindexnode", {
  modal: {
    type: "dialog",
    size: "small"
  }
});
var s;
class p extends g {
  constructor(o, n) {
    super(o, n);
    h(this, s);
    this.consumeContext(D, (i) => {
      x(this, s, i);
    });
  }
  async execute() {
    var n;
    const o = (n = _(this, s)) == null ? void 0 : n.open(this, P, {
      data: {
        unique: this.args.unique
      }
    });
    await (o == null ? void 0 : o.onSubmit().then(() => {
      console.log("ok");
    }).catch(() => {
      console.log("no");
    }));
  }
}
s = new WeakMap();
const v = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  ReindexNodeAction: p,
  default: p
}, Symbol.toStringTag, { value: "Module" }));
var $ = Object.defineProperty, j = Object.getOwnPropertyDescriptor, r = (e, t, o, n) => {
  for (var i = n > 1 ? void 0 : n ? j(t, o) : t, l = e.length - 1, u; l >= 0; l--)
    (u = e[l]) && (i = (n ? u(t, o, i) : u(i)) || i);
  return n && i && $(t, o, i), i;
};
let a = class extends w(N) {
  _handleCancel() {
    var e;
    (e = this.modalContext) == null || e.submit();
  }
  _reindex(e) {
    this._setButtonState(e, "waiting"), console.log("reindexing", e);
  }
  _setButtonState(e, t) {
    e ? this._withDescendantsState = t : this._withoutDescendantsState = t;
  }
  render() {
    var e, t;
    return d`
            <uui-dialog-layout headline="Reindex ${(e = this.data) != null && e.unique ? "node" : "all content"}">
                ${(t = this.data) != null && t.unique ? d`
                    <uui-button look="primary" .state=${this._withoutDescendantsState} @click=${() => this._reindex(!1)}>Reindex just this node</uui-button>
                    <uui-button look="secondary" .state=${this._withDescendantsState} @click=${() => this._reindex(!0)}>Reindex with descendants</uui-button>
                ` : d`
                    <uui-button look="primary" .state=${this._withDescendantsState} @click=${() => this._reindex(!0)}>Reindex all nodes</uui-button>
                `}
                <uui-button @click=${this._handleCancel}>Cancel</uui-button>
            </uui-dialog-layout>
        `;
  }
};
r([
  f({ attribute: !1 })
], a.prototype, "modalContext", 2);
r([
  f({ attribute: !1 })
], a.prototype, "data", 2);
r([
  b()
], a.prototype, "_withDescendantsState", 2);
r([
  b()
], a.prototype, "_withoutDescendantsState", 2);
a = r([
  S("our-umbraco-fulltext-search-reindex-node-modal")
], a);
const B = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  get default() {
    return a;
  }
}, Symbol.toStringTag, { value: "Module" }));
export {
  G as onInit
};
//# sourceMappingURL=assets.js.map
