using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MissingPeopleDatabase.Models;
using System.Threading.Tasks;

namespace MissingPeopleDatabase.Controllers
{
    public class AdminController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        public AdminController(RoleManager<IdentityRole> roleManager)
        {
            this.roleManager = roleManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(UserRole role)
        {
            var roleExists = await roleManager.RoleExistsAsync(role.RoleName);
            if (!roleExists)
            {
                var result = await roleManager.CreateAsync(new IdentityRole(role.RoleName));
            }
            return View();
        }
    }
}
