using System;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using System.Web.Mvc;
using System.Linq;

namespace Pruefung_Praktisch_Musterloesung.Controllers
{
    public class Lab1Controller : Controller
    {
        /**
         * 
         * ANTWORTEN BITTE HIER
         *  Aufgabe 1,2,3:
         *  
         *  1. Attackemöglichkeit: File Enumeration Attacke
         *      In der Foreach-Schlaufe unten werden alle files aus dem angegebenen Directory gelesen und an die View
         *      gesendet.
         *      http://localhost:50374/Lab1?type=../../ 
         *      Mit dieser URL kann ich im Response alle Files in der Content-Directory sehen.
         *      Im HTML-Response sehe ich alle Files in den <a>-Tags
         *  
         *  2. Attackemöglichkeit: Directory Traversials
         *      Der 'type' parameter ermöglicht es auf die ganze Server-Ordner-Struktur zuzugreifen
         *      Mit dem File Parameter konnte ich nun ein beliebiges File vom Server laden
         *      Durch probieren konnte ich so das Bären-Bild anzeigen, das über das UI nicht zugänglich sind
         *      http://localhost:50374/Lab1/Detail?file=bear1.jpg&type=bears
         *      
         * 
         * */


        public ActionResult Index()
        {
            var type = Request.QueryString["type"];

            if (string.IsNullOrEmpty(type))
            {
                type = "lions";                
            }

            // if it is an allowed type
            if (type == "lions" || type == "elephants")
            {
                var path = "~/Content/images/" + type;

                List<List<string>> fileUriList = new List<List<string>>();

                if (Directory.Exists(Server.MapPath(path)))
                {
                    var scheme = Request.Url.Scheme;
                    var host = Request.Url.Host;
                    var port = Request.Url.Port;

                    string[] fileEntries = Directory.GetFiles(Server.MapPath(path));
                    foreach (var filepath in fileEntries)
                    {
                        var filename = Path.GetFileName(filepath);
                        var imageuri = scheme + "://" + host + ":" + port + path.Replace("~", "") + "/" + filename;

                        var urilistelement = new List<string>();
                        urilistelement.Add(filename);
                        urilistelement.Add(imageuri);
                        urilistelement.Add(type);

                        fileUriList.Add(urilistelement);
                    }
                }

                return View(fileUriList);
            } else
            {
                // otherwise return empty
                return View(new List<List<string>>());
            }
        }

        public ActionResult Detail()
        {
            var file = Request.QueryString["file"];
            var type = Request.QueryString["type"];

            if (string.IsNullOrEmpty(file))
            {
                file = "Lion1.jpg";
            }
            if (string.IsNullOrEmpty(type))
            {
                file = "lions";
            }

            // All pictures must start with the type name and a .jpg at the end
            if (
                (type == "lions" && file.StartsWith("Lion") & file.EndsWith(".jpg")) || 
                (type == "elephants" && file.StartsWith("Elephant") & file.EndsWith(".jpg")))
            {
                var relpath = "~/Content/images/" + type + "/" + file;

                List<List<string>> fileUriItem = new List<List<string>>();
                var path = Server.MapPath(relpath);

                if (System.IO.File.Exists(path))
                {
                    var scheme = Request.Url.Scheme;
                    var host = Request.Url.Host;
                    var port = Request.Url.Port;
                    var absolutepath = Request.Url.AbsolutePath;

                    var filename = Path.GetFileName(file);
                    var imageuri = scheme + "://" + host + ":" + port + "/Content/images/" + type + "/" + filename;

                    var urilistelement = new List<string>();
                    urilistelement.Add(filename);
                    urilistelement.Add(imageuri);
                    urilistelement.Add(type);

                    fileUriItem.Add(urilistelement);
                    
                }
                return View(fileUriItem);
            }
            else
            {
                // otherwise return empty
                return View(new List<string>());
            }

        }
    }
}