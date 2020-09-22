namespace SuumoScraping.Extensions
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Html;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;

    public static class ReferrerLinkExtensions
    {
        public static IHtmlContent ReferrerLink(
          this IHtmlHelper htmlHelper,
          string linkText,
          IDictionary<string, object> htmlAttributes)
        {
            var referrer = htmlHelper.ViewContext.HttpContext.Request.GetTypedHeaders().Referer.ToString();

            if (referrer == null)
            {
                return HtmlString.Empty;
            }

            var referrerString = referrer.ToString();
            if (string.IsNullOrEmpty(referrerString))
            {
                return HtmlString.Empty;
            }

            var tagBuilder = new TagBuilder("a");
            tagBuilder.InnerHtml.SetHtmlContent(linkText);
            tagBuilder.MergeAttributes(htmlAttributes);
            tagBuilder.MergeAttribute("href", referrerString, true);

            return tagBuilder;
        }

        public static IHtmlContent ReferrerLink(
          this IHtmlHelper htmlHelper,
          string linkText,
          object htmlAttributes)
        {
            var dic = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            return ReferrerLink(htmlHelper, linkText, dic);
        }

        public static IHtmlContent ReferrerLink(
          this IHtmlHelper htmlHelper,
          string linkText)
        {
            return ReferrerLink(htmlHelper, linkText, (object)null);
        }
    }
}