
using library.Data;
using library.Models;
using Microsoft.AspNetCore.Mvc;

namespace library.Controllers
{
    public class NoteController : Controller
    {
        private readonly NoteRepository _notes;

        public NoteController(NoteRepository notes)
        {
            _notes = notes;
        }

        // Display all notes
        public async Task<IActionResult> Index()
        {
            var notes = await _notes.GetAllAsync();
            return View(notes);
        }

        // Show the form to add a note
        public IActionResult Create()
        {
            return View(new Note
            {
                CreatedOn = DateTime.Today
            });
        }

        // Save the new note
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Note note)
        {
            if (!ModelState.IsValid)
            {
                return View(note);
            }

            await _notes.AddAsync(note);
            return RedirectToAction(nameof(Index));
        }

        // Show the edit form
        public async Task<IActionResult> Edit(long id)
        {
            var note = await _notes.GetByIdAsync(id);

            if (note == null)
            {
                return NotFound();
            }

            return View(note);
        }

        // Save the edited note
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, Note note)
        {
            if (id != note.NoteId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(note);
            }

            var existingNote = await _notes.GetByIdAsync(id);

            if (existingNote == null)
            {
                return NotFound();
            }

            await _notes.UpdateAsync(note);
            return RedirectToAction(nameof(Index));
        }

        // Show the delete confirmation
        public async Task<IActionResult> Delete(long id)
        {
            var note = await _notes.GetByIdAsync(id);

            if (note == null)
            {
                return NotFound();
            }

            return View(note);
        }

        // Delete the note
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var note = await _notes.GetByIdAsync(id);

            if (note == null)
            {
                return NotFound();
            }

            await _notes.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
