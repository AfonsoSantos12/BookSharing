using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BookSharing.ViewModels;

namespace BookSharing.Controllers
{
    [Authorize(Roles = "admin")]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(RoleManager<IdentityRole> roleManager)
        {

            _roleManager = roleManager;

        }

        public IActionResult Index(){

            var roles = _roleManager.Roles;

            return View(roles);
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(RoleViewModel model )
        {
            if(ModelState.IsValid)
             {
                 var role = new IdentityRole{
                     Name = model.RoleName
                 };
                 
                 var result = await _roleManager.CreateAsync(role);

                 if(result.Succeeded)
                 {
                    return RedirectToAction("Index");
                 }

                 foreach(var item in result.Errors)
                 {
                     ModelState.AddModelError("", item.Description);
                 }
             }
             return View(model);
        }

        [HttpGet]

        public async Task<IActionResult> EditRole(string Id)
        {
            var role = await _roleManager.FindByIdAsync(Id);
            if(role !=null)
            {
                var model = new EditRoleViewModel{
                    Id = Id,
                    Name = role.Name
                };

                return View(model);
            }
            return RedirectToAction("Index");
        } 

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            if(ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(model.Id);
                role.Name = model.Name;

                var result = await _roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }

                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                
            }

            return View(model);
        }

        [HttpGet]

        public async Task<IActionResult> RemoveRole(string Id)
        {
            var role = await _roleManager.FindByIdAsync(Id);

            if(role !=null)
            {
                return View(role);
            }
            return RedirectToAction("Index", "Role");

           
        }

        [HttpPost, ActionName("DeleteRole")]
        [ValidateAntiForgeryToken]

        public async Task<ActionResult> RemoveRoleConfirmed (string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if(role == null)
            {
                return NotFound();
            }
            var result = await _roleManager.DeleteAsync(role);

            if(result.Succeeded)
            {
                return RedirectToAction("Index");
            }

            foreach (var item in result.Errors)
            {
                ModelState.AddModelError("", item.Description);
            }

            return View(role);
        }

                   
    }
}