using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MiPrimerWebApiM3.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet(Name = "ObtenerValores")]
        [ResponseCache(Duration =15)] //15 segundos
        [Authorize]
        public ActionResult<string> Get()
        {
            return DateTime.Now.Second.ToString();
        }

        // GET api/values/5
        [HttpGet("{id}", Name = "ObtenerValor")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost(Name = "CrearValor")]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}", Name = "ActualizarValor")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}", Name = "BorrarValor")]
        public void Delete(int id)
        {
        }
    }
}
