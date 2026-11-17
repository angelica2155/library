using Npgsql;
using library.Data;
using library.Models;
using Microsoft.AspNetCore.Mvc;

namespace library.Controllers;

public class MemberController : Controller
{
    private readonly MemberRepository _members;
    private readonly LoanRepository _loans;

    // ASP.NET Core hands in the repositories, because Program.cs registered them.
    public MemberController(MemberRepository members, LoanRepository loans)
    {
        _members = members;
        _loans = loans;
    }
    


    // GET /Member : the list of members.
    public async Task<IActionResult> Index()
    {
        var members = await _members.GetAllAsync();
        return View(members);
    }

    // GET /Member/Create : an empty form.
    public IActionResult Create()
    {
        return View(new Member());
    }

    // POST /Members/Create : the form was submitted.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Member member)
    {
        if (!ModelState.IsValid)
        {
            return View(member);
        }

        try
        {
            await _members.AddAsync(member);
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            ModelState.AddModelError("Email", "Another member already uses this email.");
            return View(member);
        }

        return RedirectToAction(nameof(Index));
    }


    // GET /Member/Edit/5 : the form, filled with member 5.
    public async Task<IActionResult> Edit(long id)
    {
        var member = await _members.GetByIdAsync(id);

        if (member == null)
        {
            return NotFound();
        }

        return View(member);
    }

    // POST /Members/Edit/5 : the changed form was submitted.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Member member)
    {
        if (!ModelState.IsValid)
        {
            return View(member);
        }

        try
        {
            await _members.UpdateAsync(member);
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            ModelState.AddModelError("Email", "Another member already uses this email.");
            return View(member);
        }

        return RedirectToAction(nameof(Index));
    }


    // GET /Member/Delete/5 : show the confirmation page
    public async Task<IActionResult> Delete(long id)
    {
        var member = await _members.GetByIdAsync(id);

        if (member == null)
        {
            return NotFound();
        }

        return View(member);
    }

    // POST /Members/Delete/5 : the delete was confirmed.
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(long id)
    {
        try
        {
            await _members.DeleteAsync(id);
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.ForeignKeyViolation

)
        {
            // 3. Load the member again, to show them on the page.
            var member = await _members.GetByIdAsync(id);
            if (member == null)
            {
                return NotFound();
            }
            ViewBag.Error = "This member has loans on record, so the database will not delete them.";
            return View("Delete", member);
        }

        return RedirectToAction(nameof(Index));
    }

    // GET /Members/Details/5 : one member and their loans.
    public async Task<IActionResult> Details(long id)
    {
        // 1. Load the member.
        var member = await _members.GetByIdAsync(id);

        if (/* 2. there is no such member */
            member == null)
        {
            // 3. Answer "not found".
            return NotFound();
        }

        // 4. Put the member and their loans together for the page.
        var details = new MemberDetails
        {
            Member = member,
            Loans = await _loans.GetByMemberAsync(id)
        };

        // 5. Show the page.
        return View(details);
    }

}

