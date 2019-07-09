using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASPDotNETCoreRESTFulWS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASPDotNETCoreRESTFulWS.Controllers
{
    [Route("api/PearsonDelete")]
    [ApiController]
    public class PearsonDeleteController : Controller
    {
        [Route("pearson/remove/{regId}")]
        // DELETE: api/<controller>
        [HttpDelete]
        public IActionResult DeletePearsonRecord(int regId)
        {
            Console.WriteLine("In deleteStudentRecord");
            return Ok(PearsonRegistration.getInstance().Remove(regId));
        }
    }
}