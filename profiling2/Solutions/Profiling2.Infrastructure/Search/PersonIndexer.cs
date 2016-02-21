using System.Linq;
using System.Text.RegularExpressions;
using Lucene.Net.Documents;
using Profiling2.Domain.Contracts.Search;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Infrastructure.Search.IndexWriters;

namespace Profiling2.Infrastructure.Search
{
    public class PersonIndexer : BaseIndexer, ILuceneIndexer<PersonIndexer>
    {
        public PersonIndexer()
        {
            this.indexWriter = PersonIndexWriterSingleton.Instance;
        }

        protected override void AddNoCommit<T>(T obj)
        {
            Person person = obj as Person;
            if (person != null && !person.Archive && this.indexWriter != null)
            {
                Document doc = new Document();

                doc.Add(new NumericField("Id", Field.Store.YES, true).SetIntValue(person.Id));

                if (!string.IsNullOrEmpty(person.MilitaryIDNumber))
                {
                    // tokenise MilitaryIDNumber field here
                    string[] parts = person.MilitaryIDNumber.Split(new char[] { ' ', ',' });

                    // add tokens as multiple values to the field 'MilitaryIDNumber'
                    foreach (string part in parts.Where(x => !string.IsNullOrEmpty(x) && !string.Equals("/", x)))
                        doc.Add(new Field("MilitaryIDNumber", part, Field.Store.YES, Field.Index.ANALYZED));
                }

                if (person.CurrentCareer != null && !string.IsNullOrEmpty(person.CurrentCareer.RankOrganizationLocationSummary))
                {
                    // strip HTML tags
                    string text = Regex.Replace(person.CurrentCareer.RankOrganizationLocationSummary, @"<[^>]*>", string.Empty);
                    
                    doc.Add(new Field("Rank", text, Field.Store.YES, Field.Index.ANALYZED));
                }

                if (person.CurrentCareer != null && !string.IsNullOrEmpty(person.CurrentCareer.FunctionUnitSummary))
                {
                    doc.Add(new Field("Function", person.CurrentCareer.FunctionUnitSummary, Field.Store.YES, Field.Index.ANALYZED));
                }

                if (!string.IsNullOrEmpty(person.Name))
                {
                    Field nameField = new Field("Name", person.Name, Field.Store.YES, Field.Index.ANALYZED);

                    // we boost names just in case they appear in the function field of other Documents (due to the old Career.Job field containing free text)
                    nameField.Boost = 1.1f;

                    doc.Add(nameField);
                }

                // aliases make multi-values for Name field
                foreach (PersonAlias alias in person.PersonAliases.Where(x => !string.IsNullOrEmpty(x.Name)))
                {
                    Field aliasField = new Field("Name", alias.Name, Field.Store.YES, Field.Index.ANALYZED);
                    aliasField.Boost = 1.05f;
                    doc.Add(aliasField);
                }

                doc.Add(new Field("IsRestrictedProfile", person.IsRestrictedProfile ? "1" : "0", Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS));

                this.indexWriter.AddDocument(doc);
            }
        }
    }
}
