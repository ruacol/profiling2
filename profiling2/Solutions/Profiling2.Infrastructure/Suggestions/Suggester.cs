using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Prf.Suggestions;
using System;

namespace Profiling2.Infrastructure.Suggestions
{
    public class Suggester
    {
        protected IList<FeatureProbabilityCalc> FeatureProbabilityCalcs { get; set; }
        protected IList<SuggestedFeatureWithReason> SingleSuggestedFeatures { get; set; }

        public Suggester()
        {
            this.SingleSuggestedFeatures = new List<SuggestedFeatureWithReason>();
        }

        protected IDictionary<int, IList<SuggestedFeatureWithReason>> SingleSuggestedFeaturesByEvent
        {
            get
            {
                IDictionary<int, IList<SuggestedFeatureWithReason>> featuresByEvent = new Dictionary<int, IList<SuggestedFeatureWithReason>>();
                foreach (SuggestedFeatureWithReason sfwr in this.SingleSuggestedFeatures)
                {
                    if (!featuresByEvent.ContainsKey(sfwr.EventID))
                        featuresByEvent[sfwr.EventID] = new List<SuggestedFeatureWithReason>();
                    featuresByEvent[sfwr.EventID].Add(sfwr);
                }
                return featuresByEvent;
            }
        }

        public void SetFeatureProbabilityCalcs(IList<FeatureProbabilityCalc> featureProbabilityCalcs)
        {
            this.FeatureProbabilityCalcs = featureProbabilityCalcs;
        }

        // TODO convert to Dictionary?
        protected double GetFeatureCoefficient(int? featureId)
        {
            if (featureId.HasValue)
            {
                if (this.FeatureProbabilityCalcs.Where(x => x.Feature.Id == featureId).Any())
                    return this.FeatureProbabilityCalcs.Where(x => x.Feature.Id == featureId).First().Probability;
            }
            return 0.0;
        }

        // calculates Feature.Weight based on feature counts stored in this.FeatureProbabilityCalcs
        protected IList<SuggestedFeatureWithReason> CalculateFeatureWeights(IList<SuggestedFeatureWithReason> features)
        {
            IList<SuggestedFeatureWithReason> populated = new List<SuggestedFeatureWithReason>();
            foreach (SuggestedFeatureWithReason sfwr in features)
            {
                SuggestedFeature f = sfwr.SuggestedFeature;
                if (f.Weight == 0 && f.FeatureID > 0)
                {
                    f.Weight = 10.0 * this.GetFeatureCoefficient(f.FeatureID);
                    sfwr.SuggestedFeature = f;
                }
                populated.Add(sfwr);
            }
            return populated;
        }

        public void AddSuggestedFeatures(IList<SuggestedFeatureWithReason> features)
        {
            this.SingleSuggestedFeatures = this.SingleSuggestedFeatures.Concat(this.CalculateFeatureWeights(features)).ToList();
        }

        public IList<SuggestionEventForPersonDTO> Suggest(Person p)
        {
            IList<SuggestionEventForPersonDTO> finalSuggestions = new List<SuggestionEventForPersonDTO>();

            foreach (KeyValuePair<int, IList<SuggestedFeatureWithReason>> kvp in this.SingleSuggestedFeaturesByEvent)
            {
                if (!p.IsResponsibleFor(kvp.Key)  // make sure that the person responsibility doesn't already exist
                    && !p.WasSuggestedEventDeclined(kvp.Key))  // make sure this suggestion hasn't already been declined
                {
                    SuggestionEventForPersonDTO dto = new SuggestionEventForPersonDTO()
                    {
                        EventID = kvp.Key,
                        EventName = kvp.Value.First().EventHeadline,
                        Score = kvp.Value.Sum(x => x.Score).ToString(),
                        Features = this.ToFeaturesXml(kvp.Value).OuterXml,
                        //SuggestionReason = string.Join(" ", kvp.Value.Select(x => x.Reason)),
                        SuggestionReasons = kvp.Value.Select(x => x.Reason).ToList()
                    };
                    finalSuggestions.Add(dto);
                }
            }

            return finalSuggestions.OrderByDescending(x => Convert.ToDouble(x.Score)).ToList();
        }

        protected XmlDocument ToFeaturesXml(IList<SuggestedFeatureWithReason> list)
        {
            XmlDocument features = new XmlDocument();
            XmlElement root = features.CreateElement("Features");
            root.SetAttribute("xmlns", "http://monuc-apps/Profiling");

            foreach (SuggestedFeatureWithReason s in list)
                root.AppendChild(s.SuggestedFeature.ToXmlElement(features));

            features.AppendChild(root);
            return features;
        }
    }
}
