using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ParkyWeb.Repository.IRepository;
using ParkyWeb.Models;
using ParkyWeb.Models.ViewModel;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace ParkyWeb.Controllers
{
    [Authorize]
    public class TrailsController : Controller
    {
        private readonly INationalParkRepository _npRepository;
        private readonly ITrailsRepository _trailsRepository;

        public TrailsController(INationalParkRepository npRepository, ITrailsRepository trailsRepository)
        {
            this._npRepository = npRepository;
            this._trailsRepository = trailsRepository;
        }

        public IActionResult Index()
        {
            return View(new Trail() { });
        }

        public async Task<IActionResult> GetAllTrail()
        {
            return Json(new { data = await _trailsRepository.GetAllAsync(SD.TrailAPIPath, HttpContext.Session.GetString("JWToken")) });

        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Upsert(int? Id)
        {
            IEnumerable<NationalPark> npList = await _npRepository.GetAllAsync(SD.NationalParkAPIPath, HttpContext.Session.GetString("JWToken"));

            TrailsVM trailsVM = new TrailsVM();
            trailsVM.NationalParkList = npList.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });

            if (Id == null)    //Add mode
            {
                trailsVM.Trail = new Trail() { };

                return View(trailsVM);
            }
            else
            {   //Edit mode

                trailsVM.Trail = await _trailsRepository.GetAsync(SD.TrailAPIPath, Id.GetValueOrDefault(), HttpContext.Session.GetString("JWToken"));
                if (trailsVM.Trail == null)
                {
                    return NotFound();
                }

                return View(trailsVM);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(TrailsVM Obj)
        {
            if (ModelState.IsValid)
            {
                if (Obj.Trail.Id == 0)
                {
                    await _trailsRepository.CreateAsync(SD.TrailAPIPath, Obj.Trail, HttpContext.Session.GetString("JWToken"));
                }
                else
                {
                    await _trailsRepository.UpdateAsync(SD.TrailAPIPath + Obj.Trail.Id, Obj.Trail, HttpContext.Session.GetString("JWToken"));
                }

                return RedirectToAction(nameof(Index));
            }
            else
            {
                IEnumerable<NationalPark> npList = await _npRepository.GetAllAsync(SD.NationalParkAPIPath, HttpContext.Session.GetString("JWToken"));

                Obj.NationalParkList = npList.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });


                return View(Obj);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _trailsRepository.DeleteAsync(SD.TrailAPIPath, id, HttpContext.Session.GetString("JWToken"));

            if (success)
            {
                return Json(new { success = true, message = "Delete successful" });
            }

            return Json(new { success = false, message = "Delete not successful" });
        }
    }
}
