using System;
using System.Collections.Generic;
using System.Linq;
using Hangfire;
using log4net;
using NHibernate;
using Profiling2.Domain.Contracts.Queries;
using Profiling2.Domain.Contracts.Tasks.Sources;
using Profiling2.Domain.DTO;
using Profiling2.Domain.Prf.Sources;
using Profiling2.Infrastructure.Util;
using SharpArch.NHibernate;
using SharpArch.NHibernate.Contracts.Repositories;

namespace Profiling2.Tasks.Sources
{
    public class FeedingSourceTasks : IFeedingSourceTasks
    {
        protected readonly ILog log = LogManager.GetLogger(typeof(FeedingSourceTasks));
        protected readonly INHibernateRepository<FeedingSource> feedingSourceRepo;
        protected readonly IFeedingSourceQuery feedingSourceQuery;
        protected readonly ISourceContentTasks sourceContentTasks;
        protected readonly ISourceTasks sourceTasks;

        public FeedingSourceTasks(INHibernateRepository<FeedingSource> feedingSourceRepo,
            IFeedingSourceQuery feedingSourceQuery,
            ISourceContentTasks sourceContentTasks,
            ISourceTasks sourceTasks)
        {
            this.feedingSourceRepo = feedingSourceRepo;
            this.feedingSourceQuery = feedingSourceQuery;
            this.sourceContentTasks = sourceContentTasks;
            this.sourceTasks = sourceTasks;
        }

        public FeedingSource SaveFeedingSource(FeedingSource fs)
        {
            return this.feedingSourceRepo.SaveOrUpdate(fs);
        }

        public IList<FeedingSourceDTO> GetFeedingSourceDTOs(bool canViewAndSearchAll, bool includeRestricted, string uploadedByUserId)
        {
            return this.feedingSourceQuery.GetFeedingSourceDTOs(canViewAndSearchAll, includeRestricted, uploadedByUserId);
        }

        public FeedingSource GetFeedingSource(int id)
        {
            return this.feedingSourceRepo.Get(id);
        }

        public FeedingSource GetFeedingSource(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                IDictionary<string, object> criteria = new Dictionary<string, object>();
                criteria.Add("Name", name);
                return this.feedingSourceRepo.FindOne(criteria);
            }
            return null;
        }

        public IDictionary<string, FeedingSourceStat> GetFeedingSourceDTOs(ISession session, DateTime start, DateTime end, bool includeRestricted)
        {
            IDictionary<string, FeedingSourceStat> dict = new Dictionary<string, FeedingSourceStat>();

            foreach (FeedingSourceDTO dto in this.feedingSourceQuery.GetFeedingSourceDTOs(session, start, end, includeRestricted))
            {
                if (!string.IsNullOrEmpty(dto.UploadedBy))
                {
                    if (!dict.ContainsKey(dto.UploadedBy))
                    {
                        dict.Add(dto.UploadedBy, new FeedingSourceStat(dto.UploadedBy));
                    }
                    dict[dto.UploadedBy].Uploaded++;
                }
                if (!string.IsNullOrEmpty(dto.ApprovedBy))
                {
                    if (!dict.ContainsKey(dto.ApprovedBy))
                    {
                        dict.Add(dto.ApprovedBy, new FeedingSourceStat(dto.ApprovedBy));
                    }
                    dict[dto.ApprovedBy].Approved++;
                }
                if (!string.IsNullOrEmpty(dto.RejectedBy))
                {
                    if (!dict.ContainsKey(dto.RejectedBy))
                    {
                        dict.Add(dto.RejectedBy, new FeedingSourceStat(dto.RejectedBy));
                    }
                    dict[dto.RejectedBy].Rejected++;
                }
            }

            return dict;
        }

        public IDictionary<string, FeedingSourceStat> GetFeedingSourceDTOsRecurring()
        {
            using (ISession session = NHibernateSession.GetDefaultSessionFactory().OpenSession())
                return this.GetFeedingSourceDTOs(session, DateTime.Now.Subtract(TimeSpan.FromDays(7)), DateTime.Now, true);
        }

        public void DeleteFeedingSource(FeedingSource fs)
        {
            if (fs != null)
                this.feedingSourceRepo.Delete(fs);
        }

        public Source FeedSource(int feedingSourceId)
        {
            FeedingSource fs = this.GetFeedingSource(feedingSourceId);
            if (fs != null && fs.FileData != null)
            {
                Source s = new Source();
                s.SourceName = fs.Name;
                s.SourcePath = fs.Name;
                s.SourceDate = DateTime.Now;
                s.FileExtension = FileUtil.GetExtension(fs.Name);
                s.FileData = fs.FileData;
                s.IsRestricted = fs.Restricted;
                s.IsReadOnly = fs.IsReadOnly;
                s.Notes = "Fed from FeedingSourceID=" + fs.Id;
                if (!string.IsNullOrEmpty(fs.UploadNotes))
                    s.Notes += "\n\n" + fs.UploadNotes;
                s.FileDateTimeStamp = fs.FileModifiedDateTime;
                foreach (SourceAuthor a in fs.SourceAuthors)
                    s.AddSourceAuthor(a);
                foreach (SourceOwningEntity e in fs.SourceOwningEntities)
                    s.AddSourceOwningEntity(e);

                // persist Source
                s = this.sourceTasks.SaveSource(s);

                // link FeedingSource with Source
                fs.Source = s;
                s.AddFeedingSource(fs);

                // persist the join
                fs = this.SaveFeedingSource(fs);

                // queue ocr scan of new source
                var jobId = BackgroundJob.Enqueue<ISourceContentTasks>(x => x.OcrScanAndSetSourceQueueable(s.Id));

                // queue indexing of the new Source
                BackgroundJob.ContinueWith<ISourceTasks>(jobId, x =>
                    x.IndexSourceQueueable(s.Id,
                        s.HasUploadedBy() ? s.GetUploadedBy().UserID : string.Empty,
                        s.SourceAuthors.Select(y => y.Author).ToList(),
                        s.SourceOwningEntities.Select(y => y.Name).ToList(),
                        s.JhroCase != null ? s.JhroCase.CaseNumber : string.Empty,
                        this.sourceTasks.GetSourceDTO(s.Id).FileSize)
                    );

                return s;
            }
            return null;
        }
    }
}
