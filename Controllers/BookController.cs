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

        [HttpGet("{id}")]
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
        public IActionResult AddBook([FromBody] string isbn, [FromBody] string name,
            [FromBody] decimal price, [FromBody] string description, [FromBody] string? image_url, 
            [FromBody] DateTime publish_year, [FromBody] string publisher, [FromBody] string author, 
            [FromBody] int category_id)
        {
            if (string.IsNullOrEmpty(isbn))
            {
                return BadRequest("Book isbn is required.");
            }

            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Book name is required.");
            }

            if (string.IsNullOrEmpty(price))
            {
                return BadRequest("Book price is required.");
            }

            if (string.IsNullOrEmpty(description))
            {
                return BadRequest("Book description is required.");
            }

            if (string.IsNullOrEmpty(publisher))
            {
                return BadRequest("Book publisher is required.");
            }

            if (string.IsNullOrEmpty(author))
            {
                return BadRequest("Book author is required.");
            }

            if (category_id == 0)
            {
                return BadRequest("Book category id is required.");
            }

            if (_bookModel.AddBook(isbn, name, price, description, image_url, publish_year, 
                publisher, author, category_id))
            {
                return Ok("Book added successfully.");
            }
            else
            {
                return BadRequest("Failed to add book.");
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateBook(int id, [FromBody] string isbn, [FromBody] string name,
            [FromBody] decimal price, [FromBody] string description, [FromBody] string? image_url,
            [FromBody] DateTime publish_year, [FromBody] string publisher, [FromBody] string author,
            [FromBody] int category_id)
        {
            if (string.IsNullOrEmpty(isbn))
            {
                return BadRequest("Book isbn is required.");
            }

            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Book name is required.");
            }

            if (string.IsNullOrEmpty(price))
            {
                return BadRequest("Book price is required.");
            }

            if (string.IsNullOrEmpty(description))
            {
                return BadRequest("Book description is required.");
            }

            if (string.IsNullOrEmpty(publisher))
            {
                return BadRequest("Book publisher is required.");
            }

            if (string.IsNullOrEmpty(author))
            {
                return BadRequest("Book author is required.");
            }

            if (category_id == 0)
            {
                return BadRequest("Book category id is required.");
            }

            if (_bookModel.UpdateBook(id, isbn, name, price, description, image_url, publish_year,
                publisher, author, category_id))
            {
                return Ok("Book updated successfully.");
            }
            else
            {
                return BadRequest("Failed to update book.");
            }
        }

        [HttpDelete("{id}")]
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
    }
}
