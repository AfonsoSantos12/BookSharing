using System.ComponentModel.DataAnnotations;

namespace Booksharing.Models
{
    public class BookModel
    {

        [Required]
        public int BookModelId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Autor { get; set; }

        [Required]
        public int Ano { get; set; }

        [Required]
        public string Genero { get; set; }
    }

}