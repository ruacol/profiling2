using System.Xml;

namespace Profiling2.Domain.Prf.Suggestions
{
    public class SuggestedFeature
    {
        public virtual int FeatureID { get; set; }
        public virtual double Weight { get; set; }
        public virtual double? IncompleteDatePenalty { get; set; }

        public SuggestedFeature() { }

        public XmlElement ToXmlElement(XmlDocument doc)
        {
            XmlElement feature = doc.CreateElement("Feature");
            feature.SetAttribute("ID", this.FeatureID.ToString());
            feature.SetAttribute("Weight", this.Weight.ToString());
            if (this.IncompleteDatePenalty.HasValue)
                feature.SetAttribute("IncompleteDatePenalty", this.IncompleteDatePenalty.Value.ToString());
            return feature;
        }
    }
}
