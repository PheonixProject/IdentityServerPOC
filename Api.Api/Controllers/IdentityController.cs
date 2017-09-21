using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Api.Api.Controllers
{
    [Route("identity")]
    [Authorize]
    public class IdentityController : ControllerBase
    {
        [HttpGet]
        //[Authorize(Roles = "api1.read")]
        public IActionResult Get()
        {
            var hasAccess = User.HasClaim("scope", "api1.read");
            return new JsonResult(from i  in User.Identities select new { i.Label, i.Name, i.AuthenticationType});
            //return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }
    }
}
