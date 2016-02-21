using System;
using Mvc.JQuery.Datatables;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels
{
    public class SourceDataTablesParam : DataTablesParam
    {
        public int? searchAdminSourceSearchId { get; set; }
        public string searchText { get; set; }
        public int? searchId { get; set; }
        public int? personId { get; set; }
        public int? eventId { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string searchName { get; set; }
        public string searchExtension { get; set; }
        public string authorSearchText { get; set; }

        public SourceDataTablesParam() { }

        public SourceDataTablesParam(DataTablesParam p)
        {
            this.iDisplayStart = p.iDisplayStart;
            this.iDisplayLength = p.iDisplayLength;
            this.iColumns = p.iColumns;
            this.sSearch = p.sSearch;
            this.bEscapeRegex = p.bEscapeRegex;
            this.iSortingCols = p.iSortingCols;
            this.sEcho = p.sEcho;
            this.bSortable = p.bSortable;
            this.bSearchable = p.bSearchable;
            this.sSearchColumns = p.sSearchColumns;
            this.iSortCol = p.iSortCol;
            this.sSortDir = p.sSortDir;
            this.bEscapeRegexColumns = p.bEscapeRegexColumns;
        }

        public DateTime? GetStartDate()
        {
            if (!string.IsNullOrEmpty(this.startDate))
            {
                DateTime s;
                if (DateTime.TryParse(this.startDate, out s))
                    return s;
            }
            return null;
        }

        public DateTime? GetEndDate()
        {
            if (!string.IsNullOrEmpty(this.endDate))
            {
                DateTime e;
                if (DateTime.TryParse(this.endDate, out e))
                    return e;
            }
            return null;
        }
    }
}