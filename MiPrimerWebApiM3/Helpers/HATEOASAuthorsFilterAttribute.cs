using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MiPrimerWebApiM3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiPrimerWebApiM3.Helpers
{
    public class HATEOASAuthorsFilterAttribute: HATEOASFilterAttribute
    {
        private readonly GeneradorEnlaces generadorEnlaces;

        public HATEOASAuthorsFilterAttribute(GeneradorEnlaces generadorEnlaces)
        {
            this.generadorEnlaces = generadorEnlaces ?? throw new ArgumentNullException(nameof(generadorEnlaces));
        }

        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var incluirHATEOAS = DebeIncluirHATEOAS(context);

            if (!incluirHATEOAS)
            {
                await next();
                return;
            }

            var result = context.Result as ObjectResult;
            var model = result.Value as List<AutorDTO> ?? throw new ArgumentNullException("Se esperaba una instancia de List<AutorDTO>");
            result.Value = generadorEnlaces.GenerarEnlaces(model);
            await next();
        }
    }
}
