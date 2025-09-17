using CMCS.Web.Data;
using CMCS.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMCS.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _context.Users.Include(u => u.Role).ToListAsync();
            return View(users);
        }

        // GET: User/EditRole/5
        public async Task<IActionResult> EditRole(int id)
        {
            var user = await _context.Users.Include(u => u.Role)
                                           .FirstOrDefaultAsync(u => u.RoleId == id);
            if (user == null) return NotFound();
            ViewBag.Roles = await _context.UserRoles.ToListAsync();
            return View(user);
        }

        // POST: User/EditRole/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRole(int id, int selectedRoleId)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.RoleId = selectedRoleId; // assign the role
            _context.Update(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
