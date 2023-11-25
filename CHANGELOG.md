# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## 4.0.0-beta001
- 4c00c6f adds appschema, and generator for it
- **BREAKING** 1ef25d9 automatically reads options from configuration (appsettings.json) when starting
- #105 adds french language to the backoffice UI / 🙏 [mikecp](https://github.com/mikecp)
- #107 adds welsh language to the backoffice UI / 🙏 [OwainJ](https://github.com/OwainJ)

## 4.0.0-alpha004
- 1e823d4 fixes sqlite compatibility in migration to nvarchar column
- 5c7c0b4 adds option to disable limiting search results to published content with a template, and add custom queries

## 4.0.0-alpha003
- #103 trims search terms before searching, to prevent exceptions with wildcard queries
- #101 fixes for async event handlers / 🙏 [enkelmedia](https://github.com/enkelmedia)

## 4.0.0-alpha002
- #94 Changed datatype of text column in database
- #89 Fixed bug in backoffice, when searching with advanced options
- #98 Fixed issue with notification handlers
- #90 Added warning logging  / 🙏 [hfloyd](https://github.com/hfloyd)
- b760e1b adds notifications to CacheService
- #67 publishedPropertySuffix is not case invariant

## 4.0.0-alpha001
- **BREAKING** #86 Changed the default rendering to use HttpClient, and abstracted the page rendering, so you can switch based on preferences / 🙏 [enkelmedia](https://github.com/enkelmedia)
- Asynced all the things, or at least a big chunk / 🙏 [enkelmedia](https://github.com/enkelmedia)
- Added tests / 🙏 [enkelmedia](https://github.com/enkelmedia)
- **BREAKING** 6d6ab68 Terminology around indexing / rendering. The `IndexingActiveKey` is now `RenderingActiveKey`, helper method `IsIndexingActive` is now `IsRenderingActive`. The old things are obsoleted, so should still work. Planning to remove them if we ever get to 5.0.0
- **BREAKING** 6d6ab68 Detecting rendering/indexing is now based on a request header, instead of a key in HttpContext.Items.
- #82 Adds configurable markup for highlighting of search terms in result summaries.  / 🙏 [enkelmedia](https://github.com/enkelmedia)

## 3.1.0

- You can now choose which index or searcher you want to use for searching.
- Fix casing error in backoffice files.

## 3.0.0

- Upgraded to Umbraco 10
- Changed to a Razor Class Library - which means static assets are now embedded. When upgrading, make sure to delete App_Plugins/FullTextSearch from your project!

## 2.1.0

- Fixes JSON serializing of models, to make the dashboard actually work
- Fixes indexing including descendants
- Fixes bug where indexing everything, or including descendants returned 406 and no result
- Adds a default pagelength of 10 items, when searching

## 2.0.0

- Ported the package to net core and Umbraco 9
- Removed the wrapping template for use when rendering - because Umbraco 9 doesn't break the same way.
- Moved configuration to IOptions specified in Startup.cs
- Changed FullTextHelper to a regular class, that can be injected into views.
- Merged changes from 1.3.0

## 1.3.0

- New highlighting engine for summaries, as the old one was too slow.
- #54 Enumerating search results
- #65 Disallowed content type aliases was also disallowed in content

## 1.2.0 - 2021-07-19

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
