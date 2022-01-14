using Microsoft.AspNetCore.Mvc;
using ParkyWeb.Repository.IRepository;
using ParkyWeb.Models;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace ParkyWeb.Controllers
{
    [Authorize]
    public class NationalParkController : Controller
    {
        private readonly INationalParkRepository _npRepository;

        public NationalParkController(INationalParkRepository npRepository)
        {
            this._npRepository = npRepository;
        }

        public IActionResult Index()
        {
            return View(new NationalPark() { });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Upsert(int? id)
        {
            var Obj = new NationalPark();

            if (id == null)
            {
                //flow will come here for Insert/Create
                return View(Obj);
            }

            //flow will come here for Update
            Obj = await _npRepository.GetAsync(SD.NationalParkAPIPath, id.GetValueOrDefault(), HttpContext.Session.GetString("JWToken"));

            if (Obj == null)
            {
                return NotFound();
            }

            return View(Obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(NationalPark obj)
        {
            if (ModelState.IsValid)
            {

                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    byte[] p1 = null;
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                    }
                    obj.Picture = p1;
                }
                else
                {
                    if (obj.Id != 0)
                    {
                        var objFromDb = await _npRepository.GetAsync(SD.NationalParkAPIPath, obj.Id, HttpContext.Session.GetString("JWToken"));
                        obj.Picture = objFromDb.Picture;
                    }
                }

                if (obj.Id == 0)
                {
                    await _npRepository.CreateAsync(SD.NationalParkAPIPath, obj, HttpContext.Session.GetString("JWToken"));
                }
                else
                {
                    await _npRepository.UpdateAsync(SD.NationalParkAPIPath + obj.Id, obj, HttpContext.Session.GetString("JWToken"));
                }

                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(obj);
            }

        }

        public async Task<IActionResult> GetAllNationalPark()
        {
            return Json(new { data = await _npRepository.GetAllAsync(SD.NationalParkAPIPath, HttpContext.Session.GetString("JWToken")) });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public async Task<IActionResult> Delete(int Id)
        {
            var status = await _npRepository.DeleteAsync(SD.NationalParkAPIPath, Id, HttpContext.Session.GetString("JWToken"));

            if (status == true)
            {
                return Json(new { success = true, message = "Delete Successful" });
            }

            return Json(new { success = false, message = "Delete Not Successful" });
        }

    }
}
