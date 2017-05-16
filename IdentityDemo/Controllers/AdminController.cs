using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using IdentityDemo.Models;
using IdentityDemo.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace IdentityDemo.Controllers
{
    [Authorize(Roles ="admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        //GET Index (return all users)
        public IActionResult Index()
        {
            return View(_context.ApplicationUser.ToList());
        }


        //GET Edit (return all users)
        public IActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            //Find the selected user from method param arg (id)
            var applicationUser = _context.ApplicationUser
                .SingleOrDefault(m => m.Id == id);
            if (applicationUser == null)
            {
                //did not find a user with that id
                return NotFound();
            }

            return View(applicationUser); //return view attaching the model
        }

        //POST Edit
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(string id, ApplicationUser model)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                //Find which user to edit using the id param
                ApplicationUser user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    //Found the user - associate form entries to properties
                    //database side  //data entry
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Address = model.Address;
                    user.City = model.City;
                    user.Province = model.Province;
                    user.PostalCode = model.PostalCode;
                    user.Email = model.Email;

                    IdentityResult result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                }
            }
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unlock(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //Find the user account for the passed in id
            ApplicationUser applicationUser = await _userManager.FindByIdAsync(id);

            if (applicationUser != null)
            {
                //unlock account
                await _userManager.ResetAccessFailedCountAsync(applicationUser); //reset failed count
                await _userManager.SetLockoutEndDateAsync(applicationUser, DateTimeOffset.MinValue);

                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        //GET:  Delete
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _userManager.FindByIdAsync(id);

            if (applicationUser == null)
            {
                return NotFound();
            }

            //Find if user is admin
            foreach(var role in await _userManager.GetRolesAsync(applicationUser))
            {
                if(role == "admin")
                {
                    ViewBag.Admin = true;
                }
                else
                {
                    ViewBag.Admin = false;
                }
            }
            return View(applicationUser);
        }

        //POST:  Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            //Find the user account for the passed in id
            var user = await _userManager.FindByIdAsync(id);
            _context.ApplicationUser.Remove(user); //remove user from DbContext
            await _context.SaveChangesAsync(); //make changes permanent

            return RedirectToAction("Index");
        }
    }
}