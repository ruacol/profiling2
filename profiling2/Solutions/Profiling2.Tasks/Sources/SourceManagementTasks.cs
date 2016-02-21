using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using log4net;
using Profiling2.Domain.Contracts.Queries;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Contracts.Tasks.Sources;
using Profiling2.Domain.DTO;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Organizations;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Prf.Sources;

namespace Profiling2.Tasks.Sources
{
    public class SourceManagementTasks : ISourceManagementTasks
    {
        protected readonly ILog log = LogManager.GetLogger(typeof(SourceManagementTasks));
        protected readonly IObjectSourceDuplicatesQuery objectSourceDuplicatesQuery;
        protected readonly ISourceTasks sourceTasks;
        protected readonly ISourceAttachmentTasks sourceAttachmentTasks;
        protected readonly IPersonTasks personTasks;
        protected readonly IEventTasks eventTasks;
        protected readonly IOrganizationTasks orgTasks;
        protected readonly ISourceQueries sourceQueries;
        protected readonly ILuceneTasks luceneTasks;

        public SourceManagementTasks(IObjectSourceDuplicatesQuery objectSourceDuplicatesQuery, 
            ISourceTasks sourceTasks,
            ISourceAttachmentTasks sourceAttachmentTasks,
            IPersonTasks personTasks,
            IEventTasks eventTasks,
            IOrganizationTasks orgTasks,
            ISourceQueries sourceQueries,
            ILuceneTasks luceneTasks)
        {
            this.objectSourceDuplicatesQuery = objectSourceDuplicatesQuery;
            this.sourceTasks = sourceTasks;
            this.sourceAttachmentTasks = sourceAttachmentTasks;
            this.personTasks = personTasks;
            this.eventTasks = eventTasks;
            this.orgTasks = orgTasks;
            this.sourceQueries = sourceQueries;
            this.luceneTasks = luceneTasks;
        }

        public IList<object[]> DuplicatesByHash(int maxResults)
        {
            return this.sourceQueries.GetDuplicatesByHash(maxResults);
        }

        public IList<object[]> DuplicatesByName()
        {
            return this.sourceQueries.DuplicatesByName(3);
        }

        public IList<SourceDTO> DuplicatesByIgnored()
        {
            return this.sourceQueries.DuplicatesByIgnored(Source.IGNORED_FILE_EXTENSIONS);
        }

        public IDictionary<SourceDTO, IList<SourceDTO>> DuplicatesByNameOf(string name)
        {
            // sort duplicates in order of import time
            IList<SourceDTO> duplicates = this.sourceQueries.DuplicatesByNameOf(name);

            // group duplicates by file size, keyed by the first one that was imported
            IDictionary<SourceDTO, IList<SourceDTO>> mergeTargets = new Dictionary<SourceDTO, IList<SourceDTO>>();
            foreach (SourceDTO source in duplicates)
            {
                SourceDTO key = null;
                foreach (KeyValuePair<SourceDTO, IList<SourceDTO>> kvp in mergeTargets)
                {
                    if (source.FileSize == kvp.Key.FileSize)
                    {
                        key = kvp.Key;
                        break;
                    }
                }

                if (key != null)
                    mergeTargets[key].Add(source);
                else
                    mergeTargets[source] = new List<SourceDTO>();
            }

            return mergeTargets;
        }

        public void MergeDuplicatesByNameOf(string name)
        {
            foreach (KeyValuePair<SourceDTO, IList<SourceDTO>> kvp in this.DuplicatesByNameOf(name))
                if (kvp.Value != null && kvp.Value.Count() > 0)
                    foreach (SourceDTO s in kvp.Value)
                        this.sourceQueries.ChangeAttachmentsToAnotherSource(s.SourceID, kvp.Key.SourceID);
        }

        protected IDictionary<string, IList<SourceDTO>> OrganizeDuplicatesByHashOf(string hash)
        {
            IDictionary<string, IList<SourceDTO>> dict = new Dictionary<string, IList<SourceDTO>>();
            
            // organize duplicates by SourcePath
            foreach (SourceDTO s in this.sourceTasks.GetSources(hash))
            {
                if (!dict.ContainsKey(s.SourcePath))
                    dict.Add(s.SourcePath, new List<SourceDTO>());
                dict[s.SourcePath].Add(s);
            }
            
            return dict;
        }

        public void CleanHashDuplicates()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            IList<object[]> list = this.DuplicatesByHash(500);
            for (int i = 0; i < list.Count; i++)
            {
                object[] row = list[i];
                log.Debug((i + 1).ToString() + " of " + list.Count + "...");
                this.CleanDuplicatesByHashOf(BitConverter.ToString(row[0] as byte[]).Replace("-", ""));
            }

            sw.Stop();
            log.Info("Took " + sw.Elapsed + ".");
        }

        // unfortunately this must run as a transaction for the merging to persist.
        public void CleanDuplicatesByHashOf(string hash)
        {
            // holds single Source per SourcePath
            IDictionary<string, SourceDTO> dict = new Dictionary<string, SourceDTO>();

            IDictionary<string, IList<SourceDTO>> dups = this.OrganizeDuplicatesByHashOf(hash);
            log.Debug("Found " + dups.Values.Sum(x => x.Count()) + " duplicates by hash " + hash + " with " + dups.Keys.Count + " unique SourcePaths.");

            // sanity check duplicate list, in case of collisions (i.e. different sources generating the same hash)
            if (dups.Values.Aggregate(new List<SourceDTO>(), (sum, i) => sum.Concat(i).ToList()).Select(x => x.FileExtension.ToLower().Trim()).Distinct().Count() > 1)
            {
                // return if the file extensions of the duplicates don't match
                log.Debug("Duplicates' file extensions differ, not merging.");
                return;
            }

            // leave behind only one duplicate per SourcePath
            foreach (KeyValuePair<string, IList<SourceDTO>> kvp in dups)
            {
                // choose a Source to keep for this SourcePath; prioritise those with FileDateTimeStamp
                IEnumerable<SourceDTO> candidatesToKeep = kvp.Value.Where(x => x.FileDateTimeStamp != null).OrderBy(x => x.FileDateTimeStamp);
                if (candidatesToKeep != null && candidatesToKeep.Any())
                {
                    dict.Add(kvp.Key, candidatesToKeep.First());
                }
                else
                {
                    dict.Add(kvp.Key, kvp.Value.First());
                }

                // for the rest...
                foreach (SourceDTO toDelete in kvp.Value.Where(x => x.SourceID != dict[kvp.Key].SourceID))
                {
                    // merge attributes with the SourceDTO we're keeping...
                    if (toDelete.IsRestricted)
                        dict[kvp.Key].IsRestricted = toDelete.IsRestricted;
                    if (toDelete.IsReadOnly)
                        dict[kvp.Key].IsReadOnly = toDelete.IsReadOnly;
                    if (string.IsNullOrEmpty(dict[kvp.Key].JhroCaseNumber))
                        dict[kvp.Key].JhroCaseNumber = toDelete.JhroCaseNumber;
                    if (string.IsNullOrEmpty(dict[kvp.Key].FileExtension))
                        dict[kvp.Key].FileExtension = toDelete.FileExtension;
                    if (string.IsNullOrEmpty(dict[kvp.Key].Notes))
                        dict[kvp.Key].Notes = toDelete.Notes;

                    // merge their attachments and archive...
                    this.sourceQueries.ChangeAttachmentsToAnotherSource(toDelete.SourceID, dict[kvp.Key].SourceID);
                    log.Debug("Merged SourceID=" + toDelete.SourceID + " into SourceID=" + dict[kvp.Key].SourceID);

                    // then delete them.
                    this.sourceTasks.DeleteSource(toDelete.SourceID);
                    this.luceneTasks.DeleteSource(toDelete.SourceID);
                    log.Debug("Deleted SourceID=" + toDelete.SourceID);
                }
            }

            // out of remaining duplicates with unique SourcePaths, choose one to keep;
            SourceDTO toKeep;
            IEnumerable<SourceDTO> sourcesWithAttachments = dict.Values.Where(x => x.FileDateTimeStamp != null).OrderBy(x => x.FileDateTimeStamp);
            if (sourcesWithAttachments != null && sourcesWithAttachments.Any())
            {
                toKeep = sourcesWithAttachments.First();
            }
            else
            {
                toKeep = dict.Values.First();
            }
            log.Debug("Keeping SourceID=" + toKeep.SourceID);

            // as for the rest, merge their attributes/attachments then archive them.
            foreach (SourceDTO toArchive in dict.Values.Where(x => x.SourceID != toKeep.SourceID))
            {
                // merge attributes
                if (toArchive.IsRestricted)
                    toKeep.IsRestricted = toArchive.IsRestricted;
                if (toArchive.IsReadOnly)
                    toKeep.IsReadOnly = toArchive.IsReadOnly;
                if (string.IsNullOrEmpty(toKeep.JhroCaseNumber))
                    toKeep.JhroCaseNumber = toArchive.JhroCaseNumber;
                if (string.IsNullOrEmpty(toKeep.FileExtension))
                    toKeep.FileExtension = toArchive.FileExtension;
                if (string.IsNullOrEmpty(toKeep.Notes))
                    toKeep.Notes = toArchive.Notes;

                // merge attachments and archive
                this.sourceQueries.ChangeAttachmentsToAnotherSource(toArchive.SourceID, toKeep.SourceID);
                this.luceneTasks.DeleteSource(toArchive.SourceID);
                log.Debug("Merged SourceID=" + toArchive.SourceID + " into SourceID=" + toKeep.SourceID);
            }

            // update merged/toKeep SourceDTO
            Source s = this.sourceTasks.GetSource(toKeep.SourceID);
            s.IsRestricted = toKeep.IsRestricted;
            s.IsReadOnly = toKeep.IsReadOnly;
            s.JhroCase = this.sourceTasks.GetJhroCase(toKeep.JhroCaseNumber);
            s.FileExtension = toKeep.FileExtension;
            s.Notes = toKeep.Notes;
            s = this.sourceTasks.SaveSource(s);
        }

        public IList<ObjectSourceDuplicateDTO> GetPersonSourceDuplicates()
        {
            return this.objectSourceDuplicatesQuery.GetPersonSourceDuplicates();
        }

        public void MergePersonSourceDuplicates()
        {
            IList<ObjectSourceDuplicateDTO> duplicates = this.objectSourceDuplicatesQuery.GetPersonSourceDuplicates();
            log.Info("Found " + duplicates.Count + " PersonSource duplicates.");
            foreach (ObjectSourceDuplicateDTO dto in duplicates)
            {
                log.Info("Merging " + dto.Count + " PersonSources PersonID=" + dto.ObjectID + " SourceID=" + dto.SourceID);

                // retrieve all duplicates
                IList<PersonSource> personSources = this.sourceAttachmentTasks.GetPersonSources(
                    this.personTasks.GetPerson(dto.ObjectID), 
                    this.sourceTasks.GetSource(dto.SourceID));

                // select PersonSource with highest PersonSource.Reliability
                PersonSource keep = personSources.OrderBy(x => x.Reliability).Last();
                personSources.Remove(keep);

                // merge
                string mergedCommentary = keep.Commentary;
                string mergedNotes = keep.Notes;
                foreach (PersonSource ps in personSources)
                {
                    if (!string.Equals(keep.Commentary, ps.Commentary))
                        mergedCommentary += "; " + ps.Commentary;

                    if (!string.Equals(keep.Notes, ps.Notes))
                        mergedNotes += "; " + ps.Notes;

                    this.sourceAttachmentTasks.DeletePersonSource(ps.Id);
                }

                // save merged PersonSource
                keep.Commentary = mergedCommentary;
                keep.Notes = "Merged with duplicates on " + string.Format("{0:ddd MMM yyyy}", DateTime.Now) + ".\n" + mergedNotes;
                this.sourceAttachmentTasks.SavePersonSource(keep);
            }
            log.Info("Finished merging PersonSource duplicates.");
        }

        public IList<ObjectSourceDuplicateDTO> GetEventSourceDuplicates()
        {
            return this.objectSourceDuplicatesQuery.GetEventSourceDuplicates();
        }

        public void MergeEventSourceDuplicates()
        {
            IList<ObjectSourceDuplicateDTO> duplicates = this.objectSourceDuplicatesQuery.GetEventSourceDuplicates();
            log.Info("Found " + duplicates.Count + " EventSource duplicates.");
            foreach (ObjectSourceDuplicateDTO dto in duplicates)
            {
                log.Info("Merging " + dto.Count + " EventSources EventID=" + dto.ObjectID + " SourceID=" + dto.SourceID);

                // retrieve all duplicates
                IList<EventSource> eventSources = this.sourceAttachmentTasks.GetEventSources(
                    this.eventTasks.GetEvent(dto.ObjectID),
                    this.sourceTasks.GetSource(dto.SourceID));

                // select EventSource with highest EventSource.Reliability
                EventSource keep = eventSources.OrderBy(x => x.Reliability).Last();
                eventSources.Remove(keep);

                // merge
                string mergedCommentary = keep.Commentary;
                string mergedNotes = keep.Notes;
                foreach (EventSource es in eventSources)
                {
                    if (!string.Equals(keep.Commentary, es.Commentary))
                        mergedCommentary += "; " + es.Commentary;

                    if (!string.Equals(keep.Notes, es.Notes))
                        mergedNotes += "; " + es.Notes;

                    this.sourceAttachmentTasks.DeleteEventSource(es.Id);
                }

                // save merged EventSource
                keep.Commentary = mergedCommentary;
                keep.Notes = "Merged with duplicates on " + string.Format("{0:ddd MMM yyyy}", DateTime.Now) + ".\n" + mergedNotes;
                this.sourceAttachmentTasks.SaveEventSource(keep);
            }
            log.Info("Finished merging EventSource duplicates.");
        }

        public IList<ObjectSourceDuplicateDTO> GetOrganizationSourceDuplicates()
        {
            return this.objectSourceDuplicatesQuery.GetOrganizationSourceDuplicates();
        }

        public void MergeOrganizationSourceDuplicates()
        {
            IList<ObjectSourceDuplicateDTO> duplicates = this.objectSourceDuplicatesQuery.GetOrganizationSourceDuplicates();
            log.Info("Found " + duplicates.Count + " OrganizationSource duplicates.");
            foreach (ObjectSourceDuplicateDTO dto in duplicates)
            {
                log.Info("Merging " + dto.Count + " OrganizationSources OrganizationID=" + dto.ObjectID + " SourceID=" + dto.SourceID);

                // retrieve all duplicates
                IList<OrganizationSource> orgSources = this.sourceAttachmentTasks.GetOrganizationSources(
                    this.orgTasks.GetOrganization(dto.ObjectID),
                    this.sourceTasks.GetSource(dto.SourceID));

                // select OrganizationSource with highest OrganizationSource.Reliability
                OrganizationSource keep = orgSources.OrderBy(x => x.Reliability).Last();
                orgSources.Remove(keep);

                // merge
                string mergedCommentary = keep.Commentary;
                string mergedNotes = keep.Notes;
                foreach (OrganizationSource os in orgSources)
                {
                    if (!string.Equals(keep.Commentary, os.Commentary))
                        mergedCommentary += "; " + os.Commentary;

                    if (!string.Equals(keep.Notes, os.Notes))
                        mergedNotes += "; " + os.Notes;

                    this.sourceAttachmentTasks.DeleteOrganizationSource(os.Id);
                }

                // save merged OrganizationSource
                keep.Commentary = mergedCommentary;
                keep.Notes = "Merged with duplicates on " + string.Format("{0:ddd MMM yyyy}", DateTime.Now) + ".\n" + mergedNotes;
                this.sourceAttachmentTasks.SaveOrganizationSource(keep);
            }
            log.Info("Finished merging OrganizationSource duplicates.");
        }

        public IList<SourceDTO> GetExtensionlessSources()
        {
            return this.sourceQueries.GetExtensionlessSources();
        }

        public IList<SourceDTO> GetScannableSourceDTOs(int days)
        {
            return this.sourceQueries.GetScannableSourceDTOs(days);
        }
    }
}
