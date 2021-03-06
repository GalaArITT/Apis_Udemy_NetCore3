﻿using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
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
    [Route("api/v1/[controller]")]
    [ApiController]
    //[HttpHeaderIsPresent("x-version","1")]
    public class AutoresController : ControllerBase
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
        [HttpGet(Name = "ObtenerAutores")]
        //[ServiceFilter(typeof(MiFiltroDeAccion))]
        [ServiceFilter(typeof(HATEOASAuthorsFilterAttribute))]
        public async Task<IActionResult> Get(bool IncluirEnlacesHateOS = false, int numeroPagina=1, int cantidad=10)
        {

            //mamadas
            //ActionResult</*IEnumerable<AutorDTO>*/ColeccionDeRecursos<AutorDTO>>

            //throw new NotImplementedException();
            //logger.LogInformation("obteniendo los autores");
            //claseB.HacerAlgo();

            var query = context.Autores.AsQueryable();

            var totalRegistros = query.Count();

            var autores = await query.Skip(cantidad*(numeroPagina-1)).Take(cantidad).ToListAsync();

            Response.Headers["X-Total-Registros"] = totalRegistros.ToString();
            Response.Headers["X-Cantidad-Paginas"] = ((int)Math.Ceiling((double) totalRegistros/cantidad)).ToString();
            var autoresDTO = mapper.Map<List<AutorDTO>>(autores);
            
            var resultado = new ColeccionDeRecursos<AutorDTO>(autoresDTO);

            if (IncluirEnlacesHateOS)
            {
                autoresDTO.ForEach(s => GenerarEnlaces(s));
                resultado.Enlaces.Add(new Enlace(Url.Link("ObtenerAutores", new { }), rel: "self", metodo: "GET"));
                resultado.Enlaces.Add(new Enlace(Url.Link("CrearAutor", new { }), rel: "CreateAuthor", metodo: "POST"));
            }

            return Ok(autoresDTO);
            //context.Autores.Include(x => x.Libros).ToList();
        }
        //api/autores/1 o api/autores/1/oliver
        //[HttpGet("{id}/{params?}", Name = "ObtenerAutor")] // para definir un valor opcional
        //[HttpGet("{id}/{params=Oliver}", Name = "ObtenerAutor")] //definir un valor predeterminado
        [HttpGet("{id}", Name = "ObtenerAutor")]
        [ServiceFilter(typeof(HATEOASAuthorFilterAttribute))]
        //[ProducesResponseType(404)]
        //[ProducesResponseType(typeof(AutorDTO),200)]
        public async Task<ActionResult<AutorDTO>> Get(int id,/*[BindRequired]*/ string param2)
        {
            /*claseB.HacerAlgo();
            logger.LogDebug($"buscando autor de {id}");*/

            //var autor = await context.Autores.Include(x => x.Libros).FirstOrDefaultAsync(x => x.Id == id);
            var autor = await context.Autores.FirstOrDefaultAsync(x => x.Id == id);

            if (autor == null)
            {
                logger.LogWarning($"el autor de Id {id} no ha sido encontrado");
                return NotFound();
            }
            //mapear el modelo autor
            var autorDTO = mapper.Map<AutorDTO>(autor);

            //GenerarEnlaces(autorDTO);

            return autorDTO;
        }

        private void GenerarEnlaces(AutorDTO autor)
        {
            autor.Enlaces.Add(new Enlace(Url.Link("ObtenerAutor", new { id = autor.Id }), rel: "self", metodo: "GET"));
            autor.Enlaces.Add(new Enlace(Url.Link("ActualizarAutor", new { id = autor.Id }), rel: "update-author", metodo: "PUT"));
            autor.Enlaces.Add(new Enlace(Url.Link("BorrarAutor", new { id = autor.Id }), rel: "delete-author", metodo: "DELETE"));
        }

        [HttpPost(Name = "CrearAutor")]
        public async Task<ActionResult> Post([FromBody] AutorCreacionDTO autorCreacion)
        {

            //TryValidateModel(autor);
            var autor = mapper.Map<Autor>(autorCreacion);
            context.Autores.Add(autor);
            await context.SaveChangesAsync();
            var autorDTO = mapper.Map<AutorDTO>(autor);

            return new CreatedAtRouteResult("ObtenerAutor", new { id = autor.Id }, autorDTO);

        }

        [HttpPut("{id}", Name = "ActualizarAutor")]
        public async Task<ActionResult> Put(int id, [FromBody] AutorCreacionDTO autorActualizacion)
        {
            //mapeando
            var autor = mapper.Map<Autor>(autorActualizacion);
            autor.Id = id;

            context.Entry(autor).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return Ok();
        }

        //actualizacion parcial
        [HttpPatch("{id}", Name = "ActualizarParcialmenteAutor")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<AutorCreacionDTO> patchDocument)
        {
            if (patchDocument ==null)
            {
                return BadRequest();
            }
            var autorDelaBD = await context.Autores.FirstOrDefaultAsync(s => s.Id == id); 
            if (autorDelaBD == null)
            {
                return NotFound();
            }
            var autorDTO = mapper.Map<AutorCreacionDTO>(autorDelaBD);
            
            patchDocument.ApplyTo(autorDTO, ModelState);
            
            mapper.Map(autorDTO, autorDelaBD);

            var isValid = TryValidateModel(autorDelaBD);
            if (!isValid)
            {
                return BadRequest(ModelState);
            }
 
            await context.SaveChangesAsync();

            return Ok();
        }
        /// <summary>
        /// Borra un elemento específico
        /// </summary>
        /// <param name="id">Id del elemento a borrar</param>   
        [HttpDelete("{id}", Name = "BorrarAutor")]

        public async Task<ActionResult<Autor>> Delete(int id)
        {
            var autorId = await context.Autores.Select(s=>s.Id).FirstOrDefaultAsync(x =>x== id);

            if (autorId == default(int))
            {
                return NotFound();
            }

            context.Remove(new Autor { Id = autorId});
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
