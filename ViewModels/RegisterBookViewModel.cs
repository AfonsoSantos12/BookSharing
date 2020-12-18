using System.ComponentModel.DataAnnotations;

namespace BookSharing.ViewModels
{
    public class RegisterBookViewModel
    {

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