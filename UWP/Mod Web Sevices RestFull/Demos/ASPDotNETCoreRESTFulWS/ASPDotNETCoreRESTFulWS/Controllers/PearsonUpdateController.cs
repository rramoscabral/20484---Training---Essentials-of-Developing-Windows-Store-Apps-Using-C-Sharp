using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASPDotNETCoreRESTFulWS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASPDotNETCoreRESTFulWS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PearsonUpdateController : Controller
    {
        // GET: api/<controller>
        [HttpPut]
        public JsonResult UpdatePearsonRecord(Pearson stdn)
        {
            Console.WriteLine("In updatePearsonRecord");
            return Json(PearsonRegistration.getInstance().UpdatePearson(stdn));
        }
    }
}