
namespace Profiling2.Web.Mvc
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.Security;
    using AutoMapper;
    using Castle.Windsor;
    // Profiling2.Web.Mvc.CastleWindsor
    using CastleWindsor;
    using CommonServiceLocator.WindsorAdapter;
    using Controllers;
    using Hangfire;
    using Hangfire.Windsor;
    using Infrastructure.NHibernateMaps;
    using log4net.Config;
    using Microsoft.Practices.ServiceLocation;
    using Newtonsoft.Json;
    using NHibernate.Cfg;
    using NHibernate.Envers.Configuration;
    using NHibernate.Envers.Configuration.Attributes;
    using Profiling2.Domain.Contracts.Tasks;
    using Profiling2.Domain.Prf;
    using Profiling2.Domain.Prf.Actions;
    using Profiling2.Domain.Prf.Careers;
    using Profiling2.Domain.Prf.Events;
    using Profiling2.Domain.Prf.Organizations;
    using Profiling2.Domain.Prf.Persons;
    using Profiling2.Domain.Prf.Units;
    using Profiling2.Domain.Scr;
    using Profiling2.Infrastructure.Aspose.Licenses;
    using Profiling2.Infrastructure.Security;
    using Profiling2.Infrastructure.Security.Identity;
    using Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels;
    using Profiling2.Web.Mvc.Areas.Screening.Controllers.ViewModels;
    using Profiling2.Web.Mvc.Controllers.ModelBinders;
    using SharpArch.NHibernate;
    using SharpArch.NHibernate.Web.Mvc;
    using SharpArch.Web.Mvc.Castle;
    using SharpArch.Web.Mvc.ModelBinder;
    using StackExchange.Profiling;
    

    /// <summary>
    /// Represents the MVC Application
    /// </summary>
    /// <remarks>
    /// For instructions on enabling IIS6 or IIS7 classic mode, 
    /// visit http://go.microsoft.com/?LinkId=9394801
    /// </remarks>
    public class MvcApplication : System.Web.HttpApplication
    {
        private WebSessionStorage webSessionStorage;

        /// <summary>
        /// Due to issues on IIS7, the NHibernate initialization must occur in Init().
        /// But Init() may be invoked more than once; accordingly, we introduce a thread-safe
        /// mechanism to ensure it's only initialized once.
        /// See http://msdn.microsoft.com/en-us/magazine/cc188793.aspx for explanation details.
        /// </summary>
        public override void Init()
        {
            base.Init();
            this.webSessionStorage = new WebSessionStorage(this);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            NHibernateInitializer.Instance().InitializeNHibernateOnce(this.InitialiseNHibernateSessions);

            if (Request.IsLocal)
            {
                // don't enable MiniProfiler for scheduled actions, which are called locally.
                if (Request.Path.IndexOf("Scheduled") < 0)
                {
                    MiniProfiler.Start();
                    MiniProfiler.Settings.MaxJsonResponseSize = 2147483647;
                }
            }
        }

        protected void Application_EndRequest()
        {
            MiniProfiler.Stop();
        }

        protected void Application_Error(object sender, EventArgs e) 
        {
            // Useful for debugging
            Exception ex = this.Server.GetLastError();
            var reflectionTypeLoadException = ex as ReflectionTypeLoadException;

            // Don't send compressed garbage on error (since using IIS Express/IIS7).
            // explanation: http://weblog.west-wind.com/posts/2011/May/02/ASPNET-GZip-Encoding-Caveats
            HttpApplication app = sender as HttpApplication;
            if (app != null && app.Response != null)
                app.Response.Filter = null;
        }

        protected void Application_Start()
        {
            XmlConfigurator.Configure();

            ViewEngines.Engines.Clear();

            ViewEngines.Engines.Add(new RazorViewEngine());

            ModelBinders.Binders.DefaultBinder = new SharpModelBinder();

            ModelBinders.Binders.Add(typeof(string), new TrimModelBinder());

            ModelBinders.Binders.Add(typeof(SourceDataTablesParam), new SourceDataTablesModelBinder());

            ModelValidatorProviders.Providers.Add(new ClientDataTypeModelValidatorProvider());

            this.InitializeServiceLocator();

            AreaRegistration.RegisterAllAreas();
            RouteRegistrar.RegisterRoutesTo(RouteTable.Routes);

            AsposeInitialiser.SetLicenses();

            this.InitialiseAutoMappings();

            // avoid problems transforming NHibernate objects to json
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new NHibernateContractResolver()
            };
        }

        /// <summary>
        /// Instantiate the container and add all Controllers that derive from
        /// WindsorController to the container.  Also associate the Controller
        /// with the WindsorContainer ControllerFactory.
        /// </summary>
        protected virtual void InitializeServiceLocator() 
        {
            IWindsorContainer container = new WindsorContainer();

            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(container));

            container.RegisterControllers(typeof(HomeController).Assembly);
            ComponentRegistrar.AddComponentsTo(container);

            // HangFire job activator
            JobActivator.Current = new WindsorJobActivator(container.Kernel);

            ServiceLocator.SetLocatorProvider(() => new WindsorServiceLocator(container));
        }

        private void InitialiseNHibernateSessions()
        {
            NHibernateSession.ConfigurationCache = new NHibernateConfigurationFileCache();

            // Uses customised NHibernateSession.
            // TODO waiting for SharpArch support: https://github.com/sharparchitecture/Sharp-Architecture/issues/83,
            // so we don't have to use custom SharpArch.NHibernate assembly.
            Action<Configuration> customConfiguration = (
                cfg =>
                {
                    cfg.SetEnversProperty(ConfigurationKey.StoreDataAtDelete, true);
                    cfg.IntegrateWithEnvers(new AttributeConfiguration());

                    // use MiniProfiler.NHibernate driver for basic SQL query profiling
                    cfg.SetProperty(NHibernate.Cfg.Environment.ConnectionDriver, typeof(StackExchange.Profiling.NHibernate.Drivers.MiniProfilerSql2008ClientDriver).AssemblyQualifiedName);
                }
            );

            NHibernateSession.Init(
                this.webSessionStorage,
                new[] { Server.MapPath("~/bin/Profiling2.Infrastructure.dll") },
                new AutoPersistenceModelGenerator().Generate(),
                Server.MapPath("~/NHibernate.config"),
                customConfiguration);
        }

        private void InitialiseAutoMappings()
        {
            Mapper.CreateMap<Request, RequestViewModel>();
            Mapper.CreateMap<RequestViewModel, Request>().ForMember(x => x.RequestAttachments, opt => opt.Ignore());

            Mapper.CreateMap<Violation, ViolationViewModel>();
            Mapper.CreateMap<Event, EventViewModel>();
            Mapper.CreateMap<EventViewModel, Event>();

            Mapper.CreateMap<Person, PersonViewModel>();
            Mapper.CreateMap<PersonViewModel, Person>();

            Mapper.CreateMap<PersonAlias, PersonAliasViewModel>();
            Mapper.CreateMap<PersonAliasViewModel, PersonAlias>();

            Mapper.CreateMap<PersonRelationship, PersonRelationshipViewModel>();
            Mapper.CreateMap<PersonRelationshipViewModel, PersonRelationship>();

            Mapper.CreateMap<LocationViewModel, Location>();
            Mapper.CreateMap<Location, LocationViewModel>();
            Mapper.CreateMap<LocationMergeViewModel, Location>();
            Mapper.CreateMap<Location, LocationMergeViewModel>();

            Mapper.CreateMap<Career, CareerViewModel>();
            Mapper.CreateMap<CareerViewModel, Career>();

            Mapper.CreateMap<Organization, OrganizationViewModel>();
            Mapper.CreateMap<OrganizationViewModel, Organization>();

            Mapper.CreateMap<Role, RoleViewModel>();
            Mapper.CreateMap<RoleViewModel, Role>();

            Mapper.CreateMap<Rank, RankViewModel>();
            Mapper.CreateMap<RankViewModel, Rank>();

            Mapper.CreateMap<Unit, UnitViewModel>();
            Mapper.CreateMap<UnitViewModel, Unit>();

            Mapper.CreateMap<UnitHierarchy, UnitHierarchyViewModel>();
            Mapper.CreateMap<UnitHierarchyViewModel, UnitHierarchy>();

            Mapper.CreateMap<UnitLocation, UnitLocationViewModel>();
            Mapper.CreateMap<UnitLocationViewModel, UnitLocation>();

            Mapper.CreateMap<UnitAlias, UnitAliasViewModel>();
            Mapper.CreateMap<UnitAliasViewModel, UnitAlias>();

            Mapper.CreateMap<ActionTakenViewModel, ActionTaken>();
            Mapper.CreateMap<ActionTaken, ActionTakenViewModel>();

            Mapper.CreateMap<EventRelationship, EventRelationshipViewModel>();
            Mapper.CreateMap<EventRelationshipViewModel, EventRelationship>();

            Mapper.CreateMap<Ethnicity, EthnicityViewModel>();
            Mapper.CreateMap<EthnicityViewModel, Ethnicity>();

            Mapper.CreateMap<Operation, OperationViewModel>();
            Mapper.CreateMap<OperationViewModel, Operation>();

            Mapper.CreateMap<OperationAlias, OperationAliasViewModel>();
            Mapper.CreateMap<OperationAliasViewModel, OperationAlias>();

            Mapper.CreateMap<UnitOperation, UnitOperationViewModel>();
            Mapper.CreateMap<UnitOperationViewModel, UnitOperation>();
        }

        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            if (Request.IsAuthenticated)
            {
                string username = HttpContext.Current.User.Identity.Name;
                var identity = new ExpandedIdentity(username);

                AdminUser user = ServiceLocator.Current.GetInstance<IUserTasks>().GetAdminUser(username);
                string[] permissions = user != null && user.AdminRoles.Any() ? user.AdminRoles
                    .Select(x => x.AdminPermissions)
                    .Aggregate((x, y) => x.Concat(y).ToList())
                    .Select(z => z.Name)
                    .Distinct()
                    .ToArray() : new string[] { };
                var principal = new PrfPrincipal(identity, 
                    Roles.Provider.GetRolesForUser(username), 
                    permissions, 
                    user != null && user.Affiliations != null ? user.Affiliations.Select(x => x.Name).ToArray() : new string[] { }
                    );

                HttpContext.Current.User = principal;
                Thread.CurrentPrincipal = principal;
            }
        }
    }
}