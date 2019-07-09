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
    public class PearsonRetriveController : Controller
    {
        // GET: api/<controller>
        [HttpGet]
        public List<Pearson> GetAllPearsons()
        {
            return PearsonRegistration.getInstance().getAllPearson();
        }

        [HttpGet("GetNextID")]
        public JsonResult GetNexID()
        {
            return Json(PearsonRegistration.getInstance().NextID);
        }

        [HttpGet("GetAllPearsonRecords")]
        public JsonResult GetAllPearsonRecords()
        {
            return Json(PearsonRegistration.getInstance().getAllPearson());
        }


    }
}