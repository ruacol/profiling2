using System;
using System.Configuration;
using System.Linq;
using System.Net.Configuration;
using NHibernate;
using Postal;
using Profiling2.Domain.Contracts;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Contracts.Tasks.Sources;
using Profiling2.Domain.Prf;
using SharpArch.NHibernate;

namespace Profiling2.Web.Mvc.Areas.System.Controllers.Tasks
{
    public class PostalEmailTasks : IPostalEmailTasks
    {
        protected readonly IUserTasks userTasks;
        protected readonly IPersonStatisticTasks personStatisticTasks;
        protected readonly IFeedingSourceTasks feedingSourceTasks;
        protected readonly ISourceStatisticTasks sourceStatisticTasks;

        public PostalEmailTasks(IUserTasks userTasks, 
            IPersonStatisticTasks personStatisticTasks, 
            IFeedingSourceTasks feedingSourceTasks,
            ISourceStatisticTasks sourceStatisticTasks)
        {
            this.userTasks = userTasks;
            this.personStatisticTasks = personStatisticTasks;
            this.feedingSourceTasks = feedingSourceTasks;
            this.sourceStatisticTasks = sourceStatisticTasks;
        }

        public void SendProfilingCountsReportEmail()
        {
            using (ISession session = NHibernateSession.GetDefaultSessionFactory().OpenSession())
            {
                dynamic email = new Email("ProfilingCountsReport");
                email.To = string.Join(",", this.userTasks.GetUserEmailsWithPermissionUsingNewSession(AdminPermission.CanViewPersonReports));
                email.From = this.GetFromEmailAddress();
                email.Subject = "[profiling] Weekly report of profiling counts (" + string.Format("{0:yyyy-MM-dd}", DateTime.Now) + ")";
                email.ProfilingCounts = this.personStatisticTasks.GetProfilingCountsView(null, session);
                email.Send();
            }
        }

        public void SendFeedingSourceReport()
        {
            dynamic email = new Email("FeedingSourceReport");
            email.To = string.Join(",", this.userTasks.GetUserEmailsWithPermissionUsingNewSession(AdminPermission.CanApproveAndRejectSources));
            email.From = this.GetFromEmailAddress();
            email.Subject = "[feeding] Weekly report of source feeding activity (" + string.Format("{0:yyyy-MM-dd}", DateTime.Now) + ")";
            email.Stats = this.feedingSourceTasks.GetFeedingSourceDTOsRecurring();
            email.Send();
        }

        public void SendSourceFolderCountsReport()
        {
            dynamic email = new Email("SourceFolderCountsReport");
            email.To = string.Join(",", this.userTasks.GetUserEmailsWithPermissionUsingNewSession(AdminPermission.CanUploadSources));
            email.From = this.GetFromEmailAddress();
            email.Subject = "[feeding] Monthly report of source folder counts (" + string.Format("{0:yyyy-MM-dd}", DateTime.Now) + ")";

            string sourceFolders = ConfigurationManager.AppSettings["SourceFolders"];
            if (!string.IsNullOrEmpty(sourceFolders))
            {
                using (ISession session = NHibernateSession.GetDefaultSessionFactory().OpenSession())
                {
                    email.Counts = this.sourceStatisticTasks.GetSourceFolderCounts(
                        sourceFolders.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList(), 
                        session);
                }
            }

            email.Send();
        }

        protected string GetFromEmailAddress()
        {
            // use Web.config 'from' address
            var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            if (smtpSection != null && !string.IsNullOrEmpty(smtpSection.From))
                return smtpSection.From;

            return null;
        }
    }
}