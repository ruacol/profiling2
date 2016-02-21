using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Profiling2.Domain.DTO
{
    public class ScreeningDifferenceDTO
    {
        public string ScreeningResult { get; set; }
        public int NumAgreed { get; set; }
        public int NumDisagreed { get; set; }

        public ScreeningDifferenceDTO()
        {
            this.NumAgreed = 0;
            this.NumDisagreed = 0;
        }
    }

    public class ScreeningEntityStatisticDTO
    {
        public string ScreeningEntity { get; set; }
        public int TotalPersonScreenings { get; set; }
        public IList<ScreeningDifferenceDTO> ConditionalityGroupDifferences { get; set; }
        public IList<ScreeningDifferenceDTO> FinalDecisionDifferences { get; set; }

        public ScreeningEntityStatisticDTO()
        {
            this.ConditionalityGroupDifferences = new List<ScreeningDifferenceDTO>();
            this.FinalDecisionDifferences = new List<ScreeningDifferenceDTO>();
        }

        public double ConditionalityGroupAgreedPercent
        {
            get
            {
                return Math.Round((double)this.ConditionalityGroupDifferences.Sum(x => x.NumAgreed) / this.TotalPersonScreenings * 100, 1);
            }
        }

        public double FinalDecisionAgreedPercent
        {
            get
            {
                return Math.Round((double)this.FinalDecisionDifferences.Sum(x => x.NumAgreed) / this.TotalPersonScreenings * 100, 1);
            }
        }
    }
}
