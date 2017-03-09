using Orchard.Indexing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Utility.Extensions;
using Orchard;
using Newtonsoft.Json.Linq;
using Orchard.Localization;
using Lombiq.Hosting.AlgoliaSearch.Helpers;

namespace Lombiq.Hosting.AlgoliaSearch.Models
{
    public class AlgoliaDocumentIndex : IDocumentIndex
    {
        private string _name;
        private string _stringValue;
        private int _intValue;
        private double _doubleValue;
        private bool _removeTags;
        private TypeCode _typeCode;

        public int ContentItemId { get; private set; }
        public JObject Document { get; private set; }
        public bool IsDirty { get; private set; }
        public Localizer T { get; set; }


        public AlgoliaDocumentIndex(int documentId, Localizer t)
        {
            Document = new JObject();
            SetContentItemId(documentId);
            IsDirty = false;
            _typeCode = TypeCode.Empty;

            T = t;
        }


        public IDocumentIndex Add(string name, DateTime value)
        {
            return Add(name, DateTimeHelper.DateTimeToUnixTimestamp(value));
        }

        public IDocumentIndex Add(string name, bool value)
        {
            return Add(name, value ? 1 : 0);
        }

        public IDocumentIndex Add(string name, double value)
        {
            PrepareForIndexing();
            _name = name;
            _doubleValue = value;
            _typeCode = TypeCode.Single;
            IsDirty = true;

            return this;
        }

        public IDocumentIndex Add(string name, int value)
        {
            PrepareForIndexing();
            _name = name;
            _intValue = value;
            _typeCode = TypeCode.Int32;
            IsDirty = true;

            return this;
        }

        public IDocumentIndex Add(string name, string value)
        {
            PrepareForIndexing();
            _name = name;
            _stringValue = value;
            _typeCode = TypeCode.String;
            IsDirty = true;

            return this;
        }

        public IDocumentIndex Analyze()
        {
            return this;
        }

        public IDocumentIndex RemoveTags()
        {
            _removeTags = true;

            return this;
        }

        public IDocumentIndex SetContentItemId(int contentItemId)
        {
            ContentItemId = contentItemId;
            Document["objectID"] = contentItemId.ToString();

            return this;
        }

        public IDocumentIndex Store()
        {
            return this;
        }

        // It's essentially actually adding the last property stored on this object to the document. This is used like
        // a public method on the interface because in Orchard the RemoveTags() and others like that are chained after 
        // the Add() methods. So we must implement the property adding this way like in Lucene. In practice this
        // means only that we have to call this method before actually saving the document in AlgoliaIndexProvider.
        public void PrepareForIndexing()
        {
            switch (_typeCode)
            {
                case TypeCode.String:
                    if (_removeTags)
                    {
                        _stringValue = _stringValue.RemoveTags(true);
                    }

                    Document[_name] = _stringValue;
                    break;
                case TypeCode.Int32:
                    Document[_name] = _intValue;
                    break;
                case TypeCode.Single:
                    Document[_name] = _doubleValue;
                    break;
                case TypeCode.Empty:
                    break;
                default:
                    throw new OrchardException(T("Unexpected index type."));
            }

            _removeTags = false;
            _typeCode = TypeCode.Empty;
        }
    }
}