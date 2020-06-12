using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookApi.Models;

namespace BookApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookItemsController : ControllerBase
    {
        private readonly BookContext _context;

        public BookItemsController(BookContext context)
        {
            _context = context;
        }

        // GET: api/BookItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookItemDTO>>> GetBookItems()
        {
            return await _context.BookItems
            .Select(x => ItemToDTO(x))
            .ToListAsync();
        }

        // GET: api/BookItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookItemDTO>> GetBookItem(long id)
        {
            var bookItem = await _context.BookItems.FindAsync(id);

            if (bookItem == null)
            {
                return NotFound();
            }

            return ItemToDTO(bookItem);
        }

        // PUT: api/BookItems/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBookItem(long id, BookItemDTO bookItemDTO)
        {
            if (id != bookItemDTO.Id)
            {
                return BadRequest();
            }

            var bookItem = await _context.BookItems.FindAsync(id);
            if (bookItem == null) 
            {
                return NotFound();
            }

            bookItem.Name = bookItemDTO.Name;
            bookItem.Author = bookItemDTO.Author;
            bookItem.Language = bookItemDTO.Language;
            bookItem.Year = bookItemDTO.Year;
            bookItem.Pages = bookItemDTO.Pages;
            bookItem.Synopsis = bookItemDTO.Synopsis;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!BookItemExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: api/BookItems
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<BookItemDTO>> CreateBookItem(BookItemDTO bookItemDTO)
        {
            var bookItem = new BookItem 
            {
                Name = bookItemDTO.Name,
                Author = bookItemDTO.Author,
                Language = bookItemDTO.Language,
                Year = bookItemDTO.Year,
                Pages = bookItemDTO.Pages,
                Synopsis = bookItemDTO.Synopsis
            };

            _context.BookItems.Add(bookItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBookItem), 
                                    new { id = bookItem.Id }, 
                                    ItemToDTO(bookItem));
        }

        // DELETE: api/BookItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBookItem(long id)
        {
            var bookItem = await _context.BookItems.FindAsync(id);
            if (bookItem == null)
            {
                return NotFound();
            }

            _context.BookItems.Remove(bookItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookItemExists(long id) =>
            _context.BookItems.Any(e => e.Id == id);

        private static BookItemDTO ItemToDTO(BookItem bookItem) =>
            new BookItemDTO 
            {
                Id = bookItem.Id,
                Name = bookItem.Name,
                Author = bookItem.Author,
                Language = bookItem.Language,
                Year = bookItem.Year,
                Pages = bookItem.Pages,
                Synopsis = bookItem.Synopsis
            };
    }
}
