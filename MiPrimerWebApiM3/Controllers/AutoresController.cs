using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiPrimerWebApiM3.Contexts;
using MiPrimerWebApiM3.Entities;
using MiPrimerWebApiM3.Helpers;
using MiPrimerWebApiM3.Models;
using MiPrimerWebApiM3.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiPrimerWebApiM3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutoresController: ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly ClaseB claseB;
        private readonly ILogger<AutoresController> logger;
        private readonly IMapper mapper;

        public AutoresController(ApplicationDbContext context, ClaseB claseB, 
            ILogger<AutoresController> logger, IMapper mapper)
        {
            this.context = context;
            this.claseB = claseB;
            this.logger = logger;
            this.mapper = mapper;
        }

        //[HttpGet("/listado")]
        //[HttpGet("listado")]
        //[HttpGet("[action]")]
        [HttpGet]
        [ServiceFilter(typeof(MiFiltroDeAccion))]
        public async Task<ActionResult<IEnumerable<AutorDTO>>> Get()
        {
            //throw new NotImplementedException();
            //logger.LogInformation("obteniendo los autores");
            //claseB.HacerAlgo();
            var autores = await context.Autores.ToListAsync();
            var autoresDTO = mapper.Map<List<AutorDTO>>(autores);

            return autoresDTO;
                //context.Autores.Include(x => x.Libros).ToList();
        }
        //api/autores/1 o api/autores/1/oliver
        //[HttpGet("{id}/{params?}", Name = "ObtenerAutor")] // para definir un valor opcional
        //[HttpGet("{id}/{params=Oliver}", Name = "ObtenerAutor")] //definir un valor predeterminado
        [HttpGet("{id}", Name = "ObtenerAutor")]
        public async Task<ActionResult<AutorDTO>> Get(int id,/*[BindRequired]*/ string param2)
        {
            claseB.HacerAlgo();
            logger.LogDebug($"buscando autor de {id}");
            var autor = await context.Autores.Include(x => x.Libros).FirstOrDefaultAsync(x => x.Id == id);

            if (autor == null)
            {
                logger.LogWarning($"el autor de Id {id} no ha sido encontrado");
                return NotFound();
            }
            //mapear el modelo autor
            var autorDTO = mapper.Map<AutorDTO>(autor);

            return autorDTO;
        }

        [HttpPost]
        public ActionResult Post([FromBody] Autor autor)
        {
            // Esto no es necesario en asp.net core 2.1 en adelante
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            TryValidateModel(autor);
            context.Autores.Add(autor);
            context.SaveChanges();
            return new CreatedAtRouteResult("ObtenerAutor", new { id = autor.Id }, autor);

        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] Autor value)
        {
            // Esto no es necesario en asp.net core 2.1
            // if (ModelState.IsValid){

            // }

            if (id != value.Id)
            {
                return BadRequest();
            }

            context.Entry(value).State = EntityState.Modified;
            context.SaveChanges();
            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult<Autor> Delete(int id)
        {
            var autor = context.Autores.FirstOrDefault(x => x.Id == id);

            if (autor == null)
            {
                return NotFound();
            }

            context.Autores.Remove(autor);
            context.SaveChanges();
            return autor;
        }
    }
}
