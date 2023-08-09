using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using new1.Models;
using System.Diagnostics;
using new1.data;

namespace new1.Controllers
{
    public class BaseController2 : Controller
    {
        private readonly applicationcontext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BaseController2(applicationcontext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewBag.IsLogged = _httpContextAccessor.HttpContext.Session.GetBool("IsLogged") ?? false;
            base.OnActionExecuting(context);
        }
    }

    public class HomeController : BaseController2
    {
        private readonly applicationcontext _dbContext;

        public HomeController(applicationcontext context, applicationcontext dbContext, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
            _dbContext = dbContext;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewBag.IsLogged = User.Identity.IsAuthenticated;
            base.OnActionExecuting(context);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Home()
        {
            return View();
        }

        [HttpGet] 
        [Route("Home/contact_us")] 
        public IActionResult contact_us()
        {
            return View();
        }

        [HttpPost]
        public IActionResult contact_us(contact_us model)
        {
            if (ViewBag.IsLogged == true)
            {
                var register = _dbContext.register.FirstOrDefault(r => r.emailId == model.emailId);
                if (register == null)
                {
                    ViewBag.Message = "Invalid email address.";
                    return View(model);
                }

                model.Register = register;

                _dbContext.Add(model);
                _dbContext.SaveChanges();

                ViewBag.Message = "Thank you for your message! We will get back to you soon.";
                return RedirectToAction("home", "account");
            }
            else
            {
                ViewBag.Message = "We Will Reply Soon To Your Email";
                return RedirectToAction("home", "account");
            }
        }


        public IActionResult ServeC()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }
    }
}
