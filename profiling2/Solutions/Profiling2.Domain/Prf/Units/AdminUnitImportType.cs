﻿using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Units
{
    public class AdminUnitImportType : Entity
    {
        public virtual string AdminUnitImportTypeName { get; set; }
        public virtual bool Archive { get; set; }
        public virtual string Notes { get; set; }

        public override string ToString()
        {
            return this.AdminUnitImportTypeName;
        }
    }
}
