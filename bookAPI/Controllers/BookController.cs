using bookAPI.Data;
using bookAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FluentValidation;

namespace bookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly BookDbContext _context;

        public BookController(BookDbContext context)
        {
            this._context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<Book>> Get()
        {
            return await _context.Books.ToListAsync();
        }

        [HttpGet("id")]
        [ProducesResponseType(typeof(Book), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id) 
        {
            var book = await _context.Books.FindAsync(id);
            return book == null ? NotFound() : Ok(book);
        }

        [HttpPost("userId")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create(Guid userId, CreateBook model)
        {
            // check if content of the book is good
            CreateBookValidator validator = new();
            var validation = validator.Validate(model);

            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(error => error.ErrorMessage).ToList();
                return BadRequest(errors);
            }


            // check if user exists
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound("User do  not exist");
            }


            // add book to database
            Book book = new();
            
            book.Name = model.Name;
            book.Title = model.Title;
            book.Description = model.Description;
            book.User = user;

            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();

            // ?????
            return CreatedAtAction(nameof(GetById), new {id = book.Id}, book);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> Update(Guid id, Book book)
        {
            if (id != book.Id) return BadRequest();

            var exists = await _context.Books.FindAsync(id);
            if (exists == null) return NotFound();

            book.UpdatedAt = DateTime.UtcNow;

            _context.Entry(book).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // 204 status
            return NoContent();

        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var book = await _context.Books.FindAsync(id);
            // 404
            if (book == null) return NotFound();

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            // 204 status
            return NoContent();

        }

        [HttpPut("/upload/{bookId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateBookImage(Guid bookId, BookImageModel model)
        {
            // check if data is good
            if (model.BookImage == null || model.BookImage.Length == 0)
            {
                // 400
                return BadRequest("No image file specified.");
            }

            // check if the book exists
            var book = await _context.Books.FindAsync(bookId);
            // 404
            if (book == null) return NotFound();

            // initializing a filename and the folder
            string uniqueFileName = bookId.ToString() + ".png";

            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // check if file already exists to delete the old one
            if(System.IO.File.Exists(filePath)) 
            {
                System.IO.File.Delete(filePath);
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.BookImage.CopyToAsync(stream);
            }

            book.ImageURL = filePath;
            book.UpdatedAt = DateTime.UtcNow;


            _context.Entry(book).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok("Book image uploaded");
        }

        [HttpDelete("book-image/{bookId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteImage(string bookId)
        {
            // check if the book exists
            var book = await _context.Books.FindAsync(bookId);
            // 404
            if (book == null) return NotFound();

            if(book.ImageURL == null) return BadRequest("There is no image");

            string filePath = book.ImageURL;

            // check if file already exists to delete the old one
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            else
            {
                return NotFound("the image do not exist");
            }

            // update book
            book.ImageURL = null;
            book.UpdatedAt = DateTime.UtcNow;

            _context.Entry(book).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();

        }

    }
}
