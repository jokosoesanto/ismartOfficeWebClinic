using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Clinic.Application.UI;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.Web.Services
{
    public interface IComponentRegistry
    {
        Task<IHtmlContent> RenderComponentAsync(IViewComponentHelper componentHelper, UIComponent component, object dataContext);
    }

    public class ComponentRegistry : IComponentRegistry
    {
        private readonly Diagnostics.IComponentResolver _resolver;

        public ComponentRegistry(Diagnostics.IComponentResolver resolver)
        {
            _resolver = resolver;
        }

        public async Task<IHtmlContent> RenderComponentAsync(IViewComponentHelper componentHelper, UIComponent component, object dataContext)
        {
            if (component.IsLazy)
            {
                var lazyDiv = new TagBuilder("div");
                lazyDiv.AddCssClass("lazy-component-placeholder text-center p-4");
                lazyDiv.Attributes.Add("data-lazy-url", component.LazyLoadUrl);
                lazyDiv.InnerHtml.AppendHtml("<div class='spinner-border text-primary' role='status'><span class='visually-hidden'>Loading...</span></div>");
                return lazyDiv;
            }

            var parameters = new Dictionary<string, object>(component.Parameters);
            
            // Inject context data if not already present
            if (!parameters.ContainsKey("modelData"))
            {
                parameters["modelData"] = dataContext;
            }

            // Task 5: Component Resolution Hardening (Runtime Fallback)
            var resolution = _resolver.ResolveComponent(component.ComponentId);
            if (resolution.Status != Diagnostics.ComponentResolutionStatus.Registered)
            {
                return RenderDiagnosticFallback(resolution);
            }

            try 
            {
                return await componentHelper.InvokeAsync(component.ComponentId, parameters);
            }
            catch (Exception ex)
            {
                resolution.Status = Diagnostics.ComponentResolutionStatus.InvalidViewComponent;
                return RenderDiagnosticFallback(resolution, ex.Message);
            }
        }

        private IHtmlContent RenderDiagnosticFallback(Diagnostics.ComponentManifest manifest, string? additionalError = null)
        {
            var div = new TagBuilder("div");
            div.AddCssClass("alert alert-danger m-2 shadow-sm border-danger border-2");
            div.InnerHtml.AppendHtml($"<h5 class='alert-heading'><i class='bi bi-bug-fill'></i> Runtime Fallback</h5>");
            div.InnerHtml.AppendHtml($"<p>Component <strong>{manifest.ComponentId}</strong> failed to render.</p>");
            div.InnerHtml.AppendHtml("<hr class='my-2'>");
            
            var ul = new TagBuilder("ul");
            ul.AddCssClass("list-unstyled small font-monospace text-muted mb-0");
            ul.InnerHtml.AppendHtml($"<li><strong>Status:</strong> {manifest.Status}</li>");
            if (!string.IsNullOrEmpty(manifest.Assembly)) ul.InnerHtml.AppendHtml($"<li><strong>Assembly:</strong> {manifest.Assembly}</li>");
            if (!string.IsNullOrEmpty(manifest.Namespace)) ul.InnerHtml.AppendHtml($"<li><strong>Namespace:</strong> {manifest.Namespace}</li>");
            ul.InnerHtml.AppendHtml($"<li><strong>Expected View:</strong> {manifest.ExpectedViewPath}</li>");
            
            div.InnerHtml.AppendHtml(ul);

            if (!string.IsNullOrEmpty(additionalError))
            {
                div.InnerHtml.AppendHtml($"<div class='mt-2 p-2 bg-light border rounded small text-break'><code>{additionalError}</code></div>");
            }

            return div;
        }
    }
}
