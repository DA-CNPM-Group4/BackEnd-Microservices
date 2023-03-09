using InfoService.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InfoService.Controllers
{
    public class BaseController : ControllerBase, IDisposable
    {
        protected SiteProvider provider = new SiteProvider();
        public void Dispose()
        {
            provider.Dispose();
        }
    }
}
