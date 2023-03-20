using LifelogBb.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace LifelogBb.Views.Shared.Components.Pagination
{
    public class Pagination : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(IPaginatedList list)
        {
            return View(list);
        }
    }
}
