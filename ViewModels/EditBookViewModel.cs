using System.ComponentModel.DataAnnotations;

namespace BookSharing.ViewModels
{

    public class EditBookViewModel{
        public int Id{get;set;}

        [Required]
        public string Name {get;set;}
    }
}