var E = (t) => {
  throw TypeError(t);
};
var _ = (t, e, r) => e.has(t) || E("Cannot " + r);
var p = (t, e, r) => (_(t, e, "read from private field"), r ? r.call(t) : e.get(t)), m = (t, e, r) => e.has(t) ? E("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(t) : e.set(t, r), b = (t, e, r, s) => (_(t, e, "write to private field"), s ? s.call(t, r) : e.set(t, r), r);
import { UMB_AUTH_CONTEXT as A } from "@umbraco-cms/backoffice/auth";
import { UmbControllerBase as N } from "@umbraco-cms/backoffice/class-api";
import { UmbContextToken as O } from "@umbraco-cms/backoffice/context-api";
import { O as y } from "./index-BfoeJ7q7.js";
import { tryExecuteAndNotify as q } from "@umbraco-cms/backoffice/resources";
class g extends Error {
  constructor(e, r, s) {
    super(s), this.name = "ApiError", this.url = r.url, this.status = r.status, this.statusText = r.statusText, this.body = r.body, this.request = e;
  }
}
class F extends Error {
  constructor(e) {
    super(e), this.name = "CancelError";
  }
  get isCancelled() {
    return !0;
  }
}
class U {
  constructor(e) {
    this._isResolved = !1, this._isRejected = !1, this._isCancelled = !1, this.cancelHandlers = [], this.promise = new Promise((r, s) => {
      this._resolve = r, this._reject = s;
      const n = (a) => {
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
      }), e(n, i, o);
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
      this.cancelHandlers.length = 0, this._reject && this._reject(new F("Request aborted"));
    }
  }
  get isCancelled() {
    return this._isCancelled;
  }
}
const h = (t) => typeof t == "string", R = (t) => h(t) && t !== "", T = (t) => t instanceof Blob, x = (t) => t instanceof FormData, H = (t) => {
  try {
    return btoa(t);
  } catch {
    return Buffer.from(t).toString("base64");
  }
}, P = (t) => {
  const e = [], r = (n, i) => {
    e.push(`${encodeURIComponent(n)}=${encodeURIComponent(String(i))}`);
  }, s = (n, i) => {
    i != null && (Array.isArray(i) ? i.forEach((o) => s(n, o)) : typeof i == "object" ? Object.entries(i).forEach(([o, a]) => s(`${n}[${o}]`, a)) : r(n, i));
  };
  return Object.entries(t).forEach(([n, i]) => s(n, i)), e.length ? `?${e.join("&")}` : "";
}, B = (t, e) => {
  const r = encodeURI, s = e.url.replace("{api-version}", t.VERSION).replace(/{(.*?)}/g, (i, o) => {
    var a;
    return (a = e.path) != null && a.hasOwnProperty(o) ? r(String(e.path[o])) : i;
  }), n = t.BASE + s;
  return e.query ? n + P(e.query) : n;
}, D = (t) => {
  if (t.formData) {
    const e = new FormData(), r = (s, n) => {
      h(n) || T(n) ? e.append(s, n) : e.append(s, JSON.stringify(n));
    };
    return Object.entries(t.formData).filter(([, s]) => s != null).forEach(([s, n]) => {
      Array.isArray(n) ? n.forEach((i) => r(s, i)) : r(s, n);
    }), e;
  }
}, f = async (t, e) => typeof e == "function" ? e(t) : e, v = async (t, e) => {
  const [r, s, n, i] = await Promise.all([
    f(e, t.TOKEN),
    f(e, t.USERNAME),
    f(e, t.PASSWORD),
    f(e, t.HEADERS)
  ]), o = Object.entries({
    Accept: "application/json",
    ...i,
    ...e.headers
  }).filter(([, a]) => a != null).reduce((a, [d, c]) => ({
    ...a,
    [d]: String(c)
  }), {});
  if (R(r) && (o.Authorization = `Bearer ${r}`), R(s) && R(n)) {
    const a = H(`${s}:${n}`);
    o.Authorization = `Basic ${a}`;
  }
  return e.body !== void 0 && (e.mediaType ? o["Content-Type"] = e.mediaType : T(e.body) ? o["Content-Type"] = e.body.type || "application/octet-stream" : h(e.body) ? o["Content-Type"] = "text/plain" : x(e.body) || (o["Content-Type"] = "application/json")), new Headers(o);
}, I = (t) => {
  var e, r;
  if (t.body !== void 0)
    return (e = t.mediaType) != null && e.includes("application/json") || (r = t.mediaType) != null && r.includes("+json") ? JSON.stringify(t.body) : h(t.body) || T(t.body) || x(t.body) ? t.body : JSON.stringify(t.body);
}, L = async (t, e, r, s, n, i, o) => {
  const a = new AbortController();
  let d = {
    headers: i,
    body: s ?? n,
    method: e.method,
    signal: a.signal
  };
  t.WITH_CREDENTIALS && (d.credentials = t.CREDENTIALS);
  for (const c of t.interceptors.request._fns)
    d = await c(d);
  return o(() => a.abort()), await fetch(r, d);
}, $ = (t, e) => {
  if (e) {
    const r = t.headers.get(e);
    if (h(r))
      return r;
  }
}, k = async (t) => {
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
}, M = (t, e) => {
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
    throw new g(t, e, s);
  if (!e.ok) {
    const n = e.status ?? "unknown", i = e.statusText ?? "unknown", o = (() => {
      try {
        return JSON.stringify(e.body, null, 2);
      } catch {
        return;
      }
    })();
    throw new g(
      t,
      e,
      `Generic Error: status: ${n}; status text: ${i}; body: ${o}`
    );
  }
}, V = (t, e) => new U(async (r, s, n) => {
  try {
    const i = B(t, e), o = D(e), a = I(e), d = await v(t, e);
    if (!n.isCancelled) {
      let c = await L(t, e, i, a, o, d, n);
      for (const j of t.interceptors.response._fns)
        c = await j(c);
      const w = await k(c), S = $(c, e.responseHeader), C = {
        url: i,
        ok: c.ok,
        status: c.status,
        statusText: c.statusText,
        body: S ?? w
      };
      M(e, C), r(C.body);
    }
  } catch (i) {
    s(i);
  }
});
class z {
  /**
  * @returns unknown OK
  * @throws ApiError
  */
  static postUmbracoFulltextsearchApiV5FulltextsearchReindexnodes(e = {}) {
    const { requestBody: r } = e;
    return V(y, {
      method: "POST",
      url: "/umbraco/fulltextsearch/api/v5/fulltextsearch/reindexnodes",
      body: r,
      mediaType: "application/json"
    });
  }
}
var l;
class G {
  constructor(e) {
    m(this, l);
    b(this, l, e);
  }
  async reindex(e, r) {
    return await q(p(this, l), z.postUmbracoFulltextsearchApiV5FulltextsearchReindexnodes({
      requestBody: {
        includeDescendants: e,
        nodeKey: r
      }
    }));
  }
}
l = new WeakMap();
var u;
class J extends N {
  constructor(r) {
    super(r);
    m(this, u);
    this.provideContext(W, this), b(this, u, new G(r)), this.consumeContext(A, (s) => {
      const n = s.getOpenApiConfiguration();
      y.TOKEN = n.token, y.BASE = n.base, y.WITH_CREDENTIALS = n.withCredentials;
    });
  }
  async reindex(r, s) {
    await p(this, u).reindex(r, s);
  }
}
u = new WeakMap();
const W = new O(J.name);
export {
  W as FULLTEXTSEARCH_CONTEXT_TOKEN,
  J as FullTextSearchContext,
  J as default
};
//# sourceMappingURL=fulltextsearch.context-BP_Dxq3u.js.map
