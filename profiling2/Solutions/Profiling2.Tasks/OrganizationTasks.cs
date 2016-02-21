using System.Collections.Generic;
using System.Linq;
using Hangfire;
using log4net;
using NHibernate;
using Profiling2.Domain.Contracts.Queries;
using Profiling2.Domain.Contracts.Queries.Procs;
using Profiling2.Domain.Contracts.Queries.Search;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.DTO;
using Profiling2.Domain.Prf.Careers;
using Profiling2.Domain.Prf.Organizations;
using Profiling2.Domain.Prf.Units;
using SharpArch.NHibernate;
using SharpArch.NHibernate.Contracts.Repositories;

namespace Profiling2.Tasks
{
    public class OrganizationTasks : IOrganizationTasks
    {
        protected readonly ILog log = LogManager.GetLogger(typeof(OrganizationTasks));
        protected readonly INHibernateRepository<Organization> orgRepo;
        protected readonly INHibernateRepository<Unit> unitRepo;
        protected readonly INHibernateRepository<Rank> rankRepo;
        protected readonly INHibernateRepository<Role> roleRepo;
        protected readonly INHibernateRepository<AdminUnitImport> adminUnitImportRepo;
        protected readonly INHibernateRepository<UnitHierarchy> unitHierarchyRepo;
        protected readonly INHibernateRepository<UnitHierarchyType> unitHierarchyTypeRepo;
        protected readonly INHibernateRepository<UnitLocation> unitLocationRepo;
        protected readonly INHibernateRepository<UnitAlias> unitAliasRepo;
        protected readonly INHibernateRepository<UnitOperation> unitOperationRepo;
        protected readonly INHibernateRepository<Operation> operationRepo;
        protected readonly INHibernateRepository<OperationAlias> operationAliasRepo;
        protected readonly IOrganizationSearchQuery orgSearchQuery;
        protected readonly IUnitQueries unitQueries;
        protected readonly IUnitSearchQuery unitSearchQuery;
        protected readonly IRankSearchQuery rankSearchQuery;
        protected readonly IRoleSearchQuery roleSearchQuery;
        protected readonly ILuceneTasks luceneTasks;
        protected readonly IMergeStoredProcQueries mergeQueries;

        public OrganizationTasks(INHibernateRepository<Organization> orgRepo,
            INHibernateRepository<Unit> unitRepo,
            INHibernateRepository<Rank> rankRepo,
            INHibernateRepository<Role> roleRepo,
            INHibernateRepository<AdminUnitImport> adminUnitImportRepo,
            INHibernateRepository<UnitHierarchy> unitHierarchyRepo,
            INHibernateRepository<UnitHierarchyType> unitHierarchyTypeRepo,
            INHibernateRepository<UnitLocation> unitLocationRepo,
            INHibernateRepository<UnitAlias> unitAliasRepo,
            INHibernateRepository<UnitOperation> unitOperationRepo,
            INHibernateRepository<Operation> operationRepo,
            INHibernateRepository<OperationAlias> operationAliasRepo,
            IOrganizationSearchQuery orgSearchQuery,
            IUnitQueries unitQueries,
            IUnitSearchQuery unitSearchQuery,
            IRankSearchQuery rankSearchQuery,
            IRoleSearchQuery roleSearchQuery,
            ILuceneTasks luceneTasks,
            IMergeStoredProcQueries mergeQueries)
        {
            this.orgRepo = orgRepo;
            this.unitRepo = unitRepo;
            this.rankRepo = rankRepo;
            this.roleRepo = roleRepo;
            this.adminUnitImportRepo = adminUnitImportRepo;
            this.unitHierarchyRepo = unitHierarchyRepo;
            this.unitHierarchyTypeRepo = unitHierarchyTypeRepo;
            this.unitLocationRepo = unitLocationRepo;
            this.unitAliasRepo = unitAliasRepo;
            this.unitOperationRepo = unitOperationRepo;
            this.operationRepo = operationRepo;
            this.operationAliasRepo = operationAliasRepo;
            this.orgSearchQuery = orgSearchQuery;
            this.unitQueries = unitQueries;
            this.unitSearchQuery = unitSearchQuery;
            this.rankSearchQuery = rankSearchQuery;
            this.roleSearchQuery = roleSearchQuery;
            this.luceneTasks = luceneTasks;
            this.mergeQueries = mergeQueries;
        }

        public Organization GetOrganization(int orgId)
        {
            return this.orgRepo.Get(orgId);
        }

        protected Organization GetOrganization(string name)
        {
            IDictionary<string, object> criteria = new Dictionary<string, object>();
            criteria.Add("OrgShortName", name);
            IList<Organization> candidates = this.orgRepo.FindAll(criteria);
            if (candidates != null && candidates.Any())
                return candidates[0];

            criteria.Remove("OrgShortName");
            criteria.Add("OrgLongName", name);
            candidates = this.orgRepo.FindAll(criteria);
            if (candidates != null && candidates.Any())
                return candidates[0];

            return null;
        }

        public IList<Organization> GetAllOrganizations()
        {
            return this.orgRepo.GetAll();
        }

        public IList<Organization> GetOrganizationsByName(string term)
        {
            return this.orgSearchQuery.GetResults(term);
        }

        public Organization GetOrCreateOrganization(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                Organization o = this.GetOrganization(name);
                if (o == null)
                    o = new Organization()
                        {
                            OrgShortName = name,
                            OrgLongName = name
                        };
                return o;
            }

            return null;
        }

        public Organization SaveOrganization(Organization o)
        {
            this.luceneTasks.UpdatePersons(o);
            this.luceneTasks.UpdateUnits(o);
            return this.orgRepo.SaveOrUpdate(o);
        }

        public bool DeleteOrganization(Organization o)
        {
            if (o != null && o.Careers.Count < 1 && o.Units.Count < 1
                && o.OrganizationResponsibilities.Count < 1
                && o.OrganizationRelationshipsAsSubject.Count < 1
                && o.OrganizationRelationshipsAsObject.Count < 1
                && o.OrganizationPhotos.Count < 1
                && o.OrganizationAliases.Count < 1)
            {
                this.orgRepo.Delete(o);
                return true;
            }
            return false;
        }

        public Unit GetUnit(int unitId)
        {
            return this.unitRepo.Get(unitId);
        }

        private Unit GetUnit(ISession session, int unitId)
        {
            return this.unitQueries.GetUnit(session, unitId);
        }

        public Unit GetUnit(string name)
        {
            IDictionary<string, object> criteria = new Dictionary<string, object>();
            criteria.Add("UnitName", name);
            IList<Unit> results = this.unitRepo.FindAll(criteria);
            if (results != null && results.Any())
                return results.First();
            return null;
        }

        public IList<Unit> GetAllUnits()
        {
            return this.GetAllUnits(null);
        }

        public IList<Unit> GetAllUnits(ISession session)
        {
            return this.unitQueries.GetAllUnits(session);
        }

        public IList<Unit> GetUnitsByName(string term)
        {
            return this.unitSearchQuery.GetResults(term);
        }

        public bool DeleteUnit(Unit unit)
        {
            // we force the user to manually remove these links. others such as UnitLocations and UnitSources may be deleted by cascade.
            if (unit != null && unit.Careers.Count == 0 && unit.OrganizationResponsibilities.Count == 0
                && unit.UnitHierarchies.Where(x => x.UnitHierarchyType.UnitHierarchyTypeName == UnitHierarchyType.NAME_HIERARCHY 
                    || x.UnitHierarchyType.UnitHierarchyTypeName == UnitHierarchyType.NAME_CHANGED_NAME_TO).Count() == 0
                && unit.UnitHierarchyChildren.Where(x => x.UnitHierarchyType.UnitHierarchyTypeName == UnitHierarchyType.NAME_HIERARCHY
                    || x.UnitHierarchyType.UnitHierarchyTypeName == UnitHierarchyType.NAME_CHANGED_NAME_TO).Count() == 0
                && unit.UnitAliases.Count == 0
                && unit.UnitOperations.Count == 0
                && unit.RequestUnits.Count == 0)
            {
                unit.AdminUnitImports.Clear();
                unit.UnitHierarchies.Clear();  // clear UnitHierarchies not checked above
                unit.UnitHierarchyChildren.Clear();

                this.log.Info("Deleting Unit, Id: " + unit.Id + ", Name: " + unit.UnitName);
                this.luceneTasks.DeleteUnit(unit.Id);
                this.unitRepo.Delete(unit);
                return true;
            }
            return false;
        }

        public Unit SaveUnit(Unit unit)
        {
            unit = this.unitRepo.SaveOrUpdate(unit);

            BackgroundJob.Enqueue<IOrganizationTasks>(x => x.LuceneUpdateUnitQueueable(unit.Id));

            return unit;
        }

        public void LuceneUpdateUnitQueueable(int unitId)
        {
            using (ISession session = NHibernateSession.GetDefaultSessionFactory().OpenSession())
            {
                Unit unit = this.GetUnit(session, unitId);
                this.luceneTasks.UpdateUnit(unit);
                this.luceneTasks.UpdatePersons(unit);
            }
        }

        public IList<UnitDTO> GetEmptyUnits()
        {
            return this.unitQueries.GetEmptyUnits();
        }

        // Bulk-run that populates Unit.Organization using Unit.Careers.Organization
        public void PopulateUnitOrganization()
        {
            IList<Unit> units = this.GetAllUnits().Where(x => x.Organization == null).ToList();
            foreach (Unit u in units)
            {
                IList<Organization> orgs = u.Careers.Where(x => x.Organization != null).Select(x => x.Organization).Distinct().ToList();
                // set Unit.Organization if all its career.Organizations are the same
                if (orgs.Count == 1)
                {
                    u.Organization = orgs.First();
                    this.unitRepo.SaveOrUpdate(u);
                }
            }
        }

        public UnitHierarchy GetUnitHierarchy(int id)
        {
            return this.unitHierarchyRepo.Get(id);
        }

        public UnitHierarchy GetUnitHierarchy(int unitId, int parentId)
        {
            Unit unit = this.GetUnit(unitId);
            Unit parentUnit = this.GetUnit(parentId);

            if (unit != null && parentUnit != null)
            {
                IDictionary<string, object> criteria = new Dictionary<string, object>();
                criteria.Add("Unit", unit);
                criteria.Add("ParentUnit", parentUnit);
                IList<UnitHierarchy> list = this.unitHierarchyRepo.FindAll(criteria);
                if (list != null && list.Any())
                    return list[0];
            }
            return null;
        }

        public UnitHierarchy SaveUnitHierarchy(UnitHierarchy uh)
        {
            if (uh.ParentUnit != null)
                BackgroundJob.Enqueue<IOrganizationTasks>(x => x.LuceneUpdateUnitQueueable(uh.ParentUnit.Id));

            if (uh.Unit != null)
                BackgroundJob.Enqueue<IOrganizationTasks>(x => x.LuceneUpdateUnitQueueable(uh.Unit.Id));

            return this.unitHierarchyRepo.SaveOrUpdate(uh);
        }

        public void DeleteUnitHierarchy(UnitHierarchy uh)
        {
            if (uh != null)
            {
                if (uh.ParentUnit != null)
                {
                    uh.ParentUnit.RemoveUnitHierarchy(uh);

                    BackgroundJob.Enqueue<IOrganizationTasks>(x => x.LuceneUpdateUnitQueueable(uh.ParentUnit.Id));
                }
                if (uh.Unit != null)
                {
                    uh.Unit.RemoveUnitHierarchy(uh);

                    BackgroundJob.Enqueue<IOrganizationTasks>(x => x.LuceneUpdateUnitQueueable(uh.Unit.Id));
                }
                this.unitHierarchyRepo.Delete(uh);
            }
        }

        public UnitHierarchyType GetUnitHierarchyType(int id)
        {
            return this.unitHierarchyTypeRepo.Get(id);
        }

        public UnitHierarchyType GetUnitHierarchyType(string name)
        {
            IDictionary<string, object> criteria = new Dictionary<string, object>();
            criteria.Add("UnitHierarchyTypeName", name);
            return this.unitHierarchyTypeRepo.FindOne(criteria);
        }

        public IList<UnitHierarchyType> GetUnitHierarchyTypes()
        {
            return this.unitHierarchyTypeRepo.GetAll();
        }

        public UnitLocation GetUnitLocation(int id)
        {
            return this.unitLocationRepo.Get(id);
        }

        public UnitLocation SaveUnitLocation(UnitLocation ul)
        {
            return this.unitLocationRepo.SaveOrUpdate(ul);
        }

        public void DeleteUnitLocation(UnitLocation ul)
        {
            if (ul != null)
                this.unitLocationRepo.Delete(ul);
        }

        public UnitAlias GetUnitAlias(int id)
        {
            return this.unitAliasRepo.Get(id);
        }

        public UnitAlias SaveUnitAlias(UnitAlias ua)
        {
            ua = this.unitAliasRepo.SaveOrUpdate(ua);

            BackgroundJob.Enqueue<IOrganizationTasks>(x => x.LuceneUpdateUnitQueueable(ua.Unit.Id));

            return ua;
        }

        public void DeleteUnitAlias(UnitAlias ua)
        {
            if (ua != null)
            {
                this.unitAliasRepo.Delete(ua);

                BackgroundJob.Enqueue<IOrganizationTasks>(x => x.LuceneUpdateUnitQueueable(ua.Unit.Id));
            }
        }

        public Rank GetRank(int id)
        {
            return this.rankRepo.Get(id);
        }

        public Rank GetRank(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                IDictionary<string, object> criteria = new Dictionary<string, object>();
                criteria.Add("RankName", name);
                return this.rankRepo.FindOne(criteria);
            }
            return null;
        }

        public IList<Rank> GetAllRanks()
        {
            return this.rankRepo.GetAll().OrderBy(x => x.RankName).ToList<Rank>();
        }

        public IList<Rank> GetRanksByName(string term)
        {
            return this.rankSearchQuery.GetResults(term);
        }

        public Rank GetOrCreateRank(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                Rank r = this.GetRank(name);
                if (r == null)
                    r = new Rank()
                    {
                        RankName = name
                    };
                return r;
            }

            return null;
        }

        public Rank SaveRank(Rank rank)
        {
            this.luceneTasks.UpdatePersons(rank);
            return this.rankRepo.SaveOrUpdate(rank);
        }

        public bool DeleteRank(Rank rank)
        {
            if (rank != null && rank.Careers.Count < 1)
            {
                this.rankRepo.Delete(rank);
                return true;
            }
            return false;
        }

        public void MigrateSomeFunctionsToRanks()
        {
            Role[] roles = new Role[]
            {
                this.roleRepo.Get(4),  // Capitaine de Frégate
                this.roleRepo.Get(5),  // Capitaine de Corvette
                this.roleRepo.Get(7)   // Lieutenant de Vaisseau
            };
            Rank[] ranks = new Rank[]
            {
                this.rankRepo.Get(116),  // Capitaine de Frégate/Lieutenant Colonel
                this.rankRepo.Get(122),  // Capitaine de Corvette/Major
                this.rankRepo.Get(118)   // Lieutenant de Vaisseau/Capitaine
            };
            foreach (int i in new int[] { 0, 1, 2 })
                this.MoveRoleToRank(roles[i], ranks[i]);
        }

        private void MoveRoleToRank(Role role, Rank rank)
        {
            if (role != null)
            {
                foreach (Career c in role.Careers.Where(x => x.Rank == null))
                {
                    c.Role = null;
                    c.Rank = rank;
                }
            }
        }

        public Role GetRole(int id)
        {
            return this.roleRepo.Get(id);
        }

        public Role GetRole(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                IDictionary<string, object> criteria = new Dictionary<string, object>();
                criteria.Add("RoleName", name);
                return this.roleRepo.FindOne(criteria);
            }
            return null;
        }

        public IList<Role> GetAllRoles()
        {
            return this.roleRepo.GetAll().OrderBy(x => x.RoleName).ToList<Role>();
        }

        public IList<Role> GetRolesByName(string term)
        {
            return this.roleSearchQuery.GetResults(term);
        }

        public Role SaveRole(Role role)
        {
            this.luceneTasks.UpdatePersons(role);
            return this.roleRepo.SaveOrUpdate(role);
        }

        public bool DeleteRole(Role role)
        {
            if (role != null && role.Careers.Count < 1)
            {
                this.roleRepo.Delete(role);
                return true;
            }
            return false;
        }

        protected void SaveAdminUnitImport(AdminUnitImport i)
        {
            i.Unit.AddAdminUnitImport(i);
            this.adminUnitImportRepo.SaveOrUpdate(i);
        }

        protected void DeleteAdminUnitImport(AdminUnitImport i)
        {
            i.Unit.RemoveAdminUnitImport(i);
            this.adminUnitImportRepo.Delete(i);
        }

        public int MergeUnits(int toKeepUnitId, int toDeleteUnitId, string userId, bool isProfilingChange)
        {
            int result = this.mergeQueries.MergeUnits(toKeepUnitId, toDeleteUnitId, userId, isProfilingChange);

            if (result == 1)
            {
                // get Unit after merge in order to have updated attributes
                Unit toKeep = this.GetUnit(toKeepUnitId);

                this.luceneTasks.UpdateUnit(toKeep);
                this.luceneTasks.DeleteUnit(toDeleteUnitId);
            }

            return result;
        }

        public UnitOperation GetUnitOperation(int id)
        {
            return this.unitOperationRepo.Get(id);
        }

        public IList<UnitOperation> GetUnitOperations(Unit unit, Operation op)
        {
            IDictionary<string, object> criteria = new Dictionary<string, object>();
            criteria.Add("Unit", unit);
            criteria.Add("Operation", op);
            return this.unitOperationRepo.FindAll(criteria);
        }

        public UnitOperation SaveUnitOperation(UnitOperation uo)
        {
            return this.unitOperationRepo.SaveOrUpdate(uo);
        }

        public void DeleteUnitOperation(UnitOperation uo)
        {
            this.unitOperationRepo.Delete(uo);
        }

        public Operation GetOperation(int id)
        {
            return this.operationRepo.Get(id);
        }

        public Operation GetOperation(string name)
        {
            IDictionary<string, object> criteria = new Dictionary<string, object>();
            criteria.Add("Name", name);
            return this.operationRepo.FindOne(criteria);
        }

        public IList<Operation> GetOperationsLike(string term)
        {
            return this.unitSearchQuery.GetOperationsLike(term);
        }

        public IList<Operation> GetAllOperations()
        {
            return this.operationRepo.GetAll().Where(x => !x.Archive).ToList();
        }

        public Operation SaveOperation(Operation op)
        {
            //foreach (Unit u in op.Units)
            //    u.AddOperation(op);
            return this.operationRepo.SaveOrUpdate(op);
        }

        public bool DeleteOperation(Operation op)
        {
            if (op != null)
            {
                if (!op.UnitOperations.Any())
                {
                    this.operationRepo.Delete(op);
                    return true;
                }
            }
            return false;
        }

        public OperationAlias GetOperationAlias(int id)
        {
            return this.operationAliasRepo.Get(id);
        }

        public OperationAlias SaveOperationAlias(OperationAlias oa)
        {
            return this.operationAliasRepo.SaveOrUpdate(oa);
        }

        public void DeleteOperationAlias(OperationAlias a)
        {
            this.operationAliasRepo.Delete(a);
        }
    }
}
