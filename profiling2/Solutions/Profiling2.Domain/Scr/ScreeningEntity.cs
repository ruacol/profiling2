using System;
using System.Collections.Generic;
using Profiling2.Domain.Prf;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Scr
{
    public class ScreeningEntity : Entity
    {
        public const string JHRO = "JHRO";
        public const string JMAC = "JMAC";
        public const string CPS = "CPS";
        public const string SWA = "SWA";

        public virtual string ScreeningEntityName { get; set; }
        public virtual string Notes { get; set; }
        public virtual bool Archive { get; set; }
        public virtual IList<AdminUser> Users { get; set; }

        /// <summary>
        /// Return screening entity names active on the given date.
        /// SWA returned only for those requests created after new SOP came into effect.
        /// </summary>
        /// <param name="requestCreated"></param>
        /// <returns></returns>
        public static string[] GetNames(DateTime requestCreated)
        {
            if (requestCreated >= new DateTime(2015, 5, 21))
                return new string[] { JHRO, JMAC, CPS, SWA };
            else
                return new string[] { JHRO, JMAC, CPS };
        }

        public override string ToString()
        {
            return this.ScreeningEntityName;
        }
    }
}
