using System;
using Profiling2.Domain.Scr;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Persons
{
    /// <summary>
    /// An instance of when a Person has been actively researched and updated by a human.  Useful when applying 3 month rule.
    /// </summary>
    public class ActiveScreening : Entity
    {
        public virtual Person Person { get; set; }
        public virtual Request Request { get; set; }
        public virtual DateTime DateActivelyScreened { get; set; }
        public virtual AdminUser ScreenedBy { get; set; }
        public virtual string Notes { get; set; }
    }
}
