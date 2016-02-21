using System;
using System.Linq;
using log4net;
using Lucene.Net.Documents;
using Profiling2.Domain.Contracts.Search;
using Profiling2.Domain.Extensions;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Sources;
using Profiling2.Infrastructure.Search.IndexWriters;

namespace Profiling2.Infrastructure.Search
{
    public class EventIndexer : BaseIndexer, ILuceneIndexer<EventIndexer>
    {
        protected readonly ILog log = LogManager.GetLogger(typeof(EventIndexer));

        public EventIndexer() 
        {
            this.indexWriter = EventIndexWriterSingleton.Instance;
        }

        protected override void AddNoCommit<T>(T obj)
        {
            Event ev = obj as Event;
            if (ev != null && !ev.Archive && this.indexWriter != null)
            {
                Document doc = new Document();

                doc.Add(new NumericField("Id", Field.Store.YES, true).SetIntValue(ev.Id));

                // deprecated field
                if (!string.IsNullOrEmpty(ev.EventName))
                    doc.Add(new Field("Name", ev.EventName, Field.Store.YES, Field.Index.ANALYZED));

                if (ev.Violations != null && ev.Violations.Any())
                    foreach (Violation v in ev.Violations)
                        doc.Add(new Field("Violation", v.Name, Field.Store.YES, Field.Index.ANALYZED));

                if (!string.IsNullOrEmpty(ev.NarrativeEn))
                    doc.Add(new Field("NarrativeEn", ev.NarrativeEn, Field.Store.YES, Field.Index.ANALYZED));

                if (!string.IsNullOrEmpty(ev.NarrativeFr))
                    doc.Add(new Field("NarrativeFr", ev.NarrativeFr, Field.Store.YES, Field.Index.ANALYZED));

                if (ev.Location != null)
                    doc.Add(new Field("Location", ev.Location.ToString(), Field.Store.YES, Field.Index.ANALYZED));

                if (!string.IsNullOrEmpty(ev.Notes))
                {
                    Field notesField = new Field("Notes", ev.Notes, Field.Store.YES, Field.Index.ANALYZED);
                    notesField.Boost = 0.9f;
                    doc.Add(notesField);
                }

                if (ev.HasStartDate())
                {
                    // minimal date for searching
                    DateTime? start = ev.GetStartDateTime();
                    if (start.HasValue)
                        doc.Add(new NumericField("StartDateSearch", Field.Store.YES, true).SetLongValue(start.Value.Ticks));
                    else
                        log.Error("Encountered invalid start date for EventID=" + ev.Id + ": " + ev.GetStartDateString());
                    doc.Add(new Field("StartDateDisplay", ev.GetStartDateString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                }

                if (ev.HasEndDate())
                {
                    // maximal date for searching
                    DateTime? end = ev.GetEndDateTime();
                    if (end.HasValue)
                        doc.Add(new NumericField("EndDateSearch", Field.Store.YES, true).SetLongValue(end.Value.Ticks));
                    else
                        log.Error("Encountered invalid end date for EventID=" + ev.Id + ": " + ev.GetEndDateString());
                    doc.Add(new Field("EndDateDisplay", ev.GetEndDateString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                }

                if (ev.JhroCases != null && ev.JhroCases.Any())
                {
                    foreach (JhroCase jc in ev.JhroCases)
                    {
                        Field caseField = new Field("JhroCaseNumber", jc.CaseNumber, Field.Store.YES, Field.Index.ANALYZED);
                        caseField.Boost = 1.5f;
                        doc.Add(caseField);
                    }
                }

                this.indexWriter.AddDocument(doc);
            }
        }
    }
}
