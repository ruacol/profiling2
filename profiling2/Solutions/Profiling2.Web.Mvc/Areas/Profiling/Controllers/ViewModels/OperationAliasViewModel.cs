using AutoMapper;
using Profiling2.Domain.Prf.Units;

namespace Profiling2.Web.Mvc.Areas.Profiling.Controllers.ViewModels
{
    public class OperationAliasViewModel
    {
        public int Id { get; set; }
        public int OperationId { get; set; }
        public string Name { get; set; }
        public bool Archive { get; set; }
        public string Notes { get; set; }

        // display only
        public string OperationName { get; set; }

        public OperationAliasViewModel() { }

        public OperationAliasViewModel(OperationAlias a)
        {
            if (a != null)
            {
                Mapper.Map(a, this);
                if (a.Operation != null)
                {
                    this.OperationId = a.Operation.Id;
                    this.OperationName = a.Operation.Name;
                }
            }
        }
    }
}