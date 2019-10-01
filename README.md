# Full Text Search

[![Build status](https://img.shields.io/appveyor/ci/skttl/our-umbraco-fulltextsearch.svg)](https://ci.appveyor.com/project/skttl/our-umbraco-fulltextsearch)
[![NuGet release](https://img.shields.io/nuget/v/Our.Umbraco.FullTextSearch.svg)](https://www.nuget.org/packages/Our.Umbraco.FullTextSearch)
[![Our Umbraco project page](https://img.shields.io/badge/our-umbraco-orange.svg)](https://our.umbraco.com/packages/website-utilities/full-text-search-8/)

Full Text Search is a fast, powerful and easy to setup search solution for Umbraco sites.

  - Searches the whole page content (also generated content)
  - Simple and easy setup for multiple search types (quoted, fuzzy matching, wildcards etc.)
  - Extends the default ExternalIndex

## Getting started

### Installation
> *Note:* Full Text Search has been developed against **Umbraco v8.1.0** and will support that version and above.

Full Text Search can be installed from either Our Umbraco package repository, NuGet, or build manually from the source-code. Remember to enable the full text search rendering/indexing in web.config. See developers guide for more information.

#### Our Umbraco package repository

To install from Our Umbraco, please download the package from:

> [https://our.umbraco.com/packages/website-utilities/full-text-search-8/](https://our.umbraco.com/packages/website-utilities/full-text-search-8/)

#### NuGet package repository

To [install from NuGet](https://www.nuget.org/packages/Our.Umbraco.FullTextSearch), you can run the following command from within Visual Studio:

	PM> Install-Package Our.Umbraco.FullTextSearch

We also have a [MyGet package repository](https://www.myget.org/gallery/umbraco-packages) - for bleeding-edge / development releases.

#### Manual build

If you prefer, you can compile  Full Text Search yourself, you'll need:

* [Visual Studio 2017 (or above, including Community Editions)](https://www.visualstudio.com/downloads/)
* Microsoft Build Tools 2015 (aka [MSBuild 15](https://www.microsoft.com/en-us/download/details.aspx?id=48159))

To clone it locally click the "Clone in Windows" button above or run the following git commands.

	git clone https://github.com/skttl/Our.Umbraco.FullTextSearch.git umbraco-full-text-search
	cd umbraco-full-text-search
	.\build.cmd

---

## Developers Guide

For details on how to use the Full Text Search package, please refer to our [Developers Guide](docs/developers-guide.md) documentation.

---

## Known Issues

Full Text Search reads the content of pages indexed, by downloading them via a web request. Because of that, your site needs to be able to call itself. If not, the content can not be collected.

---

## Contributing to this project

Anyone and everyone is welcome to contribute. Please take a moment to review the [guidelines for contributing](CONTRIBUTING.md).

* [Bug reports](CONTRIBUTING.md#bugs)
* [Feature requests](CONTRIBUTING.md#features)
* [Pull requests](CONTRIBUTING.md#pull-requests)


## Contact

Have a question?

* [Raise an issue](https://github.com/skttl/Our.Umbraco.FullTextSearch/issues) on GitHub


## Dev Team

* [Søren Kottal](https://github.com/skttl)

### Special thanks

* Thanks to [Rigardt Pheiffer](https://github.com/rigardtpheiffer) for building the original Full Text Search package, which this one is inspired by and somewhat based off.
* Icon made by [Vectors Market](https://www.flaticon.com/authors/vectors-market) from [www.flaticon.com](https://www.flaticon.com/)

## License

Copyright &copy; 2019 Søren Kottal

Licensed under the [MIT License](LICENSE.md)
