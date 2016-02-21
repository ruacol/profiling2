
namespace Profiling2.Domain.DTO
{
    public class PersonSourceDTO
    {
        public int Id { get; set; }
        public int SourceId { get; set; }
        public string SourceName { get; set; }
        public bool SourceIsRestricted { get; set; }
        public int PersonId { get; set; }
        public string PersonName { get; set; }
        public int ReliabilityId { get; set; }
        public string ReliabilityName { get; set; }
        public string Commentary { get; set; }
        public string Notes { get; set; }

        public PersonSourceDTO() { }

        public PersonSourceDTO SetPersonName(string personName)
        {
            this.PersonName = personName;
            return this;
        }
    }
}
