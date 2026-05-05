using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UIN.Library.Api.Models;

namespace UIN.Library.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BooksController : ControllerBase
    {
        private static List<Livre> livres = new List<Livre>();

        // GET: api/books
        [Authorize]
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(livres);
        }

        // GET: api/books/{id}
        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var livre = livres.FirstOrDefault(l => l.Id == id);

            if (livre == null)
                return NotFound("Livre non trouvé");

            return Ok(livre);
        }

        // POST: api/books
        [Authorize(Roles = "editor,admin")]
        [HttpPost]
        public IActionResult Create(Livre livre)
        {
            livre.Id = Guid.NewGuid(); // force un nouvel ID

            livres.Add(livre);

            return Ok(livre);
        }

        // PUT: api/books/{id}
        [Authorize(Roles = "editor,admin")]
        [HttpPut("{id}")]
        public IActionResult Update(Guid id, Livre updatedLivre)
        {
            var livre = livres.FirstOrDefault(l => l.Id == id);

            if (livre == null)
                return NotFound("Livre non trouvé");

            livre.Titre = updatedLivre.Titre;
            livre.Auteur = updatedLivre.Auteur;
            livre.ISBN = updatedLivre.ISBN;
            livre.AnneePublication = updatedLivre.AnneePublication;
            livre.Categorie = updatedLivre.Categorie;
            livre.Disponible = updatedLivre.Disponible;

            return Ok(livre);
        }

        // DELETE: api/books/{id}
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var livre = livres.FirstOrDefault(l => l.Id == id);

            if (livre == null)
                return NotFound("Livre non trouvé");

            livres.Remove(livre);

            return Ok("Livre supprimé");
        }
    }
}