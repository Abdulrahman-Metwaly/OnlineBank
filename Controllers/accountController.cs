using Microsoft.AspNetCore.Mvc;
using new1.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using new1.data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace new1.Controllers
{
    public static class SessionExtensions
    {
        public static void SetBool(this ISession session, string key, bool value)
        {
            session.SetString(key, value.ToString());
        }

        public static bool? GetBool(this ISession session, string key)
        {
            var value = session.GetString(key);
            return string.IsNullOrEmpty(value) ? (bool?)null : bool.Parse(value);
        }
    }

    public class BaseController : Controller
    {
        private readonly applicationcontext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BaseController(applicationcontext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        protected register GetRegisterModelByEmail(string email)
        {
            return _context.register.FirstOrDefault(r => r.emailId == email);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewBag.IsLogged = _httpContextAccessor.HttpContext.Session.GetBool("IsLogged") ?? false;
            base.OnActionExecuting(context);
        }
    }

    public class AccountController : BaseController
    {
        private readonly applicationcontext _context;

        public AccountController(applicationcontext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
            _context = context;
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            ViewBag.IsLogged = false;

            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult Deposit()
        {
            return View(new Deposit());
        }

        public IActionResult Deposit(Deposit depositForm)
        {
            if (!ModelState.IsValid)
            {
                return View(depositForm);
            }

            var userName = HttpContext.Session.GetString("emailId");
            var homeModel = _context.home.FirstOrDefault(h => h.emailId == userName);

            if (homeModel == null)
            {
                return NotFound();
            }

            if (decimal.TryParse(depositForm.Amount.ToString(), out decimal depositAmount))
            {
                homeModel.Amount += depositAmount;
                _context.home.Update(homeModel);
                _context.SaveChanges();
            }
            else
            {
                ModelState.AddModelError("Amount", "Invalid amount value.");
                return View(depositForm);
            }

            return RedirectToAction("Home");
        }


        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ChangePassword(ChangePassword model)
        {
                string userEmail = HttpContext.Session.GetString("emailId");

                var user = _context.register.FirstOrDefault(u => u.emailId == userEmail);

                if (user != null && user.password == model.OldPassword)
                {
                    user.password = model.NewPassword;

                    _context.SaveChanges();

                    return RedirectToAction("home", "account");
                }

                ModelState.AddModelError(string.Empty, "Invalid email or password.");

            return View(model);
        }



        [HttpGet]
        public IActionResult Setting()
        {
            var accountInfo = _context.register.FirstOrDefault(); 

            if (accountInfo == null)
            {
                return NotFound();
            }

            return View(accountInfo);
        }

        [HttpGet]
        public IActionResult Transaction()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Transaction(string recipientEmail, decimal amount)
        {
            var senderEmail = HttpContext.Session.GetString("emailId");

            var senderAccount = _context.home.FirstOrDefault(h => h.emailId == senderEmail);
            var recipientAccount = _context.home.FirstOrDefault(h => h.emailId == recipientEmail);

            if (senderEmail == recipientEmail)
            {
                ModelState.AddModelError("", "Sender and recipient emails cannot be the same.");
                return View("Transaction");
            }

            if (senderAccount == null || recipientAccount == null)
            {
                ModelState.AddModelError("", "Invalid sender or recipient account.");
                return View("Transaction");
            }

            if (senderAccount.Amount < amount)
            {
                ModelState.AddModelError("", "Insufficient balance in the sender's account.");
                return View("Transaction");
            }

            senderAccount.Amount -= amount;
            recipientAccount.Amount += amount;

            _context.SaveChanges();

            return RedirectToAction("home", "account");
        }


        [HttpGet]
        public IActionResult ContactUsInfo()
        {
            string sessionEmail = HttpContext.Session.GetString("emailId");

            var contactInfo = _context.contact_us.Where(c => c.emailId == sessionEmail).ToList();

            if (contactInfo.Count == 0)
            {
                // Display default values or handle the absence of data
                contactInfo.Add(new contact_us { Subject = "No Subject", Message = "No Message", Answer = "No Answer" });
            }

            return View(contactInfo);
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View(new register());
        }

        [HttpPost]
        public IActionResult Register(register user, IFormFile imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    imageFile.CopyTo(memoryStream);
                    user.Imagefile = memoryStream.ToArray();
                }
            }

            _context.register.Add(user);
            _context.SaveChanges();

            var homeModel = new home
            {
                emailId = user.emailId,
                first_name = user.first_name,
                last_name = user.last_name,
                imagefile = user.Imagefile,
                Amount = 0
            };

            _context.home.Add(homeModel);
            _context.SaveChanges();

            ViewBag.Message = "Thank you for your register!";
            return RedirectToAction("login", "account");
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Login(login model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.register.FirstOrDefault(u => u.emailId == model.emailId && u.password == model.password);
                if (user != null)
                {
                    HttpContext.Session.SetString("emailId", user.emailId);
                    HttpContext.Session.SetBool("IsLogged", true);

                    return RedirectToAction("Home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid email or password.");
                }
            }

            HttpContext.Session.SetBool("IsLogged", false);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Home(bool? isLogged)
        {
            var userName = HttpContext.Session.GetString("emailId");
            if (string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("Index", "Home");
            }

            var home = _context.home.FirstOrDefault(m => m.emailId == userName);

            if (home == null)
            {
                return NotFound();
            }

            if (home.imagefile != null && home.imagefile.Length > 0)
            {
                var base64String = Convert.ToBase64String(home.imagefile);
                var imageSrc = string.Format("data:image;base64,{0}", base64String);
                ViewBag.ImageSrc = imageSrc;
            }
            else
            {
                ViewBag.ImageSrc = "path/to/placeholder-image.jpg";
            }

            var model = new home
            {
                first_name = home.first_name,
                last_name = home.last_name,
                emailId = home.emailId,
                Amount = home.Amount,
                imagefile = home.imagefile
            };

            return View(model);
        }

    }
}