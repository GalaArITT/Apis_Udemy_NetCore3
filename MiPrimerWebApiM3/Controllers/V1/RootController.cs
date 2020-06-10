using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiPrimerWebApiM3.Models;

namespace MiPrimerWebApiM3.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class RootController : ControllerBase
    {
        [HttpGet(Name = "GetRoot")]
        public ActionResult<IEnumerable<Enlace>> Get()
        {
            List<Enlace> enlaces = new List<Enlace>();

            // Aquí colocamos los links
            enlaces.Add(new Enlace(href: Url.Link("GetRoot", new { }), rel: "self", metodo: "GET"));
            enlaces.Add(new Enlace(href: Url.Link("ObtenerAutores", new { }), rel: "autores", metodo: "GET"));
            enlaces.Add(new Enlace(href: Url.Link("CrearAutor", new { }), rel: "crear-autor", metodo: "POST"));
            enlaces.Add(new Enlace(href: Url.Link("ObtenerValores", new { }), rel: "valores", metodo: "GET"));
            enlaces.Add(new Enlace(href: Url.Link("CrearValor", new { }), rel: "crear-valor", metodo: "POST"));

            return enlaces;
        }
    }
}
