import { UMB_AUTH_CONTEXT as s } from "@umbraco-cms/backoffice/auth";
import { UMB_DOCUMENT_ROOT_ENTITY_TYPE as l, UMB_DOCUMENT_ENTITY_TYPE as c, UMB_DOCUMENT_DETAIL_REPOSITORY_ALIAS as r } from "@umbraco-cms/backoffice/document";
const m = {
  type: "entityAction",
  kind: "default",
  alias: "our.umbraco.fulltextsearch.reindex.action",
  name: "ReindexNode",
  weight: -100,
  forEntityTypes: [l, c],
  api: () => import("./reindex.action-DG1dTn-f.js"),
  elementName: "our-umbraco-fulltext-search-actions-entity-reindexnode",
  meta: {
    icon: "icon-alarm-clock",
    label: "#fullTextSearch_reindex",
    repositoryAlias: r
  }
}, u = [m], p = {
  type: "modal",
  alias: "our.umbraco.fulltextsearch.modals.reindexnode",
  name: "Reindex node",
  js: () => import("./reindexnode.element-C5_DoL2X.js")
}, T = [p], E = [
  {
    type: "localization",
    alias: "Our.Umbraco.FullTextSearch.Localizations.En",
    name: "English",
    meta: {
      culture: "en"
    },
    js: () => import("./en-BhWH3rme.js")
  },
  {
    type: "localization",
    alias: "Our.Umbraco.FullTextSearch.Localizations.Da",
    name: "Danish",
    meta: {
      culture: "da"
    },
    js: () => import("./da-BgQK7Sz-.js")
  },
  {
    type: "localization",
    alias: "Our.Umbraco.FullTextSearch.Localizations.Cy",
    name: "Welsh",
    meta: {
      culture: "cy"
    },
    js: () => import("./cy-Co3J_yCx.js")
  },
  {
    type: "localization",
    alias: "Our.Umbraco.FullTextSearch.Localizations.Fr",
    name: "French",
    meta: {
      culture: "fr"
    },
    js: () => import("./fr-3pqRkfVr.js")
  },
  {
    type: "localization",
    alias: "Our.Umbraco.FullTextSearch.Localizations.Nb",
    name: "Norwegian bokmål",
    meta: {
      culture: "nb"
    },
    js: () => import("./nb-_r7CNvV3.js")
  },
  {
    type: "localization",
    alias: "Our.Umbraco.FullTextSearch.Localizations.Nl",
    name: "Dutch",
    meta: {
      culture: "nl"
    },
    js: () => import("./nl-q1MxHDQM.js")
  }
], d = E, x = [
  {
    type: "globalContext",
    alias: "our.umbraco.fulltextsearch.context",
    name: "Full Text Search context",
    js: () => import("./fulltextsearch.context-BLn8Y4qb.js")
  }
], h = [...x];
class n {
  constructor() {
    this._fns = [];
  }
  eject(e) {
    const t = this._fns.indexOf(e);
    t !== -1 && (this._fns = [
      ...this._fns.slice(0, t),
      ...this._fns.slice(t + 1)
    ]);
  }
  use(e) {
    this._fns = [...this._fns, e];
  }
}
const o = {
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
    request: new n(),
    response: new n()
  }
}, _ = (i, e) => {
  e.registerMany([
    ...u,
    ...T,
    ...d,
    ...h
  ]), i.consumeContext(s, (t) => {
    const a = t.getOpenApiConfiguration();
    o.TOKEN = a.token, o.BASE = a.base, o.WITH_CREDENTIALS = a.withCredentials;
  });
};
export {
  o as O,
  _ as o
};
//# sourceMappingURL=index-DB07aDTd.js.map
