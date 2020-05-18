using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Web.ViewModels;

namespace Web.Components
{
    public class PopupViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string title, string message) =>
                        View(new PopupViewModel
                        {
                            Title = title,
                            Message = message
                        });
    }
}
