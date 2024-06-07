using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GymTraining.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace GymTraining.Controllers
{
    [Authorize(Roles = "Trainer")]
    public class TrainersDataController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TrainersDataController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: TrainersData
        public async Task<IActionResult> Index()
        {
            ApplicationUser? currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (currentUser == null)
            {
                return Unauthorized();
            }
            //var applicationDbContext = _context.TrainersData.Include(t => t.ApplicationUser);
            var applicationDbContext = _context.TrainersData
                .Where(a => a.ApplicationUserId == currentUser.Id).Include(t => t.ApplicationUser);
            IEnumerable<SportsmanData> sportsmenData = _context.SportsmenData
                .Where(s => s.TrainerData.ApplicationUserId == currentUser.Id)
                .Include(s => s.TrainerData);
            if (sportsmenData != null)
            {
                ViewData["sportsmenData"] = sportsmenData.ToList();
            }
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: TrainersData/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainerData = await _context.TrainersData
                .Include(t => t.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trainerData == null)
            {
                return NotFound();
            }

            return View(trainerData);
        }

        // GET: TrainersData/Create
        public async Task<IActionResult> CreateAsync()
        {
            ApplicationUser? currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (currentUser == null)
            {
                return Unauthorized();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users.Where(u => u.Id == currentUser.Id), "Id", "Id");
            ViewData["UserId"] = currentUser.Id;
            return View();
        }

        // POST: TrainersData/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ApplicationUserId,Name")] TrainerData trainerData)
        {
            if (ModelState.IsValid)
            {
                TrainerData? trainerDataExists = await _context.TrainersData
                    .Where(t => t.ApplicationUserId == trainerData.ApplicationUserId)
                    .FirstOrDefaultAsync();
                if (trainerDataExists != null)
                {
                    ModelState.AddModelError("", "Інформація про тренера вже створена.");
                    ViewData["ApplicationUserId"] = new SelectList(_context.Users.Where(u => u.Id == trainerData.ApplicationUserId), "Id", "Id", trainerData.ApplicationUserId);
                    return View(trainerData);
                }
                _context.Add(trainerData);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users.Where(u => u.Id == trainerData.ApplicationUserId), "Id", "Id", trainerData.ApplicationUserId);
            return View(trainerData);
        }

        // GET: TrainersData/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainerData = await _context.TrainersData.FindAsync(id);
            if (trainerData == null)
            {
                return NotFound();
            }
            ApplicationUser? currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (currentUser == null)
            {
                return Unauthorized();
            }
            else if (currentUser.Id != trainerData.ApplicationUserId)
            {
                return View("AccessDenied");
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users.Where(u => u.Id == trainerData.ApplicationUserId), "Id", "Id", trainerData.ApplicationUserId);
            return View(trainerData);
        }

        // POST: TrainersData/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ApplicationUserId,Name")] TrainerData trainerData)
        {
            if (id != trainerData.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trainerData);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainerDataExists(trainerData.Id))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.Users.Where(u => u.Id == trainerData.ApplicationUserId), "Id", "Id", trainerData.ApplicationUserId);
            return View(trainerData);
        }

        // GET: TrainersData/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainerData = await _context.TrainersData
                .Include(t => t.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trainerData == null)
            {
                return NotFound();
            }
            ApplicationUser? currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (currentUser == null)
            {
                return Unauthorized();
            }
            else if (currentUser.Id != trainerData.ApplicationUserId)
            {
                return View("AccessDenied");
            }
            IEnumerable<SportsmanData> sportsmenData = _context.SportsmenData.
                Where(s => s.TrainerDataId == trainerData.Id);
            if (sportsmenData != null && sportsmenData.ToList().Count() > 0)
            {
                ViewData["ErrorMessage"] = "Не можна вилучити тренера коли у тренера є спортсмени.";
                return View("HandleError");
            }

            return View(trainerData);
        }

        // POST: TrainersData/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trainerData = await _context.TrainersData.FindAsync(id);
            if (trainerData != null)
            {
                _context.TrainersData.Remove(trainerData);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: TrainersData/SportsmanTraining/5
        public async Task<IActionResult> SportsmanTraining(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ApplicationUser? currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (currentUser == null)
            {
                return Unauthorized();
            }
            TrainerData? trainerData = await _context.TrainersData
                .Where(t => t.ApplicationUserId == currentUser.Id).FirstOrDefaultAsync();
            if (trainerData == null) 
            {
                ModelState.AddModelError("", "Інформація про тренера не знайдена.");
                return BadRequest(ModelState);
            }
            SportsmanData? sportsmanData = await _context.SportsmenData
                .Where(s => s.Id == id)
                .FirstOrDefaultAsync();
            if (sportsmanData == null)
            {
                return NotFound();
            }
            if (sportsmanData.TrainerDataId != trainerData.Id)
            {
                return View("AccessDenied");
            }
            IEnumerable<Training> trainingSchedules = _context.TrainingSchedules
                    .Where(t => t.SportsmanDataId == sportsmanData.Id)
                    .Include(t => t.Exercise).Include(t => t.SportsmanData)
                    .OrderBy(t => t.TrainingStartTime);
            ViewData["sportsmanData"] = sportsmanData;
            return View(trainingSchedules.ToList());
        }

        private bool TrainerDataExists(int id)
        {
            return _context.TrainersData.Any(e => e.Id == id);
        }
    }
}
