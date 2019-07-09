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
    public class PearsonRegistrationController : Controller
    {
        // POST: api/<controller>
        [HttpPost]
        public PearsonRegistrationReply RegisterPearson(Pearson _pearsonRegistration)
        {
            Console.WriteLine("In registerPearson");
            PearsonRegistrationReply pearsonRegReply = new PearsonRegistrationReply();
            PearsonRegistration.getInstance().Add(_pearsonRegistration);
            pearsonRegReply.Name = _pearsonRegistration.Name;
            pearsonRegReply.ID = _pearsonRegistration.ID;
            pearsonRegReply.Email = _pearsonRegistration.Email;
            pearsonRegReply.RegistrationStatus = "Successful";
            return pearsonRegReply;

        }

        /// <summary>
        /// /Adiciona uma pessoa e retorna o resultado no formato IActionResult
        /// </summary>
        /// <param name="Objeto Pearson"></param>
        /// <returns>Retorna um IActionResult</returns>
        [HttpPost("InsertPearson")]
        public IActionResult InsertPearson(Pearson _pearsonRegistration)
        {
            Console.WriteLine("In registerPearson");
            PearsonRegistrationReply pearsonRegReply = new PearsonRegistrationReply();
            PearsonRegistration.getInstance().Add(_pearsonRegistration);
            pearsonRegReply.ID = _pearsonRegistration.ID;
            pearsonRegReply.Name = _pearsonRegistration.Name;
            pearsonRegReply.Email = _pearsonRegistration.Email;
            pearsonRegReply.RegistrationStatus = "Successful";
            return Ok(pearsonRegReply);

        }


        /// <summary>
        /// Adiciona uma pessoa e retorna o resultado no formato JSON
        /// </summary>
        /// <param name="Objecto Pearson"></param>
        /// <returns>Resultado no formato JSON</returns>
        [Route("pearson/")]
        [HttpPost("AddPearson")]
        public JsonResult AddPearson(Pearson _pearsonRegd)
        {
            Console.WriteLine("In registerPearson");
            PearsonRegistrationReply pearsonRegReply = new PearsonRegistrationReply();
            PearsonRegistration.getInstance().Add(_pearsonRegd);
            pearsonRegReply.ID = _pearsonRegd.ID;
            pearsonRegReply.Name = _pearsonRegd.Name;
            pearsonRegReply.Email = _pearsonRegd.Email;
            pearsonRegReply.RegistrationStatus = "Successful";
            return Json(pearsonRegReply);
        }
    }
}