# 53 Runtime Rendering Trace (Stack Trace)

## Keterangan
Berikut adalah Stack Trace lengkap dari exception yang berhasil ditangkap (di-*dump*) pada HTTP 500 error di rute `/Patient/Create`.

## Exception Details
- **Exception Type**: `System.InvalidOperationException`
- **Message**: `A view component named 'PatientForm' could not be found. A view component must be a public non-abstract class, not contain any generic parameters, and either be decorated with 'ViewComponentAttribute' or have a class name ending with the 'ViewComponent' suffix. A view component must not be decorated with 'NonViewComponentAttribute'.`

## Call Stack

```text
System.InvalidOperationException: A view component named 'PatientForm' could not be found. A view component must be a public non-abstract class, not contain any generic parameters, and either be decorated with 'ViewComponentAttribute' or have a class name ending with the 'ViewComponent' suffix. A view component must not be decorated with 'NonViewComponentAttribute'.
   at Microsoft.AspNetCore.Mvc.ViewComponents.DefaultViewComponentHelper.InvokeAsync(String name, Object arguments)
   at Clinic.Web.Services.ComponentRegistry.RenderComponentAsync(IViewComponentHelper componentHelper, UIComponent component, Object dataContext)
   at AspNetCoreGeneratedDocument.Views_Shared__RegionLayout#1.<ExecuteAsync>d__0#1.MoveNext()
   at Microsoft.AspNetCore.Mvc.Razor.RazorView.RenderPageCoreAsync(IRazorPage page, ViewContext context)
   at Microsoft.AspNetCore.Mvc.Razor.RazorView.RenderPageAsync(IRazorPage page, ViewContext context, Boolean invokeViewStarts)
   at Microsoft.AspNetCore.Mvc.Razor.RazorView.RenderLayoutAsync(ViewContext context, ViewBufferTextWriter bodyWriter)
   at Microsoft.AspNetCore.Mvc.Razor.RazorView.RenderAsync(ViewContext context)
   at Microsoft.AspNetCore.Mvc.ViewFeatures.ViewExecutor.ExecuteAsync(ViewContext viewContext, String contentType, Nullable`1 statusCode)
   at Microsoft.AspNetCore.Mvc.ViewFeatures.ViewExecutor.ExecuteAsync(ActionContext actionContext, IView view, ViewDataDictionary viewData, ITempDataDictionary tempData, String contentType, Nullable`1 statusCode)
   at Microsoft.AspNetCore.Mvc.ViewFeatures.ViewResultExecutor.ExecuteAsync(ActionContext context, ViewResult result)
   at Microsoft.AspNetCore.Mvc.ViewResult.ExecuteResultAsync(ActionContext context)
   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeNextResultFilterAsync>g__Awaited|30_0[TFilter,TFilterAsync](ResourceInvoker invoker, Task lastTask, State next, Scope scope, Object state, Boolean isCompleted)
   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.Rethrow(ResultExecutedContextSealed context)
   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.ResultNext[TFilter,TFilterAsync](State& next, Scope& scope, Object& state, Boolean& isCompleted)
   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.InvokeResultFilters()
--- End of stack trace from previous location ---
   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeNextResourceFilter>g__Awaited|25_0(ResourceInvoker invoker, Task lastTask, State next, Scope scope, Object state, Boolean isCompleted)
   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.Rethrow(ResourceExecutedContextSealed context)
   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.Next(State& next, Scope& scope, Object& state, Boolean& isCompleted)
   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.InvokeFilterPipelineAsync()
--- End of stack trace from previous location ---
   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeAsync>g__Awaited|17_0(ResourceInvoker invoker, Task task, IDisposable scope)
   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeAsync>g__Awaited|17_0(ResourceInvoker invoker, Task task, IDisposable scope)
   at Microsoft.AspNetCore.Authorization.AuthorizationMiddleware.Invoke(HttpContext context)
   at Microsoft.AspNetCore.Authentication.AuthenticationMiddleware.Invoke(HttpContext context)
   at Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddlewareImpl.Invoke(HttpContext context)
```

## Analisis Lapisan
1. Eksekusi `IViewComponentHelper.InvokeAsync` berada persis sebelum batas rendering internal ASP.NET.
2. Tidak ditemukan indikasi intervensi dari *Controller* pada stack trace atas; *Controller* (dan *Action Filter* sejenisnya) mendelegasikan tugas rendering sepenuhnya kepada `ViewResultExecutor`.
3. Exception berujung pada `DeveloperExceptionPageMiddleware`, membuktikan tidak ada tangkapan (_catch_) tingkat tinggi lainnya di pipeline kita yang berhasil mengeksekusi *fallback UI*.
