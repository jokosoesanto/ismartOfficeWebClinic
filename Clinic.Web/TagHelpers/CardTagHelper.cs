using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Clinic.Web.TagHelpers
{
    [HtmlTargetElement("clinic-card")]
    public class CardTagHelper : TagHelper
    {
        public string Title { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.SetAttribute("class", "card shadow-sm mb-4");

            var iconHtml = !string.IsNullOrEmpty(Icon) ? $"<i class=\"bi {Icon} me-2\"></i>" : "";
            var headerHtml = !string.IsNullOrEmpty(Title) 
                ? $"<div class=\"card-header bg-transparent border-bottom\"><h5 class=\"m-0\">{iconHtml}{Title}</h5></div>" 
                : "";

            output.PreContent.SetHtmlContent($"{headerHtml}<div class=\"card-body\">");
            output.PostContent.SetHtmlContent("</div>");
        }
    }
}
