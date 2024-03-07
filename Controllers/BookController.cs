using BookStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly BookModel _bookModel;

        public BookController(IConfiguration configuration)
        {
            _bookModel = new BookModel(configuration);
        }

        [HttpGet]
        public IActionResult GetAllBooks()
        {
            var books = _bookModel.GetAllBooks();
            return Ok(books);
        }

        [HttpGet("id={id}")]
        public IActionResult GetBookById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid book ID.");
            }

            var book = _bookModel.GetBookById(id);

            if (book == null)
            {
                return NotFound("Book not found.");
            }

            return Ok(book);
        }

        [HttpPost]
        public IActionResult AddBook([FromBody] Book book)
        {
            if (book == null)
            {
                return BadRequest("Book data is required.");
            }

            if (_bookModel.AddBook(book))
            {
                return Ok("Book added successfully.");
            }
            else
            {
                return BadRequest("Failed to add book.");
            }
        }

        [HttpPut("id={id}")]
        public IActionResult UpdateBook(int id, [FromBody] Book book)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid book ID.");
            }

            if (book == null)
            {
                return BadRequest("Book data is required.");
            }

            if (_bookModel.UpdateBook(id, book))
            {
                return Ok("Book updated successfully.");
            }
            else
            {
                return BadRequest("Failed to update book.");
            }
        }

        [HttpDelete("id={id}")]
        public IActionResult DeleteBook(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid book ID.");
            }

            if (_bookModel.DeleteBook(id))
            {
                return Ok("Book deleted successfully.");
            }
            else
            {
                return BadRequest("Failed to delete book.");
            }
        }

        [HttpGet("search")]
        public IActionResult SearchBooks(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return BadRequest("Keyword cannot be empty");
            }

            var books = _bookModel.SearchBooks(keyword);
            return Ok(books);
        }
    }
}
