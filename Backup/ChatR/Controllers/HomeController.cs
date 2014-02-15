using System.Linq;
using System.Web.Mvc;
using ChatR.Models;

namespace ChatR.Controllers
{
    public class HomeController : Controller
    {
        private InMemoryRepository _repository;

        public HomeController()
        {
            _repository = InMemoryRepository.GetInstance();
        }

        //
        // GET: /Home/
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string username, string roomname)
        {
            if (string.IsNullOrEmpty(username))
            {
                ModelState.AddModelError("username", "Username is required");
                return View();
            }
            if (string.IsNullOrEmpty(roomname))
            {
                ModelState.AddModelError("roomnam", "Roomname is required");
                return View();
            }
            // if we have an already logged user with the same username, then append a random number to it
            if (_repository.Users.Any(u => u.Username.Equals(username)))
            {
                username = _repository.GetRandomizedUsername(username);
            }
            return View("Chat", "_Layout", new object[] {username,  roomname});
        }
    }
}
