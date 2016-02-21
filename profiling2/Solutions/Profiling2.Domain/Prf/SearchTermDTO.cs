using Profiling2.Domain.Prf.Careers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Profiling2.Domain.Prf
{
    /// <summary>
    /// This class used to populate inputs to PRF_SP_Search_SearchForPerson_NHibernate stored proc.
    /// </summary>
    public class SearchTerm
    {
        protected string Term { get; set; }
        protected bool IsQuoted { get; set; }
        protected IList<string> TermList { get; set; }

        public string MilitaryId { get; set; }
        public string FormattedPartialName { get; set; }
        public string FormattedExactName { get; set; }
        public int? RankId { get; set; }
        public int? RoleId { get; set; }

        public SearchTerm(string searchText, IList<Rank> ranks, IList<Role> roles)
        {
            this.Term = searchText;

            if (!string.IsNullOrEmpty(searchText))
            {
                this.IsQuoted = searchText.Trim().StartsWith("\"") && searchText.Trim().EndsWith("\"");

                this.TermList = (from s in searchText.Split(new char[] { ' ' }) 
                                 where !string.IsNullOrEmpty(s) 
                                 select s.Trim()).ToList<string>();

                if (searchText.IndexOf(' ') < 0)
                    this.MilitaryId = searchText.Replace("-", "").Replace("/", "").Replace(".", "");

                if (!this.IsQuoted)
                    this.FormattedPartialName = string.Join(";", this.TermList);

                this.FormattedExactName = string.Join(";", (from permutation in this.GetPermutations(new List<string>(), this.TermList, string.Empty)
                                                            select (this.IsQuoted ? permutation.Replace("\"", "") : permutation)).ToArray<string>());

                if (ranks != null && roles != null)
                    foreach (string term in this.TermList)
                    {
                        if (ranks.Where(x => x.RankName == term).Any())
                            this.RankId = ranks.Where(x => string.Equals(x.RankName, term, StringComparison.OrdinalIgnoreCase)).First().Id;
                        if (roles.Where(x => x.RoleName == term).Any())
                            this.RoleId = roles.Where(x => string.Equals(x.RoleName, term, StringComparison.OrdinalIgnoreCase)).First().Id;
                    }
            }
        }

        /// <summary>
        /// Retrieves a list of all permutations of phrases derived from a series of words. Copied
        /// </summary>
        /// <param name="allPhrases">The list of phrases already derived. Should be an instantiated empty list for the initial call.</param>
        /// <param name="terms">The series of words from which the phrases are derived.</param>
        /// <param name="current">The current phrase that is being constructed. Should be empty string for the initial call.</param>
        /// <returns>List of all permutations of phrases</returns>
        private IList<string> GetPermutations(IList<string> allPhrases, IList<string> terms, string current)
        {
            List<string> temp;
            //base step
            if (terms.Count < 1)
            {
                allPhrases.Add(current.Trim());
            }
            //recursive step
            else
            {
                for (int i = 0; i < terms.Count; i++)
                {
                    temp = new List<string>(terms);
                    temp.RemoveAt(i);
                    allPhrases = GetPermutations(allPhrases, temp, current + terms[i] + " ");
                }
            }
            return allPhrases;
        }
    }
}
