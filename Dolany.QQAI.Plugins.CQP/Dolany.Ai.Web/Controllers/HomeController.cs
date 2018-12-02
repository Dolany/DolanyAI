using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dolany.Ai.Web.Models;
using System.Collections.Immutable;

namespace Dolany.Ai.Web.Controllers
{
    public class HomeController : Controller
    {
        //private static ImmutableList<string> msgList = ImmutableList.Create<string>();

        //private static MQReceiver Receiver = new MQReceiver();

        public IActionResult Index()
        {
            //Receiver.Receive(MQConfig.InformationQueue, s => AppendList(s));
            //ViewBag.MList = msgList.ToList();
            return View();
        }

        //private void AppendList(string msg)
        //{
        //    msgList.Add(msg);
        //}

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

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
    }
}
