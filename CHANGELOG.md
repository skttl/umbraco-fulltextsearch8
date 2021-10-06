# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## Unreleased
- New highlighting engine for summaries, as the old one was too slow.

## 1.2.0
- Fix bug when caching cultured content, cached the default culture for all cultures instead
- Changed some info-logging to debug
- Removed task.run stuff when caching
- Adds version number to Umbraco.Sys.ServerVariables for retrieving in dashboard
- Adds functionality for generating a wrapping template for use when rendering (https://github.com/skttl/umbraco-fulltextsearch8/issues/47)
- Adds ability to limit which content types are being searched

## 1.1.1 - 2020-12-07
- Fix: Fix status queries when no disallowed types are added
- Fix: Wrong dictionary key for enabled/disabled

## 1.1.0 - 2020-12-07

- Fix: Excludes nodes without a template when searching
- Fix: Removes cached culture content, when culture is no longer available in the cached node
- Fix: Fixes wrong count of incorrectly indexed, and missing nodes in the status dashboard
- Fix: Fixes wrong version number in dashboard
- Feature: Adds configuration summary view to status dashboard
- Feature: Adds option to reload configuration without restarting site

## 1.0.1 - 2020-11-24

- Fixed Save&Publish fails in Umbraco if template throws exceptions #47

## 1.0.0 - 2020-11-01

- Changed the config format #35 **BREAKING**
- Switch to using RenderTemplate() for getting the content #36 **BREAKING**
- Only index nodes with a template and check umbracoNaviHide #27
- Adding a doctype to DisallowedContentTypeAliases doesn't remove already indexed pages #29
- When HighlightSearchTerms = true, ellipsis are always shown #23
- Fuzzy search does not return expected values #25
- No results returned if you do not have "Allow varying by culture" set to true on document type. #4
- No results when no specific culture is used #17
- Search returns 0 result with culture set #24
- added new dashboard in Settings section to see index status, reindex and test search.
- added health checks to see index status

## 0.2.0 - 2020-04-02
Lot's of improvements and fixes, for example v8.6 upgrade problems fixed and non-variant websites support.

## 0.1.0 - 2019-10-01
Initial release
