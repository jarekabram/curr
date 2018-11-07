using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;

namespace MvcMovie.Controllers
{
    public class MainPageController : Controller
    {
        // 
        // GET: /Main/

        public string Index()
        {
            return "Index";
        }

        // 
        // GET: /Main/Welcome/ 

        public string Welcome()
        {
            return "Welcome";
        }
    }
}