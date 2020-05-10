using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_32_State_Managament_Uygulama.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Hata()
        {
            return View();
        }

        public int Islem()
        {
            int a = 0;
            int b = 2;
            int c = b / a; // 0'a bölme hatası alacağız.
            return c;
        }
    }
}