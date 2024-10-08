var P = (t) => {
  throw TypeError(t);
};
var q = (t, e, n) => e.has(t) || P("Cannot " + n);
var a = (t, e, n) => (q(t, e, "read from private field"), n ? n.call(t) : e.get(t)), h = (t, e, n) => e.has(t) ? P("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(t) : e.set(t, n), f = (t, e, n, r) => (q(t, e, "write to private field"), r ? r.call(t, n) : e.set(t, n), n);
import { UMB_AUTH_CONTEXT as W } from "@umbraco-cms/backoffice/auth";
import { UMB_DOCUMENT_ROOT_ENTITY_TYPE as ne, UMB_DOCUMENT_ENTITY_TYPE as re, UMB_DOCUMENT_DETAIL_REPOSITORY_ALIAS as ie } from "@umbraco-cms/backoffice/document";
import { UmbEntityActionBase as se } from "@umbraco-cms/backoffice/entity-action";
import { UmbModalToken as oe, UMB_MODAL_MANAGER_CONTEXT as ae } from "@umbraco-cms/backoffice/modal";
import { LitElement as de, html as C, property as K, state as X, customElement as le } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin as ce } from "@umbraco-cms/backoffice/element-api";
import { UmbControllerBase as ue } from "@umbraco-cms/backoffice/class-api";
import { UmbContextToken as xe } from "@umbraco-cms/backoffice/context-api";
import { tryExecuteAndNotify as y } from "@umbraco-cms/backoffice/resources";
import { UmbObjectState as F, UmbStringState as H } from "@umbraco-cms/backoffice/observable-api";
import { UMB_NOTIFICATION_CONTEXT as he } from "@umbraco-cms/backoffice/notification";
const fe = {
  type: "entityAction",
  kind: "default",
  alias: "our.umbraco.fulltextsearch.reindex.action",
  name: "ReindexNode",
  weight: -100,
  forEntityTypes: [ne, re],
  api: () => Promise.resolve().then(() => je),
  elementName: "our-umbraco-fulltext-search-actions-entity-reindexnode",
  meta: {
    icon: "icon-alarm-clock",
    label: "#fullTextSearch_reindex",
    repositoryAlias: ie
  }
}, me = [fe], ge = {
  type: "modal",
  alias: "our.umbraco.fulltextsearch.modals.reindexnode",
  name: "Reindex node",
  js: () => Promise.resolve().then(() => qe)
}, ye = [ge], be = [
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
    js: () => Promise.resolve().then(() => Be)
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
    js: () => Promise.resolve().then(() => Ke)
  },
  {
    type: "localization",
    alias: "Our.Umbraco.FullTextSearch.Localizations.Nb",
    name: "Norwegian bokmål",
    meta: {
      culture: "nb"
    },
    js: () => Promise.resolve().then(() => Je)
  },
  {
    type: "localization",
    alias: "Our.Umbraco.FullTextSearch.Localizations.Nl",
    name: "Dutch",
    meta: {
      culture: "nl"
    },
    js: () => Promise.resolve().then(() => Qe)
  }
], pe = be, Te = [
  {
    type: "globalContext",
    alias: "our.umbraco.fulltextsearch.context",
    name: "Full Text Search context",
    js: () => Promise.resolve().then(() => Ue)
  }
], Se = [...Te];
class L extends Error {
  constructor(e, n, r) {
    super(r), this.name = "ApiError", this.url = n.url, this.status = n.status, this.statusText = n.statusText, this.body = n.body, this.request = e;
  }
}
class _e extends Error {
  constructor(e) {
    super(e), this.name = "CancelError";
  }
  get isCancelled() {
    return !0;
  }
}
class Ne {
  constructor(e) {
    this._isResolved = !1, this._isRejected = !1, this._isCancelled = !1, this.cancelHandlers = [], this.promise = new Promise((n, r) => {
      this._resolve = n, this._reject = r;
      const i = (d) => {
        this._isResolved || this._isRejected || this._isCancelled || (this._isResolved = !0, this._resolve && this._resolve(d));
      }, s = (d) => {
        this._isResolved || this._isRejected || this._isCancelled || (this._isRejected = !0, this._reject && this._reject(d));
      }, o = (d) => {
        this._isResolved || this._isRejected || this._isCancelled || this.cancelHandlers.push(d);
      };
      return Object.defineProperty(o, "isResolved", {
        get: () => this._isResolved
      }), Object.defineProperty(o, "isRejected", {
        get: () => this._isRejected
      }), Object.defineProperty(o, "isCancelled", {
        get: () => this._isCancelled
      }), e(i, s, o);
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
      this.cancelHandlers.length = 0, this._reject && this._reject(new _e("Request aborted"));
    }
  }
  get isCancelled() {
    return this._isCancelled;
  }
}
class $ {
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
    request: new $(),
    response: new $()
  }
}, k = (t) => typeof t == "string", D = (t) => k(t) && t !== "", M = (t) => t instanceof Blob, J = (t) => t instanceof FormData, Ie = (t) => {
  try {
    return btoa(t);
  } catch {
    return Buffer.from(t).toString("base64");
  }
}, Ae = (t) => {
  const e = [], n = (i, s) => {
    e.push(`${encodeURIComponent(i)}=${encodeURIComponent(String(s))}`);
  }, r = (i, s) => {
    s != null && (Array.isArray(s) ? s.forEach((o) => r(i, o)) : typeof s == "object" ? Object.entries(s).forEach(([o, d]) => r(`${i}[${o}]`, d)) : n(i, s));
  };
  return Object.entries(t).forEach(([i, s]) => r(i, s)), e.length ? `?${e.join("&")}` : "";
}, ve = (t, e) => {
  const n = encodeURI, r = e.url.replace("{api-version}", t.VERSION).replace(/{(.*?)}/g, (s, o) => {
    var d;
    return (d = e.path) != null && d.hasOwnProperty(o) ? n(String(e.path[o])) : s;
  }), i = t.BASE + r;
  return e.query ? i + Ae(e.query) : i;
}, ke = (t) => {
  if (t.formData) {
    const e = new FormData(), n = (r, i) => {
      k(i) || M(i) ? e.append(r, i) : e.append(r, JSON.stringify(i));
    };
    return Object.entries(t.formData).filter(([, r]) => r != null).forEach(([r, i]) => {
      Array.isArray(i) ? i.forEach((s) => n(r, s)) : n(r, i);
    }), e;
  }
}, E = async (t, e) => typeof e == "function" ? e(t) : e, we = async (t, e) => {
  const [n, r, i, s] = await Promise.all([
    E(e, t.TOKEN),
    E(e, t.USERNAME),
    E(e, t.PASSWORD),
    E(e, t.HEADERS)
  ]), o = Object.entries({
    Accept: "application/json",
    ...s,
    ...e.headers
  }).filter(([, d]) => d != null).reduce((d, [m, c]) => ({
    ...d,
    [m]: String(c)
  }), {});
  if (D(n) && (o.Authorization = `Bearer ${n}`), D(r) && D(i)) {
    const d = Ie(`${r}:${i}`);
    o.Authorization = `Basic ${d}`;
  }
  return e.body !== void 0 && (e.mediaType ? o["Content-Type"] = e.mediaType : M(e.body) ? o["Content-Type"] = e.body.type || "application/octet-stream" : k(e.body) ? o["Content-Type"] = "text/plain" : J(e.body) || (o["Content-Type"] = "application/json")), new Headers(o);
}, Ee = (t) => {
  var e, n;
  if (t.body !== void 0)
    return (e = t.mediaType) != null && e.includes("application/json") || (n = t.mediaType) != null && n.includes("+json") ? JSON.stringify(t.body) : k(t.body) || M(t.body) || J(t.body) ? t.body : JSON.stringify(t.body);
}, Re = async (t, e, n, r, i, s, o) => {
  const d = new AbortController();
  let m = {
    headers: s,
    body: r ?? i,
    method: e.method,
    signal: d.signal
  };
  t.WITH_CREDENTIALS && (m.credentials = t.CREDENTIALS);
  for (const c of t.interceptors.request._fns)
    m = await c(m);
  return o(() => d.abort()), await fetch(n, m);
}, Ce = (t, e) => {
  if (e) {
    const n = t.headers.get(e);
    if (k(n))
      return n;
  }
}, Fe = async (t) => {
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
}, De = (t, e) => {
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
    const i = e.status ?? "unknown", s = e.statusText ?? "unknown", o = (() => {
      try {
        return JSON.stringify(e.body, null, 2);
      } catch {
        return;
      }
    })();
    throw new L(
      t,
      e,
      `Generic Error: status: ${i}; status text: ${s}; body: ${o}`
    );
  }
}, b = (t, e) => new Ne(async (n, r, i) => {
  try {
    const s = ve(t, e), o = ke(e), d = Ee(e), m = await we(t, e);
    if (!i.isCancelled) {
      let c = await Re(t, e, s, d, o, m, i);
      for (const te of t.interceptors.response._fns)
        c = await te(c);
      const Z = await Fe(c), ee = Ce(c, e.responseHeader), z = {
        url: s,
        ok: c.ok,
        status: c.status,
        statusText: c.statusText,
        body: ee ?? Z
      };
      De(e, z), n(z.body);
    }
  } catch (s) {
    r(s);
  }
});
class p {
  /**
  * @returns unknown OK
  * @throws ApiError
  */
  static getUmbracoFulltextsearchApiV5FulltextsearchConfig() {
    return b(l, {
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
    return b(l, {
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
    return b(l, {
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
    return b(l, {
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
    return b(l, {
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
    return b(l, {
      method: "POST",
      url: "/umbraco/fulltextsearch/api/v5/fulltextsearch/reindexnodes",
      body: n,
      mediaType: "application/json"
    });
  }
}
const ut = (t, e) => {
  e.registerMany([
    ...me,
    ...ye,
    ...pe,
    ...Se
  ]), t.consumeContext(W, (n) => {
    const r = n.getOpenApiConfiguration();
    l.TOKEN = r.token, l.BASE = r.base, l.WITH_CREDENTIALS = r.withCredentials;
  });
}, Oe = new oe("our.umbraco.fulltextsearch.modals.reindexnode", {
  modal: {
    type: "dialog",
    size: "small"
  }
});
var v;
class B extends se {
  constructor(n, r) {
    super(n, r);
    h(this, v);
    this.consumeContext(ae, (i) => {
      f(this, v, i);
    });
  }
  async execute() {
    var n;
    (n = a(this, v)) == null || n.open(this, Oe, {
      data: {
        unique: this.args.unique
      }
    });
  }
}
v = new WeakMap();
const je = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  ReindexNodeAction: B,
  default: B
}, Symbol.toStringTag, { value: "Module" }));
var u;
class Me {
  constructor(e) {
    h(this, u);
    f(this, u, e);
  }
  async config() {
    return await y(a(this, u), p.getUmbracoFulltextsearchApiV5FulltextsearchConfig());
  }
  async indexStatus() {
    return await y(a(this, u), p.getUmbracoFulltextsearchApiV5FulltextsearchIndexstatus());
  }
  async incorrectIndexedNodes(e) {
    return await y(a(this, u), p.getUmbracoFulltextsearchApiV5FulltextsearchIncorrectindexednodes({
      pageNumber: e
    }));
  }
  async indexedNodes(e) {
    return await y(a(this, u), p.getUmbracoFulltextsearchApiV5FulltextsearchIndexednodes({
      pageNumber: e
    }));
  }
  async missingNodes(e) {
    return await y(a(this, u), p.getUmbracoFulltextsearchApiV5FulltextsearchMissingnodes({
      pageNumber: e
    }));
  }
  async reindex(e, n) {
    return await y(a(this, u), p.postUmbracoFulltextsearchApiV5FulltextsearchReindexnodes({
      requestBody: {
        includeDescendants: e,
        nodeIds: n
      }
    }));
  }
}
u = new WeakMap();
var x, T, S, _, N, I;
class j extends ue {
  constructor(n) {
    super(n);
    h(this, x);
    h(this, T);
    h(this, S);
    h(this, _);
    h(this, N);
    h(this, I);
    f(this, T, new F(void 0)), this.config = a(this, T).asObservable(), f(this, S, new F(void 0)), this.indexStatus = a(this, S).asObservable(), f(this, _, new F(void 0)), this.indexedNodes = a(this, _).asObservable(), f(this, N, new H(void 0)), this.incorrectIndexedNodes = a(this, N).asObservable(), f(this, I, new H(void 0)), this.missingIndexedNodes = a(this, I).asObservable(), this.provideContext(U, this), f(this, x, new Me(n)), this.consumeContext(W, (r) => {
      const i = r.getOpenApiConfiguration();
      l.TOKEN = i.token, l.BASE = i.base, l.WITH_CREDENTIALS = i.withCredentials;
    });
  }
  async getConfig() {
    const { data: n } = await a(this, x).config();
    n && a(this, T).setValue(n);
  }
  async reindex(n, r) {
    await a(this, x).reindex(n, r);
  }
  async getIndexStatus() {
    const { data: n } = await a(this, x).indexStatus();
    n && a(this, S).setValue(n);
  }
  async getIndexedNodes(n) {
    const { data: r } = await a(this, x).indexedNodes(n);
    r && a(this, _).setValue(r);
  }
  async getIncorrectIndexedNodes(n) {
    const { data: r } = await a(this, x).incorrectIndexedNodes(n);
    r && a(this, N).setValue(r);
  }
  async getMissingNodes(n) {
    const { data: r } = await a(this, x).missingNodes(n);
    r && a(this, I).setValue(r);
  }
}
x = new WeakMap(), T = new WeakMap(), S = new WeakMap(), _ = new WeakMap(), N = new WeakMap(), I = new WeakMap();
const U = new xe(j.name), Ue = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  FULLTEXTSEARCH_CONTEXT_TOKEN: U,
  FullTextSearchContext: j,
  default: j
}, Symbol.toStringTag, { value: "Module" }));
var ze = Object.defineProperty, Pe = Object.getOwnPropertyDescriptor, Y = (t) => {
  throw TypeError(t);
}, w = (t, e, n, r) => {
  for (var i = r > 1 ? void 0 : r ? Pe(e, n) : e, s = t.length - 1, o; s >= 0; s--)
    (o = t[s]) && (i = (r ? o(e, n, i) : o(i)) || i);
  return r && i && ze(e, n, i), i;
}, Q = (t, e, n) => e.has(t) || Y("Cannot " + n), O = (t, e, n) => (Q(t, e, "read from private field"), e.get(t)), G = (t, e, n) => e.has(t) ? Y("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(t) : e.set(t, n), V = (t, e, n, r) => (Q(t, e, "write to private field"), e.set(t, n), n), A, R;
let g = class extends ce(de) {
  constructor() {
    super(), G(this, A), G(this, R), this.consumeContext(U, (t) => {
      V(this, R, t);
    }), this.consumeContext(he, (t) => {
      V(this, A, t);
    });
  }
  _handleCancel() {
    var t;
    (t = this.modalContext) == null || t.submit();
  }
  async _reindex(t) {
    var n, r, i, s, o;
    if (!this.modalContext) return;
    (n = this.modalContext) == null || n.submit();
    const e = (r = O(this, A)) == null ? void 0 : r.stay("default", {
      data: {
        headline: this.localize.term("fullTextSearch_reindexing"),
        message: this.localize.term("fullTextSearch_reindexingMessage")
      }
    });
    await ((s = O(this, R)) == null ? void 0 : s.reindex(t, [Number((i = this.modalContext) == null ? void 0 : i.data.unique) || 0])), e == null || e.close(), (o = O(this, A)) == null || o.peek("positive", {
      data: {
        headline: this.localize.term("fullTextSearch_reindexed"),
        message: this.localize.term("fullTextSearch_reindexedMessage")
      }
    });
  }
  render() {
    var t, e;
    return C`
            <uui-dialog-layout headline="${this.localize.term((t = this.modalContext) != null && t.data.unique ? "fullTextSearch_reindexNode" : "fullTextSearch_reindexAllNodes")}">
                ${(e = this.modalContext) != null && e.data.unique ? C`
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
R = /* @__PURE__ */ new WeakMap();
w([
  K({ attribute: !1 })
], g.prototype, "modalContext", 2);
w([
  K({ attribute: !1 })
], g.prototype, "data", 2);
w([
  X()
], g.prototype, "_withDescendantsState", 2);
w([
  X()
], g.prototype, "_withoutDescendantsState", 2);
g = w([
  le("our-umbraco-fulltext-search-reindex-node-modal")
], g);
const qe = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  get default() {
    return g;
  }
}, Symbol.toStringTag, { value: "Module" })), He = {
  fullTextSearch: {
    allIndexableNodesAreIndexed: "All indexable nodes has full text content in index",
    couldntGetIncorrectIndexedNodes: "Couldn't get incorrectly indexed nodes",
    couldntGetMissingNodes: "Couldn't get missing nodes",
    externalIndexNotFound: "ExternalIndex not found",
    fullTextSearchIsDisabled: "FullTextSearch is disabled",
    fullTextSearchIsEnabled: "FullTextSearch is enabled",
    indexableNodesDescription: "The total number of indexable nodes, according to the current Full Text Search config",
    indexedNodesDescription: "The total number of indexed nodes searchable by Full Text Search",
    incorrectIndexedNodesDescription: "The total number of indexed nodes that should not be indexed according to the current Full Text Search config",
    missingNodesDescription: "The total number of missing indexed nodes, according to the current Full Text Search config",
    nodesAreIncorrectlyIndexed: "{0} node(s) are incorrectly indexed with full text content",
    nodesAreMissingInIndex: "{0} node(s) are missing full text content in index",
    reindex: "Reindex",
    reindexAllNodes: "Reindex all nodes",
    reindexing: "Reindexing...",
    reindexingMessage: "This can take a while, please be patient",
    reindexNode: "Reindex node",
    reindexed: "Reindexed",
    reindexedMessage: "Reindexing complete",
    reindexNodes: "Reindex nodes",
    reindexWithDescendants: "Reindex with descendants"
  }
}, Le = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  default: He
}, Symbol.toStringTag, { value: "Module" })), $e = {
  fullTextSearch: {
    allIndexableNodesAreIndexed: "Alle indekserbare noder har full text indhold i indekset",
    couldntGetIncorrectIndexedNodes: "Kunne ikke hente fejlagtigt indekserede noder",
    couldntGetMissingNodes: "Kunne ikke hente manglende noder",
    externalIndexNotFound: "ExternalIndex blev ikke fundet",
    fullTextSearchIsDisabled: "FullTextSearch er deaktiveret",
    fullTextSearchIsEnabled: "FullTextSearch is aktiveret",
    incorrectIndexedNodesDescription: "Antal noder der ikke burde være indekseret, ifølge den nuværende konfiguration",
    indexableNodesDescription: "Antal noder der kan indekseres, ifølge den nuværende konfiguration",
    indexedNodesDescription: "Antal noder der kan søges frem med Full Text Search",
    missingNodesDescription: "Antal noder der mangler full text indhold i indekset",
    nodesAreIncorrectlyIndexed: "{0} node(r) er fejlagtigt indekseret med full text indhold",
    nodesAreMissingInIndex: "{0} node(r) mangler full text indhold i indekset",
    reindex: "Reindeksér",
    reindexAllNodes: "Reindeksér alle noder",
    reindexed: "Reindekseret",
    reindexedMessage: "Reindeksering færdig",
    reindexing: "Reindekserer...",
    reindexingMessage: "Dette kan tage et øjeblik",
    reindexNode: "Reindeksér node",
    reindexNodes: "Reindeksér noder",
    reindexWithDescendants: "Reindeksér med undernoder"
  }
}, Be = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  default: $e
}, Symbol.toStringTag, { value: "Module" })), Ge = {
  fullTextSearch: {
    allIndexableNodesAreIndexed: "Mae gan bob nod a ellir ei mynegeio gynnwys testun llawn yn yr mynegai",
    couldntGetIncorrectIndexedNodes: "Methu â chael nodau wedi'u mynegeio' yn anghywir",
    couldntGetMissingNodes: "Methwyd â chael nodau coll",
    externalIndexNotFound: "Heb ddod o hyd i ExternalIndex",
    fullTextSearchIsDisabled: "Mae FullTextSearch wedi'i analluogi",
    fullTextSearchIsEnabled: "Mae FullTextSearch wedi'i alluogi",
    incorrectIndexedNodesDescription: "Cyfanswm y nodau mynegeiedig na ddylai fod wedi'u mynegeio yn ôl y ffurfweddiad Full Text Search presennol",
    indexableNodesDescription: "Cyfanswm y nodau y gellir eu mynegeio, yn ôl y ffurfweddiad Full Text Search presennol",
    indexedNodesDescription: "Cyfanswm y nodau mynegeiedig y gellir eu chwilio gyda Full Text Search",
    missingNodesDescription: "Cyfanswm y nodau mynegeiedig sydd ar goll, yn ôl y ffurfweddiad Full Text Search presennol",
    nodesAreIncorrectlyIndexed: "Mae {0} nod(au) wedi'u mynegeio'n anghywir gyda chynnwys testun llawn",
    nodesAreMissingInIndex: "Mae {0} nod(au) yn methu eu cynnwys testun llawn yn yr mynegai",
    reindex: "Ail-fynegi",
    reindexAllNodes: "Ail-fynegi all nodau",
    reindexed: "Ail-fynegiwyd",
    reindexedMessage: "Wedi ail-fynegiwyd",
    reindexing: "Ail-fynegio...",
    reindexingMessage: "Wedi ail-fynegio",
    reindexNodes: "Ail-fynegi Nodau",
    reindexWithDescendants: "Ail-fynegi gyda disgynyddion"
  }
}, Ve = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  default: Ge
}, Symbol.toStringTag, { value: "Module" })), We = {
  fullTextSearch: {
    allIndexableNodesAreIndexed: "Tous les noeuds indexables ont leur contenu intégral dans l'index",
    couldntGetIncorrectIndexedNodes: "Impossible de récupérer les noeuds indexés incorrectement",
    couldntGetMissingNodes: "Impossible de récupérer les noeuds manquant",
    externalIndexNotFound: "ExternalIndex n'a pas été trouvé",
    fullTextSearchIsDisabled: "FullTextSearch est désactivé",
    fullTextSearchIsEnabled: "FullTextSearch est activé",
    incorrectIndexedNodesDescription: "Le nombre total de noeuds indexés qui ne devraient pas l'être selon la configuration actuelle du Full Text Search",
    indexableNodesDescription: "Le nombre total de noeuds indexables, selon la configuration actuelle du Full Text Search",
    indexedNodesDescription: "Le nombre total de noeuds indexés qui peuvent être cherchés par le Full Text Search",
    missingNodesDescription: "Le nombre total de noeuds indexés manquant, selon la configuration actuelle du Full Text Search",
    nodesAreIncorrectlyIndexed: "{0} noeud(s) sont incorrectement indexés avec leur contenu intégral",
    nodesAreMissingInIndex: "{0} noeud(s) n'ont pas leur contenu intégral dans l'index",
    reindex: "Indexer à nouveau",
    reindexAllNodes: "Indexer à nouveau tous les nœuds",
    reindexed: "Nouvelle indexation",
    reindexedMessage: "Nouvelle indexation terminée",
    reindexing: "Nouvelle indexation en cours...",
    reindexingMessage: "Veuillez patienter",
    reindexNode: "Indexer à nouveau le nœud",
    reindexNodes: "Indexer à nouveau les noeuds",
    reindexWithDescendants: "Indexer à nouveau, y compris les descendants"
  }
}, Ke = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  default: We
}, Symbol.toStringTag, { value: "Module" })), Xe = {
  fullTextSearch: {
    allIndexableNodesAreIndexed: "Alle indekserbare noder har fulltekstinnhold i indeksen",
    couldntGetIncorrectIndexedNodes: "Kunne ikke hente feilaktig indekserte noder",
    couldntGetMissingNodes: "Kunne ikke hente manglende noder",
    externalIndexNotFound: "ExternalIndex ikke funnet",
    fullTextSearchIsDisabled: "FullTextSearch er deaktivert",
    fullTextSearchIsEnabled: "FullTextSearch er aktivert",
    incorrectIndexedNodesDescription: "Det totale antall indekserte noder som ikke skal indekseres i henhold til gjeldende Full Text Search-konfigurasjon",
    indexableNodesDescription: "Det totale antall indekserbare noder, i henhold til gjeldende Full Text Search konfigurasjon",
    indexedNodesDescription: "Det totale antall indekserte noder som kan søkes etter av Full Text Search",
    missingNodesDescription: "Totalt antall manglende indekserte noder, i henhold til gjeldende Full Text Search konfigurasjon",
    nodesAreIncorrectlyIndexed: "{0} node(r) er feilaktig indeksert med fulltekstinnhold",
    nodesAreMissingInIndex: "{0} node(r) mangler fulltekstinnhold i indeksen",
    reindex: "Reindekser",
    reindexAllNodes: "Reindekser alle noder",
    reindexed: "Reindekseret",
    reindexedMessage: "Reindeksering ferdig",
    reindexing: "Reindekserer...",
    reindexingMessage: "Vennligst vent",
    reindexNode: "Reindekser node",
    reindexNodes: "Reindekser noder",
    reindexWithDescendants: "Reindekser med etterkommere"
  }
}, Je = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  default: Xe
}, Symbol.toStringTag, { value: "Module" })), Ye = {
  fullTextSearch: {
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
}, Qe = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  default: Ye
}, Symbol.toStringTag, { value: "Module" }));
export {
  ut as onInit
};
//# sourceMappingURL=assets.js.map
