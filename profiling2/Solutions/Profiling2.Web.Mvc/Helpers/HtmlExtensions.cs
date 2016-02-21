using System.Web;
using System.Web.Mvc;
using Helpers;

namespace Profiling2.Web.Mvc.Helpers
{
    public static class HtmlExtensions
    {
        public static IHtmlString DateLabel(this HtmlHelper html, int year, int month, int day)
        {
            return html.Raw(
                MvcHtmlString.Create(
                    new DateLabel(year, month, day, false).ToString()
                ).ToString()
            );
        }

        public static IHtmlString DateShortLabel(this HtmlHelper html, int year, int month, int day)
        {
            return html.Raw(
                MvcHtmlString.Create(
                    new DateLabel(year, month, day, true).ToString()
                ).ToString()
            );
        }

        public static IHtmlString ScreeningResultLabel(this HtmlHelper html, string text)
        {
            return html.Raw(
                MvcHtmlString.Create(
                    new ScreeningResultLabel(text).ToString()
                ).ToString()
            );
        }

        public static IHtmlString DiffOutput(this HtmlHelper html, string oldStr, string newStr)
        {
            return html.Raw(
                MvcHtmlString.Create(
                    new HtmlDiff(oldStr, newStr).Build()
                ).ToString()
            );
        }
    }
}