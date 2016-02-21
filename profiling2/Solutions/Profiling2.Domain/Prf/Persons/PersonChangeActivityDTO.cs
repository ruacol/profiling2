using System;
using System.Globalization;

namespace Profiling2.Domain.Prf.Persons
{
    public class PersonChangeActivityDTO
    {
        public int LogNo { get; set; }
        public string Who { get; set; }
        public string What { get; set; }
        public string When { get; set; }
        public string PreviousValues { get; set; }
        public string NonProfilingChange { get; set; }

        public PersonChangeActivityDTO() { }

        public DateTime? WhenDate
        {
            get
            {
                try
                {
                    return DateTime.ParseExact(this.When, new string[] 
                    { 
                        "yyyy/MM/dd HH:mm:ss",
                        "yyyy/MM/dd",
                        "dd/MM/yyyy",  // returned by modified profiles stored proc
                        "M/d/yyyy h:mm:ss tt",  // returned by profile change activity stored proc
                        "dd/MM/yyyy h:mm:ss tt"  // exists in deleted profiles stored proc
                    }, CultureInfo.CurrentCulture, DateTimeStyles.None);
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
    }
}
