using System.Web;
using System.Web.Mvc;
using Profiling2.Domain.Scr;

namespace Profiling2.Web.Mvc.Helpers
{
    public class ScreeningResultLabel : IHtmlString
    {
        private readonly string text;

        public ScreeningResultLabel(string text)
        {
            this.text = text;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.text))
            {
                return string.Empty;
            }
            else
            {
                var wrapper = new TagBuilder("span");
                wrapper.AddCssClass("label");
                if (!string.IsNullOrEmpty(this.text))
                {
                    if (ScreeningResult.GREEN == this.text)
                        wrapper.AddCssClass("label-success");
                    else if (ScreeningResult.YELLOW == this.text)
                        wrapper.AddCssClass("label-warning");
                    else if (ScreeningResult.RED == this.text)
                        wrapper.AddCssClass("label-important");
                }
                wrapper.InnerHtml = text;

                return wrapper.ToString();
            }
        }

        public string ToHtmlString()
        {
            return ToString();
        }
    }
}