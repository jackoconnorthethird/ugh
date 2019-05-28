using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Prayingtopass.Models;

namespace BeltExam.Controllers {
    public class HomeController : Controller {
        private HomeContext dbContext;

        public HomeController (HomeContext context) {
            dbContext = context;
        }

        [HttpGet ("")]
        public IActionResult Index () {
            return View ();
        }

        [HttpPost ("register")]
        public IActionResult Register (User newUser) {
            if (ModelState.IsValid) {
                if (dbContext.Users.Any (u => u.Email == newUser.Email)) {
                    ModelState.AddModelError ("Email", "Email already in use");
                    return View ("Index");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User> ();
                newUser.Password = Hasher.HashPassword (newUser, newUser.Password);
                dbContext.Users.Add (newUser);
                dbContext.SaveChanges ();
                HttpContext.Session.SetInt32 ("UserId", newUser.UserId);
                Console.WriteLine (newUser);
                return RedirectToAction ("Home");
            } else {
                return View ("Index");
            }
        }

        [HttpPost ("login")]
        public IActionResult Login (LoginUser logUser) {
            if (ModelState.IsValid) {
                User userInDb = dbContext.Users.FirstOrDefault (u => u.Email == logUser.LoginEmail);
                if (userInDb == null) {
                    ModelState.AddModelError ("Email", "That email/password is not signed up yet");
                    return View ("Index");
                }
                var hasher = new PasswordHasher<LoginUser> ();
                var result = hasher.VerifyHashedPassword (logUser, userInDb.Password, logUser.LoginPassword);

                if (result == 0) {
                    ModelState.AddModelError ("Password", "Password is incorrect");
                    return View ("Index");
                }
                HttpContext.Session.SetInt32 ("UserId", userInDb.UserId);
                return RedirectToAction ("Home");
            } else {
                return View ("Index");
            }
        }

        [HttpGet ("Home")]
        public IActionResult Home () {
            if (HttpContext.Session.GetInt32 ("UserId") == null) {
                return RedirectToAction ("Index");
            }

            User thisUser = dbContext.Users.FirstOrDefault (u => u.UserId == HttpContext.Session.GetInt32 ("UserId"));
            ViewBag.ThisUser = thisUser;

            List<Actvty> EveryActivity = dbContext.Activities.Include (w => w.ActivityAttendees).ThenInclude (a => a.User).OrderBy (e => e.ActivityDate).ToList ();

            foreach (Actvty a in EveryActivity.ToList ()) {
                if (a.ActivityDate < DateTime.Now) {
                    EveryActivity.Remove (a);
                }
            }
            ViewBag.AllActivities = EveryActivity;

            List<User> userCreators = dbContext.Users.ToList ();
            ViewBag.Creators = userCreators;
            return View ();
        }

        [HttpGet ("new")]
        public IActionResult AddActivity () {
            if (HttpContext.Session.GetInt32 ("UserId") == null) {
                return RedirectToAction ("Index");
            }
            return View ();
        }

        [HttpPost ("CreateActivity")]
        public IActionResult CreateActivity (Actvty AddActivity) {
            if (ModelState.IsValid) {
                AddActivity.PlannerId = (int) HttpContext.Session.GetInt32 ("UserId");
                dbContext.Add (AddActivity);
                dbContext.SaveChanges ();
                Actvty thisActivity = dbContext.Activities.OrderByDescending (w => w.CreatedAt).FirstOrDefault ();
                return Redirect ("/activity/" + thisActivity.ActvtyId);
            }
            return View ("AddActivity", AddActivity);
        }

        [HttpGet ("activity/{actId}")]
        public IActionResult ActivityDetail (int actId) {
            if (HttpContext.Session.GetInt32 ("UserId") == null) {
                return RedirectToAction ("Index");
            }

            User thisUser = dbContext.Users.FirstOrDefault (u => u.UserId == HttpContext.Session.GetInt32 ("UserId"));
            ViewBag.ThisUser = thisUser;

            Actvty thisActivity = dbContext.Activities.FirstOrDefault (w => w.ActvtyId == actId);
            ViewBag.ThisActivity = thisActivity;

            User eventCoord = dbContext.Users.FirstOrDefault (ec => ec.UserId == thisActivity.PlannerId);
            ViewBag.EventCoordinator = eventCoord;

            var actParticipants = dbContext.Activities.Include (w => w.ActivityAttendees).ThenInclude (u => u.User).FirstOrDefault (w => w.ActvtyId == actId);

            ViewBag.AllParticipants = actParticipants.ActivityAttendees;
            return View ("ActivityDetail");
        }

        [HttpGet ("delete/{actId}")]
        public IActionResult DeleteActivity (int actId) {
            Actvty actToDelete = dbContext.Activities.FirstOrDefault (w => w.ActvtyId == actId);
            dbContext.Activities.Remove (actToDelete);
            dbContext.SaveChanges ();
            return RedirectToAction ("Home");
        }

        [HttpGet ("leave/{partId}")]
        public IActionResult NoActivity (int partId) {
            Participation participation = dbContext.Participations.FirstOrDefault (a => a.ParticipationId == partId);
            dbContext.Participations.Remove (participation);
            dbContext.SaveChanges ();
            return RedirectToAction ("Home");
        }

        [HttpGet ("join/{actId}")]
        public IActionResult YesActivity (int actId) {
            Actvty thisActivity = dbContext.Activities.FirstOrDefault (a => a.ActvtyId == actId);
            User usersActivities = dbContext.Users
                .Include (a => a.AttendedActs)
                .ThenInclude (e => e.Activity)
                .ToList ().FirstOrDefault (u => u.UserId == HttpContext.Session.GetInt32 ("UserId"));

            bool canAttend = true;
            foreach (var a in usersActivities.AttendedActs) {
                if (a.Activity.ActivityDate.Date == thisActivity.ActivityDate.Date) {
                    canAttend = false;
                    Console.WriteLine ("This user cannot attend activity: " + thisActivity.Title);
                }
            }

            if (canAttend) {
                Participation participation = new Participation ();
                participation.UserId = (int) HttpContext.Session.GetInt32 ("UserId");
                participation.ActivityId = actId;
                dbContext.Participations.Add (participation);
                dbContext.SaveChanges ();
            }

            return RedirectToAction ("Home");
        }

        [HttpGet ("logout")]
        public IActionResult Logout () {
            HttpContext.Session.Clear ();
            return RedirectToAction ("Index");
        }

    }
}