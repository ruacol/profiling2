using Profiling2.Domain;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels;
using SharpArch.Web.Mvc.JsonNet;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Profiling2.Web.Mvc.Areas.API.Controllers
{
    public class PersonsController : BaseApiController
    {
        protected readonly IPersonTasks personTasks;
        protected readonly ILuceneTasks luceneTasks;

        public PersonsController(IPersonTasks personTasks, ILuceneTasks luceneTasks)
        {
            this.personTasks = personTasks;
            this.luceneTasks = luceneTasks;
        }

        public JsonNetResult Get(int id)
        {
            Person person = this.personTasks.GetPerson(id);
            if (person != null)
            {
                return JsonNet(new PersonDataTableLuceneView()
                    {
                        Id = person.Id.ToString(),
                        Name = new string[] { person.Name }.Concat(person.PersonAliases.Select(x => x.Name)),
                        MilitaryIDNumber = string.IsNullOrEmpty(person.MilitaryIDNumber) ? new string[] { } : person.MilitaryIDNumber.Split(',').AsEnumerable(),
                        Rank = person.CurrentCareer != null && !string.IsNullOrEmpty(person.CurrentCareer.RankOrganizationLocationSummary) ? person.CurrentCareer.RankOrganizationLocationSummary : null,
                        Function = person.CurrentCareer != null && !string.IsNullOrEmpty(person.CurrentCareer.FunctionUnitSummary) ? person.CurrentCareer.FunctionUnitSummary : null
                    });
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return JsonNet(string.Empty);
            }
        }

        public JsonNetResult Search()
        {
            string term = Request.QueryString["term"];
            if (!string.IsNullOrEmpty(term))
            {
                term = term.Trim();

                // conduct search - don't show restricted profiles
                IList<LuceneSearchResult> results = this.luceneTasks.PersonSearch(term, 50, false);
                return JsonNet(results.Select(x => new PersonDataTableLuceneView(x)).ToArray());
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return JsonNet(string.Empty);
            }
        }
    }
}