﻿using System;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Scr.Person
{
    public class AdminRequestPersonImport : Entity
    {
        public virtual RequestPerson RequestPerson { get; set; }
        public virtual DateTime ImportDate { get; set; }
        public virtual int PreviousID { get; set; }
        public virtual string Notes { get; set; }
        public virtual bool Archive { get; set; }
    }
}
