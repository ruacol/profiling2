using System;
using NHibernate.Envers;
using System.Threading;

namespace Profiling2.Domain
{
    [Serializable]
    public class RevinfoListener : IRevisionListener
    {
        public void NewRevision(object revisionEntity)
        {
            REVINFO revinfo = (REVINFO)revisionEntity;
            revinfo.UserName = Thread.CurrentPrincipal.Identity.Name;
        }
    }
}
