using SharpArch.Web.Mvc.Castle;

namespace Profiling2.Web.Mvc.CastleWindsor
{
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using HrdbWebServiceClient.Contracts;
    using HrdbWebServiceClient.Infrastructure;
    using NHibernate;
    using Profiling2.Domain.Contracts.Queries.Audit;
    using Profiling2.Domain.Contracts.Search;
    using Profiling2.Domain.Prf.Actions;
    using Profiling2.Domain.Prf.Careers;
    using Profiling2.Domain.Prf.Persons;
    using Profiling2.Domain.Prf.Responsibility;
    using Profiling2.Infrastructure.Queries.Audit;
    using Profiling2.Infrastructure.Search;
    using SharpArch.Domain.PersistenceSupport;
    using SharpArch.NHibernate;
    using SharpArch.NHibernate.Contracts.Repositories;

    public class ComponentRegistrar
    {
        public static void AddComponentsTo(IWindsorContainer container) 
        {
            AddGenericRepositoriesTo(container);
            AddCustomRepositoriesTo(container);
            AddQueryObjectsTo(container);
            AddIndexersTo(container);
            AddTasksTo(container);
            AddCommandsTo(container);
        }

        private static void AddTasksTo(IWindsorContainer container)
        {
            container.Register(
                Types
                    .FromAssemblyNamed("Profiling2.Tasks")
                    .Pick()
                    .WithService.FirstNonGenericCoreInterface("Profiling2.Domain"));
        }

        private static void AddCustomRepositoriesTo(IWindsorContainer container) 
        {
            container.Register(
                Types
                    .FromAssemblyNamed("Profiling2.Infrastructure")
                    .Pick()
                    .Unless(t => t.BaseType == typeof(BaseIndexer))
                    .WithService.FirstNonGenericCoreInterface("Profiling2.Domain"));

            // Manually register HrdbWebServiceClient repository.
            container.Register(Component.For<IHrdbEntitiesRepository>().ImplementedBy<HrdbEntitiesRepository>());
        }

        private static void AddIndexersTo(IWindsorContainer container)
        {
            container.Register(
                Component.For(typeof(ILuceneIndexer<PersonIndexer>)).ImplementedBy<PersonIndexer>(),
                Component.For(typeof(ILuceneIndexer<EventIndexer>)).ImplementedBy<EventIndexer>(),
                Component.For(typeof(ILuceneIndexer<UnitIndexer>)).ImplementedBy<UnitIndexer>(),
                Component.For(typeof(ILuceneIndexer<RequestIndexer>)).ImplementedBy<RequestIndexer>(),
                Component.For(typeof(ILuceneIndexer<SourceIndexer>)).ImplementedBy<SourceIndexer>(),
                Component.For(typeof(IScreeningResponseIndexer)).ImplementedBy<ScreeningResponseIndexer>()
                );
        }

        private static void AddGenericRepositoriesTo(IWindsorContainer container)
        {
            container.Register(
                Component.For(typeof(IPersonAuditable<Person>))
                    .ImplementedBy<PersonRevisionsQuery>(),
                Component.For(typeof(IPersonAuditable<Career>))
                    .ImplementedBy<CareerRevisionsQuery>(),
                Component.For(typeof(IPersonAuditable<PersonAlias>))
                    .ImplementedBy<PersonAliasRevisionsQuery>(),
                Component.For(typeof(IPersonAuditable<PersonSource>))
                    .ImplementedBy<PersonSourceRevisionsQuery>(),
                Component.For(typeof(IPersonAuditable<PersonPhoto>))
                    .ImplementedBy<PersonPhotoRevisionsQuery>(),
                Component.For(typeof(IPersonAuditable<PersonRelationship>))
                    .ImplementedBy<PersonRelationshipRevisionsQuery>(),
                Component.For(typeof(IPersonAuditable<ActionTaken>))
                    .ImplementedBy<ActionTakenRevisionsQuery>(),
                Component.For(typeof(IPersonAuditable<PersonResponsibility>))
                    .ImplementedBy<PersonResponsibilityRevisionsQuery>(),
                Component.For(typeof(IPersonAuditable<PersonRestrictedNote>))
                    .ImplementedBy<PersonRestrictedNoteRevisionsQuery>());

            container.Register(
                Component.For(typeof(IQuery))
                    .ImplementedBy(typeof(NHibernateQuery))
                    .Named("NHibernateQuery"));

            container.Register(
                Component.For(typeof(IEntityDuplicateChecker))
                    .ImplementedBy(typeof(EntityDuplicateChecker))
                    .Named("entityDuplicateChecker"));

            container.Register(
                Component.For(typeof(INHibernateRepository<>))
                    .ImplementedBy(typeof(NHibernateRepository<>))
                    .Named("nhibernateRepositoryType")
                    .Forward(typeof(IRepository<>)));

            container.Register(
                Component.For(typeof(INHibernateRepositoryWithTypedId<,>))
                    .ImplementedBy(typeof(NHibernateRepositoryWithTypedId<,>))
                    .Named("nhibernateRepositoryWithTypedId")
                    .Forward(typeof(IRepositoryWithTypedId<,>)));

            container.Register(
                    Component.For(typeof(ISessionFactoryKeyProvider))
                        .ImplementedBy(typeof(DefaultSessionFactoryKeyProvider))
                        .Named("sessionFactoryKeyProvider"));

            container.Register(
                    Component.For(typeof(SharpArch.Domain.Commands.ICommandProcessor))
                        .ImplementedBy(typeof(SharpArch.Domain.Commands.CommandProcessor))
                        .Named("commandProcessor"));
                
        }

        private static void AddQueryObjectsTo(IWindsorContainer container) 
        {
            container.Register(
                Types.FromAssemblyNamed("Profiling2.Web.Mvc")
                    .Pick()
                    .WithService.FirstInterface());
        }

        private static void AddCommandsTo(IWindsorContainer container)
        {
            container.Register(
                Types.FromAssemblyNamed("Profiling2.Tasks")
                    .Pick()
                    .WithService.FirstInterface());
        }
    }
}