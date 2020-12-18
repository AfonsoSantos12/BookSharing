using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookSharing.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BookSharing.Utilities;
using BookSharing.ViewModels;
using Booksharing.Models;
using System;
using BookSharing.Data;
using Microsoft.EntityFrameworkCore;

namespace BookSharing.Controllers
{

    //[Authorize(Roles = "admin")]
    public class BookController : Controller
    {

        private readonly BookSharingDbContext _context;

        public BookController(BookSharingDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register([Bind("Name,Autor,Ano,Genero")] RegisterBookViewModel model)
        {
            if (ModelState.IsValid)
            {
                var Books = new BookModel
                {
                    Name = model.Name,
                    Autor = model.Autor,
                    Ano = model.Ano,
                    Genero = model.Genero
                };
                _context.Add(Books);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(model);
        }


        public async Task<IActionResult> Remove(int? bookid)
        {
            if (bookid == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.BookModelId == bookid);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveConfirmed(int bookid)
        {
            var book = await _context.Books.FindAsync(bookid);
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    


        [HttpGet]

        public async Task<IActionResult> EditBook(int Id)
        {
            var book = await _context.Books.FindAsync(Id);
            if (book != null)
            {
                var model = new EditBookViewModel
                {
                    Id = book.BookModelId,
                    Name = book.Name
                };

                return View(model);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> EditBook(EditBookViewModel model)
        {
            if (ModelState.IsValid)
            {
                var book = await _context.Books.FindAsync(model.Id);
                book.Name = model.Name;

                _context.Books.Update(book);
                await _context.SaveChangesAsync();

            }

            return View(model);
        }





    }

}