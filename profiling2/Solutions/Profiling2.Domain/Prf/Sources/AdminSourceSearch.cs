using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Sources
{
    /// <summary>
    /// An instance of a search conducted by a user.
    /// </summary>
    public class AdminSourceSearch : Entity
    {
        public virtual string OrSearchTerms { get; set; }
        public virtual string AndSearchTerms { get; set; }
        public virtual string ExcludeSearchTerms { get; set; }
        public virtual DateTime SearchDateTime { get; set; }
        public virtual AdminUser SearchedByAdminUser { get; set; }
        public virtual int NumOfMatchingSources { get; set; }
        public virtual bool Archive { get; set; }
        public virtual IList<AdminReviewedSource> AdminReviewedSources { get; set; }

        public AdminSourceSearch()
        {
            this.AdminReviewedSources = new List<AdminReviewedSource>();
        }

        // Parses a given search string into the fields of the current data model
        public AdminSourceSearch(string searchTerm)
        {
            this.AdminReviewedSources = new List<AdminReviewedSource>();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                IList<string> excludeTerms = new List<string>();
                IList<string> includeTerms = new List<string>();

                // extract negative phrases
                var negativePhraseMatches = Regex.Matches(searchTerm, "-\"(.*?)\"");
                if (negativePhraseMatches.Count > 0)
                {
                    foreach (Match m in negativePhraseMatches)
                    {
                        excludeTerms.Add("\"" + m.Groups[1].Value + "\"");
                        searchTerm = searchTerm.Replace(m.Groups[1].Value, "");
                    }
                    searchTerm = searchTerm.Replace("\"", "").Replace("-", "");
                }

                // extract positive phrases
                var positivePhraseMatches = Regex.Matches(searchTerm, "\"(.*?)\"");
                if (positivePhraseMatches.Count > 0)
                {
                    foreach (Match m in positivePhraseMatches)
                    {
                        includeTerms.Add("\"" + m.Groups[1].Value + "\"");
                        searchTerm = searchTerm.Replace(m.Groups[1].Value, "");
                    }
                    searchTerm = searchTerm.Replace("\"", "");
                }

                // extract individual words
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    string[] words = searchTerm.Split(' ');
                    foreach (string word in words)
                    {
                        if (!string.IsNullOrEmpty(word))
                        {
                            if (word.StartsWith("-"))
                                excludeTerms.Add(word.Replace("-", ""));
                            else
                                includeTerms.Add(word);
                        }
                    }
                }

                this.AndSearchTerms = string.Join(" ", includeTerms);
                this.ExcludeSearchTerms = string.Join(" ", excludeTerms);
            }
            this.SearchDateTime = DateTime.Now;
        }

        protected IList<string> ExtractTerms(string str)
        {
            IList<string> terms = new List<string>();

            if (!string.IsNullOrEmpty(str))
            {
                string remaining = str;

                // extract phrases
                var matches = Regex.Matches(str, "\"(.*?)\"");
                if (matches.Count > 0)
                {
                    foreach (Match m in matches)
                    {
                        terms.Add(m.Groups[1].Value);
                        remaining = remaining.Replace(m.Groups[1].Value, "");
                    }
                }
                remaining = remaining.Replace("\"", "");

                // extract individual words
                if (!string.IsNullOrEmpty(remaining))
                {
                    string[] words = remaining.Split(' ');
                    foreach (string word in words)
                        if (!string.IsNullOrEmpty(word))
                            terms.Add(word);
                }
            }

            return terms;
        }

        // List of search terms without quotes; used in retrieving identical searches.
        protected IList<string> AndSearchTermsList
        {
            get
            {
                return this.ExtractTerms(this.AndSearchTerms);
            }
        }

        protected IList<string> ExcludeSearchTermsList
        {
            get
            {
                return this.ExtractTerms(this.ExcludeSearchTerms);
            }
        }

        public virtual IEnumerable<string> AndSearchTermsQuoted
        {
            get
            {
                return (from obj in this.AndSearchTermsList
                        select "\"" + obj + "\"");
            }
        }

        public virtual IEnumerable<string> ExcludeSearchTermsQuoted
        {
            get
            {
                return (from obj in this.ExcludeSearchTermsList
                        select "\"" + obj + "\"");
            }
        }

        // Passed to MSSQL CONTAINSTABLE function for full text search
        protected string AndSearchTermsFTS
        {
            get
            {
                return string.Join(" AND ", this.AndSearchTermsQuoted);
            }
        }

        protected string ExcludeSearchTermsFTS
        {
            get
            {
                return string.Join(" AND NOT ", this.ExcludeSearchTermsQuoted);
            }
        }

        public virtual string FullTextSearchTerm
        {
            get
            {
                // only include ExcludeSearchTerms if there are AndSearchTerms
                string fullTextSearchTerm = this.AndSearchTermsFTS;
                if (!string.IsNullOrEmpty(fullTextSearchTerm) && !string.IsNullOrEmpty(this.ExcludeSearchTerms))
                    fullTextSearchTerm += " AND NOT " + this.ExcludeSearchTermsFTS;
                return fullTextSearchTerm;
            }
        }
    }
}
