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
using System.Globalization;

namespace GymTraining.Controllers
{
    public class TrainingSchedulesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TrainingSchedulesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: TrainingSchedules
        [Authorize(Roles = "Trainer, Sportsman")]
        public async Task<IActionResult> Index()
        {
            ApplicationUser? currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (currentUser == null)
            {
                return Unauthorized();
            }
            IList<string> userRoles = new List<string>();
            userRoles = await _userManager.GetRolesAsync(currentUser);
            if (userRoles.Contains("Trainer"))
            {
                TrainerData? trainerData = _context.TrainersData
                .Where(t => t.ApplicationUserId == currentUser.Id).FirstOrDefault();
                if (trainerData != null)
                {
                    IEnumerable<SportsmanData> sportsmen = _context.SportsmenData
                        .Where(s => s.TrainerDataId == trainerData.Id);
                    List<int> sportsmenId = new List<int>();
                    foreach (SportsmanData sportsman in sportsmen)
                    {
                        sportsmenId.Add(sportsman.Id);
                    }
                    var trainingSchedules = _context.TrainingSchedules
                        .Where(t => sportsmenId.Contains(t.SportsmanDataId))
                        .Include(t => t.Exercise).Include(t => t.SportsmanData)
                        .OrderBy(t => t.TrainingStartTime);
                    return View(await trainingSchedules.ToListAsync());
                }
            }
            if (userRoles.Contains("Sportsman"))
            {
                SportsmanData? sportsmanData = await _context.SportsmenData
                    .Where(s => s.ApplicationUserId == currentUser.Id)
                    .FirstOrDefaultAsync();
                if (sportsmanData != null)
                {
                    var trainingSchedules = _context.TrainingSchedules
                        .Where(t => t.SportsmanDataId == sportsmanData.Id)
                        .Include(t => t.Exercise).Include(t => t.SportsmanData)
                        .OrderBy(t => t.TrainingStartTime);
                    return View(await trainingSchedules.ToListAsync());
                }
            }
            //var applicationDbContext = _context.TrainingSchedules.Include(t => t.Exercise).Include(t => t.SportsmanData);
            //return View(await applicationDbContext.ToListAsync());
            List<Training> training = new List<Training>();
            return View(training);
        }

        // GET: TrainingSchedules/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var training = await _context.TrainingSchedules
                .Include(t => t.Exercise)
                .Include(t => t.SportsmanData)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (training == null)
            {
                return NotFound();
            }

            return View(training);
        }

        // GET: TrainingSchedules/Create
        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> CreateAsync()
        {
            ApplicationUser? currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (currentUser == null)
            {
                return Unauthorized();
            }
            TrainerData? trainerData = await _context.TrainersData
                .Where(t => t.ApplicationUserId == currentUser.Id)
                .FirstOrDefaultAsync();
            if (trainerData == null)
            {
                ModelState.AddModelError("", "Інформація про тренера не знайдена.");
                return BadRequest(ModelState);
            }
            ViewData["ExerciseId"] = new SelectList(_context.Exercises, "Id", "Name");
            ViewData["SportsmanDataId"] = new SelectList(_context.SportsmenData.Where(s => s.TrainerDataId == trainerData.Id), "Id", "Name");
            //ViewData["SportsmanDataId"] = new SelectList(_context.SportsmenData, "Id", "Name");
            return View();
        }

        // POST: TrainingSchedules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> Create([Bind("Id,SportsmanDataId,TrainingStartTime,ExerciseId,Meal,IsCompleted")] Training training)
        {
            ApplicationUser? currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (currentUser == null)
            {
                return Unauthorized();
            }
            TrainerData? trainerData = await _context.TrainersData
                .Where(t => t.ApplicationUserId == currentUser.Id)
                .FirstOrDefaultAsync();
            if (trainerData == null)
            {
                ModelState.AddModelError("", "Інформація про тренера не знайдена.");
                return BadRequest(ModelState);
            }
            if (ModelState.IsValid)
            {
                if (training.SportsmanDataId == 0)
                {
                    ViewData["ErrorMessage"] = "Спорстмен відсутній. Зараз спортсмена може тренувати інший тренер.";
                    return View("HandleError");
                }
                _context.Add(training);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ExerciseId"] = new SelectList(_context.Exercises, "Id", "Name", training.ExerciseId);
            ViewData["SportsmanDataId"] = new SelectList(_context.SportsmenData.Where(s => s.TrainerDataId == trainerData.Id), "Id", "Name", training.SportsmanDataId);
            return View(training);
        }

        // GET: TrainingSchedules/Edit/5
        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var training = await _context.TrainingSchedules.FindAsync(id);
            if (training == null)
            {
                return NotFound();
            }
            ApplicationUser? currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (currentUser == null)
            {
                return Unauthorized();
            }
            TrainerData? trainerData = await _context.TrainersData
                .Where(t => t.ApplicationUserId == currentUser.Id)
                .FirstOrDefaultAsync();
            if (trainerData == null)
            {
                ModelState.AddModelError("", "Інформація про тренера не знайдена.");
                return BadRequest(ModelState);
            }
            ViewData["ExerciseId"] = new SelectList(_context.Exercises, "Id", "Name", training.ExerciseId);
            ViewData["SportsmanDataId"] = new SelectList(_context.SportsmenData.Where(s => s.TrainerDataId == trainerData.Id), "Id", "Name", training.SportsmanDataId);
            return View(training);
        }

        // POST: TrainingSchedules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SportsmanDataId,TrainingStartTime,ExerciseId,Meal,IsCompleted")] Training training)
        {
            if (id != training.Id)
            {
                return NotFound();
            }
            ApplicationUser? currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (currentUser == null)
            {
                return Unauthorized();
            }
            TrainerData? trainerData = await _context.TrainersData
                .Where(t => t.ApplicationUserId == currentUser.Id)
                .FirstOrDefaultAsync();
            if (trainerData == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                if (training.SportsmanDataId == 0)
                {
                    ViewData["ErrorMessage"] = "Спорстмен відсутній. Зараз спортсмена може тренувати інший тренер.";
                    return View("HandleError");
                }
                if (training.ExerciseId == 0)
                {
                    ViewData["ErrorMessage"] = "Вправа  відсутня.";
                    return View("HandleError");
                }
                try
                {
                    _context.Update(training);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainingExists(training.Id))
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
            ViewData["ExerciseId"] = new SelectList(_context.Exercises, "Id", "Id", training.ExerciseId);
            ViewData["SportsmanDataId"] = new SelectList(_context.SportsmenData.Where(s => s.TrainerDataId == trainerData.Id), "Id", "Name", training.SportsmanDataId);
            return View(training);
        }

        // GET: TrainingSchedules/Delete/5
        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var training = await _context.TrainingSchedules
                .Include(t => t.Exercise)
                .Include(t => t.SportsmanData)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (training == null)
            {
                return NotFound();
            }
            ApplicationUser? currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (currentUser == null)
            {
                return Unauthorized();
            }
            TrainerData? trainerData = await _context.TrainersData
                .Where(t => t.ApplicationUserId == currentUser.Id)
                .FirstOrDefaultAsync();
            if (trainerData == null)
            {
                ModelState.AddModelError("", "Інформація про тренера не знайдена.");
                return BadRequest(ModelState);
            }
            else if (training.SportsmanData != null && 
                training.SportsmanData.TrainerDataId != trainerData.Id) 
            {
                return View("AccessDenied");
            }

            return View(training);
        }

        // POST: TrainingSchedules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var training = await _context.TrainingSchedules.FindAsync(id);
            if (training != null)
            {
                _context.TrainingSchedules.Remove(training);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: TrainingSchedules/TrainingCheckAjax/5/true/
        [HttpPost]
        [Route("/TrainingSchedules/TrainingCheckAjax/{id:int?}/{trainingChecked:bool?}")]
        [Authorize(Roles = "Sportsman, Administrator")]
        public async Task<ExercisesDone> TrainingCheckAjax(int? id, bool? trainingChecked)
        {
            ExercisesDone exercisesDone = new ExercisesDone();
            if (id == null)
            {
                exercisesDone.Error = "Тренування не знайдене.";
                return exercisesDone;
            }
            ApplicationUser? currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (currentUser == null)
            {
                exercisesDone.Error = "Користувач не авторизований.";
                return exercisesDone;
            }
            Training? training = await _context.TrainingSchedules.FindAsync(id);
            if (training == null)
            {
                exercisesDone.Error = "Тренування не знайдене.";
                return exercisesDone;
            }
            SportsmanData? sportsmanData = await _context.SportsmenData
                .Where(s => s.ApplicationUserId == currentUser.Id).Include(s => s.TrainerData)
                .FirstOrDefaultAsync();
            if (sportsmanData == null)
            {
                exercisesDone.Error = "Дані спорсмена не знайдені.";
                return exercisesDone;
            }
            else if (sportsmanData.Id != training.SportsmanDataId)
            {
                exercisesDone.Error = "У вас нема доступу до цього тренування.";
                return exercisesDone;
            }
            if (trainingChecked != null)
            {
                training.IsCompleted = trainingChecked.Value;
            }
            try
            {
                _context.Update(training);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                exercisesDone.Error = ex.Message;
                return exercisesDone;
            }
            exercisesDone.Done = _context.TrainingSchedules
                .Where(t => t.SportsmanDataId == sportsmanData.Id &&
                t.IsCompleted == true).Count();
            return exercisesDone;
        }

        private bool TrainingExists(int id)
        {
            return _context.TrainingSchedules.Any(e => e.Id == id);
        }
    }
}
