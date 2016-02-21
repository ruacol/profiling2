using System.Collections.Generic;
using NHibernate;
using Profiling2.Domain.DTO;
using Profiling2.Domain.Prf.Careers;
using Profiling2.Domain.Prf.Organizations;
using Profiling2.Domain.Prf.Units;

namespace Profiling2.Domain.Contracts.Tasks
{
    public interface IOrganizationTasks
    {
        Organization GetOrganization(int orgId);

        IList<Organization> GetAllOrganizations();

        IList<Organization> GetOrganizationsByName(string term);

        Organization GetOrCreateOrganization(string name);

        Organization SaveOrganization(Organization o);

        bool DeleteOrganization(Organization o);

        Unit GetUnit(int unitId);

        Unit GetUnit(string name);

        IList<Unit> GetAllUnits();

        IList<Unit> GetAllUnits(ISession session);

        IList<Unit> GetUnitsByName(string term);

        bool DeleteUnit(Unit unit);

        Unit SaveUnit(Unit unit);

        void LuceneUpdateUnitQueueable(int unitId);

        IList<UnitDTO> GetEmptyUnits();

        void PopulateUnitOrganization();

        UnitHierarchy GetUnitHierarchy(int id);

        UnitHierarchy GetUnitHierarchy(int unitId, int parentId);

        UnitHierarchy SaveUnitHierarchy(UnitHierarchy uh);

        void DeleteUnitHierarchy(UnitHierarchy uh);

        UnitHierarchyType GetUnitHierarchyType(int id);

        UnitHierarchyType GetUnitHierarchyType(string name);

        IList<UnitHierarchyType> GetUnitHierarchyTypes();

        UnitLocation GetUnitLocation(int id);

        UnitLocation SaveUnitLocation(UnitLocation ul);

        void DeleteUnitLocation(UnitLocation ul);

        UnitAlias GetUnitAlias(int id);

        UnitAlias SaveUnitAlias(UnitAlias ua);

        void DeleteUnitAlias(UnitAlias ua);

        Rank GetRank(int id);

        Rank GetRank(string name);

        IList<Rank> GetAllRanks();

        IList<Rank> GetRanksByName(string term);

        Rank GetOrCreateRank(string name);

        Rank SaveRank(Rank rank);

        bool DeleteRank(Rank rank);

        void MigrateSomeFunctionsToRanks();

        Role GetRole(int id);

        Role GetRole(string name);

        IList<Role> GetAllRoles();

        IList<Role> GetRolesByName(string term);

        Role SaveRole(Role role);

        bool DeleteRole(Role role);

        /// <summary>
        /// Perform unit merge.  Updates Lucene Unit index after merge.
        /// 
        /// Deleted unit is audited via Profiling1 auditing mechanism via the stored proc (and NOT Envers).  UnitLocation, UnitAlias, UnitSource
        /// changes not audited by the stored proc.
        /// </summary>
        /// <param name="toKeepUnitId"></param>
        /// <param name="toDeleteUnitId"></param>
        /// <param name="userId">Logged-in user's UN ID, or string username.</param>
        /// <param name="isProfilingChange">Whether logged-in user is member of Profiling team or not.</param>
        /// <returns></returns>
        int MergeUnits(int toKeepUnitId, int toDeleteUnitId, string userId, bool isProfilingChange);

        UnitOperation GetUnitOperation(int id);

        IList<UnitOperation> GetUnitOperations(Unit unit, Operation op);

        UnitOperation SaveUnitOperation(UnitOperation uo);

        void DeleteUnitOperation(UnitOperation uo);

        Operation GetOperation(int id);

        Operation GetOperation(string name);

        IList<Operation> GetOperationsLike(string term);

        IList<Operation> GetAllOperations();

        Operation SaveOperation(Operation op);

        bool DeleteOperation(Operation op);

        OperationAlias GetOperationAlias(int id);

        OperationAlias SaveOperationAlias(OperationAlias oa);

        void DeleteOperationAlias(OperationAlias a);
    }
}
