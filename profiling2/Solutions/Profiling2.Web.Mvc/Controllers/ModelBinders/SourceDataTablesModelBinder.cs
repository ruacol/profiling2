using System;
using System.Collections.Specialized;
using System.Web.Mvc;
using Mvc.JQuery.Datatables;
using Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels;

namespace Profiling2.Web.Mvc.Controllers.ModelBinders
{
    public class SourceDataTablesModelBinder : DataTablesModelBinder, IModelBinder
    {
        public new object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            SourceDataTablesParam p = new SourceDataTablesParam((DataTablesParam)base.BindModel(controllerContext, bindingContext));

            NameValueCollection @params = controllerContext.HttpContext.Request.Params;
            p.searchAdminSourceSearchId = string.IsNullOrEmpty(@params["searchAdminSourceSearchId"]) ? null : (int?)Convert.ToInt32(@params["searchAdminSourceSearchId"]);
            p.searchText = string.IsNullOrEmpty(@params["searchText"]) ? null : @params["searchText"].Trim();
            p.searchId = string.IsNullOrEmpty(@params["searchId"]) ? null : (int?)Convert.ToInt32(@params["searchId"]);
            p.personId = string.IsNullOrEmpty(@params["personId"]) ? null : (int?)Convert.ToInt32(@params["personId"]);
            p.eventId = string.IsNullOrEmpty(@params["eventId"]) ? null : (int?)Convert.ToInt32(@params["eventId"]);
            p.startDate = string.IsNullOrEmpty(@params["startDate"]) ? null : @params["startDate"].Trim();
            p.endDate = string.IsNullOrEmpty(@params["endDate"]) ? null : @params["endDate"].Trim();
            p.searchName = string.IsNullOrEmpty(@params["searchName"]) ? null : @params["searchName"].Trim();
            p.searchExtension = string.IsNullOrEmpty(@params["searchExtension"]) ? null : @params["searchExtension"].Trim();
            p.authorSearchText = string.IsNullOrEmpty(@params["authorSearchText"]) ? null : @params["authorSearchText"].Trim();
            return p;
        }
    }
}