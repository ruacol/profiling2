using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Lucene.Net.Documents;
using Profiling2.Domain.Contracts.Search;
using Profiling2.Domain.Extensions;
using Profiling2.Domain.Prf.Units;
using Profiling2.Infrastructure.Search.IndexWriters;

namespace Profiling2.Infrastructure.Search
{
    public class UnitIndexer : BaseIndexer, ILuceneIndexer<UnitIndexer>
    {
        protected readonly ILog log = LogManager.GetLogger(typeof(UnitIndexer));

        public UnitIndexer() 
        {
            this.indexWriter = UnitIndexWriterSingleton.Instance;
        }

        protected override void AddNoCommit<T>(T obj)
        {
            Unit unit = obj as Unit;
            if (unit != null && !unit.Archive && this.indexWriter != null)
            {
                Document doc = new Document();

                doc.Add(new NumericField("Id", Field.Store.YES, true).SetIntValue(unit.Id));

                if (!string.IsNullOrEmpty(unit.UnitName))
                {
                    Field nameField = new Field("Name", unit.UnitName, Field.Store.YES, Field.Index.ANALYZED);
                    nameField.Boost = 2.0f;
                    doc.Add(nameField);
                }

                // aliases make multi-values for Name field
                foreach (UnitAlias alias in unit.UnitAliases.Where(x => !string.IsNullOrEmpty(x.UnitName) && !x.Archive))
                {
                    Field aliasField = new Field("Name", alias.UnitName, Field.Store.YES, Field.Index.ANALYZED);
                    aliasField.Boost = 1.5f;
                    doc.Add(aliasField);
                }

                // add name changes
                foreach (UnitHierarchy uh in unit.GetParentChangedNameUnitHierarchiesRecursive(new List<UnitHierarchy>()).Where(x => x.ParentUnit != null))
                {
                    Field field = new Field("ParentNameChange", uh.ParentUnit.UnitName, Field.Store.YES, Field.Index.ANALYZED);
                    field.Boost = 0.5f;
                    doc.Add(field);
                }

                foreach (UnitHierarchy uh in unit.GetChildChangedNameUnitHierarchiesRecursive(new List<UnitHierarchy>()).Where(x => x.Unit != null))
                {
                    Field field = new Field("ChildNameChange", uh.Unit.UnitName, Field.Store.YES, Field.Index.ANALYZED);
                    field.Boost = 0.5f;
                    doc.Add(field);
                }

                if (!string.IsNullOrEmpty(unit.BackgroundInformation))
                {
                    doc.Add(new Field("BackgroundInformation", unit.BackgroundInformation, Field.Store.YES, Field.Index.ANALYZED));
                }

                if (unit.Organization != null)
                {
                    doc.Add(new Field("Organization", unit.Organization.OrgShortName, Field.Store.YES, Field.Index.ANALYZED));
                }

                if (unit.HasStartDate())
                {
                    // minimal date for searching
                    DateTime? start = unit.GetStartDateTime();
                    if (!start.HasValue)
                        log.Error("Encountered invalid start date for UnitID=" + unit.Id + ": " + unit.GetStartDateString());
                    doc.Add(new Field("StartDateDisplay", unit.GetStartDateString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                }

                if (unit.HasEndDate())
                {
                    // maximal date for searching
                    DateTime? end = unit.GetEndDateTime();
                    if (!end.HasValue)
                        log.Error("Encountered invalid end date for UnitID=" + unit.Id + ": " + unit.GetEndDateString());
                    doc.Add(new Field("EndDateDisplay", unit.GetEndDateString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                }

                this.indexWriter.AddDocument(doc);
            }
        }
    }
}
