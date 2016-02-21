using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using log4net;
using NHibernate;
using NHibernate.Exceptions;
using Profiling2.Domain;
using Profiling2.Domain.Contracts.Queries;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Contracts.Tasks.Sources;
using Profiling2.Domain.DTO;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Organizations;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Prf.Sources;
using Profiling2.Infrastructure.Util;
using SharpArch.NHibernate;
using SharpArch.NHibernate.Contracts.Repositories;
using StackExchange.Profiling;
using TikaOnDotNet;

namespace Profiling2.Tasks.Sources
{
    public class SourceTasks : ISourceTasks
    {
        private readonly ILog log = LogManager.GetLogger(typeof(SourceTasks));
        private readonly INHibernateRepository<Source> sourceRepository;
        private readonly INHibernateRepository<Reliability> reliabilityRepo;
        private readonly INHibernateRepository<Language> languageRepo;
        private readonly INHibernateRepository<Person> personRepo;
        private readonly INHibernateRepository<Organization> orgRepo;
        private readonly INHibernateRepository<JhroCase> jhroCaseRepo;
        private readonly INHibernateRepository<SourceIndexLog> sourceIndexLogRepo;
        private readonly ISourceQueries sourceQueries;
        private readonly ISourceDataTablesQuery sourceDtQuery;
        private readonly ISourceLogQueries sourceLogQueries;
        private readonly IJhroCaseQueries jhroCaseQueries;
        private readonly IEventTasks eventTasks;
        private readonly ILuceneTasks luceneTasks;
        private readonly ISourcePermissionTasks sourcePermissionTasks;

        public SourceTasks(INHibernateRepository<Source> sourceRepository, 
            INHibernateRepository<Reliability> reliabilityRepo,
            INHibernateRepository<Language> languageRepo,
            INHibernateRepository<Person> personRepo,
            INHibernateRepository<Organization> orgRepo,
            INHibernateRepository<JhroCase> jhroCaseRepo,
            INHibernateRepository<SourceIndexLog> sourceIndexLogRepo,
            INHibernateRepository<SourceAuthor> sourceAuthorRepo,
            ISourceQueries sourceQueries,
            ISourceDataTablesQuery sourceDtQuery,
            ISourceLogQueries sourceLogQueries,
            IJhroCaseQueries jhroCaseQueries,
            IEventTasks eventTasks,
            ILuceneTasks luceneTasks,
            ISourcePermissionTasks sourcePermissionTasks)
        {
            this.sourceRepository = sourceRepository;
            this.reliabilityRepo = reliabilityRepo;
            this.languageRepo = languageRepo;
            this.personRepo = personRepo;
            this.orgRepo = orgRepo;
            this.jhroCaseRepo = jhroCaseRepo;
            this.sourceIndexLogRepo = sourceIndexLogRepo;
            this.sourceQueries = sourceQueries;
            this.sourceDtQuery = sourceDtQuery;
            this.sourceLogQueries = sourceLogQueries;
            this.jhroCaseQueries = jhroCaseQueries;
            this.eventTasks = eventTasks;
            this.luceneTasks = luceneTasks;
            this.sourcePermissionTasks = sourcePermissionTasks;
        }

        public Source GetSource(int sourceId)
        {
            return this.sourceRepository.Get(sourceId);
        }

        public Source GetSource(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                IDictionary<string, object> criteria = new Dictionary<string, object>();
                criteria.Add("SourceName", name);
                IList<Source> results = this.sourceRepository.FindAll(criteria);
                if (results != null && results.Any())
                    return results.First();
            }
            return null;
        }

        public IList<SourceDTO> GetSources(string hash)
        {
            if (!string.IsNullOrEmpty(hash))
                return this.sourceQueries.GetSourceDTOsByHash(hash);
            return null;
        }

        public Source GetSource(IStatelessSession session, int id)
        {
            return this.sourceQueries.GetSource(session, id);
        }

        public IList<string> GetSourceAuthors(IStatelessSession session, int id)
        {
            return this.sourceQueries.GetSourceAuthors(session, id);
        }

        public IList<string> GetSourceAuthors(int id)
        {
            return this.sourceQueries.GetSourceAuthors(id);
        }

        public IList<string> GetSourceOwners(IStatelessSession session, int id)
        {
            return this.sourceQueries.GetSourceOwners(session, id);
        }

        public IList<string> GetSourceOwners(int id)
        {
            return this.sourceQueries.GetSourceOwners(id);
        }

        public SourceDTO GetSourceDTO(int id)
        {
            return this.sourceQueries.GetSourceDTO(id);
        }

        public Int64 GetSourceSize(int sourceId)
        {
            SourceDTO dto = this.GetSourceDTO(sourceId);
            if (dto != null)
                return dto.FileSize;
            return 0;
        }

        public IList<Source> GetSourcesByFilename(string term)
        {
            return this.sourceQueries.GetSourcesByName(term);
        }

        public IList<SourceDTO> GetAllSourceDTOs(IStatelessSession session, bool excludeBinaryIndexedSources, bool excludeSourceLogged)
        {
            return this.sourceQueries.GetAllSourceDTOs(session, excludeBinaryIndexedSources, excludeSourceLogged);
        }

        public IList<SourceDTO> GetAllSourceDTOs(bool excludeBinaryIndexedSources, bool excludeSourceLogged)
        {
            return this.sourceQueries.GetAllSourceDTOs(null, excludeBinaryIndexedSources, excludeSourceLogged);
        }

        public Stream GetSourceData(int sourceId, bool hasOcrText)
        {
            return this.sourceQueries.GetSourceData(sourceId, hasOcrText);
        }

        public Source SaveSource(Source s)
        {
            return this.SaveSource(null, s);
        }

        public Source SaveSource(ISession session, Source s)
        {
            // check if we can automatically add a SourceOwningEntity
            if (!string.IsNullOrEmpty(s.SourcePath))
            {
                IList<SourceOwningEntity> owners = this.sourcePermissionTasks.GetSourceOwningEntities(session, s.GetSourcePathOnly());
                if (owners != null && owners.Any())
                {
                    s.AddSourceOwningEntity(owners.First());
                }
            }

            this.sourceQueries.SaveSource(session, s);

            return s;
        }

        /// <summary>
        /// Use this method when indexing a Source instead of calling luceneTasks method directly.  Populates some fields
        /// manually in order to avoid using a standard NHibernate session.
        /// </summary>
        public void IndexSource(Source s, string uploadedByUserId, IList<string> authors, IList<string> owners, string jhroCaseNumber, long fileSize)
        {
            if (s != null)
            {
                // when indexing, the several *HorsSession fields need to be populated
                s.SetUploadedByHorsSession(uploadedByUserId);
                s.SetAuthorsHorsSession(authors);
                s.SetOwnersHorsSession(owners);
                s.SetJhroCaseNumberHorsSession(jhroCaseNumber);
                s.SetFileSizeHorsSession(fileSize);
                this.luceneTasks.UpdateSource(s);
            }
        }

        public void IndexSourceQueueable(int sourceId, string uploadedBy, IList<string> authors, IList<string> owners, string caseNumber, long fileSize)
        {
            using (IStatelessSession session = NHibernateSession.GetDefaultSessionFactory().OpenStatelessSession())
            {
                this.IndexSource(this.GetSource(session, sourceId),
                    uploadedBy, authors, owners, caseNumber, fileSize);
            }
        }

        public void DeleteSource(int sourceId)
        {
            this.sourceQueries.DeleteSource(sourceId);
        }

        public int GetSearchTotal(bool canAccessRestricted, string searchName, string searchExt, string searchText, DateTime? start, DateTime? end, string authorSearchText)
        {
            return this.sourceDtQuery.GetSearchTotal(canAccessRestricted, searchName, searchExt, searchText, start, end, authorSearchText);
        }

        public IList<SourceSearchResultDTO> GetPaginatedResults(bool canAccessRestricted, int iDisplayStart, int iDisplayLength,
            string searchName, string searchExt, string searchText, DateTime? start, DateTime? end, IList<int> adminSourceSearchIds,
            int iSortingCols, List<int> iSortCol, List<string> sSortDir,
            int userId, int? personId, int? eventId, string authorSearchText)
        {
            return this.sourceDtQuery.GetPaginatedResults(canAccessRestricted, iDisplayStart, iDisplayLength, 
                searchName, searchExt, searchText, start, end, adminSourceSearchIds,
                iSortingCols, iSortCol, sSortDir,
                userId, personId, eventId, authorSearchText);
        }

        public IList<Reliability> GetReliabilities()
        {
            return this.reliabilityRepo.GetAll();
        }

        public Reliability GetReliability(int id)
        {
            return this.reliabilityRepo.Get(id);
        }

        public IList<Language> GetLanguages()
        {
            return this.languageRepo.GetAll();
        }

        public Language GetLanguage(int languageId)
        {
            return this.languageRepo.Get(languageId);
        }

        public IList<SourceFolderDTO> GetFolderDTOs(string prefix)
        {
            var profiler = MiniProfiler.Current;
            // case insensitive dictionary so that \\profiling\profiling is equivalent to \\profiling\Profiling
            IDictionary<string, SourceFolderDTO> folders = new Dictionary<string, SourceFolderDTO>(StringComparer.InvariantCultureIgnoreCase);
            IEnumerable<string> paths;
            
            using (profiler.Step("GetSourcePaths()"))
                paths = this.sourceQueries.GetSourcePaths(prefix).Where(x => !string.IsNullOrEmpty(x));

            using (profiler.Step("Looping through paths"))
            {
                foreach (string path in paths)
                {
                    // get folder name
                    int i = path.LastIndexOf(Path.DirectorySeparatorChar);
                    if (i > 0)
                    {
                        string folder = path.Substring(0, i);

                        // ensure unique
                        if (!folders.ContainsKey(folder))
                            folders.Add(folder, new SourceFolderDTO(folder));

                        // create folder DTOs for each parent folder - in order not to miss those folders that have no files
                        while (folder.IndexOf(Path.DirectorySeparatorChar) >= 0 && folder.Length > 2)
                        {
                            // get parent folder
                            folder = folder.Substring(0, folder.LastIndexOf(Path.DirectorySeparatorChar));

                            if (!folders.ContainsKey(folder) && folder.Length > 2)
                                folders.Add(folder, new SourceFolderDTO(folder));
                        }
                    }
                }
            }

            return folders.Values.OrderBy(x => x.Folder).ToList();
        }

        public IDictionary<string, int> GetFolderCounts(int depth)
        {
            IDictionary<string, int> counts = new Dictionary<string, int>();

            foreach (string s in this.sourceQueries.GetSourcePaths(null).Where(x => !string.IsNullOrEmpty(x)))  // includes file names
            {
                // get folder name - not using Path.GetDirectoryName because of max char length limitation
                int i = IndexOfNth(s, '\\', depth);
                string folder = i >= 0 ? s.Substring(0, i) : string.Empty;
                folder = folder.ToLower();

                if (!counts.ContainsKey(folder))
                    counts[folder] = 0;

                counts[folder]++;
            }

            return counts;
        }

        private static int IndexOfNth(string str, char c, int n)
        {
            int s = -1;

            if (!string.IsNullOrEmpty(str))
            {
                for (int i = 0; i < n; i++)
                {
                    int x = str.IndexOf(c, s + 1);

                    if (x == -1) break;

                    s = x;
                }
            }

            return s;
        }

        public void ArchiveSourcePath(string prefix)
        {
            this.sourceQueries.ArchiveSourcePathPrefix(prefix);

            // update index
            foreach (LuceneSearchResult result in this.luceneTasks.SourcePathPrefixSearch(prefix, int.MaxValue, true, true, null, null, null, true))
            {
                string idStr = null;
                if (result.FieldValues.ContainsKey("Id"))
                    foreach (string s in result.FieldValues["Id"])
                        idStr = s;

                int id = 0;
                if (int.TryParse(idStr, out id))
                    if (id > 0)
                        this.luceneTasks.DeleteSource(id);
            }
        }

        public IList<SourceDTO> GetUnindexableMediaSources()
        {
            return this.GetAllSourceDTOs(false, false)
                .Where(x => MIMEAssistant.GetMIMEType(x.SourceName).StartsWith("image")
                    || MIMEAssistant.GetMIMEType(x.SourceName).StartsWith("audio")
                    || MIMEAssistant.GetMIMEType(x.SourceName).StartsWith("video")
                    || Source.IGNORED_FILE_EXTENSIONS.Contains(x.FileExtension))
                .OrderBy(x => x.SourceID)
                .ToList();
        }

        public IList<SourceDTO> GetSourcesToIndex(IStatelessSession session, int startSourceId, int num, string prefix)
        {
            // get list of sources to index
            IList<SourceDTO> dtos = this.GetAllSourceDTOs(session, true, false);

            // filter for path prefix
            if (!string.IsNullOrEmpty(prefix))
                dtos = dtos.Where(x => !string.IsNullOrEmpty(x.SourcePath) && x.SourcePath.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase)).ToList();

            // filtering out non-text media files
            dtos = dtos.Where(x => !MIMEAssistant.GetMIMEType(x.SourceName).StartsWith("image")
                    && !MIMEAssistant.GetMIMEType(x.SourceName).StartsWith("audio")
                    && !MIMEAssistant.GetMIMEType(x.SourceName).StartsWith("video"))
                .Where(x => !Source.IGNORED_FILE_EXTENSIONS.Contains(x.FileExtension))
                .Where(x => x.SourceID > startSourceId)
                .OrderBy(x => x.SourceID)
                .ToList();

            log.Info("Found " + dtos.Count + " sources to index, starting from SourceID=" + startSourceId + " with prefix=" + prefix + "." 
                + (num > 0 ? " Taking " + num + "." : string.Empty));

            if (num > 0)
                dtos = dtos.Take(num).ToList();

            return dtos;
        }

        public int AddSourcesToIndex(IStatelessSession statelessSession, IList<SourceDTO> dtos)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            int added = 0;
            using (ISession session = NHibernateSession.GetDefaultSessionFactory().OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    foreach (SourceDTO dto in dtos)
                    {
                        try
                        {
                            // don't re-index a source that's already indexed, or failed its last index attempt
                            // TODO pass a parameter allowing re-indexing of sources that failed indexing
                            if (this.GetSourceIndexLog(session, dto.SourceID) == null)
                            {
                                Source s = this.GetSource(statelessSession, dto.SourceID);
                                this.IndexSource(s, dto.UploadedByUserID,
                                    this.GetSourceAuthors(statelessSession, dto.SourceID),
                                    this.GetSourceOwners(statelessSession, dto.SourceID),
                                    dto.JhroCaseNumber, dto.FileSize);
                                added++;

                                // flag this source as successfully indexed
                                this.sourceLogQueries.SaveSourceIndexLog(session, new SourceIndexLog()
                                {
                                    SourceID = dto.SourceID,
                                    DateTime = DateTime.Now
                                });
                            }
                        }
                        catch (TextExtractionException e)
                        {
                            this.HandleSourceIndexException(session, dto.SourceID, e);
                        }
                        catch (GenericADOException e)
                        {
                            this.HandleSourceIndexException(session, dto.SourceID, e);
                        }
                        catch (OutOfMemoryException e)
                        {
                            this.HandleSourceIndexException(session, dto.SourceID, e);
                        }
                        catch (Exception e)
                        {
                            this.HandleSourceIndexException(session, dto.SourceID, e);
                        }
                    }

                    transaction.Commit();
                }
            }

            sw.Stop();
            log.Info("Added " + added + " sources out of " + dtos.Count + " to Lucene index, took: " + sw.Elapsed);

            return added;
        }

        protected void HandleSourceIndexException(ISession session, int sourceId, Exception e)
        {
            this.sourceLogQueries.SaveSourceIndexLog(session, new SourceIndexLog()
            {
                SourceID = sourceId,
                LogSummary = e.Message,
                Log = e.ToString(),
                DateTime = DateTime.Now
            });
        }

        public IList<SourceIndexLog> GetSourceIndexLogsWithErrors()
        {
            return this.sourceLogQueries.GetSourceIndexLogsWithErrors();
        }

        public SourceIndexLog GetSourceIndexLog(ISession session, int sourceId)
        {
            return this.sourceLogQueries.GetSourceIndexLog(session, sourceId);
        }

        public void DeleteAllSourceIndexLogs()
        {
            log.Info("Deleting all SourceIndexLogs.");
            foreach (SourceIndexLog sil in this.sourceIndexLogRepo.GetAll())
                this.sourceIndexLogRepo.Delete(sil);
        }

        public int CountSourceIndexLogs()
        {
            return this.sourceLogQueries.CountSourceIndexLogs();
        }

        public int CountSourceIndexLogErrors()
        {
            return this.sourceLogQueries.CountSourceIndexLogErrors();
        }

        public IList<SourceLog> GetSourceLogsWithErrors()
        {
            return this.sourceLogQueries.GetSourceLogsWithErrors();
        }

        public IList<SourceDTO> GetSourceDTOsWithPasswordErrors()
        {
            return this.sourceQueries.GetSourceDTOsWithPasswordErrors();
        }

        public int CountSourceLogs()
        {
            return this.sourceLogQueries.CountSourceLogs();
        }

        public int CountSourceLogsWithPasswordErrors()
        {
            return this.sourceLogQueries.CountSourceLogsWithPasswordErrors();
        }

        public JhroCase GetJhroCase(int id)
        {
            return this.jhroCaseRepo.Get(id);
        }

        public JhroCase GetJhroCase(string caseNumber)
        {
            IDictionary<string, object> criteria = new Dictionary<string, object>();
            criteria.Add("CaseNumber", caseNumber);
            return this.jhroCaseRepo.FindOne(criteria);
        }

        public JhroCase GetJhroCase(ISession session, string caseNumber)
        {
            return this.jhroCaseQueries.GetJhroCase(session, caseNumber);
        }

        public IList<JhroCase> GetJhroCases()
        {
            return this.jhroCaseRepo.GetAll().OrderBy(x => x.CaseNumber).ToList();
        }

        public IList<JhroCase> SearchJhroCases(string term)
        {
            return this.jhroCaseQueries.SearchJhroCases(term).OrderBy(x => x.CaseNumber).ToList();
        }

        public JhroCase SaveJhroCase(JhroCase jhroCase)
        {
            if (jhroCase != null)
                return this.jhroCaseRepo.SaveOrUpdate(jhroCase);
            return jhroCase;
        }

        public JhroCase SaveJhroCase(ISession session, JhroCase jhroCase)
        {
            this.jhroCaseQueries.SaveJhroCase(session, jhroCase);
            return jhroCase;
        }

        public void PopulateSourceOwners()
        {
            IList<SourceOwningEntity> entities = this.sourcePermissionTasks.GetAllSourceOwningEntities().Where(x => !string.IsNullOrEmpty(x.SourcePathPrefix)).ToList();

            IList<SourceDTO> dtos = this.GetAllSourceDTOs(false, false).Where(x => !string.IsNullOrEmpty(x.SourcePath)).ToList();
            log.Debug("Matching " + dtos.Count + " sources with their owners...");
            foreach (SourceDTO dto in dtos)
                foreach (SourceOwningEntity e in entities)
                    if (dto.SourcePath.StartsWith(e.SourcePathPrefix, System.StringComparison.InvariantCultureIgnoreCase))
                        this.sourceQueries.InsertSourceOwner(dto.SourceID, e.Id);
            log.Debug("Finished populating sources with their owners.");
        }
    }
}
