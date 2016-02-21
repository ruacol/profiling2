using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Profiling2.Domain.Contracts
{
    public interface IPostalEmailTasks
    {
        void SendProfilingCountsReportEmail();

        void SendFeedingSourceReport();

        void SendSourceFolderCountsReport();
    }
}
