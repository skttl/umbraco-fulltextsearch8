const _ = {
  bodySerializer: (r) => JSON.stringify(
    r,
    (t, e) => typeof e == "bigint" ? e.toString() : e
  )
}, G = ({
  onRequest: r,
  onSseError: t,
  onSseEvent: e,
  responseTransformer: a,
  responseValidator: i,
  sseDefaultRetryDelay: l,
  sseMaxRetryAttempts: c,
  sseMaxRetryDelay: n,
  sseSleepFn: o,
  url: u,
  ...s
}) => {
  let d;
  const x = o ?? ((f) => new Promise((y) => setTimeout(y, f)));
  return { stream: async function* () {
    let f = l ?? 3e3, y = 0;
    const j = s.signal ?? new AbortController().signal;
    for (; !j.aborted; ) {
      y++;
      const q = s.headers instanceof Headers ? s.headers : new Headers(s.headers);
      d !== void 0 && q.set("Last-Event-ID", d);
      try {
        const S = {
          redirect: "follow",
          ...s,
          body: s.serializedBody,
          headers: q,
          signal: j
        };
        let m = new Request(u, S);
        r && (m = await r(u, S));
        const p = await (s.fetch ?? globalThis.fetch)(m);
        if (!p.ok)
          throw new Error(
            `SSE failed: ${p.status} ${p.statusText}`
          );
        if (!p.body) throw new Error("No body in SSE response");
        const w = p.body.pipeThrough(new TextDecoderStream()).getReader();
        let O = "";
        const $ = () => {
          try {
            w.cancel();
          } catch {
          }
        };
        j.addEventListener("abort", $);
        try {
          for (; ; ) {
            const { done: V, value: L } = await w.read();
            if (V) break;
            O += L;
            const k = O.split(`

`);
            O = k.pop() ?? "";
            for (const J of k) {
              const F = J.split(`
`), C = [];
              let I;
              for (const b of F)
                if (b.startsWith("data:"))
                  C.push(b.replace(/^data:\s*/, ""));
                else if (b.startsWith("event:"))
                  I = b.replace(/^event:\s*/, "");
                else if (b.startsWith("id:"))
                  d = b.replace(/^id:\s*/, "");
                else if (b.startsWith("retry:")) {
                  const D = Number.parseInt(
                    b.replace(/^retry:\s*/, ""),
                    10
                  );
                  Number.isNaN(D) || (f = D);
                }
              let z, B = !1;
              if (C.length) {
                const b = C.join(`
`);
                try {
                  z = JSON.parse(b), B = !0;
                } catch {
                  z = b;
                }
              }
              B && (i && await i(z), a && (z = await a(z))), e == null || e({
                data: z,
                event: I,
                id: d,
                retry: f
              }), C.length && (yield z);
            }
          }
        } finally {
          j.removeEventListener("abort", $), w.releaseLock();
        }
        break;
      } catch (S) {
        if (t == null || t(S), c !== void 0 && y >= c)
          break;
        const m = Math.min(
          f * 2 ** (y - 1),
          n ?? 3e4
        );
        await x(m);
      }
    }
  }() };
}, M = (r) => {
  switch (r) {
    case "label":
      return ".";
    case "matrix":
      return ";";
    case "simple":
      return ",";
    default:
      return "&";
  }
}, Q = (r) => {
  switch (r) {
    case "form":
      return ",";
    case "pipeDelimited":
      return "|";
    case "spaceDelimited":
      return "%20";
    default:
      return ",";
  }
}, K = (r) => {
  switch (r) {
    case "label":
      return ".";
    case "matrix":
      return ";";
    case "simple":
      return ",";
    default:
      return "&";
  }
}, U = ({
  allowReserved: r,
  explode: t,
  name: e,
  style: a,
  value: i
}) => {
  if (!t) {
    const n = (r ? i : i.map((o) => encodeURIComponent(o))).join(Q(a));
    switch (a) {
      case "label":
        return `.${n}`;
      case "matrix":
        return `;${e}=${n}`;
      case "simple":
        return n;
      default:
        return `${e}=${n}`;
    }
  }
  const l = M(a), c = i.map((n) => a === "label" || a === "simple" ? r ? n : encodeURIComponent(n) : E({
    allowReserved: r,
    name: e,
    value: n
  })).join(l);
  return a === "label" || a === "matrix" ? l + c : c;
}, E = ({
  allowReserved: r,
  name: t,
  value: e
}) => {
  if (e == null)
    return "";
  if (typeof e == "object")
    throw new Error(
      "Deeply-nested arrays/objects aren’t supported. Provide your own `querySerializer()` to handle these."
    );
  return `${t}=${r ? e : encodeURIComponent(e)}`;
}, W = ({
  allowReserved: r,
  explode: t,
  name: e,
  style: a,
  value: i,
  valueOnly: l
}) => {
  if (i instanceof Date)
    return l ? i.toISOString() : `${e}=${i.toISOString()}`;
  if (a !== "deepObject" && !t) {
    let o = [];
    Object.entries(i).forEach(([s, d]) => {
      o = [
        ...o,
        s,
        r ? d : encodeURIComponent(d)
      ];
    });
    const u = o.join(",");
    switch (a) {
      case "form":
        return `${e}=${u}`;
      case "label":
        return `.${u}`;
      case "matrix":
        return `;${e}=${u}`;
      default:
        return u;
    }
  }
  const c = K(a), n = Object.entries(i).map(
    ([o, u]) => E({
      allowReserved: r,
      name: a === "deepObject" ? `${e}[${o}]` : o,
      value: u
    })
  ).join(c);
  return a === "label" || a === "matrix" ? c + n : n;
}, X = /\{[^{}]+\}/g, Y = ({ path: r, url: t }) => {
  let e = t;
  const a = t.match(X);
  if (a)
    for (const i of a) {
      let l = !1, c = i.substring(1, i.length - 1), n = "simple";
      c.endsWith("*") && (l = !0, c = c.substring(0, c.length - 1)), c.startsWith(".") ? (c = c.substring(1), n = "label") : c.startsWith(";") && (c = c.substring(1), n = "matrix");
      const o = r[c];
      if (o == null)
        continue;
      if (Array.isArray(o)) {
        e = e.replace(
          i,
          U({ explode: l, name: c, style: n, value: o })
        );
        continue;
      }
      if (typeof o == "object") {
        e = e.replace(
          i,
          W({
            explode: l,
            name: c,
            style: n,
            value: o,
            valueOnly: !0
          })
        );
        continue;
      }
      if (n === "matrix") {
        e = e.replace(
          i,
          `;${E({
            name: c,
            value: o
          })}`
        );
        continue;
      }
      const u = encodeURIComponent(
        n === "label" ? `.${o}` : o
      );
      e = e.replace(i, u);
    }
  return e;
}, Z = ({
  baseUrl: r,
  path: t,
  query: e,
  querySerializer: a,
  url: i
}) => {
  const l = i.startsWith("/") ? i : `/${i}`;
  let c = (r ?? "") + l;
  t && (c = Y({ path: t, url: c }));
  let n = e ? a(e) : "";
  return n.startsWith("?") && (n = n.substring(1)), n && (c += `?${n}`), c;
};
function ee(r) {
  const t = r.body !== void 0;
  if (t && r.bodySerializer)
    return "serializedBody" in r ? r.serializedBody !== void 0 && r.serializedBody !== "" ? r.serializedBody : null : r.body !== "" ? r.body : null;
  if (t)
    return r.body;
}
const te = async (r, t) => {
  const e = typeof t == "function" ? await t(r) : t;
  if (e)
    return r.scheme === "bearer" ? `Bearer ${e}` : r.scheme === "basic" ? `Basic ${btoa(e)}` : e;
}, H = ({
  allowReserved: r,
  array: t,
  object: e
} = {}) => (i) => {
  const l = [];
  if (i && typeof i == "object")
    for (const c in i) {
      const n = i[c];
      if (n != null)
        if (Array.isArray(n)) {
          const o = U({
            allowReserved: r,
            explode: !0,
            name: c,
            style: "form",
            value: n,
            ...t
          });
          o && l.push(o);
        } else if (typeof n == "object") {
          const o = W({
            allowReserved: r,
            explode: !0,
            name: c,
            style: "deepObject",
            value: n,
            ...e
          });
          o && l.push(o);
        } else {
          const o = E({
            allowReserved: r,
            name: c,
            value: n
          });
          o && l.push(o);
        }
    }
  return l.join("&");
}, re = (r) => {
  var e;
  if (!r)
    return "stream";
  const t = (e = r.split(";")[0]) == null ? void 0 : e.trim();
  if (t) {
    if (t.startsWith("application/json") || t.endsWith("+json"))
      return "json";
    if (t === "multipart/form-data")
      return "formData";
    if (["application/", "audio/", "image/", "video/"].some(
      (a) => t.startsWith(a)
    ))
      return "blob";
    if (t.startsWith("text/"))
      return "text";
  }
}, se = (r, t) => {
  var e, a;
  return t ? !!(r.headers.has(t) || (e = r.query) != null && e[t] || (a = r.headers.get("Cookie")) != null && a.includes(`${t}=`)) : !1;
}, ae = async ({
  security: r,
  ...t
}) => {
  for (const e of r) {
    if (se(t, e.name))
      continue;
    const a = await te(e, t.auth);
    if (!a)
      continue;
    const i = e.name ?? "Authorization";
    switch (e.in) {
      case "query":
        t.query || (t.query = {}), t.query[i] = a;
        break;
      case "cookie":
        t.headers.append("Cookie", `${i}=${a}`);
        break;
      case "header":
      default:
        t.headers.set(i, a);
        break;
    }
  }
}, N = (r) => Z({
  baseUrl: r.baseUrl,
  path: r.path,
  query: r.query,
  querySerializer: typeof r.querySerializer == "function" ? r.querySerializer : H(r.querySerializer),
  url: r.url
}), P = (r, t) => {
  var a;
  const e = { ...r, ...t };
  return (a = e.baseUrl) != null && a.endsWith("/") && (e.baseUrl = e.baseUrl.substring(0, e.baseUrl.length - 1)), e.headers = v(r.headers, t.headers), e;
}, ne = (r) => {
  const t = [];
  return r.forEach((e, a) => {
    t.push([a, e]);
  }), t;
}, v = (...r) => {
  const t = new Headers();
  for (const e of r) {
    if (!e)
      continue;
    const a = e instanceof Headers ? ne(e) : Object.entries(e);
    for (const [i, l] of a)
      if (l === null)
        t.delete(i);
      else if (Array.isArray(l))
        for (const c of l)
          t.append(i, c);
      else l !== void 0 && t.set(
        i,
        typeof l == "object" ? JSON.stringify(l) : l
      );
  }
  return t;
};
class T {
  constructor() {
    this.fns = [];
  }
  clear() {
    this.fns = [];
  }
  eject(t) {
    const e = this.getInterceptorIndex(t);
    this.fns[e] && (this.fns[e] = null);
  }
  exists(t) {
    const e = this.getInterceptorIndex(t);
    return !!this.fns[e];
  }
  getInterceptorIndex(t) {
    return typeof t == "number" ? this.fns[t] ? t : -1 : this.fns.indexOf(t);
  }
  update(t, e) {
    const a = this.getInterceptorIndex(t);
    return this.fns[a] ? (this.fns[a] = e, t) : !1;
  }
  use(t) {
    return this.fns.push(t), this.fns.length - 1;
  }
}
const ie = () => ({
  error: new T(),
  request: new T(),
  response: new T()
}), oe = H({
  allowReserved: !1,
  array: {
    explode: !0,
    style: "form"
  },
  object: {
    explode: !0,
    style: "deepObject"
  }
}), ce = {
  "Content-Type": "application/json"
}, R = (r = {}) => ({
  ..._,
  headers: ce,
  parseAs: "auto",
  querySerializer: oe,
  ...r
}), le = (r = {}) => {
  let t = P(R(), r);
  const e = () => ({ ...t }), a = (u) => (t = P(t, u), e()), i = ie(), l = async (u) => {
    const s = {
      ...t,
      ...u,
      fetch: u.fetch ?? t.fetch ?? globalThis.fetch,
      headers: v(t.headers, u.headers),
      serializedBody: void 0
    };
    s.security && await ae({
      ...s,
      security: s.security
    }), s.requestValidator && await s.requestValidator(s), s.body !== void 0 && s.bodySerializer && (s.serializedBody = s.bodySerializer(s.body)), (s.body === void 0 || s.serializedBody === "") && s.headers.delete("Content-Type");
    const d = N(s);
    return { opts: s, url: d };
  }, c = async (u) => {
    const { opts: s, url: d } = await l(u), x = {
      redirect: "follow",
      ...s,
      body: ee(s)
    };
    let g = new Request(d, x);
    for (const h of i.request.fns)
      h && (g = await h(g, s));
    const A = s.fetch;
    let f = await A(g);
    for (const h of i.response.fns)
      h && (f = await h(f, g, s));
    const y = {
      request: g,
      response: f
    };
    if (f.ok) {
      const h = (s.parseAs === "auto" ? re(f.headers.get("Content-Type")) : s.parseAs) ?? "json";
      if (f.status === 204 || f.headers.get("Content-Length") === "0") {
        let w;
        switch (h) {
          case "arrayBuffer":
          case "blob":
          case "text":
            w = await f[h]();
            break;
          case "formData":
            w = new FormData();
            break;
          case "stream":
            w = f.body;
            break;
          case "json":
          default:
            w = {};
            break;
        }
        return s.responseStyle === "data" ? w : {
          data: w,
          ...y
        };
      }
      let p;
      switch (h) {
        case "arrayBuffer":
        case "blob":
        case "formData":
        case "json":
        case "text":
          p = await f[h]();
          break;
        case "stream":
          return s.responseStyle === "data" ? f.body : {
            data: f.body,
            ...y
          };
      }
      return h === "json" && (s.responseValidator && await s.responseValidator(p), s.responseTransformer && (p = await s.responseTransformer(p))), s.responseStyle === "data" ? p : {
        data: p,
        ...y
      };
    }
    const j = await f.text();
    let q;
    try {
      q = JSON.parse(j);
    } catch {
    }
    const S = q ?? j;
    let m = S;
    for (const h of i.error.fns)
      h && (m = await h(S, f, g, s));
    if (m = m || {}, s.throwOnError)
      throw m;
    return s.responseStyle === "data" ? void 0 : {
      error: m,
      ...y
    };
  }, n = (u) => (s) => c({ ...s, method: u }), o = (u) => async (s) => {
    const { opts: d, url: x } = await l(s);
    return G({
      ...d,
      body: d.body,
      headers: d.headers,
      method: u,
      onRequest: async (g, A) => {
        let f = new Request(g, A);
        for (const y of i.request.fns)
          y && (f = await y(f, d));
        return f;
      },
      url: x
    });
  };
  return {
    buildUrl: N,
    connect: n("CONNECT"),
    delete: n("DELETE"),
    get: n("GET"),
    getConfig: e,
    head: n("HEAD"),
    interceptors: i,
    options: n("OPTIONS"),
    patch: n("PATCH"),
    post: n("POST"),
    put: n("PUT"),
    request: c,
    setConfig: a,
    sse: {
      connect: o("CONNECT"),
      delete: o("DELETE"),
      get: o("GET"),
      head: o("HEAD"),
      options: o("OPTIONS"),
      patch: o("PATCH"),
      post: o("POST"),
      put: o("PUT"),
      trace: o("TRACE")
    },
    trace: n("TRACE")
  };
}, fe = le(R({
  baseUrl: "http://localhost:37830/"
}));
export {
  fe as c
};
//# sourceMappingURL=client.gen-BACO-MNf.js.map
