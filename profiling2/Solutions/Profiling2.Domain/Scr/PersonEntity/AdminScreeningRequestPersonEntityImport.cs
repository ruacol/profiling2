﻿using System;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Scr.PersonEntity
{
    public class AdminScreeningRequestPersonEntityImport : Entity
    {
        public virtual ScreeningRequestPersonEntity ScreeningRequestPersonEntity { get; set; }
        public virtual DateTime ImportDate { get; set; }
        public virtual int PreviousID { get; set; }
        public virtual string Notes { get; set; }
        public virtual bool Archive { get; set; }
    }
}
