using Profiling2.Domain.Prf.Actions;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels
{
    public class ActionTakenTypeViewModel
    {
        public int Id { get; set; }
        public string ActionTakenTypeName { get; set; }
        public string Notes { get; set; }
        public bool IsRemedial { get; set; }
        public bool IsDisciplinary { get; set; }

        public ActionTakenTypeViewModel() { }

        public ActionTakenTypeViewModel(ActionTakenType type)
        {
            if (type != null)
            {
                this.Id = type.Id;
                this.ActionTakenTypeName = type.ActionTakenTypeName;
                this.Notes = type.Notes;
                this.IsRemedial = type.IsRemedial;
                this.IsDisciplinary = type.IsDisciplinary;
            }
        }
    }
}