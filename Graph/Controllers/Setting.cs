using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DA;
using DM;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Graph.Controllers
{
    public class Setting : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            var model = new DM.Setting();

            model.AllSettings = new SettingsDa().GetAllSettings();

            return View("Setting", model);
        }

        public IActionResult AddUpdate(DM.Setting setting)
        {
            if (!string.IsNullOrWhiteSpace(setting.Key))
            {
                var sda = new SettingsDa();
                sda.UpsertSetting(setting.Key, setting.Value);
            }
            
            var model = new DM.Setting();

            model.AllSettings = new SettingsDa().GetAllSettings();
            return View("Setting", model);
        }
    }
}
