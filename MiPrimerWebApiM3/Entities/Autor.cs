using MiPrimerWebApiM3.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MiPrimerWebApiM3.Entities
{
    [Table("Autores")]
    public class Autor : IValidatableObject
    {
        public int Id { get; set; }
        [Required]
        [StringLength(10,ErrorMessage ="El campo debe tener {1} caracteres o menos")]
        //[PrimeraLetraMayuscula]
        public string Nombre { get; set; }
        public string Identificacion { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public List<Libro> Libros { get; set; }
        //[Range(18,120)]
        //public int Edad { get; set; }
        ////[CreditCard]
        //public string CreditCard { get; set; }
        //[Url]
        //public string Url { get; set; }

        //validación especializada por modelo 
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(Nombre))
            {
                var primerLetra = Nombre[0].ToString();
                if (primerLetra != primerLetra.ToUpper())
                {
                    yield return new ValidationResult("La primera letra debe ser mayuscula.", new string[] { nameof(Nombre) });
                }
            }
        }
    }
}
