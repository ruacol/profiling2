using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using log4net;
using NHibernate;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Contracts.Tasks.Screenings;
using Profiling2.Domain.Contracts.Tasks.Sources;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Prf.Sources;
using Profiling2.Domain.Prf.Units;
using Profiling2.Domain.Scr;
using Profiling2.Domain.Scr.PersonEntity;
using SharpArch.NHibernate;

namespace Profiling2.Tasks
{
    public class BackgroundTasks : IBackgroundTasks
    {
        protected static readonly ILog log = LogManager.GetLogger(typeof(BackgroundTasks));

        protected readonly ILuceneTasks luceneTasks;
        protected readonly IOrganizationTasks orgTasks;
        protected readonly IPersonTasks personTasks;
        protected readonly IEventTasks eventTasks;
        protected readonly IRequestTasks requestTasks;
        protected readonly IScreeningTasks screeningTasks;
        protected readonly ISourceTasks sourceTasks;

        public BackgroundTasks(ILuceneTasks luceneTasks,
            IOrganizationTasks orgTasks,
            IPersonTasks personTasks,
            IEventTasks eventTasks,
            IRequestTasks requestTasks,
            IScreeningTasks screeningTasks,
            ISourceTasks sourceTasks)
        {
            this.luceneTasks = luceneTasks;
            this.orgTasks = orgTasks;
            this.personTasks = personTasks;
            this.eventTasks = eventTasks;
            this.requestTasks = requestTasks;
            this.screeningTasks = screeningTasks;
            this.sourceTasks = sourceTasks;
        }

        public void ResetLuceneIndexes()
        {
            this.luceneTasks.DeletePersonIndexes();
            this.CreatePersonIndex();

            this.luceneTasks.DeleteUnitIndexes();
            this.CreateUnitIndex();

            this.luceneTasks.DeleteEventIndexes();
            this.CreateEventIndex();

            this.luceneTasks.DeleteRequestIndexes();
            this.CreateRequestIndex();

            this.luceneTasks.DeleteScreeningResponseIndexes();
            this.CreateScreeningResponseIndex();
        }

        public void CreatePersonIndex()
        {
            ISession session;
            int rowCount = 0;
            using (session = NHibernateSession.GetDefaultSessionFactory().OpenSession())
                rowCount = this.personTasks.GetPersonsCount(session);

            log.Info("Adding " + rowCount + " persons to Lucene index...");
            Stopwatch sw = new Stopwatch();
            sw.Start();

            int pageSize = 1000;
            int personsIndexed = 0;
            for (int page = 1; (page - 1) * pageSize <= rowCount; page++)
            {
                // use a new session for each batch of Person processing - helps put off OutOfMemoryException
                using (session = NHibernateSession.GetDefaultSessionFactory().OpenSession())
                {
                    IList<Person> persons = this.personTasks.GetPersons(session, pageSize, page);

                    this.luceneTasks.CreatePersonIndexes(persons, page == 1);

                    personsIndexed += persons.Count;
                    log.Info("Processed " + personsIndexed + " persons...");
                }
            }

            sw.Stop();
            log.Info("Finished adding " + personsIndexed + " persons to Lucene index, took: " + sw.Elapsed);
        }

        public void CreateUnitIndex()
        {
            using (ISession session = NHibernateSession.GetDefaultSessionFactory().OpenSession())
            {
                IList<Unit> units = this.orgTasks.GetAllUnits(session).Where(x => !x.Archive).ToList();
                log.Info("Adding " + units.Count + " units to Lucene index...");
                this.luceneTasks.CreateUnitIndexes(units);
                log.Info("Finished adding " + units.Count + " units to Lucene index.");
            }
        }

        public void CreateEventIndex()
        {
            using (ISession session = NHibernateSession.GetDefaultSessionFactory().OpenSession())
            {
                IList<Event> events = this.eventTasks.GetAllEvents(session).Where(x => !x.Archive).ToList();
                log.Info("Adding " + events.Count + " events to Lucene index...");
                this.luceneTasks.CreateEventIndexes(events);
                log.Info("Finished adding " + events.Count + " events to Lucene index.");
            }
        }

        public void CreateRequestIndex()
        {
            using (ISession session = NHibernateSession.GetDefaultSessionFactory().OpenSession())
            {
                IList<Request> requests = this.requestTasks.GetAllRequests(session).Where(x => !x.Archive).ToList();
                log.Info("Adding " + requests.Count + " requests to Lucene index...");
                this.luceneTasks.CreateRequestIndexes(requests);
                log.Info("Finished adding " + requests.Count + " requests to Lucene index.");
            }
        }

        public void CreateScreeningResponseIndex()
        {
            using (ISession session = NHibernateSession.GetDefaultSessionFactory().OpenSession())
            {
                IList<ScreeningRequestPersonEntity> responses = this.screeningTasks.GetMostRecentScreeningRequestPersonEntities(session);
                log.Info("Adding " + responses.Count + " screening responses to Lucene index...");
                this.luceneTasks.CreateScreeningResponseIndexes(responses);
                log.Info("Finished adding " + responses.Count + " screening responses to Lucene index.");
            }
        }

        /// <summary>
        /// Indexes newly added sources.  
        /// For each source checks PRF_SourceIndexLog if indexed before; on first run may be long running - on the order of 7 hours.
        /// Idempotent - recommended usage is to be called once a night in order to catch sources imported by DocumentImportConsole
        /// which weren't indexed.
        /// </summary>
        public void UpdateSourceIndex()
        {
            using (IStatelessSession session = NHibernateSession.GetDefaultSessionFactory().OpenStatelessSession())
            {
                IList<SourceDTO> dtos = this.sourceTasks.GetSourcesToIndex(session, 0, 0, null);

                Stopwatch sw = new Stopwatch();
                sw.Start();
                int added = 0;

                // batch process sources - AddSourcesToIndex mainly concerned with holding an IStatelessSession.
                for (int i = 0; i < dtos.Count; i += 1000)
                    added += this.sourceTasks.AddSourcesToIndex(session, dtos.Skip(i).Take(1000).ToList());

                sw.Stop();
                log.Info("Finished adding " + added + " out of " + dtos.Count + " sources to Lucene index, in: " + sw.Elapsed);
            }
        }
    }
}
