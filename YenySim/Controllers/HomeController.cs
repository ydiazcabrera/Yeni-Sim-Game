using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YenySim.Models;

namespace YenySim.Controllers
{
    public class HomeController : Controller
    {
        YenySimSaveEntities db = new YenySimSaveEntities();
        Random random = new Random();
        public ActionResult Index()
        {
           
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Game()//specify what city to pull out
        {

            City city = db.Cities.ToList().First();
            return View(city);
        }
        public ActionResult Harvest(string resource)
        {
            City city = db.Cities.ToList().First();

            
                
                if (city.Actions > 0 && resource.ToLower() == "water")
                {
                     city.Actions--;
                     city.Water += random.Next(1, 6);
                     ViewBag.Message = $"You have harvest {city.Water} units of water.";
                }
                else if (city.Actions > 0 && resource.ToLower() == "food")
                {
                    city.Actions--;
                    city.Food += random.Next(1, 5);
                ViewBag.Message = $"You have harvest {city.Food} units of Food.";
                }
                else if (city.Actions > 0 && resource.ToLower() == "wood")
                {
                    city.Actions--;
                    city.Wood += random.Next(1, 6);
                ViewBag.Message = $"You have gathered {city.Wood} units of wood.";
                }

                db.Cities.AddOrUpdate(city);
                db.SaveChanges();
           

            return View("Game",city);
            
        }

        public ActionResult Build( string resource)
        {
            City city = db.Cities.ToList().First();

           
                
                    if (resource.ToLower() == "houses" && city.Wood >= 5 && city.Actions > 0 )
                    {
                        city.Actions--;
                        city.Wood -= 5;
                        city.Houses++;
                        city.Villagers++;
                    }
                    else if ( resource.ToLower() == "wells" && city.Wood >= 6 && city.Actions > 0)
                    {
                        city.Actions--;
                        city.Wood -= 6;
                        city.Wells++;
                    }
                  
            db.Cities.AddOrUpdate(city);
            db.SaveChanges();
            return View("Game",city);
           
        }
        [HttpPost]
        public ActionResult EndOFDay()
        {
            City city = db.Cities.ToList().First();
            // To add the water at end of day
            for (int i=0; i<city.Wells; i++)
            {
                city.Water++;
            }
            //Feed villagers
            for(int i=0; i< city.Villagers; i++)
            {
                city.Food--;
                city.Water--;
            }
            for (int i = 0; i < city.Villagers; i++)
            {
                city.Actions++;
            }
         
            // if not enough food or water, kill a villager
            if(city.Water<=0 && city.Food <= 0)
            {
                city.Houses--;
                city.Villagers--;
                
            }
            if (city.Villagers >= 10)
            {
                return RedirectToAction("Win");
            }

            if (city.Villagers <= 0)
            { return RedirectToAction("GameOver"); }

            city.Day++;

            db.Cities.AddOrUpdate(city);
            db.SaveChanges();
            return RedirectToAction("Game");
        }
        public ActionResult GameOver()
        {
            return View();
        }
        public ActionResult Win()
        {
            return View();
        }
        public ActionResult NewGame()
        {
            City city = db.Cities.ToList().First();
            //Set game state to game start
            city.Actions = 1;
            city.Day = 1;
            city.Food = 6;
            city.Houses = 1;
            city.Villagers = 1;
            city.Water = 6;
            city.Wells = 0;
            city.Wood = 0;

            db.Cities.AddOrUpdate(city);
            db.SaveChanges();

            return RedirectToAction("Game");
        }

    }
}

    
