using DA;
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

            //Clear the Model.
            ModelState.Clear();

            var model = new DM.Setting();

            model.AllSettings = new SettingsDa().GetAllSettings();
            return View("Setting", model);
        }
        [Route("Setting/Delete/{key}")]
        public IActionResult Delete(string key)
        {
            var sda = new SettingsDa();
            sda.Delete(key);

            ModelState.Clear();

            var model = new DM.Setting();

            model.AllSettings = new SettingsDa().GetAllSettings();
            return View("Setting", model);
        }
    }
}
