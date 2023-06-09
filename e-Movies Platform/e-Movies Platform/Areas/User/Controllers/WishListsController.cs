﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using e_Movies_Platform.Data;
using e_Movies_Platform.Models;
using Microsoft.Data.SqlClient;
using e_Movies_Platform.Data.Migrations;
using Microsoft.AspNetCore.Identity;

namespace e_Movies_Platform.Areas.User.Controllers
{
    [Area("User")]
    public class WishListsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public WishListsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: User/WishLists
        public async Task<IActionResult> Index(int pg = 1, string searchString = "", string sortOrder = "")
        {

            ViewData["MovieSortParm"] = sortOrder == "Movie" ? "movie_desc" : "Movie";
            ViewData["CurrentFilter"] = searchString;
            const int pageSize = 5;
            if (pg < 1)
                pg = 1;

            var user = await _userManager.GetUserAsync(User);
            List<WishList> wishlist = await _context.WishList.Where(w => w.User == user).ToListAsync();
            if (!String.IsNullOrEmpty(searchString))
            {
                wishlist = await _context.WishList.Where(c => c.User == user).Where(c => c.Name.ToLower().Contains(searchString.ToLower())).ToListAsync();
            }

            if (!String.IsNullOrEmpty(sortOrder))
            {
                switch (sortOrder)
                {
                    case "Movie":
                        wishlist = await _context.WishList.Where(w => w.User == user).OrderBy(c => c.Name).ToListAsync();
                        break;
                    case "movie_desc":
                        wishlist = await _context.WishList.Where(w => w.User == user).OrderByDescending(c => c.Name).ToListAsync();
                        break;
                }
            }

            int recsCount = wishlist.Count();

            var pager = new Pager(recsCount, pg, pageSize);

            int recSkip = (pg - 1) * pageSize;

            var data = wishlist.Skip(recSkip).Take(pager.PageSize).ToList();

            this.ViewBag.Pager = pager;

            return View(data);
            //return View(await _context.WishList.ToListAsync();
        }

        // GET: User/WishLists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.WishList == null)
            {
                return NotFound();
            }

            var wishList = await _context.WishList
                .FirstOrDefaultAsync(m => m.Id == id);
            if (wishList == null)
            {
                return NotFound();
            }

            return View(wishList);
        }

        // GET: User/WishLists/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: User/WishLists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Year,Genre")] WishList wishList)
        {
            var user = await _userManager.GetUserAsync(User);
            wishList.User = user;
            if (ModelState.IsValid)
            {
                _context.Add(wishList);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(wishList);
        }

        // GET: User/WishLists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.WishList == null)
            {
                return NotFound();
            }

            var wishList = await _context.WishList.FindAsync(id);
            if (wishList == null)
            {
                return NotFound();
            }
            return View(wishList);
        }

        // POST: User/WishLists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("Id,Name,Year,Genre")] WishList wishList)
        {
            if (id != wishList.Id)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            wishList.User = user;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(wishList);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WishListExists(wishList.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(wishList);
        }

        // GET: User/WishLists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.WishList == null)
            {
                return NotFound();
            }

            var wishList = await _context.WishList
                .FirstOrDefaultAsync(m => m.Id == id);
            if (wishList == null)
            {
                return NotFound();
            }

            return View(wishList);
        }

        // POST: User/WishLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (_context.WishList == null)
            {
                return Problem("Entity set 'ApplicationDbContext.WishList'  is null.");
            }
            var wishList = await _context.WishList.FindAsync(id);
            if (wishList != null)
            {
                _context.WishList.Remove(wishList);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WishListExists(int? id)
        {
          return _context.WishList.Any(e => e.Id == id);
        }
    }
}
