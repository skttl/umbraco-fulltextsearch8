var O = (t) => {
  throw TypeError(t);
};
var M = (t, e, n) => e.has(t) || O("Cannot " + n);
var d = (t, e, n) => (M(t, e, "read from private field"), n ? n.call(t) : e.get(t)), p = (t, e, n) => e.has(t) ? O("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(t) : e.set(t, n), y = (t, e, n, r) => (M(t, e, "write to private field"), r ? r.call(t, n) : e.set(t, n), n);
import { UMB_AUTH_CONTEXT as H } from "@umbraco-cms/backoffice/auth";
import { UMB_DOCUMENT_ROOT_ENTITY_TYPE as Q, UMB_DOCUMENT_ENTITY_TYPE as Z, UMB_DOCUMENT_DETAIL_REPOSITORY_ALIAS as ee } from "@umbraco-cms/backoffice/document";
import { UmbEntityActionBase as te } from "@umbraco-cms/backoffice/entity-action";
import { UmbModalToken as ne, UMB_MODAL_MANAGER_CONTEXT as re } from "@umbraco-cms/backoffice/modal";
import { LitElement as se, html as C, property as V, state as G, customElement as ie } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin as oe } from "@umbraco-cms/backoffice/element-api";
import { UmbControllerBase as de } from "@umbraco-cms/backoffice/class-api";
import { UmbContextToken as ae } from "@umbraco-cms/backoffice/context-api";
import { tryExecuteAndNotify as m } from "@umbraco-cms/backoffice/resources";
import { UmbObjectState as F, UmbStringState as B } from "@umbraco-cms/backoffice/observable-api";
const le = {
  type: "entityAction",
  kind: "default",
  alias: "our.umbraco.fulltextsearch.reindex.action",
  name: "ReindexNode",
  weight: -100,
  forEntityTypes: [Q, Z],
  api: () => Promise.resolve().then(() => Ce),
  elementName: "our-umbraco-fulltext-search-actions-entity-reindexnode",
  meta: {
    icon: "icon-alarm-clock",
    label: "#fullTextSearch_reindex",
    repositoryAlias: ee
  }
}, ce = [le], ue = {
  type: "modal",
  alias: "our.umbraco.fulltextsearch.modals.reindexnode",
  name: "Reindex node",
  js: () => Promise.resolve().then(() => Me)
}, he = [ue], pe = [
  {
    type: "localization",
    alias: "Our.Umbraco.FullTextSearch.Localizations.En",
    name: "English",
    meta: {
      culture: "en"
    },
    js: () => Promise.resolve().then(() => Le)
  },
  {
    type: "localization",
    alias: "Our.Umbraco.FullTextSearch.Localizations.Da",
    name: "Danish",
    meta: {
      culture: "da"
    },
    js: () => Promise.resolve().then(() => qe)
  },
  {
    type: "localization",
    alias: "Our.Umbraco.FullTextSearch.Localizations.Cy",
    name: "Welsh",
    meta: {
      culture: "cy"
    },
    js: () => Promise.resolve().then(() => Ve)
  },
  {
    type: "localization",
    alias: "Our.Umbraco.FullTextSearch.Localizations.Fr",
    name: "French",
    meta: {
      culture: "fr"
    },
    js: () => Promise.resolve().then(() => We)
  },
  {
    type: "localization",
    alias: "Our.Umbraco.FullTextSearch.Localizations.Nb",
    name: "Norwegian bokmål",
    meta: {
      culture: "nb"
    },
    js: () => Promise.resolve().then(() => Ke)
  },
  {
    type: "localization",
    alias: "Our.Umbraco.FullTextSearch.Localizations.Nl",
    name: "Dutch",
    meta: {
      culture: "nl"
    },
    js: () => Promise.resolve().then(() => Xe)
  }
], ye = pe, ge = [
  {
    type: "globalContext",
    alias: "our.umbraco.fulltextsearch.context",
    name: "Full Text Search context",
    js: () => Promise.resolve().then(() => Ee)
  }
], fe = [...ge];
class L extends Error {
  constructor(e, n, r) {
    super(r), this.name = "ApiError", this.url = n.url, this.status = n.status, this.statusText = n.statusText, this.body = n.body, this.request = e;
  }
}
class me extends Error {
  constructor(e) {
    super(e), this.name = "CancelError";
  }
  get isCancelled() {
    return !0;
  }
}
class xe {
  constructor(e) {
    this._isResolved = !1, this._isRejected = !1, this._isCancelled = !1, this.cancelHandlers = [], this.promise = new Promise((n, r) => {
      this._resolve = n, this._reject = r;
      const s = (a) => {
        this._isResolved || this._isRejected || this._isCancelled || (this._isResolved = !0, this._resolve && this._resolve(a));
      }, i = (a) => {
        this._isResolved || this._isRejected || this._isCancelled || (this._isRejected = !0, this._reject && this._reject(a));
      }, o = (a) => {
        this._isResolved || this._isRejected || this._isCancelled || this.cancelHandlers.push(a);
      };
      return Object.defineProperty(o, "isResolved", {
        get: () => this._isResolved
      }), Object.defineProperty(o, "isRejected", {
        get: () => this._isRejected
      }), Object.defineProperty(o, "isCancelled", {
        get: () => this._isCancelled
      }), e(s, i, o);
    });
  }
  get [Symbol.toStringTag]() {
    return "Cancellable Promise";
  }
  then(e, n) {
    return this.promise.then(e, n);
  }
  catch(e) {
    return this.promise.catch(e);
  }
  finally(e) {
    return this.promise.finally(e);
  }
  cancel() {
    if (!(this._isResolved || this._isRejected || this._isCancelled)) {
      if (this._isCancelled = !0, this.cancelHandlers.length)
        try {
          for (const e of this.cancelHandlers)
            e();
        } catch (e) {
          console.warn("Cancellation threw an error", e);
          return;
        }
      this.cancelHandlers.length = 0, this._reject && this._reject(new me("Request aborted"));
    }
  }
  get isCancelled() {
    return this._isCancelled;
  }
}
class U {
  constructor() {
    this._fns = [];
  }
  eject(e) {
    const n = this._fns.indexOf(e);
    n !== -1 && (this._fns = [
      ...this._fns.slice(0, n),
      ...this._fns.slice(n + 1)
    ]);
  }
  use(e) {
    this._fns = [...this._fns, e];
  }
}
const l = {
  BASE: "",
  CREDENTIALS: "include",
  ENCODE_PATH: void 0,
  HEADERS: void 0,
  PASSWORD: void 0,
  TOKEN: void 0,
  USERNAME: void 0,
  VERSION: "Latest",
  WITH_CREDENTIALS: !1,
  interceptors: {
    request: new U(),
    response: new U()
  }
}, D = (t) => typeof t == "string", E = (t) => D(t) && t !== "", R = (t) => t instanceof Blob, W = (t) => t instanceof FormData, be = (t) => {
  try {
    return btoa(t);
  } catch {
    return Buffer.from(t).toString("base64");
  }
}, we = (t) => {
  const e = [], n = (s, i) => {
    e.push(`${encodeURIComponent(s)}=${encodeURIComponent(String(i))}`);
  }, r = (s, i) => {
    i != null && (Array.isArray(i) ? i.forEach((o) => r(s, o)) : typeof i == "object" ? Object.entries(i).forEach(([o, a]) => r(`${s}[${o}]`, a)) : n(s, i));
  };
  return Object.entries(t).forEach(([s, i]) => r(s, i)), e.length ? `?${e.join("&")}` : "";
}, Te = (t, e) => {
  const n = encodeURI, r = e.url.replace("{api-version}", t.VERSION).replace(/{(.*?)}/g, (i, o) => {
    var a;
    return (a = e.path) != null && a.hasOwnProperty(o) ? n(String(e.path[o])) : i;
  }), s = t.BASE + r;
  return e.query ? s + we(e.query) : s;
}, ve = (t) => {
  if (t.formData) {
    const e = new FormData(), n = (r, s) => {
      D(s) || R(s) ? e.append(r, s) : e.append(r, JSON.stringify(s));
    };
    return Object.entries(t.formData).filter(([, r]) => r != null).forEach(([r, s]) => {
      Array.isArray(s) ? s.forEach((i) => n(r, i)) : n(r, s);
    }), e;
  }
}, _ = async (t, e) => typeof e == "function" ? e(t) : e, ke = async (t, e) => {
  const [n, r, s, i] = await Promise.all([
    _(e, t.TOKEN),
    _(e, t.USERNAME),
    _(e, t.PASSWORD),
    _(e, t.HEADERS)
  ]), o = Object.entries({
    Accept: "application/json",
    ...i,
    ...e.headers
  }).filter(([, a]) => a != null).reduce((a, [g, c]) => ({
    ...a,
    [g]: String(c)
  }), {});
  if (E(n) && (o.Authorization = `Bearer ${n}`), E(r) && E(s)) {
    const a = be(`${r}:${s}`);
    o.Authorization = `Basic ${a}`;
  }
  return e.body !== void 0 && (e.mediaType ? o["Content-Type"] = e.mediaType : R(e.body) ? o["Content-Type"] = e.body.type || "application/octet-stream" : D(e.body) ? o["Content-Type"] = "text/plain" : W(e.body) || (o["Content-Type"] = "application/json")), new Headers(o);
}, Ne = (t) => {
  var e, n;
  if (t.body !== void 0)
    return (e = t.mediaType) != null && e.includes("application/json") || (n = t.mediaType) != null && n.includes("+json") ? JSON.stringify(t.body) : D(t.body) || R(t.body) || W(t.body) ? t.body : JSON.stringify(t.body);
}, Se = async (t, e, n, r, s, i, o) => {
  const a = new AbortController();
  let g = {
    headers: i,
    body: r ?? s,
    method: e.method,
    signal: a.signal
  };
  t.WITH_CREDENTIALS && (g.credentials = t.CREDENTIALS);
  for (const c of t.interceptors.request._fns)
    g = await c(g);
  return o(() => a.abort()), await fetch(n, g);
}, De = (t, e) => {
  if (e) {
    const n = t.headers.get(e);
    if (D(n))
      return n;
  }
}, Ie = async (t) => {
  if (t.status !== 204)
    try {
      const e = t.headers.get("Content-Type");
      if (e) {
        const n = ["application/octet-stream", "application/pdf", "application/zip", "audio/", "image/", "video/"];
        if (e.includes("application/json") || e.includes("+json"))
          return await t.json();
        if (n.some((r) => e.includes(r)))
          return await t.blob();
        if (e.includes("multipart/form-data"))
          return await t.formData();
        if (e.includes("text/"))
          return await t.text();
      }
    } catch (e) {
      console.error(e);
    }
}, _e = (t, e) => {
  const r = {
    400: "Bad Request",
    401: "Unauthorized",
    402: "Payment Required",
    403: "Forbidden",
    404: "Not Found",
    405: "Method Not Allowed",
    406: "Not Acceptable",
    407: "Proxy Authentication Required",
    408: "Request Timeout",
    409: "Conflict",
    410: "Gone",
    411: "Length Required",
    412: "Precondition Failed",
    413: "Payload Too Large",
    414: "URI Too Long",
    415: "Unsupported Media Type",
    416: "Range Not Satisfiable",
    417: "Expectation Failed",
    418: "Im a teapot",
    421: "Misdirected Request",
    422: "Unprocessable Content",
    423: "Locked",
    424: "Failed Dependency",
    425: "Too Early",
    426: "Upgrade Required",
    428: "Precondition Required",
    429: "Too Many Requests",
    431: "Request Header Fields Too Large",
    451: "Unavailable For Legal Reasons",
    500: "Internal Server Error",
    501: "Not Implemented",
    502: "Bad Gateway",
    503: "Service Unavailable",
    504: "Gateway Timeout",
    505: "HTTP Version Not Supported",
    506: "Variant Also Negotiates",
    507: "Insufficient Storage",
    508: "Loop Detected",
    510: "Not Extended",
    511: "Network Authentication Required",
    ...t.errors
  }[e.status];
  if (r)
    throw new L(t, e, r);
  if (!e.ok) {
    const s = e.status ?? "unknown", i = e.statusText ?? "unknown", o = (() => {
      try {
        return JSON.stringify(e.body, null, 2);
      } catch {
        return;
      }
    })();
    throw new L(
      t,
      e,
      `Generic Error: status: ${s}; status text: ${i}; body: ${o}`
    );
  }
}, x = (t, e) => new xe(async (n, r, s) => {
  try {
    const i = Te(t, e), o = ve(e), a = Ne(e), g = await ke(t, e);
    if (!s.isCancelled) {
      let c = await Se(t, e, i, a, o, g, s);
      for (const J of t.interceptors.response._fns)
        c = await J(c);
      const Y = await Ie(c), X = De(c, e.responseHeader), j = {
        url: i,
        ok: c.ok,
        status: c.status,
        statusText: c.statusText,
        body: X ?? Y
      };
      _e(e, j), n(j.body);
    }
  } catch (i) {
    r(i);
  }
});
class b {
  /**
  * @returns unknown OK
  * @throws ApiError
  */
  static getUmbracoFulltextsearchApiV5FulltextsearchConfig() {
    return x(l, {
      method: "GET",
      url: "/umbraco/fulltextsearch/api/v5/fulltextsearch/config"
    });
  }
  /**
  * @returns unknown OK
  * @throws ApiError
  */
  static getUmbracoFulltextsearchApiV5FulltextsearchIncorrectindexednodes(e = {}) {
    const { pageNumber: n } = e;
    return x(l, {
      method: "GET",
      url: "/umbraco/fulltextsearch/api/v5/fulltextsearch/incorrectindexednodes",
      query: {
        pageNumber: n
      }
    });
  }
  /**
  * @returns unknown OK
  * @throws ApiError
  */
  static getUmbracoFulltextsearchApiV5FulltextsearchIndexednodes(e = {}) {
    const { pageNumber: n } = e;
    return x(l, {
      method: "GET",
      url: "/umbraco/fulltextsearch/api/v5/fulltextsearch/indexednodes",
      query: {
        pageNumber: n
      }
    });
  }
  /**
  * @returns unknown OK
  * @throws ApiError
  */
  static getUmbracoFulltextsearchApiV5FulltextsearchIndexstatus() {
    return x(l, {
      method: "GET",
      url: "/umbraco/fulltextsearch/api/v5/fulltextsearch/indexstatus"
    });
  }
  /**
  * @returns unknown OK
  * @throws ApiError
  */
  static getUmbracoFulltextsearchApiV5FulltextsearchMissingnodes(e = {}) {
    const { pageNumber: n } = e;
    return x(l, {
      method: "GET",
      url: "/umbraco/fulltextsearch/api/v5/fulltextsearch/missingnodes",
      query: {
        pageNumber: n
      }
    });
  }
  /**
  * @returns unknown OK
  * @throws ApiError
  */
  static postUmbracoFulltextsearchApiV5FulltextsearchReindexnodes(e = {}) {
    const { requestBody: n } = e;
    return x(l, {
      method: "POST",
      url: "/umbraco/fulltextsearch/api/v5/fulltextsearch/reindexnodes",
      body: n,
      mediaType: "application/json"
    });
  }
}
const at = (t, e) => {
  e.registerMany([
    ...ce,
    ...he,
    ...ye,
    ...fe
  ]), t.consumeContext(H, (n) => {
    const r = n.getOpenApiConfiguration();
    l.TOKEN = r.token, l.BASE = r.base, l.WITH_CREDENTIALS = r.withCredentials;
  });
}, Ae = new ne("our.umbraco.fulltextsearch.modals.reindexnode", {
  modal: {
    type: "dialog",
    size: "small"
  }
});
var S;
class q extends te {
  constructor(n, r) {
    super(n, r);
    p(this, S);
    this.consumeContext(re, (s) => {
      y(this, S, s);
    });
  }
  async execute() {
    var r;
    const n = (r = d(this, S)) == null ? void 0 : r.open(this, Ae, {
      data: {
        unique: this.args.unique
      }
    });
    await (n == null ? void 0 : n.onSubmit().then(() => {
      console.log("ok");
    }).catch(() => {
      console.log("no");
    }));
  }
}
S = new WeakMap();
const Ce = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  ReindexNodeAction: q,
  default: q
}, Symbol.toStringTag, { value: "Module" }));
var u;
class Fe {
  constructor(e) {
    p(this, u);
    y(this, u, e);
  }
  async config() {
    return await m(d(this, u), b.getUmbracoFulltextsearchApiV5FulltextsearchConfig());
  }
  async indexStatus() {
    return await m(d(this, u), b.getUmbracoFulltextsearchApiV5FulltextsearchIndexstatus());
  }
  async incorrectIndexedNodes(e) {
    return await m(d(this, u), b.getUmbracoFulltextsearchApiV5FulltextsearchIncorrectindexednodes({
      pageNumber: e
    }));
  }
  async indexedNodes(e) {
    return await m(d(this, u), b.getUmbracoFulltextsearchApiV5FulltextsearchIndexednodes({
      pageNumber: e
    }));
  }
  async missingNodes(e) {
    return await m(d(this, u), b.getUmbracoFulltextsearchApiV5FulltextsearchMissingnodes({
      pageNumber: e
    }));
  }
  async reindex(e, n) {
    return await m(d(this, u), b.postUmbracoFulltextsearchApiV5FulltextsearchReindexnodes({
      requestBody: {
        includeDescendants: e,
        nodeIds: n
      }
    }));
  }
}
u = new WeakMap();
var h, w, T, v, k, N;
class P extends de {
  constructor(n) {
    super(n);
    p(this, h);
    p(this, w);
    p(this, T);
    p(this, v);
    p(this, k);
    p(this, N);
    y(this, w, new F(void 0)), this.config = d(this, w).asObservable(), y(this, T, new F(void 0)), this.indexStatus = d(this, T).asObservable(), y(this, v, new F(void 0)), this.indexedNodes = d(this, v).asObservable(), y(this, k, new B(void 0)), this.incorrectIndexedNodes = d(this, k).asObservable(), y(this, N, new B(void 0)), this.missingIndexedNodes = d(this, N).asObservable(), this.provideContext(z, this), y(this, h, new Fe(n)), this.consumeContext(H, (r) => {
      const s = r.getOpenApiConfiguration();
      l.TOKEN = s.token, l.BASE = s.base, l.WITH_CREDENTIALS = s.withCredentials;
    });
  }
  async getConfig() {
    const { data: n } = await d(this, h).config();
    n && d(this, w).setValue(n);
  }
  async reindex(n, r) {
    await d(this, h).reindex(n, r);
  }
  async getIndexStatus() {
    const { data: n } = await d(this, h).indexStatus();
    n && d(this, T).setValue(n);
  }
  async getIndexedNodes(n) {
    const { data: r } = await d(this, h).indexedNodes(n);
    r && d(this, v).setValue(r);
  }
  async getIncorrectIndexedNodes(n) {
    const { data: r } = await d(this, h).incorrectIndexedNodes(n);
    r && d(this, k).setValue(r);
  }
  async getMissingNodes(n) {
    const { data: r } = await d(this, h).missingNodes(n);
    r && d(this, N).setValue(r);
  }
}
h = new WeakMap(), w = new WeakMap(), T = new WeakMap(), v = new WeakMap(), k = new WeakMap(), N = new WeakMap();
const z = new ae(P.name), Ee = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  FULLTEXTSEARCH_CONTEXT_TOKEN: z,
  FullTextSearchContext: P,
  default: P
}, Symbol.toStringTag, { value: "Module" }));
var Pe = Object.defineProperty, Re = Object.getOwnPropertyDescriptor, $ = (t) => {
  throw TypeError(t);
}, I = (t, e, n, r) => {
  for (var s = r > 1 ? void 0 : r ? Re(e, n) : e, i = t.length - 1, o; i >= 0; i--)
    (o = t[i]) && (s = (r ? o(e, n, s) : o(s)) || s);
  return r && s && Pe(e, n, s), s;
}, K = (t, e, n) => e.has(t) || $("Cannot " + n), ze = (t, e, n) => (K(t, e, "read from private field"), e.get(t)), je = (t, e, n) => e.has(t) ? $("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(t) : e.set(t, n), Oe = (t, e, n, r) => (K(t, e, "write to private field"), e.set(t, n), n), A;
let f = class extends oe(se) {
  constructor() {
    super(), je(this, A), this.consumeContext(z, (t) => {
      Oe(this, A, t);
    });
  }
  _handleCancel() {
    var t;
    (t = this.modalContext) == null || t.submit();
  }
  _reindex(t) {
    var e;
    this._setButtonState(t, "waiting"), (e = ze(this, A)) == null || e.reindex(t, []), console.log("reindexing", t);
  }
  _setButtonState(t, e) {
    t ? this._withDescendantsState = e : this._withoutDescendantsState = e;
  }
  render() {
    var t, e;
    return C`
            <uui-dialog-layout headline="${(t = this.data) != null && t.unique ? "#fullTextSearch_reindexNode" : "#fullTextSearch_reindexAllNodes"}">
                ${(e = this.data) != null && e.unique ? C`
                    <uui-button look="primary" .state=${this._withoutDescendantsState} @click=${() => this._reindex(!1)}>
                        <umb-localize key="fullTextSearch_reindexJustThisNode">
                            Reindex just this node
                        </umb-localize></uui-button>
                    <uui-button look="secondary" .state=${this._withDescendantsState} @click=${() => this._reindex(!0)}>
                        <umb-localize key="fullTextSearch_reindexWithDescendants">
                            Reindex with descendants
                        </umb-localize>
                    </uui-button>
                ` : C`
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
A = /* @__PURE__ */ new WeakMap();
I([
  V({ attribute: !1 })
], f.prototype, "modalContext", 2);
I([
  V({ attribute: !1 })
], f.prototype, "data", 2);
I([
  G()
], f.prototype, "_withDescendantsState", 2);
I([
  G()
], f.prototype, "_withoutDescendantsState", 2);
f = I([
  ie("our-umbraco-fulltext-search-reindex-node-modal")
], f);
const Me = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  get default() {
    return f;
  }
}, Symbol.toStringTag, { value: "Module" })), Be = {
  fullTextSearch: {
    indexStatus: "Index Status",
    indexableNodes: "Indexable Nodes",
    indexableNodesDescription: "The total number of indexable nodes, according to the current Full Text Search config",
    allIndexableNodesAreIndexed: "All indexable nodes has full text content in index",
    indexedNodes: "Indexed Nodes",
    indexedNodesDescription: "The total number of indexed nodes searchable by Full Text Search",
    missingNodes: "Missing Nodes",
    missingNodesDescription: "The total number of missing indexed nodes, according to the current Full Text Search config",
    couldntGetMissingNodes: "Couldn't get missing nodes",
    nodesAreMissingInIndex: "{0} node(s) are missing full text content in index",
    incorrectIndexedNodes: "Incorrectly Indexed Nodes",
    incorrectIndexedNodesDescription: "The total number of indexed nodes that should not be indexed according to the current Full Text Search config",
    couldntGetIncorrectIndexedNodes: "Couldn't get incorrectly indexed nodes",
    nodesAreIncorrectlyIndexed: "{0} node(s) are incorrectly indexed with full text content",
    reindexNode: "Reindex Node",
    reindexNodes: "Reindex Nodes",
    reindexedNodes: "Reindexed {0} node(s)",
    reindexing: "Reindexing...",
    reindex: "Reindex",
    reindexDescription: "Select whether or not to reindex all nodes",
    reindexWithDescendants: "Reindex with descendants",
    includeDescendants: "Include descendants",
    allNodes: "All nodes",
    selectNodes: "Select nodes",
    selectNodesDescription: "Select the nodes to reindex",
    selectedNodes: "Selected nodes",
    description: "Description",
    developedBy: "Developed by",
    sponsoredBy: "Sponsored by",
    status: "Status",
    search: "Search",
    enterKeywordsHere: "Enter keywords here",
    advancedSettings: "Advanced settings",
    inspect: "Inspect",
    searchType: "Search type",
    searchTypeDescription: "The type of search to perform.",
    titleProperties: "Title properties",
    titlePropertiesDescription: "Adds field names to use for title properties. Note, that this overrides the config setting, so you need to add all wanted fields for titles here.",
    titleBoost: "Title boost",
    titleBoostDescription: "Set the boosting value for the title properties, to make titles more important than body text when searching.",
    bodyProperties: "Body properties",
    bodyPropertiesDescription: "Adds field names to use for body properties. Note, that this overrides the config setting, so you need to add all wanted fields for bodytext here.",
    summaryProperties: "Summary properties",
    summaryPropertiesDescription: "Adds field names to use for summary properties. Note, that if you don't specify any summary properties, the body properties will be used instead.",
    summaryLength: "Summary length",
    summaryLengthDescription: "Sets the summary length in the results. The default is 300 characters.",
    rootNodes: "Root nodes",
    rootNodesDescription: "With this setting, you can limit search results to be descendants of the selected nodes.",
    culture: "Culture",
    cultureDescription: "This is used to define which culture to search in. You should probably always set this, but it might work without it, in invariant sites.",
    enableWildcards: "Enable wildcards",
    enableWildcardsDescription: "This enables or disables use of wildcards in the search terms. Wildcard characters are added automatically to each of the terms.",
    fuzzyness: "Fuzzyness",
    fuzzynessDescription: "Fuzzyness is used to match your search term with similar words. This method sets the fuzzyness parameter of the search. The default is 0.8. If wildcards is enabled, fuzzyness will not be used.",
    allowedContentTypes: "Allowed Content Types",
    allowedContentTypesDescription: "Limit the search to nodes of the specified Content Type Aliases.",
    fullTextSearchIsDisabled: "FullTextSearch is disabled",
    fullTextSearchIsEnabled: "FullTextSearch is enabled",
    externalIndexNotFound: "ExternalIndex not found",
    enabled: "Enabled",
    defaultTitleField: "Default Title field",
    indexingActiveKey: '"Indexing Active" key',
    disallowedContentTypeAliases: "Disallowed Content Type aliases",
    disallowedPropertyAliases: "Disallowed property aliases",
    xPathsToRemove: "XPaths to remove from content",
    fullTextContentField: "Full Text Content field",
    fullTextPathField: "Full Text Path field"
  }
}, Le = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  default: Be
}, Symbol.toStringTag, { value: "Module" })), Ue = {
  fullTextSearch: {
    indexStatus: "Index Status",
    indexableNodes: "Indekserbare noder",
    indexableNodesDescription: "Antal noder der kan indekseres, ifølge den nuværende konfiguration",
    allIndexableNodesAreIndexed: "Alle indekserbare noder har full text indhold i indekset",
    indexedNodes: "Indekserede noder",
    indexedNodesDescription: "Antal noder der kan søges frem med Full Text Search",
    missingNodes: "Manglende noder",
    missingNodesDescription: "Antal noder der mangler full text indhold i indekset",
    couldntGetMissingNodes: "Kunne ikke hente manglende noder",
    nodesAreMissingInIndex: "{0} node(r) mangler full text indhold i indekset",
    incorrectIndexedNodes: "Fejlagtigt indekserede noder",
    incorrectIndexedNodesDescription: "Antal noder der ikke burde være indekseret, ifølge den nuværende konfiguration",
    couldntGetIncorrectIndexedNodes: "Kunne ikke hente fejlagtigt indekserede noder",
    nodesAreIncorrectlyIndexed: "{0} node(r) er fejlagtigt indekseret med full text indhold",
    reindexNodes: "Reindeksér noder",
    reindexedNodes: "Reindekserede {0} node(r)",
    reindexing: "Reindekserer...",
    reindex: "Reindeksér",
    reindexDescription: "Vælg om alle noder skal reindekseres",
    reindexWithDescendants: "Reindeksér med undernoder",
    includeDescendants: "Inkludér undernoder",
    allNodes: "Alle noder",
    selectNodes: "Vælg noder",
    selectNodesDescription: "Vælg noder at reindeksere",
    selectedNodes: "Valgte noder",
    description: "Beskrivelse",
    developedBy: "Udviklet af",
    sponsoredBy: "Sponsoreret af",
    status: "Status",
    search: "Søg",
    enterKeywordsHere: "Indtast søgeord her",
    advancedSettings: "Avancerede indstillinger",
    inspect: "Inspicér",
    searchType: "Søgningstype",
    searchTypeDescription: "Hvilken slags søgning skal der foretages",
    titleProperties: "Titelfelter",
    titlePropertiesDescription: "Vælg felter der skal bruges til titlen for søgeresultatet. Bemærk, dette overskriver standardkonfigurationen, vælg derfor alle ønskede felter her.",
    titleBoost: "Titelboost",
    titleBoostDescription: "Sæt boost værdien for titelfelterne, så titler vægter højere end indhold i søgningen.",
    bodyProperties: "Indholdsfelter",
    bodyPropertiesDescription: "Vælg felter der skal bruges til indholdet for søgeresultatet. Bemærk, dette overskriver standardkonfigurationen, vælg derfor alle ønskede felter her.",
    summaryProperties: "Beskrivelsesfelter",
    summaryPropertiesDescription: "Vælg felter der skal bruges til beskrivelsen af søgeresultatet. Bemærk, dette overskriver standardkonfigurationen, vælg derfor alle ønskede felter her.",
    summaryLength: "Beskrivelseslængde",
    summaryLengthDescription: "Angiv beskrivelseslængden for søgeresultaterne. Standardindstillingen er 300 tegn.",
    rootNodes: "Rodnoder",
    rootNodesDescription: "Med denne indstilling, kan du begrænse søgeresultaterne, til at være undernoder til den/de valgte noder.",
    culture: "Sprog",
    cultureDescription: "Vælg hvilket sprog der skal søges på. Du bør altid vælge dette, men søgningen virker muligvis uden sprog, hvis dit site ikke er sprogvarieret.",
    enableWildcards: "Aktiver wildcards",
    enableWildcardsDescription: "Dette aktiverer wildcards i søgningen, wildcard tegnet tilføjes automatisk til hver søgeterm.",
    fuzzyness: "Fuzzyness",
    fuzzynessDescription: "Fuzzyness bruges til at matche en søgeterm med lignende termer. Standardindstillingen er 0.8. Hvis wildcards er aktiveret, vil fuzzyness ikke blive brugt.",
    allowedContentTypes: "Tilladte dokumenttyper",
    allowedContentTypesDescription: "Begrænser søgningen til indhold af de valgte dokumenttypealiaser.",
    externalIndexNotFound: "ExternalIndex blev ikke fundet",
    fullTextSearchIsDisabled: "FullTextSearch er deaktiveret",
    fullTextSearchIsEnabled: "FullTextSearch is aktiveret",
    enabled: "Aktiveret",
    defaultTitleField: "Standard titel felt",
    indexingActiveKey: '"Aktiv indeksering" nøgle',
    disallowedContentTypeAliases: "Udelukkede dokumenttype aliaser",
    disallowedPropertyAliases: "Udelukkede egenskabsaliaser",
    xPathsToRemove: "Fjern XPaths fra indhold",
    fullTextContentField: "Full Text indholdsfelt",
    fullTextPathField: "Full Text sti felt"
  }
}, qe = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  default: Ue
}, Symbol.toStringTag, { value: "Module" })), He = {
  fullTextSearch: {
    indexStatus: "Statws Mynegai",
    indexableNodes: "Nodau Mynegeiddadwy",
    indexableNodesDescription: "Cyfanswm y nodau y gellir eu mynegeio, yn ôl y ffurfweddiad Full Text Search presennol",
    allIndexableNodesAreIndexed: "Mae gan bob nod a ellir ei mynegeio gynnwys testun llawn yn yr mynegai",
    indexedNodes: "Nodau Mynegeiedig",
    indexedNodesDescription: "Cyfanswm y nodau mynegeiedig y gellir eu chwilio gyda Full Text Search",
    missingNodes: "Nodau ar Goll",
    missingNodesDescription: "Cyfanswm y nodau mynegeiedig sydd ar goll, yn ôl y ffurfweddiad Full Text Search presennol",
    couldntGetMissingNodes: "Methwyd â chael nodau coll",
    nodesAreMissingInIndex: "Mae {0} nod(au) yn methu eu cynnwys testun llawn yn yr mynegai",
    incorrectIndexedNodes: "Nodau sydd wedi'u Mynegeio yn Anghywir",
    incorrectIndexedNodesDescription: "Cyfanswm y nodau mynegeiedig na ddylai fod wedi'u mynegeio yn ôl y ffurfweddiad Full Text Search presennol",
    couldntGetIncorrectIndexedNodes: "Methu â chael nodau wedi'u mynegeio' yn anghywir",
    nodesAreIncorrectlyIndexed: "Mae {0} nod(au) wedi'u mynegeio'n anghywir gyda chynnwys testun llawn",
    reindexNodes: "Ail-fynegi Nodau",
    reindexedNodes: "Mae {0} nod(au) wedi'u hail-fynegeio",
    reindexing: "Ail-fynegio...",
    reindex: "Ail-fynegi",
    reindexDescription: "Dewiswch a ydych am ail-fynegi pob nod ai peidio",
    reindexWithDescendants: "Ail-fynegi gyda disgynyddion",
    includeDescendants: "Cynnwys disgynyddion",
    allNodes: "Pob nod",
    selectNodes: "Dewis nodau",
    selectNodesDescription: "Dewiswch y nodau i'w hail-fynegi",
    selectedNodes: "Nodau a ddewiswyd",
    description: "Disgrifiad",
    developedBy: "Datblygwyd gan",
    sponsoredBy: "Noddwyd gan",
    status: "Statws",
    search: "Chwilio",
    enterKeywordsHere: "Rhowch eiriau allweddol yma",
    advancedSettings: "Gosodiadau Uwch",
    inspect: "Archwilio",
    searchType: "Math o chwiliad",
    searchTypeDescription: "Y math o chwiliad i'w wneud.",
    titleProperties: "Priodweddau teitl",
    titlePropertiesDescription: "Ychwanegwch enwau maes i'w defnyddio ar gyfer priodweddau teitl. Sylwer, fod hyn yn diystyru'r gosodiad ffurfweddiad, felly bydd angen ychwanegu pob maes rydych am eu defnyddio ar gyfer teitlau yma.",
    titleBoost: "Hyby teitl",
    titleBoostDescription: "Gosodwch y gwerth hyby ar gyfer y priodweddau teitl, i wneud teitlau'n bwysicach na thestun y corff wrth chwilio.",
    bodyProperties: "Priodweddau'r corff",
    bodyPropertiesDescription: "Ychwanegwch enwau maes i'w defnyddio ar gyfer priodweddau'r corff. Sylwer, fod hyn yn diystyru'r gosodiad ffurfweddiad, felly bydd angen ychwanegu pob maes rydych am eu defnyddio ar gyfer testun y corff yma.",
    summaryProperties: "Priodoleddau Crynodeb",
    summaryPropertiesDescription: "Ychwanegwch enwau maes i'w defnyddio ar gyfer priodweddau crynodeb. Sylwer, os nad ydych yn nodi unrhyw briodweddau crynodeb, bydd y priodweddau corff yn cael eu defnyddio yn lle.",
    summaryLength: "Hyd y Crynodeb",
    summaryLengthDescription: "Gosodwch hyd y crynodeb yn y canlyniadau. Y diofyn yw 300 o gymeriadau.",
    rootNodes: "Nodau gwraidd",
    rootNodesDescription: "Gyda'r gosodiad hwn, gallwch gyfyngu'r canlyniadau chwilio i fod yn ddisgynyddion y nodau a ddewiswyd.",
    culture: "Diwylliant",
    cultureDescription: "Defnyddir hwn i ddiffinio pa ddiwylliant i chwilio ynddo. Mae'n debyg y dylech osod hwn bob amser, ond gallai weithio hebddo, mewn safleoedd un iaith.",
    enableWildcards: "Galluogi cardiau gwyllt",
    enableWildcardsDescription: "Mae hyn yn galluogi neu'n analluogi defnydd o gardiau gwyllt yn y termau chwilio. Mae nodau cerdyn gwyllt yn cael eu hychwanegu'n awtomatig at bob un o'r termau.",
    fuzzyness: "Aneglurder",
    fuzzynessDescription: "Defnyddir aneglurder i gyfateb eich term chwilio gyda geiriau tebyg. Mae'r dull hwn yn gosod y paramedr aneglurder y chwiliad. Y rhagosodiad yw 0.8. Os yw cardiau gwyllt yn cael eu galluogi, ni ddefnyddir aneglurder.",
    allowedContentTypes: "Mathau o Gynnwys a ganiateir",
    allowedContentTypesDescription: "Cyfyngu'r chwiliad i nodau efo enwau arall fathau o gynnwys penodedig.",
    fullTextSearchIsDisabled: "Mae FullTextSearch wedi'i analluogi",
    fullTextSearchIsEnabled: "Mae FullTextSearch wedi'i alluogi",
    externalIndexNotFound: "Heb ddod o hyd i ExternalIndex",
    enabled: "Wedi'i alluogi",
    defaultTitleField: "Maes Teitl diofyn",
    indexingActiveKey: 'Allwedd "Indexing Active"',
    disallowedContentTypeAliases: "Enwau arall y mathau o gynnwys na caniateir",
    disallowedPropertyAliases: "Enwau arall y Eiddo na caniateir",
    xPathsToRemove: "Llwybrau XPath i'w tynnu o'r cynnwys",
    fullTextContentField: "Maes Cynnwys Testun Llawn",
    fullTextPathField: "Maes Llwybr Testun Llawn"
  }
}, Ve = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  default: He
}, Symbol.toStringTag, { value: "Module" })), Ge = {
  fullTextSearch: {
    indexStatus: "Statut de l'Index",
    indexableNodes: "Noeuds Indexables",
    indexableNodesDescription: "Le nombre total de noeuds indexables, selon la configuration actuelle du Full Text Search",
    allIndexableNodesAreIndexed: "Tous les noeuds indexables ont leur contenu intégral dans l'index",
    indexedNodes: "Noeuds Indexés",
    indexedNodesDescription: "Le nombre total de noeuds indexés qui peuvent être cherchés par le Full Text Search",
    missingNodes: "Noeuds manquant",
    missingNodesDescription: "Le nombre total de noeuds indexés manquant, selon la configuration actuelle du Full Text Search",
    couldntGetMissingNodes: "Impossible de récupérer les noeuds manquant",
    nodesAreMissingInIndex: "{0} noeud(s) n'ont pas leur contenu intégral dans l'index",
    incorrectIndexedNodes: "Noeuds Indexés Incorrectement",
    incorrectIndexedNodesDescription: "Le nombre total de noeuds indexés qui ne devraient pas l'être selon la configuration actuelle du Full Text Search",
    couldntGetIncorrectIndexedNodes: "Impossible de récupérer les noeuds indexés incorrectement",
    nodesAreIncorrectlyIndexed: "{0} noeud(s) sont incorrectement indexés avec leur contenu intégral",
    reindexNodes: "Indexer à nouveau les noeuds",
    reindexedNodes: "{0} noeud(s) ont été indexés à nouveau",
    reindexing: "Nouvelle indexation en cours...",
    reindex: "Indexer à nouveau",
    reindexDescription: "Choisissez s'il faut ou pas indexer à nouveau tous les noeuds",
    reindexWithDescendants: "Indexer à nouveau, y compris les descendants",
    includeDescendants: "Inclure les descendants",
    allNodes: "Tous les noeuds",
    selectNodes: "Sélectionner les noeuds",
    selectNodesDescription: "Sélectionner les noeuds à indexer à nouveau",
    selectedNodes: "Noeuds sélectionnés",
    description: "Description",
    developedBy: "Developpé par",
    sponsoredBy: "Sponsorisé par",
    status: "Statut",
    search: "Chercher",
    enterKeywordsHere: "Introduire les mots-clés ici",
    advancedSettings: "Réglages avancés",
    inspect: "Inspecter",
    searchType: "Type de recherche",
    searchTypeDescription: "Le type de recherche à exécuter.",
    titleProperties: "Propriétés pour le Titre",
    titlePropertiesDescription: "Ajouter le nom des champs à utiliser pour les propriété du titre. Notez que ceci remplace le paramètre de configuration, vous devez donc ajouter ici tous les champs souhaités pour le titre.",
    titleBoost: "Mise en avant du titre",
    titleBoostDescription: "Définissez la valeur de mise en avant pour les propriétés du titre, afin de rendre les titres plus importants que le corps du texte lors de la recherche.",
    bodyProperties: "Propriétés du Corps",
    bodyPropertiesDescription: "Ajouter le nom des champs à utiliser pour les propriétés du corps. Notez que ceci remplace le paramètre de configuration, vous devez donc ajouter ici tous les champs souhaités pour le corps du texte.",
    summaryProperties: "Propriétés du Résumé",
    summaryPropertiesDescription: "Ajouter le nom des champs à utiliser pour les propriétés du résumé. Notez que si vous ne spécifiez aucune propriété du résumé, les propriétés du corps seront utilisées à la place.",
    summaryLength: "Longueur du résumé",
    summaryLengthDescription: "Fixe la longueur du résumé dans les résultats. La valeur par défaut est de 300 caractères.",
    rootNodes: "Noeuds racine",
    rootNodesDescription: "Avec ce réglage, vous pouvez limiter les résultats de recherche aux descendants des noeuds sélectionnés.",
    culture: "Culture",
    cultureDescription: "Ceci est utilisé pour définir la culture dans laquelle la recherche est faite. Vous devriez probablement toujours la spécifier, mais cela pourrait fonctionner sans, notamment dans les sites invariants.",
    enableWildcards: "Activer les caractères génériques",
    enableWildcardsDescription: "Ceci active ou désactive l'utilisation de caractères génériques (wildcards) dans les termes de recherches. Les caractères génériques sont ajoutés automatiquement à chacun des termes.",
    fuzzyness: "Approximation",
    fuzzynessDescription: "L'approximation est utilisée pour faire correspondre votre terme de recherche avec des mots similaires. Cette méthode fixe le niveau d'approximation de la recherche. La valeur par défaut est 0,8. Si les caractères génériques sont activés, l'approximation ne sera pas utilisée.",
    allowedContentTypes: "Types de Contenu Autorisés",
    allowedContentTypesDescription: "Limite la recherche aux noeuds des Types de Contenu spécifiés.",
    fullTextSearchIsDisabled: "FullTextSearch est désactivé",
    fullTextSearchIsEnabled: "FullTextSearch est activé",
    externalIndexNotFound: "ExternalIndex n'a pas été trouvé",
    enabled: "Activé",
    defaultTitleField: "Champ par défaut pour le Titre",
    indexingActiveKey: 'Clé "Indexation Active"',
    disallowedContentTypeAliases: "Alias des Types de Contenu non-autorisés",
    disallowedPropertyAliases: "Alias des propriétés non-autorisées",
    xPathsToRemove: "XPaths à supprimer du contenu",
    fullTextContentField: "Champ du Contenu de Texte Intégral",
    fullTextPathField: "Champ du Chemin vers le Contenu Intégral"
  }
}, We = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  default: Ge
}, Symbol.toStringTag, { value: "Module" })), $e = {
  fullTextSearch: {
    indexStatus: "Indeksstatus",
    indexableNodes: "Indekserbare noder",
    indexableNodesDescription: "Det totale antall indekserbare noder, i henhold til gjeldende Full Text Search konfigurasjon",
    allIndexableNodesAreIndexed: "Alle indekserbare noder har fulltekstinnhold i indeksen",
    indexedNodes: "Indekserte noder",
    indexedNodesDescription: "Det totale antall indekserte noder som kan søkes etter av Full Text Search",
    missingNodes: "Manglende noder",
    missingNodesDescription: "Totalt antall manglende indekserte noder, i henhold til gjeldende Full Text Search konfigurasjon",
    couldntGetMissingNodes: "Kunne ikke hente manglende noder",
    nodesAreMissingInIndex: "{0} node(r) mangler fulltekstinnhold i indeksen",
    incorrectIndexedNodes: "Feilaktig indekserte noder",
    incorrectIndexedNodesDescription: "Det totale antall indekserte noder som ikke skal indekseres i henhold til gjeldende Full Text Search-konfigurasjon",
    couldntGetIncorrectIndexedNodes: "Kunne ikke hente feilaktig indekserte noder",
    nodesAreIncorrectlyIndexed: "{0} node(r) er feilaktig indeksert med fulltekstinnhold",
    reindexNodes: "Reindekser noder",
    reindexedNodes: "Reindekserte {0} node(r)",
    reindexing: "Reindekserer...",
    reindex: "Reindekser",
    reindexDescription: "Velg om alle noder skal reindekseres",
    reindexWithDescendants: "Reindekser med etterkommere",
    includeDescendants: "Inkluder etterkommere",
    allNodes: "Alle noder",
    selectNodes: "Velg noder",
    selectNodesDescription: "Velg nodene som skal reindekseres",
    selectedNodes: "Valgte noder",
    description: "Beskrivelse",
    developedBy: "Utviklet av",
    sponsoredBy: "Sponset av",
    status: "Status",
    search: "Søk",
    enterKeywordsHere: "Skriv inn nøkkelord her",
    advancedSettings: "Avanserte innstillinger",
    inspect: "Inspiser",
    searchType: "Søketype",
    searchTypeDescription: "Typen søk du vil utføre.",
    titleProperties: "Tittelegenskaper",
    titlePropertiesDescription: "Legger til feltnavn som brukes for tittelegenskaper. Merk at dette overstyrer innstillingene, så du må legge til alle ønskede felt for titler her.",
    titleBoost: "Tittelboost",
    titleBoostDescription: "Still inn boostingsverdien for tittelegenskapene for å gjøre titler viktigere enn brødtekst når du søker.",
    bodyProperties: "Brødtekstegenskaper",
    bodyPropertiesDescription: "Legger til feltnavn som brukes for brødtekstegenskaper. Merk at dette overstyrer innstillingene, så du må legge til alle ønskede felt for brødtekst her.",
    summaryProperties: "Oppsummeringsegenskaper",
    summaryPropertiesDescription: "Legger til feltnavn som skal brukes for oppsummeringsegenskaper. Merk at hvis du ikke spesifiserer noen oppsummeringsegenskaper, vil brødtekstegenskapene brukes i stedet.",
    summaryLength: "Oppsummeringslengde",
    summaryLengthDescription: "Angir sammendragets lengde i resultatene. Standardverdien er 300 tegn.",
    rootNodes: "Hovednoder",
    rootNodesDescription: "Med denne innstillingen kan du begrense søkeresultatene til å være etterkommere av de valgte nodene.",
    culture: "Kultur",
    cultureDescription: "Dette brukes til å definere hvilken kultur du skal søke i. Du bør sannsynligvis alltid angi dette, men det kan fungere uten den, på uforanderlige nettsteder.",
    enableWildcards: "Aktiver jokertegn",
    enableWildcardsDescription: "Disse aktiverer eller deaktiverer bruk av jokertegn i søkeordene. Jokertegn blir automatisk lagt til hvert av søkeordene.",
    fuzzyness: "Fuzzyness",
    fuzzynessDescription: "Fuzzyness brukes til å matche søkeordet ditt med lignende ord. Denne metoden angir fuzzyness-parameteren for søket. Standard er 0,8. Hvis jokertegn er aktivert, vil ikke fuzzyness brukes.",
    fullTextSearchIsDisabled: "FullTextSearch er deaktivert",
    externalIndexNotFound: "ExternalIndex ikke funnet"
  }
}, Ke = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  default: $e
}, Symbol.toStringTag, { value: "Module" })), Ye = {
  fullTextSearch: {
    indexStatus: "Index status",
    indexableNodes: "Indexeerbare nodes",
    indexableNodesDescription: "Het totaal aantal indexeerbare nodes volgens de huidige Full Text Search configuratie",
    indexedNodes: "Geïndexeerde nodes",
    indexedNodesDescription: "Het totaal aantal geïndexeerde nodes die doorzoekbaar zijn door Full Text Search",
    missingNodes: "Ontbrekende nodes",
    missingNodesDescription: "Het totaal aantal ontbrekende nodes volgens de huidige Full Text Search configuratie",
    incorrectIndexedNodes: "Foutief geïndexeerde nodes",
    incorrectIndexedNodesDescription: "Het totaal aantal geïndexeerde nodes, die dat niet zouden moeten zijn, volgens de huidige Full Text Search configuratie",
    reindexNodes: "Herindexeer nodes",
    reindexing: "Herindexeren...",
    reindex: "Herindexeer",
    reindexDescription: "Selecteer of alle nodes opnieuw geïndexeerd moeten worden",
    reindexWithDescendants: "Herindexeren inclusief onderliggende",
    includeDescendants: "Inclusief onderliggende",
    allNodes: "Alle nodes",
    selectNodes: "Selecteer nodes",
    selectNodesDescription: "Selecteer de nodes om te herindexeren",
    selectedNodes: "Geselecteerde nodes",
    description: "Omschrijving",
    developedBy: "Ontwikkeld door",
    sponsoredBy: "Gesponsord door"
  }
}, Xe = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  default: Ye
}, Symbol.toStringTag, { value: "Module" }));
export {
  at as onInit
};
//# sourceMappingURL=assets.js.map
