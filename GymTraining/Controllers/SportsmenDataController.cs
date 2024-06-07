using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GymTraining.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace GymTraining.Controllers
{
    [Authorize(Roles = "Sportsman")]
    public class SportsmenDataController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SportsmenDataController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: SportsmenData
        public async Task<IActionResult> Index()
        {
            ApplicationUser? currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (currentUser == null)
            {
                return Unauthorized();
            }
            //var applicationDbContext = _context.SportsmenData.Include(s => s.ApplicationUser).Include(s => s.TrainerData);
            var applicationDbContext = _context.SportsmenData.Where(a => a.ApplicationUserId == currentUser.Id).Include(s => s.ApplicationUser).Include(s => s.TrainerData);
            SportsmanData? sportsmanData = await _context.SportsmenData
                .Where(s => s.ApplicationUserId == currentUser.Id)
                .FirstOrDefaultAsync();
            if (sportsmanData != null)
            {
                IEnumerable<Training> trainingSchedules = _context.TrainingSchedules
                    .Where(t => t.SportsmanDataId == sportsmanData.Id)
                    .Include(t => t.Exercise).Include(t => t.SportsmanData)
                    .OrderBy(t => t.TrainingStartTime);
                if (trainingSchedules != null)
                {
                    ViewData["trainingSchedules"] = trainingSchedules.ToList();
                }
            }
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: SportsmenData/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sportsmanData = await _context.SportsmenData
                .Include(s => s.ApplicationUser)
                .Include(s => s.TrainerData)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sportsmanData == null)
            {
                return NotFound();
            }

            return View(sportsmanData);
        }

        // GET: SportsmenData/Create
        public async Task<IActionResult> CreateAsync()
        {
            ApplicationUser? currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (currentUser == null)
            {
                return Unauthorized();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users.Where(u => u.Id == currentUser.Id), "Id", "Id");
            ViewData["TrainerDataId"] = new SelectList(_context.TrainersData, "Id", "Name");
            return View();
        }

        // POST: SportsmenData/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ApplicationUserId,Name,TrainerDataId")] SportsmanData sportsmanData)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser? currentUser = await _userManager.GetUserAsync(HttpContext.User);
                if (currentUser == null)
                {
                    return Unauthorized();
                }
                SportsmanData? sportsmanDataExists = await _context.SportsmenData
                    .Where(s => s.ApplicationUserId == sportsmanData.ApplicationUserId)
                    .FirstOrDefaultAsync();
                if (sportsmanDataExists != null)
                {
                    ModelState.AddModelError("", "Інформація про спортсмена вже створена.");
                    ViewData["ApplicationUserId"] = new SelectList(_context.Users.Where(u => u.Id == sportsmanData.ApplicationUserId), "Id", "Id", sportsmanData.ApplicationUserId);
                    ViewData["TrainerDataId"] = new SelectList(_context.TrainersData, "Id", "Name", sportsmanData.TrainerDataId);
                    return View(sportsmanData);
                }    
                _context.Add(sportsmanData);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users.Where(u => u.Id == sportsmanData.ApplicationUserId), "Id", "Id", sportsmanData.ApplicationUserId);
            ViewData["TrainerDataId"] = new SelectList(_context.TrainersData, "Id", "Name", sportsmanData.TrainerDataId);
            return View(sportsmanData);
        }

        // GET: SportsmenData/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sportsmanData = await _context.SportsmenData.FindAsync(id);
            if (sportsmanData == null)
            {
                return NotFound();
            }
            ApplicationUser? currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (currentUser == null)
            {
                return Unauthorized();
            }
            else if (currentUser.Id != sportsmanData.ApplicationUserId)
            {
                return View("AccessDenied");
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users.Where(u => u.Id == currentUser.Id), "Id", "Id", sportsmanData.ApplicationUserId);
            ViewData["TrainerDataId"] = new SelectList(_context.TrainersData, "Id", "Name", sportsmanData.TrainerDataId);
            return View(sportsmanData);
        }

        // POST: SportsmenData/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ApplicationUserId,Name,TrainerDataId")] SportsmanData sportsmanData)
        {
            if (id != sportsmanData.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sportsmanData);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SportsmanDataExists(sportsmanData.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users.Where(u => u.Id == sportsmanData.ApplicationUserId), "Id", "Id", sportsmanData.ApplicationUserId);
            ViewData["TrainerDataId"] = new SelectList(_context.TrainersData, "Id", "Name", sportsmanData.TrainerDataId);
            return View(sportsmanData);
        }

        // GET: SportsmenData/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sportsmanData = await _context.SportsmenData
                .Include(s => s.ApplicationUser)
                .Include(s => s.TrainerData)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sportsmanData == null)
            {
                return NotFound();
            }
            ApplicationUser? currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (currentUser == null)
            {
                return Unauthorized();
            }
            else if (currentUser.Id != sportsmanData.ApplicationUserId)
            {
                return View("AccessDenied");
            }

            return View(sportsmanData);
        }

        // POST: SportsmenData/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sportsmanData = await _context.SportsmenData.FindAsync(id);
            if (sportsmanData != null)
            {
                ApplicationUser? currentUser = await _userManager.GetUserAsync(HttpContext.User);
                if (currentUser == null)
                {
                    return Unauthorized();
                }
                else if (currentUser.Id != sportsmanData.ApplicationUserId)
                {
                    return View("AccessDenied");
                }
                _context.SportsmenData.Remove(sportsmanData);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SportsmanDataExists(int id)
        {
            return _context.SportsmenData.Any(e => e.Id == id);
        }
    }
}
