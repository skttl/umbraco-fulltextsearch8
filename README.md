# Full Text Search

[![NuGet release](https://img.shields.io/nuget/v/Our.Umbraco.FullTextSearch.svg)](https://www.nuget.org/packages/Our.Umbraco.FullTextSearch)

Full Text Search is a fast, powerful and easy to setup search solution for Umbraco sites.

- Searches the whole page content (also generated content)
- Simple and easy setup for multiple search types (quoted, fuzzy matching, wildcards etc.)
- Extends the default ExternalIndex

## Getting started

### Installation

Full Text Search can be installed via NuGet

#### Choose the right version for your Umbraco Project

|FullTextSearch version|Compatible Umbraco Version|Documentation / developers guide|
|-|-|-|
|1.x.x|8.1.x - 8.x.x|[v1.x Developers Guide](docs/developers-guide-v1.md)|
|2.x.x|9.0.0 - 9.x.x|[v2.x Developers Guide](docs/developers-guide-v2.md)|
|3.x.x|10.0.0 - 12.x.x|[v2/3 Developers Guide](docs/developers-guide-v2.md)|
|**4.x.x**|10.0.0 - 13.x.x|[v4.x Developers Guide](docs/developers-guide-v4.md)|
[**5.x.x**]14.0.0 - ]|[v5.x Developers Guide](docs/developers-guide-v5.md)|

Only the latest minor is actively maintained.

#### Install via NuGet package repository

To [install from NuGet](https://www.nuget.org/packages/Our.Umbraco.FullTextSearch), you can run the following command from within Visual Studio:

    PM> Install-Package Our.Umbraco.FullTextSearch

---

## Contributing to this project

Anyone and everyone is welcome to contribute. Please take a moment to review the [guidelines for contributing](CONTRIBUTING.md).

- [Bug reports](CONTRIBUTING.md#bugs)
- [Feature requests](CONTRIBUTING.md#features)
- [Pull requests](CONTRIBUTING.md#pull-requests)

## Contact

Have a question?

- [Raise an issue](https://github.com/skttl/Our.Umbraco.FullTextSearch/issues) on GitHub

## Dev Team

- [Søren Kottal](https://github.com/skttl)

### Special thanks

- [Markus Johansson](https://github.com/enkelmedia) for contributing the HttpPageRenderer, tests, async code etc.
- Thanks to [Rigardt Pheiffer](https://github.com/rigardtpheiffer) for building the original Full Text Search package, which this one is inspired by and somewhat based off.
- Icon made by [Vectors Market](https://www.flaticon.com/authors/vectors-market) from [www.flaticon.com](https://www.flaticon.com/)

## License

Copyright &copy; 2022 Søren Kottal

Licensed under the [MIT License](LICENSE.md)
