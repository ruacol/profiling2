using System;
using SharpArch.Domain.DomainModel;
using Profiling2.Domain.Prf;

namespace Profiling2.Domain.Scr.PersonRecommendation
{
    public class ScreeningRequestPersonRecommendationHistory : Entity
    {
        public virtual ScreeningRequestPersonRecommendation ScreeningRequestPersonRecommendation { get; set; }
        public virtual ScreeningStatus ScreeningStatus { get; set; }
        public virtual DateTime DateStatusReached { get; set; }
        public virtual bool Archive { get; set; }
        public virtual string Notes { get; set; }
        public virtual AdminUser AdminUser { get; set; }
    }
}
