using System.ComponentModel.DataAnnotations;

namespace BookSharing.ViewModels
{
     public class RoleViewModel
    {
        [Required]
        public string RoleName { get; set; }


    }

}