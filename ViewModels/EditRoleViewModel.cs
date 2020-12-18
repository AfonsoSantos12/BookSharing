using System.ComponentModel.DataAnnotations;

namespace BookSharing.ViewModels
{

    public class EditRoleViewModel{
        public string Id{get;set;}

        [Required]

        public string Name {get;set;}
    }
}