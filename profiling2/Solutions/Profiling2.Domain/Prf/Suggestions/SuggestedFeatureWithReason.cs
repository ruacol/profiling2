
namespace Profiling2.Domain.Prf.Suggestions
{
    public class SuggestedFeatureWithReason
    {
        public SuggestedFeature SuggestedFeature { get; set; }
        public int EventID { get; set; }
        public string EventHeadline { get; set; }
        public string Reason { get; set; }

        public SuggestedFeatureWithReason() { }

        public double Score
        {
            get
            {
                if (this.SuggestedFeature != null)
                {
                    return this.SuggestedFeature.IncompleteDatePenalty.HasValue && this.SuggestedFeature.IncompleteDatePenalty > 0
                        ? this.SuggestedFeature.IncompleteDatePenalty.Value * this.SuggestedFeature.Weight
                        : this.SuggestedFeature.Weight;
                }
                return 0.0;
            }
        }
    }
}
