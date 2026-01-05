import { UMB_DOCUMENT_DETAIL_REPOSITORY_ALIAS as a, UMB_DOCUMENT_ROOT_ENTITY_TYPE as e, UMB_DOCUMENT_ENTITY_TYPE as t } from "@umbraco-cms/backoffice/document";
const o = [
  {
    alias: "our.umbraco.fulltextsearch.entrypoint",
    name: "Our.Umbraco.FullTextSearch.EntryPoint",
    type: "backofficeEntryPoint",
    js: () => import("./entrypoint-BvEYOnO2.js")
  }
], i = {
  type: "entityAction",
  kind: "default",
  alias: "our.umbraco.fulltextsearch.reindex.action",
  name: "ReindexNode",
  weight: -100,
  forEntityTypes: [e, t],
  api: () => import("./reindex.action-DG1dTn-f.js"),
  elementName: "our-umbraco-fulltext-search-actions-entity-reindexnode",
  meta: {
    icon: "icon-alarm-clock",
    label: "#fullTextSearch_reindex",
    repositoryAlias: a
  }
}, l = [i], n = {
  type: "modal",
  alias: "our.umbraco.fulltextsearch.modals.reindexnode",
  name: "Reindex node",
  js: () => import("./reindexnode.element-DK8m4Ock.js")
}, r = [n], c = [
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
], s = c, u = [
  ...o,
  ...l,
  ...r,
  ...s
];
export {
  u as manifests
};
//# sourceMappingURL=assets.js.map
