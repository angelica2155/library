using library.Data;
using library.Models;
using Microsoft.AspNetCore.Mvc;

namespace library.Controllers
{
    public class BooksController
    {
        private readonly BookRepository _books;

        // ASP.NET Core hands in the repository, because Program.cs registered it.
        public BooksController(BookRepository books)
        {
            _books = books;
        }
       

    }


}



