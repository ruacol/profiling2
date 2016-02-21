using System.Collections.Generic;
using System.Linq;
using Profiling2.Domain.Prf.Persons;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels
{
    public class PersonDataTableView
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Aliases { get; set; }
        //public string DateOfBirth { get; set; }
        //public string PlaceOfBirth { get; set; }
        public string MilitaryIDNumber { get; set; }

        public PersonDataTableView() { }

        public PersonDataTableView(Person p, IEnumerable<PersonAlias> aliases)
        {
            this.Id = p.Id;
            this.FirstName = p.FirstName;
            this.LastName = p.LastName;
            this.Aliases = string.Join("; ", (from alias in aliases select alias.Name).ToArray<string>());
            //this.DateOfBirth = p.DateOfBirth;
            //this.PlaceOfBirth = p.PlaceOfBirth;
            this.MilitaryIDNumber = p.MilitaryIDNumber;
        }

        public PersonDataTableView(SearchForPersonDTO p)
        {
            this.Id = p.PersonID;
            this.FirstName = p.FirstName;
            this.LastName = p.LastName;
            this.Aliases = p.Aliases;
            this.MilitaryIDNumber = p.MilitaryIDNumber;
        }
    }
}