
namespace Profiling2.Domain.Prf.Persons
{
    public class DeletedProfilesAuditDTO : PersonChangeActivityDTO
    {
        public string PersonID { get; set; }
        public string Person { get; set; }
    }
}
