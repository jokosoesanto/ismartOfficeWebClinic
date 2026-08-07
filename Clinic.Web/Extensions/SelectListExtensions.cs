using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Clinic.Web.Extensions
{
    public static class SelectListExtensions
    {
        public static IEnumerable<SelectListItem> ToSelectList<T>(
            this IEnumerable<T> items,
            Func<T, object> valueSelector,
            Func<T, string> textSelector,
            object? selectedValue = null)
        {
            if (items == null)
            {
                return Enumerable.Empty<SelectListItem>();
            }

            return items.Select(item =>
            {
                var value = valueSelector(item)?.ToString();
                return new SelectListItem
                {
                    Value = value,
                    Text = textSelector(item),
                    Selected = selectedValue != null && value == selectedValue.ToString()
                };
            });
        }
    }
}
