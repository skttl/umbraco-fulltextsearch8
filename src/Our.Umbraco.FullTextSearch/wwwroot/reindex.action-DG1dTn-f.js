var a = (t) => {
  throw TypeError(t);
};
var i = (t, o, e) => o.has(t) || a("Cannot " + e);
var r = (t, o, e) => (i(t, o, "read from private field"), e ? e.call(t) : o.get(t)), u = (t, o, e) => o.has(t) ? a("Cannot add the same private member more than once") : o instanceof WeakSet ? o.add(t) : o.set(t, e), c = (t, o, e, n) => (i(t, o, "write to private field"), n ? n.call(t, e) : o.set(t, e), e);
import { UmbEntityActionBase as d } from "@umbraco-cms/backoffice/entity-action";
import { UmbModalToken as l, UMB_MODAL_MANAGER_CONTEXT as x } from "@umbraco-cms/backoffice/modal";
const E = new l("our.umbraco.fulltextsearch.modals.reindexnode", {
  modal: {
    type: "dialog",
    size: "small"
  }
});
var s;
class M extends d {
  constructor(e, n) {
    super(e, n);
    u(this, s);
    this.consumeContext(x, (m) => {
      c(this, s, m);
    });
  }
  async execute() {
    var n;
    const e = (n = r(this, s)) == null ? void 0 : n.open(this, E, {
      data: {
        unique: this.args.unique
      }
    });
    await (e == null ? void 0 : e.onSubmit());
  }
}
s = new WeakMap();
export {
  M as ReindexNodeAction,
  M as default
};
//# sourceMappingURL=reindex.action-DG1dTn-f.js.map
