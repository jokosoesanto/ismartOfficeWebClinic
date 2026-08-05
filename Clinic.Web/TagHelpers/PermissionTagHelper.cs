using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Clinic.Web.TagHelpers
{
    [HtmlTargetElement(Attributes = "asp-permission")]
    public class PermissionTagHelper : TagHelper
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PermissionTagHelper(IAuthorizationService authorizationService, IHttpContextAccessor httpContextAccessor)
        {
            _authorizationService = authorizationService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HtmlAttributeName("asp-permission")]
        public string Permission { get; set; } = string.Empty;

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (string.IsNullOrEmpty(Permission))
            {
                return;
            }

            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity.IsAuthenticated)
            {
                output.SuppressOutput();
                return;
            }

            var authResult = await _authorizationService.AuthorizeAsync(user, Permission);

            if (!authResult.Succeeded)
            {
                output.SuppressOutput();
            }
        }
    }
}
