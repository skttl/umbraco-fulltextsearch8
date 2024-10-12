var j = (t) => {
  throw TypeError(t);
};
var v = (t, e, r) => e.has(t) || j("Cannot " + r);
var c = (t, e, r) => (v(t, e, "read from private field"), r ? r.call(t) : e.get(t)), m = (t, e, r) => e.has(t) ? j("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(t) : e.set(t, r), y = (t, e, r, s) => (v(t, e, "write to private field"), s ? s.call(t, r) : e.set(t, r), r);
import { UMB_AUTH_CONTEXT as P } from "@umbraco-cms/backoffice/auth";
import { UmbControllerBase as B } from "@umbraco-cms/backoffice/class-api";
import { UmbContextToken as D } from "@umbraco-cms/backoffice/context-api";
import { O as h } from "./index-DB07aDTd.js";
import { tryExecuteAndNotify as x } from "@umbraco-cms/backoffice/resources";
import { UmbObjectState as N, UmbStringState as I } from "@umbraco-cms/backoffice/observable-api";
class U extends Error {
  constructor(e, r, s) {
    super(s), this.name = "ApiError", this.url = r.url, this.status = r.status, this.statusText = r.statusText, this.body = r.body, this.request = e;
  }
}
class L extends Error {
  constructor(e) {
    super(e), this.name = "CancelError";
  }
  get isCancelled() {
    return !0;
  }
}
class $ {
  constructor(e) {
    this._isResolved = !1, this._isRejected = !1, this._isCancelled = !1, this.cancelHandlers = [], this.promise = new Promise((r, s) => {
      this._resolve = r, this._reject = s;
      const n = (o) => {
        this._isResolved || this._isRejected || this._isCancelled || (this._isResolved = !0, this._resolve && this._resolve(o));
      }, a = (o) => {
        this._isResolved || this._isRejected || this._isCancelled || (this._isRejected = !0, this._reject && this._reject(o));
      }, i = (o) => {
        this._isResolved || this._isRejected || this._isCancelled || this.cancelHandlers.push(o);
      };
      return Object.defineProperty(i, "isResolved", {
        get: () => this._isResolved
      }), Object.defineProperty(i, "isRejected", {
        get: () => this._isRejected
      }), Object.defineProperty(i, "isCancelled", {
        get: () => this._isCancelled
      }), e(n, a, i);
    });
  }
  get [Symbol.toStringTag]() {
    return "Cancellable Promise";
  }
  then(e, r) {
    return this.promise.then(e, r);
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
      this.cancelHandlers.length = 0, this._reject && this._reject(new L("Request aborted"));
    }
  }
  get isCancelled() {
    return this._isCancelled;
  }
}
const S = (t) => typeof t == "string", A = (t) => S(t) && t !== "", F = (t) => t instanceof Blob, O = (t) => t instanceof FormData, G = (t) => {
  try {
    return btoa(t);
  } catch {
    return Buffer.from(t).toString("base64");
  }
}, M = (t) => {
  const e = [], r = (n, a) => {
    e.push(`${encodeURIComponent(n)}=${encodeURIComponent(String(a))}`);
  }, s = (n, a) => {
    a != null && (Array.isArray(a) ? a.forEach((i) => s(n, i)) : typeof a == "object" ? Object.entries(a).forEach(([i, o]) => s(`${n}[${i}]`, o)) : r(n, a));
  };
  return Object.entries(t).forEach(([n, a]) => s(n, a)), e.length ? `?${e.join("&")}` : "";
}, k = (t, e) => {
  const r = encodeURI, s = e.url.replace("{api-version}", t.VERSION).replace(/{(.*?)}/g, (a, i) => {
    var o;
    return (o = e.path) != null && o.hasOwnProperty(i) ? r(String(e.path[i])) : a;
  }), n = t.BASE + s;
  return e.query ? n + M(e.query) : n;
}, z = (t) => {
  if (t.formData) {
    const e = new FormData(), r = (s, n) => {
      S(n) || F(n) ? e.append(s, n) : e.append(s, JSON.stringify(n));
    };
    return Object.entries(t.formData).filter(([, s]) => s != null).forEach(([s, n]) => {
      Array.isArray(n) ? n.forEach((a) => r(s, a)) : r(s, n);
    }), e;
  }
}, E = async (t, e) => typeof e == "function" ? e(t) : e, J = async (t, e) => {
  const [r, s, n, a] = await Promise.all([
    E(e, t.TOKEN),
    E(e, t.USERNAME),
    E(e, t.PASSWORD),
    E(e, t.HEADERS)
  ]), i = Object.entries({
    Accept: "application/json",
    ...a,
    ...e.headers
  }).filter(([, o]) => o != null).reduce((o, [f, d]) => ({
    ...o,
    [f]: String(d)
  }), {});
  if (A(r) && (i.Authorization = `Bearer ${r}`), A(s) && A(n)) {
    const o = G(`${s}:${n}`);
    i.Authorization = `Basic ${o}`;
  }
  return e.body !== void 0 && (e.mediaType ? i["Content-Type"] = e.mediaType : F(e.body) ? i["Content-Type"] = e.body.type || "application/octet-stream" : S(e.body) ? i["Content-Type"] = "text/plain" : O(e.body) || (i["Content-Type"] = "application/json")), new Headers(i);
}, W = (t) => {
  var e, r;
  if (t.body !== void 0)
    return (e = t.mediaType) != null && e.includes("application/json") || (r = t.mediaType) != null && r.includes("+json") ? JSON.stringify(t.body) : S(t.body) || F(t.body) || O(t.body) ? t.body : JSON.stringify(t.body);
}, K = async (t, e, r, s, n, a, i) => {
  const o = new AbortController();
  let f = {
    headers: a,
    body: s ?? n,
    method: e.method,
    signal: o.signal
  };
  t.WITH_CREDENTIALS && (f.credentials = t.CREDENTIALS);
  for (const d of t.interceptors.request._fns)
    f = await d(f);
  return i(() => o.abort()), await fetch(r, f);
}, X = (t, e) => {
  if (e) {
    const r = t.headers.get(e);
    if (S(r))
      return r;
  }
}, Q = async (t) => {
  if (t.status !== 204)
    try {
      const e = t.headers.get("Content-Type");
      if (e) {
        const r = ["application/octet-stream", "application/pdf", "application/zip", "audio/", "image/", "video/"];
        if (e.includes("application/json") || e.includes("+json"))
          return await t.json();
        if (r.some((s) => e.includes(s)))
          return await t.blob();
        if (e.includes("multipart/form-data"))
          return await t.formData();
        if (e.includes("text/"))
          return await t.text();
      }
    } catch (e) {
      console.error(e);
    }
}, Y = (t, e) => {
  const s = {
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
  if (s)
    throw new U(t, e, s);
  if (!e.ok) {
    const n = e.status ?? "unknown", a = e.statusText ?? "unknown", i = (() => {
      try {
        return JSON.stringify(e.body, null, 2);
      } catch {
        return;
      }
    })();
    throw new U(
      t,
      e,
      `Generic Error: status: ${n}; status text: ${a}; body: ${i}`
    );
  }
}, b = (t, e) => new $(async (r, s, n) => {
  try {
    const a = k(t, e), i = z(e), o = W(e), f = await J(t, e);
    if (!n.isCancelled) {
      let d = await K(t, e, a, o, i, f, n);
      for (const H of t.interceptors.response._fns)
        d = await H(d);
      const q = await Q(d), V = X(d, e.responseHeader), _ = {
        url: a,
        ok: d.ok,
        status: d.status,
        statusText: d.statusText,
        body: V ?? q
      };
      Y(e, _), r(_.body);
    }
  } catch (a) {
    s(a);
  }
});
class p {
  /**
  * @returns unknown OK
  * @throws ApiError
  */
  static getUmbracoFulltextsearchApiV5FulltextsearchConfig() {
    return b(h, {
      method: "GET",
      url: "/umbraco/fulltextsearch/api/v5/fulltextsearch/config"
    });
  }
  /**
  * @returns unknown OK
  * @throws ApiError
  */
  static getUmbracoFulltextsearchApiV5FulltextsearchIncorrectindexednodes(e = {}) {
    const { pageNumber: r } = e;
    return b(h, {
      method: "GET",
      url: "/umbraco/fulltextsearch/api/v5/fulltextsearch/incorrectindexednodes",
      query: {
        pageNumber: r
      }
    });
  }
  /**
  * @returns unknown OK
  * @throws ApiError
  */
  static getUmbracoFulltextsearchApiV5FulltextsearchIndexednodes(e = {}) {
    const { pageNumber: r } = e;
    return b(h, {
      method: "GET",
      url: "/umbraco/fulltextsearch/api/v5/fulltextsearch/indexednodes",
      query: {
        pageNumber: r
      }
    });
  }
  /**
  * @returns unknown OK
  * @throws ApiError
  */
  static getUmbracoFulltextsearchApiV5FulltextsearchIndexstatus() {
    return b(h, {
      method: "GET",
      url: "/umbraco/fulltextsearch/api/v5/fulltextsearch/indexstatus"
    });
  }
  /**
  * @returns unknown OK
  * @throws ApiError
  */
  static getUmbracoFulltextsearchApiV5FulltextsearchMissingnodes(e = {}) {
    const { pageNumber: r } = e;
    return b(h, {
      method: "GET",
      url: "/umbraco/fulltextsearch/api/v5/fulltextsearch/missingnodes",
      query: {
        pageNumber: r
      }
    });
  }
  /**
  * @returns unknown OK
  * @throws ApiError
  */
  static postUmbracoFulltextsearchApiV5FulltextsearchReindexnodes(e = {}) {
    const { requestBody: r } = e;
    return b(h, {
      method: "POST",
      url: "/umbraco/fulltextsearch/api/v5/fulltextsearch/reindexnodes",
      body: r,
      mediaType: "application/json"
    });
  }
}
var l;
class Z {
  constructor(e) {
    m(this, l);
    y(this, l, e);
  }
  async config() {
    return await x(c(this, l), p.getUmbracoFulltextsearchApiV5FulltextsearchConfig());
  }
  async indexStatus() {
    return await x(c(this, l), p.getUmbracoFulltextsearchApiV5FulltextsearchIndexstatus());
  }
  async incorrectIndexedNodes(e) {
    return await x(c(this, l), p.getUmbracoFulltextsearchApiV5FulltextsearchIncorrectindexednodes({
      pageNumber: e
    }));
  }
  async indexedNodes(e) {
    return await x(c(this, l), p.getUmbracoFulltextsearchApiV5FulltextsearchIndexednodes({
      pageNumber: e
    }));
  }
  async missingNodes(e) {
    return await x(c(this, l), p.getUmbracoFulltextsearchApiV5FulltextsearchMissingnodes({
      pageNumber: e
    }));
  }
  async reindex(e, r) {
    return await x(c(this, l), p.postUmbracoFulltextsearchApiV5FulltextsearchReindexnodes({
      requestBody: {
        includeDescendants: e,
        nodeIds: r
      }
    }));
  }
}
l = new WeakMap();
var u, g, T, w, R, C;
class ee extends B {
  constructor(r) {
    super(r);
    m(this, u);
    m(this, g);
    m(this, T);
    m(this, w);
    m(this, R);
    m(this, C);
    y(this, g, new N(void 0)), this.config = c(this, g).asObservable(), y(this, T, new N(void 0)), this.indexStatus = c(this, T).asObservable(), y(this, w, new N(void 0)), this.indexedNodes = c(this, w).asObservable(), y(this, R, new I(void 0)), this.incorrectIndexedNodes = c(this, R).asObservable(), y(this, C, new I(void 0)), this.missingIndexedNodes = c(this, C).asObservable(), this.provideContext(te, this), y(this, u, new Z(r)), this.consumeContext(P, (s) => {
      const n = s.getOpenApiConfiguration();
      h.TOKEN = n.token, h.BASE = n.base, h.WITH_CREDENTIALS = n.withCredentials;
    });
  }
  async getConfig() {
    const { data: r } = await c(this, u).config();
    r && c(this, g).setValue(r);
  }
  async reindex(r, s) {
    await c(this, u).reindex(r, s);
  }
  async getIndexStatus() {
    const { data: r } = await c(this, u).indexStatus();
    r && c(this, T).setValue(r);
  }
  async getIndexedNodes(r) {
    const { data: s } = await c(this, u).indexedNodes(r);
    s && c(this, w).setValue(s);
  }
  async getIncorrectIndexedNodes(r) {
    const { data: s } = await c(this, u).incorrectIndexedNodes(r);
    s && c(this, R).setValue(s);
  }
  async getMissingNodes(r) {
    const { data: s } = await c(this, u).missingNodes(r);
    s && c(this, C).setValue(s);
  }
}
u = new WeakMap(), g = new WeakMap(), T = new WeakMap(), w = new WeakMap(), R = new WeakMap(), C = new WeakMap();
const te = new D(ee.name);
export {
  te as FULLTEXTSEARCH_CONTEXT_TOKEN,
  ee as FullTextSearchContext,
  ee as default
};
//# sourceMappingURL=fulltextsearch.context-BLn8Y4qb.js.map
