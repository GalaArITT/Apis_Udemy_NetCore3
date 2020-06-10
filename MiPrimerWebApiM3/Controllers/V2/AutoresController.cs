using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiPrimerWebApiM3.Contexts;
using MiPrimerWebApiM3.Entities;
using MiPrimerWebApiM3.Helpers;

namespace MiPrimerWebApiM3.Controllers.V2
{
    [Route("api/v2/[controller]")]
    [ApiController]
    [HttpHeaderIsPresent("x-version", "2")]
    public class AutoresController : ControllerBase
    {
        private ApplicationDbContext context;

        public AutoresController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet(Name = "ObtenerAutoresV2")]
        [ServiceFilter(typeof(HATEOASAuthorsFilterAttribute))]
        public async Task<ActionResult<IEnumerable<Autor>>> Get()
        {
            var autores = await context.Autores.ToListAsync();
            return autores;
        }
    }
}
