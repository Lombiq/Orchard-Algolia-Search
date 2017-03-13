using Algolia.Search;
using Lombiq.Hosting.AlgoliaSearch.Helpers;
using Lombiq.Hosting.AlgoliaSearch.Models;
using Orchard.Indexing;
using Orchard.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using Orchard.Environment;

namespace Lombiq.Hosting.AlgoliaSearch.Services
{
    public class AlgoliaSearchBuilder : ISearchBuilder
    {
        private readonly Work<IAlgoliaManager> _algoliaManagerWork;

        private string _indexName;
        private Query _query;
        private int _count;
        private int _skip;
        private List<string> _filters;
        private List<string> _numericFilters;


        public ILogger Logger { get; set; }


        public AlgoliaSearchBuilder(string indexName, Work<IAlgoliaManager> algoliaManagerWork)
        {
            _algoliaManagerWork = algoliaManagerWork;
            _indexName = indexName;
            _query = new Query();
            _skip = 0;
            _count = 1000;
            _filters = new List<string>();
            _numericFilters = new List<string>();

            Logger = NullLogger.Instance;
        }


        public ISearchBuilder Ascending()
        {
            return this;
        }

        public ISearchBuilder AsFilter()
        {
            return this;
        }

        public int Count()
        {
            CreateQuery();

            int hitsCount;
            if (int.TryParse((string)_algoliaManagerWork.Value.GetIndex(_indexName).Search(_query)["nbHits"], out hitsCount))
            {
                return hitsCount > 1000 ? 1000 : hitsCount;
            }

            return 0;
        }

        public ISearchBuilder ExactMatch()
        {
            return this;
        }

        public ISearchBuilder Forbidden()
        {
            return this;
        }

        public ISearchHit Get(int documentId)
        {
            return new AlgoliaSearchHit(_algoliaManagerWork.Value.GetIndex(_indexName).GetObject(documentId.ToString()));
        }

        public ISearchBits GetBits()
        {
            throw new NotSupportedException();
        }

        public ISearchBuilder Mandatory()
        {
            return this;
        }

        public ISearchBuilder NotAnalyzed()
        {
            return this;
        }

        public ISearchBuilder Parse(string[] defaultFields, string query, bool escape = true)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentException("Query can't be empty.");
            }

            _query.SetQueryString(query);

            return this;
        }

        public ISearchBuilder Parse(string defaultField, string query, bool escape = true)
        {
            return Parse(new[] { defaultField }, query, escape);
        }

        public IEnumerable<ISearchHit> Search()
        {
            CreateQuery();

            Logger.Debug("Searching: {0}", _query.GetQueryString());

            var result = _algoliaManagerWork.Value.GetIndex(_indexName).Search(_query);

            Logger.Debug("Search results: {0}", result["nbHits"]);

            return result["hits"].Children().Select(c => new AlgoliaSearchHit(c));
        }

        public ISearchBuilder Slice(int skip, int count)
        {
            if (skip < 0)
            {
                throw new ArgumentException("Skip must be greater or equal to zero.");
            }

            if (count <= 0)
            {
                throw new ArgumentException("Count must be greater than zero.");
            }

            _skip = skip;
            _count = count;

            return this;
        }

        public ISearchBuilder SortBy(string name)
        {
            return this;
        }

        public ISearchBuilder SortByBoolean(string name)
        {
            return this;
        }

        public ISearchBuilder SortByDateTime(string name)
        {
            return this;
        }

        public ISearchBuilder SortByDouble(string name)
        {
            return this;
        }

        public ISearchBuilder SortByInteger(string name)
        {
            return this;
        }

        public ISearchBuilder SortByString(string name)
        {
            return this;
        }

        public ISearchBuilder Weighted(float weight)
        {
            return this;
        }

        public ISearchBuilder WithField(string field, double value)
        {
            return AddFieldNumericFilter(field, value.ToString(CultureInfo.InvariantCulture));
        }

        public ISearchBuilder WithField(string field, int value)
        {
            return AddFieldNumericFilter(field, value.ToString());
        }

        public ISearchBuilder WithField(string field, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                _filters.Add(field + ":" + value);
            }

            return this;
        }

        public ISearchBuilder WithField(string field, DateTime value)
        {
            return AddFieldNumericFilter(field,
                DateTimeHelper
                    .DateTimeToUnixTimestamp(value)
                    .ToString(CultureInfo.InvariantCulture));
        }

        public ISearchBuilder WithField(string field, bool value)
        {
            return WithField(field, value ? 1 : 0);
        }

        public ISearchBuilder WithinRange(string field, string min, string max, bool includeMin = true, bool includeMax = true)
        {
            return this;
        }

        public ISearchBuilder WithinRange(string field, DateTime? min, DateTime? max, bool includeMin = true, bool includeMax = true)
        {
            return AddRangeNumericFilter(
                field,
                DateTimeHelper
                    .DateTimeToUnixTimestamp(min.Value)
                    .ToString(CultureInfo.InvariantCulture),
                DateTimeHelper
                    .DateTimeToUnixTimestamp(max.Value)
                    .ToString(CultureInfo.InvariantCulture),
                includeMin,
                includeMax);
        }

        public ISearchBuilder WithinRange(string field, double? min, double? max, bool includeMin = true, bool includeMax = true)
        {
            return AddRangeNumericFilter(
                field,
                min.Value.ToString(CultureInfo.InvariantCulture),
                max.Value.ToString(CultureInfo.InvariantCulture),
                includeMin,
                includeMax);
        }

        public ISearchBuilder WithinRange(string field, int? min, int? max, bool includeMin = true, bool includeMax = true)
        {
            return AddRangeNumericFilter(
                field,
                min.ToString(),
                max.ToString(),
                includeMin,
                includeMax);
        }


        private ISearchBuilder AddRangeNumericFilter(string field, string min, string max, bool includeMin = true, bool includeMax = true)
        {
            if (min != null && max != null)
            {
                _numericFilters.Add(
                    field +
                    ">" +
                    (includeMin ? "=" : "") +
                    min +
                    ", " +
                    field +
                    "<" +
                    (includeMax ? "=" : "") +
                    max);
            }
            else if (min != null)
            {
                _numericFilters.Add(
                    field +
                    ">" +
                    (includeMin ? "=" : "") +
                    min);
            }
            else if (max != null)
            {
                _numericFilters.Add(
                    field +
                    "<" +
                    (includeMax ? "=" : "") +
                    max);
            }

            return this;
        }

        private ISearchBuilder AddFieldNumericFilter(string field, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                _numericFilters.Add(field + "=" + value);
            }

            return this;
        }

        private void CreateQuery()
        {
            _query.SetOffset(_skip);

            if (_count > 0)
            {
                _query.SetLength(_count);
            }

            if (_filters.Any())
            {
                var filterStringBuilder = new StringBuilder();
                for (int i = 0; i < _filters.Count; i++)
                {
                    if (i == _filters.Count - 1)
                    {
                        filterStringBuilder.Append(_filters[i]);
                    }
                    else
                    {
                        filterStringBuilder.Append(_filters[i] + " OR ");
                    }
                }

                _query.SetFilters(filterStringBuilder.ToString());
            }

            if (_numericFilters.Any())
            {
                var numericFilterStringBuilder = new StringBuilder();
                for (int i = 0; i < _numericFilters.Count; i++)
                {
                    if (i == _numericFilters.Count - 1)
                    {
                        numericFilterStringBuilder.Append(_numericFilters[i]);
                    }
                    else
                    {
                        numericFilterStringBuilder.Append(_numericFilters[i] + ", ");
                    }
                }

                _query.SetNumericFilters(numericFilterStringBuilder.ToString());
            }

            Logger.Debug("New search query: {0}", _query.GetQueryString());
        }
    }
}