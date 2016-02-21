using Profiling2.Domain.Scr;

namespace Profiling2.Web.Mvc.Areas.Screening.Controllers.ViewModels
{
    public class ValidateViewModel : RequestViewModel
    {
        public bool ForwardRequest { get; set; }

        public bool RejectRequest { get; set; }

        public string RejectReason { get; set; }

        public ValidateViewModel() { }

        public ValidateViewModel(Request request) : base(request) { }
    }
}