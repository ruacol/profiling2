using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Persons;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Suggestions
{
    /// <summary>
    /// When a person is suggested as having responsibility for an event, the algorithm's reasons are stored here.
    /// </summary>
    public class AdminSuggestionPersonResponsibility : Entity
    {
        public virtual Person Person { get; set; }
        public virtual Event Event { get; set; }
        public virtual bool IsAccepted { get; set; }
        public virtual XmlDocument SuggestionFeatures { get; set; }
        public virtual DateTime DecisionDateTime { get; set; }
        public virtual AdminUser DecisionAdminUser { get; set; }
        public virtual bool Archive { get; set; }
        public virtual string Notes { get; set; }

        public virtual IList<SuggestedFeature> SuggestedFeaturesList
        {
            get
            {
                if (this.SuggestionFeatures != null)
                {
                    IList<SuggestedFeature> sfs = new List<SuggestedFeature>();

                    foreach (XmlNode xn in this.SuggestionFeatures.DocumentElement.ChildNodes)
                    {
                        SuggestedFeature sf = new SuggestedFeature();
                        if (xn.Attributes["ID"] != null)
                            sf.FeatureID = Convert.ToInt32(xn.Attributes["ID"].Value);
                        if (xn.Attributes["Weight"] != null)
                            sf.Weight = Convert.ToDouble(xn.Attributes["Weight"].Value);
                        if (xn.Attributes["IncompleteDataPenalty"] != null)
                            sf.IncompleteDatePenalty = Convert.ToDouble(xn.Attributes["IncompleteDataPenalty"]);
                        sfs.Add(sf);
                    }

                    return sfs;
                }
                return null;
            }
        }

        public virtual bool IncludesFeatureID(int featureId)
        {
            if (this.SuggestedFeaturesList != null)
                return this.SuggestedFeaturesList.Where(x => x.FeatureID == featureId).Any();
            return false;
        }
    }
}
