using System.Collections.Generic;
using NHibernate;
using Profiling2.Domain.DTO;
using Profiling2.Domain.Prf.Units;

namespace Profiling2.Domain.Contracts.Queries
{
    public interface IUnitQueries
    {
        IList<UnitDTO> GetEmptyUnits();

        IList<Unit> GetAllUnits(ISession session);

        Unit GetUnit(ISession session, int unitId);
    }
}
