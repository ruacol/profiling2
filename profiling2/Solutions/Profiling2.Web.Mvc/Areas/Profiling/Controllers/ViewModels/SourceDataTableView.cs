using Profiling2.Domain.Prf.Sources;
using Profiling2.Infrastructure.Util;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels
{
    public class SourceDataTableView : SourceSearchResultDTO
    {
        public int Id { get; set; }

        public SourceDataTableView() { }

        public SourceDataTableView(SourceSearchResultDTO dto)
        {
            this.PopulateBaseSourceDTO(dto);

            this.IsRelevant = dto.IsRelevant;
            this.ReviewedDateTime = dto.ReviewedDateTime;
            this.IsAttached = dto.IsAttached;
            this.Rank = dto.Rank;
        }

        public SourceDataTableView(SourceDTO dto)
        {
            this.PopulateBaseSourceDTO(dto);
        }

        protected void PopulateBaseSourceDTO(BaseSourceDTO dto)
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
        }
    }
}