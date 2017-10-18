# Hosting - Algolia Search Readme



This module integrates [Algolia](https://www.algolia.com/) with the existing indexing and search features of [Orchard CMS](http://orchardproject.net/). If you don't have an account already then [click here to register](https://www.algolia.com/users/sign_up?utm_source=lombiq) and start using Algolia search in Orchard. If you want to just check out how the module works from the frontend, see the [Orchard Algolia Search Demo site](https://algoliasearchdemo.dotnest.com) on DotNest.

Hosting - Algolia Search is part of the [Hosting Suite](https://dotnest.com/knowledge-base/topics/lombiq-hosting-suite), which is a complete Orchard DevOps technology suite for building and maintaining scalable Orchard applications. The module is also available for [DotNest](https://dotnest.com/) sites.

The module deliberately doesn't provide any styling for the search feature apart from what's already there in the Algolia SDK. You need to style it and configure its capabilities adapted to the site that you're using it on; this can be easily done, see below.

The module's source is available in two public source repositories, automatically mirrored in both directions with [Git-hg Mirror](https://githgmirror.com):

- [https://bitbucket.org/Lombiq/hosting-algolia-search](https://bitbucket.org/Lombiq/hosting-algolia-search) (Mercurial repository)
- [https://github.com/Lombiq/Orchard-Algolia-Search](https://github.com/Lombiq/Orchard-Algolia-Search) (Git repository)

This project is developed by [Lombiq Technologies Ltd](http://lombiq.com/), one of the core developers of Orchard itself. Commercial-grade support is available through Lombiq.


## Configuration

1. Fill out the API keys in Algolia Search Settings in the Orchard admin.
2. Index some content in Orchard as usual.
3. At this point the search from in the UI will work as expected (because it doesn't use the backend search services), but the admin search won't.
4. Check out your newly created index in the [Algolia Dashboard](https://www.algolia.com/dashboard).
5. If you want the admin search work properly, then configure [Searchable Attributes](https://www.algolia.com/explorer#?index=pages&tab=ranking) (this is like when you set the fields in search settings in Orchard) and [Attributes for faceting](https://www.algolia.com/explorer#?index=pages&tab=display) (this needs to be configured for the `WithField()` and `WithinRange()` methods to work).


## Unsupported Orchard features

- The sorting methods in `AlgoliaSearchBuilder` won't do anything because Algolia has its own relevance based sorting: https://www.algolia.com/doc/guides/relevance/sorting/. You can configure it on the Algolia dashboard.


## Search on the frontend

- It uses [InstantSearch.js](https://community.algolia.com/instantsearch.js/).
- Override *Search.SearchForm.cshtml* if you want a completely different UI.
- Override *Algolia.Searchit.Template.cshtml* if you just want to display hits differently. Note that when you override this template you have to use the [Mustache](https://mustache.github.io/) syntax.


## Implementation notes

- After rebuilding an index all your settings will be lost for that particular index so you need to configure it again in the Algolia dashboard.
- The methods which aren't implemented are either not supported or can be configured from the Algolia dashboard.
- The maximum number of hits is 1000 because [it's recommended in order to keep good performance](https://www.algolia.com/doc/api-client/csharp/parameters/#paginationlimitedto). With `AlgoliaSearchBuilder.Count()` it's the maximum number you can get so the UI won't display a higher number than what can properly be displayed.