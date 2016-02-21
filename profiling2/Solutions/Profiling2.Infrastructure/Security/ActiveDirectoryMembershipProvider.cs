using System;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.Web.Security;
using log4net;
using Microsoft.Practices.ServiceLocation;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Prf;

namespace Profiling2.Infrastructure.Security
{
    public class ActiveDirectoryMembershipProvider : MembershipProvider
    {
        protected readonly static ILog log = LogManager.GetLogger(typeof(ActiveDirectoryMembershipProvider));

        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override bool EnablePasswordReset
        {
            get { throw new NotImplementedException(); }
        }

        public override bool EnablePasswordRetrieval
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override string GetUserNameByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredPasswordLength
        {
            get { throw new NotImplementedException(); }
        }

        public override int PasswordAttemptWindow
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { throw new NotImplementedException(); }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresUniqueEmail
        {
            get { throw new NotImplementedException(); }
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }

        public override bool ValidateUser(string username, string password)
        {
            string domain = ConfigurationManager.AppSettings["ActiveDirectoryDomainName"];
            string activeDirectoryUser = ConfigurationManager.AppSettings["ActiveDirectoryUser"];

            using (PrincipalContext pc = string.IsNullOrEmpty(activeDirectoryUser)
                ? new PrincipalContext(ContextType.Domain, domain)
                : new PrincipalContext(ContextType.Domain, domain, 
                    activeDirectoryUser, ConfigurationManager.AppSettings["ActiveDirectoryUserPassword"])
                )
            {
                bool validated = pc.ValidateCredentials(username, password, ContextOptions.Negotiate);
                if (validated)
                {
                    // attempt to update local AdminUser.UserID and AdminUser.UserName
                    UserPrincipal up = UserPrincipal.FindByIdentity(pc, username);
                    if (up != null)
                    {
                        try
                        {
                            AdminUser user = this.GetUserTasks().GetAdminUser(username);

                            if (user == null)
                            {
                                // get user's old user ID if exists...
                                string oldUserId = (string)up.GetType().GetProperty(ConfigurationManager.AppSettings["ActiveDirectoryUserIDFieldName"]).GetValue(up, null);

                                if (!string.IsNullOrEmpty(oldUserId))
                                {
                                    user = this.GetUserTasks().GetAdminUser(oldUserId);

                                    if (user == null)
                                    {
                                        // we don't create users here
                                    }
                                    else
                                    {
                                        // update the user's ID, Name and email
                                        log.Info("Updating " + oldUserId + "'s UserID to " + username + ".");
                                        this.GetUserTasks().UpdateUser(oldUserId, username, up.GivenName, up.Surname, up.EmailAddress);
                                    }
                                }
                            }
                            else
                            {
                                // always update name and email from AD
                                this.GetUserTasks().UpdateUser(username, null, up.GivenName, up.Surname, up.EmailAddress);
                            }
                        }
                        catch (Exception e)
                        {
                            log.Error("Error updating attributes from Active Directory for: " + username, e);
                        }
                    }
                }
                return validated;
            }
        }

        private IUserTasks GetUserTasks()
        {
            return ServiceLocator.Current.GetInstance<IUserTasks>();
        }
    }
}
