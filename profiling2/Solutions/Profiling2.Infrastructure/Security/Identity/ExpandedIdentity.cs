using System;
using System.Security.Principal;
using Microsoft.Practices.ServiceLocation;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Prf;

namespace Profiling2.Infrastructure.Security.Identity
{
    /// <summary>
    /// <para>Custom Identity containing expanded name attributes; intended as performance saver when displaying full names in headers
    /// that appear on every page.</para>
    /// <para>May be extended in order to add attributes if desired (see MonuscoIdentity for example).</para> 
    /// <para>Hassles:
    /// 1. Inherits MarshalByRefObject in order to get around Cassini/ASP.NET Development Server issue with custom identities:
    ///   http://connect.microsoft.com/VisualStudio/feedback/details/274696/using-custom-identities-in-asp-net-fails-when-using-the-asp-net-developement-server
    ///   http://www.lhotka.net/weblog/CommentView,guid,cfcaf6c4-63cf-4cf1-8361-ed3db07496a4.aspx
    /// 2. No longer inherits from GenericIdentity - breaks in .NET 4.5 when ClaimsIdentity was introduced, even if .NET 4.0 targetted.
    ///   http://stackoverflow.com/questions/19091534/claimsidentity-nullreferenceexception-after-installing-net-4-5
    /// </para>
    /// </summary>
    public class ExpandedIdentity : MarshalByRefObject, IIdentity
    {
        public string Name { get; private set; }

        public string AuthenticationType { get; private set; }

        public bool IsAuthenticated
        {
            get { return !string.IsNullOrEmpty(Name); }
        }

        // TODO since this class is instantiated on every request (see Global.asax.cs), this DB call occurs once every request.
        private string _fullName { get; set; }
        public virtual string FullName
        {
            get 
            {
                if (this._fullName == null)
                {
                    AdminUser user = ServiceLocator.Current.GetInstance<IUserTasks>().GetAdminUser(this.Name);
                    if (user != null)
                        this._fullName = user.UserName;
                }
                return this._fullName;
            }
        }

        public virtual string DisplayName
        {
            get
            {
                string displayName = this.FullName;
                if (!string.IsNullOrEmpty(this.FullName))
                {
                    displayName += " (" + this.Name + ")";
                }
                else
                {
                    displayName += this.Name;
                }
                return displayName;
            }
        }

        public ExpandedIdentity(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            this.Name = name;
            this.AuthenticationType = string.Empty;
        }

        public ExpandedIdentity(string name, string authenticationType)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (authenticationType == null)
                throw new ArgumentNullException("authenticationType");

            this.Name = name;
            this.AuthenticationType = authenticationType;
        }
    }
}
