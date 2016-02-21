using System;

namespace Profiling2.Domain.Prf.Suggestions
{
    /**
     * http://www.ats.ucla.edu/stat/mult_pkg/faq/general/odds_ratio.htm
     * http://luna.cas.usf.edu/~mbrannic/files/regression/Logistic.html
     * http://en.wikipedia.org/wiki/Logit
     */
    public class FeatureProbabilityCalc
    {
        public AdminSuggestionFeaturePersonResponsibility Feature { get; set; }
        public int IsAcceptedCount { get; set; }
        public int IsNotAcceptedCount { get; set; }

        public FeatureProbabilityCalc(AdminSuggestionFeaturePersonResponsibility feature, int isAcceptedCount, int isNotAcceptedCount)
        {
            this.Feature = feature;
            this.IsAcceptedCount = isAcceptedCount;
            this.IsNotAcceptedCount = isNotAcceptedCount;
        }

        public double Probability
        {
            get
            {
                if (this.IsAcceptedCount > 0)
                {
                    if (this.IsNotAcceptedCount > 0)
                        return this.IsAcceptedCount / Convert.ToDouble(this.IsAcceptedCount + this.IsNotAcceptedCount);
                    else
                        return 0.9999999;
                }
                else
                    return 0.0000001;
            }
        }

        public double Odds
        {
            get
            {
                return this.Probability / (1 - this.Probability);
            }
        }

        public double Logit
        {
            get
            {
                return Math.Log(this.Odds);
            }
        }

        // Logit as defined in stored proc
        public double SPLogit
        {
            get
            {
                return Math.Log10(this.Odds);
            }
        }

        // When used with this.Logit, returns this.Probability.
        // Used in the stored proc with this.SPLogit as the weighted coefficient for scoring events.
        public double GetLogitProbability(double logit)
        {
            double e = Math.Exp(logit);
            return e / (1 + e);
        }
    }
}
