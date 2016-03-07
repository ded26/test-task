using System;
using System.Text;
using System.Web.Mvc;
using TwitterApp.Models;

namespace TwitterApp.Helpers
{
    public static class PagedList
    {
        public static MvcHtmlString PageLinks(this HtmlHelper html, PageInfo pageInfo, Func<int, string> pageUrlFunc)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 1; i <= pageInfo.TotalPages; i++)
            {
                TagBuilder tag = new TagBuilder("a");
                tag.MergeAttribute("href", pageUrlFunc(i));
                tag.InnerHtml = i.ToString();
                if (i == pageInfo.CurrentPage)
                    tag.AddCssClass("btn-primary");
                tag.AddCssClass("btn btn-default");
                result.Append(tag);
            }
            return MvcHtmlString.Create(result.ToString());
        }
    }
}