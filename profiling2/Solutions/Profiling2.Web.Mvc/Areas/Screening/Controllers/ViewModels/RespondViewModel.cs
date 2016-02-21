using System.Collections.Generic;
using System.Linq;
using Profiling2.Domain.Scr;
using Profiling2.Domain.Scr.Person;
using Profiling2.Domain.Scr.PersonEntity;
using Profiling2.Web.Mvc.Validators;

namespace Profiling2.Web.Mvc.Areas.Screening.Controllers.ViewModels
{
    [IsNotPending]
    [RequiresResponse]
    public class RespondViewModel
    {
        public int Id { get; set; }

        public bool SubmitResponse { get; set; }

        // indexed by RequestPersonID
        public IDictionary<int, ScreeningRequestPersonEntityViewModel> Responses { get; set; }

        public RespondViewModel() 
        {
            this.SubmitResponse = false;
            this.Responses = new Dictionary<int, ScreeningRequestPersonEntityViewModel>();
        }

        public RespondViewModel(Request request, IEnumerable<ScreeningResult> srs, ScreeningEntity screeningEntity)
        {
            this.Id = request.Id;
            this.SubmitResponse = false;
            this.Responses = new Dictionary<int, ScreeningRequestPersonEntityViewModel>();

            if (screeningEntity != null)
            {
                foreach (RequestPerson rp in request.Persons.Where(x => !x.Archive))
                {
                    ScreeningRequestPersonEntity srpe = rp.GetMostRecentScreeningRequestPersonEntity(screeningEntity.ScreeningEntityName);
                    ScreeningRequestPersonEntityViewModel vm = new ScreeningRequestPersonEntityViewModel(srpe);

                    vm.PopulateDropDowns(srs);
                    this.Responses[rp.Id] = vm;
                }
            }
        }
    }
}