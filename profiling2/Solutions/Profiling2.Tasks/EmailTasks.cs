using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using log4net;
using Profiling2.Domain.Contracts.Services;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Contracts.Tasks.Screenings;
using Profiling2.Domain.Contracts.Tasks.Sources;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Prf.Sources;
using Profiling2.Domain.Scr;
using Profiling2.Domain.Scr.Proposed;

namespace Profiling2.Tasks
{
    public class EmailTasks : IEmailTasks
    {
        protected readonly ILog log = LogManager.GetLogger(typeof(EmailTasks));
        protected readonly IRequestTasks requestTasks;
        protected readonly IEmailService emailService;
        protected readonly IUserTasks userTasks;
        protected readonly IFeedingSourceTasks feedingSourceTasks;

        protected static string APPLICATION_URL = ConfigurationManager.AppSettings["ApplicationUrl"];

        public EmailTasks(IRequestTasks requestTasks, IEmailService emailService, IUserTasks userTasks, IFeedingSourceTasks feedingSourceTasks)
        {
            this.requestTasks = requestTasks;
            this.emailService = emailService;
            this.userTasks = userTasks;
            this.feedingSourceTasks = feedingSourceTasks;
        }

        /// <summary>
        /// AdminUser oriented interface to emailService.SendMailMessage().
        /// 
        /// TODO throw exceptions?  Maintaining status quo for now.
        /// </summary>
        /// <param name="recipients"></param>
        /// <param name="sender"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="isHtml"></param>
        protected void SendMailMessage(IList<AdminUser> recipients, AdminUser sender, string subject, string body, bool isHtml)
        {
            if (recipients != null && recipients.Any())
            {
                string[] recipientEmails = recipients.Select(x => x.Email).ToArray();

                if (recipientEmails.Any())
                {
                    this.emailService.SendMailMessage(recipientEmails, sender != null ? sender.Email : null, subject, body, isHtml);
                }
                else
                {
                    log.Error("These email recipients do not have email addresses: " + string.Join(", ", recipients.Select(x => x.UserID).ToArray()));
                }
            }
        }

        public void SendRequestSentForFinalDecisionEmail(string username, int requestId)
        {
            Request request = this.requestTasks.Get(requestId);
            string subject = "Notification - Request Requires Final Decision";
            string body = "This is an automated notification. The screening request "
                + "<a href='" + APPLICATION_URL + "Screening/Finalize/Request/" + request.Id.ToString() + "'>" 
                + request.RequestName + " (" + request.ReferenceNumber + ")</a> has been sent for final decision.";

            this.SendMailMessage(this.userTasks.GetUsersWithRole(AdminRole.ScreeningRequestFinalDecider),
                this.userTasks.GetAdminUser(username),
                subject, body, true);
        }

        public void SendRequestSentForValidationEmail(string username, int requestId)
        {
            Request request = this.requestTasks.Get(requestId);
            string subject = "Notification - Request Requires Validation";
            string body = "This is an automated notification. The screening request " 
                + "<a href='" + APPLICATION_URL + "Screening/Validate/Request/" + request.Id.ToString() + "'>" 
                + request.RequestName + " (" + request.ReferenceNumber + ")</a> has been submitted to the ODSRSG RoL for validation.";

            this.SendMailMessage(this.userTasks.GetUsersWithRole(AdminRole.ScreeningRequestValidator),
                this.userTasks.GetAdminUser(username),
                subject, body, true);
        }

        public void SendPersonsProposedEmail(string username, int requestId)
        {
            // do we want a special permission here?
            IList<AdminUser> recipients = this.userTasks.GetUsersWithPermission(AdminPermission.CanApproveAndRejectSources);

            // TODO customisable email notifications
            recipients.Add(new AdminUser() { Email = "monusco-hq-dsrsgma@un.org" });

            Request request = this.requestTasks.Get(requestId);
            string subject = "Notification - Persons Proposed in Integrated Profiling and Conditionality System";
            string body = "A user (" + username + ") has proposed adding the following persons to the Integrated Profiling and Conditionality System.\n";
            foreach (RequestProposedPerson rpp in request.ProposedPersons.Where(x => !x.Archive))
            {
                body += "\n";
                body += "Name: " + rpp.PersonName + "\n";
                if (!string.IsNullOrEmpty(rpp.MilitaryIDNumber))
                    body += "Military ID: " + rpp.MilitaryIDNumber + "\n";
                if (!string.IsNullOrEmpty(rpp.Notes))
                    body += "Notes: " + rpp.Notes + "\n";
            }

            this.SendMailMessage(recipients,
                this.userTasks.GetAdminUser(username),
                subject, body, false);
        }

        public void SendRespondedToEmail(string username, int requestId)
        {
            Request request = this.requestTasks.Get(requestId);
            AdminUser user = this.userTasks.GetAdminUser(username);
            if (request != null && user != null)
            {
                ScreeningEntity screeningEntity = user.GetScreeningEntity();

                string subject = "Notification - Response to Screening Request";
                string body = "This is an automated notification. " + screeningEntity + " has responded to screening request: "
                    + "<a href='" + APPLICATION_URL + "Screening/Consolidate/Request/" + request.Id.ToString() + "'>" 
                    + request.Headline + "</a>.<br />";

                //foreach (RequestPerson rp in request.Persons.Where(x => !x.Archive))
                //{
                //    body += "\n";
                //    body += "Name: " + rp.Person.Name + "\n";
                //    ScreeningRequestPersonEntity srpe = rp.GetScreeningRequestPersonEntity(screeningEntity.ScreeningEntityName);
                //    if (srpe != null)
                //        body += "Color: " + srpe.ScreeningResult + "\n";
                //}

                this.SendMailMessage(this.userTasks.GetUsersWithRole(AdminRole.ScreeningRequestConsolidator),
                    this.userTasks.GetAdminUser(username),
                    subject, body, true);
            }
        }

        public void SendProfileRequestEmail(string username, Person person)
        {
            AdminUser user = this.userTasks.GetAdminUser(username);
            if (user != null && person != null)
            {
                string subject = "Notification - Profile Requested through Integrated Profiling and Conditionality System";
                string body = "A user (" + username + ") has requested the following profile."
                            + "<br /><br />Person ID: " + person.Id.ToString()
                            + "<br />Person Name: <a href='" + APPLICATION_URL + "Profiling/Persons/Details/" + person.Id.ToString() + "'>" 
                            + person.Name + "</a>";

                // do we need a new permission here?
                this.SendMailMessage(this.userTasks.GetUsersWithPermission(AdminPermission.CanApproveAndRejectSources),
                    user,
                    subject, body, true);
            }
        }

        public void SendRequestCompletedEmail(string username, int requestId)
        {
            Request request = this.requestTasks.Get(requestId);
            if (request != null)
            {
                // notify all conditionality participants
                IList<AdminUser> recipients = this.userTasks.GetUsersWithRole(AdminRole.ScreeningRequestConditionalityParticipant).ToList();

                AdminUser creator = request.Creator;
                if (creator != null)
                {
                    // notify request initiator
                    recipients.Add(this.userTasks.GetAdminUser(request.Creator.UserID));

                    // notify members of initiator's request entity
                    foreach (RequestEntity re in creator.RequestEntities)
                        foreach (AdminUser au in re.Users.Where(x => !x.Archive))
                            recipients.Add(this.userTasks.GetAdminUser(au.UserID));

                    // notify validators
                    foreach (AdminUser validator in this.userTasks.GetUsersWithRole(AdminRole.ScreeningRequestValidator))
                        recipients.Add(validator);
                }

                string subject = "Notification - Request Completed";
                string body = "This is an automated notification. The screening request "
                    + "<a href='" + APPLICATION_URL + "Screening/Requests/Details/" + request.Id.ToString() + "'>" 
                    + request.RequestName + " (" + request.ReferenceNumber + ")</a> has been completed.";

                this.SendMailMessage(recipients, this.userTasks.GetAdminUser(username), subject, body, true);
            }
        }

        public void SendRequestForwardedToConditionalityParticipantsEmail(string username, int requestId)
        {
            Request request = this.requestTasks.Get(requestId);
            if (request != null)
            {
                string subject = "Notification - Screening Request Requires Response";
                string body = "This is an automated notification. The screening request "
                    + "<a href='" + APPLICATION_URL + "Screening/Inputs/Respond/" + request.Id.ToString() + "'>" 
                    + request.RequestName + " (" + request.ReferenceNumber + ")</a> has been sent from the Office of the DSRSG RoL and is requiring response.";

                this.SendMailMessage(this.userTasks.GetUsersWithRole(AdminRole.ScreeningRequestConditionalityParticipant), 
                    this.userTasks.GetAdminUser(username), 
                    subject, body, true);
            }
        }

        public void SendRequestRejectedEmail(string username, int requestId, string reason)
        {
            Request request = this.requestTasks.Get(requestId);
            if (request != null)
            {
                IList<AdminUser> recipients = new List<AdminUser>();

                AdminUser creator = request.Creator;
                if (creator != null)
                {
                    // notify request initiator
                    recipients.Add(this.userTasks.GetAdminUser(request.Creator.UserID));

                    // notify members of initiator's request entity
                    foreach (RequestEntity re in creator.RequestEntities)
                        foreach (AdminUser au in re.Users.Where(x => !x.Archive))
                            recipients.Add(this.userTasks.GetAdminUser(au.UserID));
                }

                string subject = "Notification - Request Rejected";
                string body = "This is an automated notification. The screening request "
                    + "<a href='" + APPLICATION_URL + "Screening/Requests/Details/" + request.Id.ToString() + "'>" 
                    + request.RequestName + " (" + request.ReferenceNumber + ")</a> has been rejected.";
                body += "<br /><br />" + reason;

                this.SendMailMessage(recipients, this.userTasks.GetAdminUser(username), subject, body, true);
            }
        }

        public void SendFeedingSourceUploadedEmail(FeedingSource fs)
        {
            if (fs != null && fs.UploadedBy != null)
            {
                string subject = "[feeding] A file was just uploaded and is awaiting approval";
                string body = "This is an automated notification.<br /><br />";
                body += fs.UploadedBy.Headline 
                    + " just uploaded the file <a href='" + APPLICATION_URL + "Sources/Feeding/Details/" + fs.Id.ToString() + "'>" + fs.Name + "</a>";
                if (fs.Restricted)
                {
                    body += " (restricted)";
                }
                body += ", which is awaiting approval.";
                if (!string.IsNullOrEmpty(fs.UploadNotes))
                {
                    body += "<br /><br />Notes from uploader:";
                    body += "<br /><br />" + fs.UploadNotes.Replace("\r\n", "<br />").Replace("\n", "<br />");
                }

                this.SendMailMessage(this.userTasks.GetUsersWithPermission(AdminPermission.CanApproveAndRejectSources),
                    this.userTasks.GetAdminUser(fs.UploadedBy.UserID), 
                    subject, body, true);
            }
            else
            {
                log.Error("Problem with FeedingSource(ID=" + fs.Id + "), didn't send 'FeedingSourceUploaded' email.");
            }
        }

        public void SendFeedingSourcesUploadedEmail(IList<FeedingSource> sources)
        {
            if (sources != null && sources.Count > 0)
            {
                if (sources.Count == 1)
                {
                    this.SendFeedingSourceUploadedEmail(sources.First());
                }
                else
                {
                    FeedingSource firstSource = sources.First();

                    string subject = "[feeding] Several files were just uploaded and are awaiting approval";
                    string body = "This is an automated notification.<br /><br />";
                    body += firstSource.UploadedBy.Headline + " just uploaded the following files which are awaiting approval:<br /><br />";
                    body += "<ul>";
                    foreach (FeedingSource fs in sources)
                    {
                        body += "<li><a href='" + APPLICATION_URL + "Sources/Feeding/Details/" + fs.Id.ToString() + "'>" + fs.Name + "</a>";
                        if (fs.Restricted)
                        {
                            body += " (restricted)";
                        }
                        body += "</li>";
                    }
                    body += "</ul>";
                    if (!string.IsNullOrEmpty(firstSource.UploadNotes))
                    {
                        body += "<br /><br />Notes from uploader:";
                        body += "<br /><br />" + firstSource.UploadNotes.Replace("\r\n", "<br />").Replace("\n", "<br />");
                    }

                    this.SendMailMessage(this.userTasks.GetUsersWithPermission(AdminPermission.CanApproveAndRejectSources),
                        this.userTasks.GetAdminUser(firstSource.UploadedBy.UserID),
                        subject, body, true);
                }
            }
        }

        public void SendFeedingSourceApprovedEmail(FeedingSource fs)
        {
            if (fs != null && fs.ApprovedBy != null && fs.Source != null)
            {
                string subject = "[feeding] Your uploaded file was just approved and is available for attaching";
                string body = "This is an automated notification.<br /><br />";
                body += fs.ApprovedBy.Headline
                    + " just approved the file <a href='" + APPLICATION_URL + "Sources/Feeding/Details/" + fs.Id.ToString() + "'>"
                    + fs.Name + "</a>";
                if (fs.Restricted)
                {
                    body += " (restricted)";
                }
                body += ", which is available for attaching to persons, events or units.  Source ID is <a href='" + APPLICATION_URL + "Profiling/Sources#info/" + fs.Source.Id + "'>" + fs.Source.Id + "</a>.";

                this.SendMailMessage(new List<AdminUser>() { this.userTasks.GetAdminUser(fs.UploadedBy.UserID) },
                        this.userTasks.GetAdminUser(fs.ApprovedBy.UserID),
                        subject, body, true);
            }
            else
            {
                log.Error("Problem with FeedingSource(ID=" + fs.Id + "), didn't send 'FeedingSourceApproved' email.");
            }
        }

        public void SendFeedingSourceRejectedEmail(FeedingSource fs)
        {
            if (fs != null && fs.RejectedBy != null)
            {
                string subject = "[feeding] Your uploaded file was just rejected";
                string body = "This is an automated notification.<br /><br />";
                body += fs.RejectedBy.Headline 
                    + " just rejected the file <a href='" + APPLICATION_URL + "Sources/Feeding/Details/" + fs.Id.ToString() + "'>" + fs.Name + "</a>";
                if (fs.Restricted)
                {
                    body += " (restricted)";
                }
                body += ".";
                if (!string.IsNullOrEmpty(fs.RejectedReason))
                {
                    body += "<br /><br />Reason:";
                    body += "<br /><br />" + fs.RejectedReason;
                }

                this.SendMailMessage(new List<AdminUser>() { this.userTasks.GetAdminUser(fs.UploadedBy.UserID) },
                        this.userTasks.GetAdminUser(fs.RejectedBy.UserID),
                        subject, body, true);
            }
            else
            {
                log.Error("Problem with FeedingSource(ID=" + fs.Id + "), didn't send 'FeedingSourceRejected' email.");
            }
        }
    }
}
