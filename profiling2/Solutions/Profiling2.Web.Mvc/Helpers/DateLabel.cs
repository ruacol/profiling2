using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace Profiling2.Web.Mvc.Helpers
{
    public class DateLabel : IHtmlString
    {
        protected readonly int? year;
        protected readonly int? month;
        protected readonly int? day;
        protected readonly bool shortLabel;

        public DateLabel(int? year, int? month, int? day, bool shortLabel)
        {
            this.year = year;
            this.month = month;
            this.day = day;
            this.shortLabel = shortLabel;
        }

        public override string ToString()
        {
            var wrapper = new TagBuilder("span");
            if ((!year.HasValue && !month.HasValue && !day.HasValue)
                || (year.HasValue && month.HasValue && day.HasValue 
                    && year.Value == 0 && month.Value == 0 && day.Value == 0))
            {
                if (shortLabel)
                    return "-";
                else
                {
                    wrapper.AddCssClass("muted");
                    wrapper.InnerHtml = "No date specified.";
                }
            }
            else
            {
                IList<string> components = new List<string>();
                if (year.HasValue && year.Value > 0)
                    components.Add(year.Value.ToString());
                else
                    components.Add("-");
                if (month.HasValue && month.Value > 0)
                {
                    string m = month.Value.ToString();
                    if (month.Value < 10)
                        m = "0" + m;
                    components.Add(m);
                }
                else
                    components.Add("-");
                if (day.HasValue && day.Value > 0)
                {
                    string d = day.Value.ToString();
                    if (day.Value < 10)
                        d = "0" + d;
                    components.Add(d);
                }
                else
                    components.Add("-");
                wrapper.InnerHtml = string.Join("/", components);
            }
            return wrapper.ToString();
        }

        public string ToHtmlString()
        {
            return ToString();
        }
    }
}