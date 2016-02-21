
namespace Profiling2.Domain.Prf.Persons
{
    public class SearchForPersonDTO
    {
        public virtual int PersonID { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string MilitaryIDNumber { get; set; }
        public virtual string Aliases { get; set; }
        public virtual decimal Score { get; set; }
        public virtual int RowNumber { get; set; }
        public virtual int TotalRecords { get; set; }

        public SearchForPersonDTO() { }
    }
}
