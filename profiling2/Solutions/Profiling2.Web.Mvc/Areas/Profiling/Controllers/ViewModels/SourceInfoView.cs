using System.Collections.Generic;
using System.Linq;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Prf.Sources;
using Profiling2.Domain.Prf.Units;
using Profiling2.Infrastructure.Util;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels
{
    public class SourceInfoView : SourceDTO
    {
        public int Id { get; set; }

        // collections
        public IList<object> AdminSourceImports { get; set; }
        public IList<object> AdminReviewedSources { get; set; }
        public IList<object> PersonSources { get; set; }
        public IList<object> EventSources { get; set; }
        public IList<object> UnitSources { get; set; }
        public IList<object> OperationSources { get; set; }

        // for display purposes on Source info page
        public IDictionary<string, object> DocumentProperties { get; set; }
        public object ParentSource { get; set; }
        public IList<object> ChildSources { get; set; }
        public IList<object> SourceAuthors { get; set; }
        public IList<object> SourceOwners { get; set; }

        public SourceInfoView() { }

        public SourceInfoView(SourceDTO dto)
        {
            this.Id = dto.SourceID;
            this.SourceID = dto.SourceID;
            this.SourceName = dto.SourceName;
            this.FullReference = dto.FullReference;
            this.SourcePath = dto.SourcePath;
            this.SourceDate = dto.SourceDate;
            this.FileExtension = FileUtil.GetExtension(dto.SourceName);
            this.IsRestricted = dto.IsRestricted;
            this.FileDateTimeStamp = dto.FileDateTimeStamp;
            this.Archive = dto.Archive;
            this.IsReadOnly = dto.IsReadOnly;
            this.Notes = dto.Notes;
            this.IsPublic = dto.IsPublic;

            this.FileSize = dto.FileSize;
            this.JhroCaseID = dto.JhroCaseID;
            this.JhroCaseNumber = dto.JhroCaseNumber;
            this.HasOcrText = dto.HasOcrText;
            this.UploadedByUserID = dto.UploadedByUserID;
        }

        public void SetAdminReviewedSources(IList<AdminReviewedSource> reviews)
        {
            this.AdminReviewedSources = new List<object>();
            foreach (AdminReviewedSource ars in reviews)
                this.AdminReviewedSources.Add(new
                {
                    ReviewedDateTime = ars.ReviewedDateTime,
                    AdminSourceSearchId = ars.AdminSourceSearch.Id,
                    AndSearchTerms = ars.AdminSourceSearch.AndSearchTermsQuoted,
                    ExcludeSearchTerms = ars.AdminSourceSearch.ExcludeSearchTermsQuoted,
                    IsRelevant = ars.IsRelevant,
                    WasDownloaded = ars.WasDownloaded,
                    WasPreviewed = ars.WasPreviewed,
                    User = ars.AdminSourceSearch.SearchedByAdminUser.Headline
                });
        }

        public void SetAdminSourceImports(IList<AdminSourceImport> imports)
        {
            this.AdminSourceImports = new List<object>();
            foreach (AdminSourceImport asi in imports)
                this.AdminSourceImports.Add(new { SourcePath = asi.SourcePath, ImportDate = asi.ImportDate });
        }

        public void SetPersonSources(IList<PersonSource> personSources)
        {
            this.PersonSources = new List<object>();
            foreach (PersonSource ps in personSources)
                this.PersonSources.Add(new
                {
                    Id = ps.Person.Id,
                    Name = ps.Person.Name,
                    Reliability = (ps.Reliability != null ? ps.Reliability.ReliabilityName : string.Empty),
                    Commentary = ps.Commentary
                });
        }

        public void SetEventSources(IList<EventSource> eventSources)
        {
            this.EventSources = new List<object>();
            foreach (EventSource es in eventSources)
                this.EventSources.Add(new
                {
                    Id = es.Event.Id,
                    Name = es.Event.Headline,
                    Reliability = (es.Reliability != null ? es.Reliability.ReliabilityName : string.Empty),
                    Commentary = es.Commentary
                });
        }

        public void SetUnitSources(IList<UnitSource> unitSources)
        {
            this.UnitSources = new List<object>();
            foreach (UnitSource us in unitSources)
                this.UnitSources.Add(new
                {
                    Id = us.Unit.Id,
                    Name = us.Unit.UnitName,
                    Reliability = (us.Reliability != null ? us.Reliability.ReliabilityName : string.Empty),
                    Commentary = us.Commentary
                });
        }

        public void SetOperationSources(IList<OperationSource> operationSources)
        {
            this.OperationSources = new List<object>();
            foreach (OperationSource os in operationSources)
                this.OperationSources.Add(new
                {
                    Id = os.Operation.Id,
                    Name = os.Operation.Name,
                    Reliability = (os.Reliability != null ? os.Reliability.ReliabilityName : string.Empty),
                    Commentary = os.Commentary
                });
        }

        public void SetParentSource(SourceRelationship relationship)
        {
            if (relationship != null)
                this.ParentSource = new { Id = (relationship.ParentSource != null ? relationship.ParentSource.Id as int? : null), SourcePath = relationship.ParentSourcePath };
        }

        public void SetChildSources(IList<SourceRelationship> relationships)
        {
            if (relationships != null && relationships.Any())
                this.ChildSources = relationships.Select(x => new { Id = x.ChildSource.Id, SourcePath = x.ChildSource.SourcePath }).ToList<object>();
        }

        public void SetSourceAuthors(IList<string> authors)
        {
            if (authors != null && authors.Any())
                this.SourceAuthors = authors.Select(x => new { Author = x }).ToList<object>();
        }

        public void SetSourceOwners(IList<string> owners)
        {
            if (owners != null && owners.Any())
                this.SourceOwners = owners.Select(x => new { Name = x }).ToList<object>();
        }
    }
}