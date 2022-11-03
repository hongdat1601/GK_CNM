using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GiuaKy_CNM.Data;
using GiuaKy_CNM.Models;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.CodeAnalysis.Operations;

namespace GiuaKy_CNM.Controllers
{
    public class BookController : Controller
    {
        private readonly BookContext _context;
        private readonly IAmazonS3 _amazonS3;

        public BookController(BookContext context, IAmazonS3 amazonS3)
        {
            _context = context;
            _amazonS3 = amazonS3;
        }

        // GET: Book
        public async Task<IActionResult> Index()
        {
            return View(await _context.Books.ToListAsync());
        }

        // GET: Book/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        public async Task<IActionResult> Download(int id)
        {
            Book? book = await _context.Books.FindAsync(id);
            if (book == null || book.PdfPath == null)
            {
                return NotFound();
            }

            string filePath = book.PdfPath;

            var getObject = new GetObjectRequest()
            {
                BucketName = "bucket-dat",
                Key = filePath,
            };

            using var res = await _amazonS3.GetObjectAsync(getObject);
    
            using var memory = new MemoryStream();
            await res.ResponseStream.CopyToAsync(memory);

            memory.Position = 0;
            return File(memory.ToArray(), "application/octet-stream", Path.GetFileName(filePath));
        }

        // GET: Book/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Book/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id, Name, Author, Categories, PublishDate, Image, Pdf")] Book book)
        {
            if (ModelState.IsValid)
            {

                if (book.Image != null)
                {
                    var putRequest = new PutObjectRequest()
                    {
                        BucketName = "bucket-dat",
                        Key = book.Image.FileName,
                        InputStream = book.Image.OpenReadStream(),
                        ContentType = book.Image.ContentType,
                    };

                    var result = await _amazonS3.PutObjectAsync(putRequest);
                    book.ImagePath = book.Image.FileName;
                }

                if (book.Pdf != null)
                {
                    var putRequest = new PutObjectRequest()
                    {
                        BucketName = "bucket-dat",
                        Key = book.Pdf.FileName,
                        InputStream = book.Pdf.OpenReadStream(),
                        ContentType = book.Pdf.ContentType,
                       
                    };

                    var result = await _amazonS3.PutObjectAsync(putRequest);
                    book.PdfPath = book.Pdf.FileName;
                }

                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Book/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // POST: Book/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Author,Categories,PublishDate")] Book book)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var oldBook = await _context.Books.FindAsync(id);
                    oldBook!.Name = book.Name;
                    oldBook.Author = book.Author;
                    oldBook.Categories = book.Categories;
                    oldBook.PublishDate = book.PublishDate;

                    _context.Update(oldBook);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
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
            return View(book);
        }

        // GET: Book/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Book/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Books == null)
            {
                return Problem("Entity set 'BookContext.Books'  is null.");
            }
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}
