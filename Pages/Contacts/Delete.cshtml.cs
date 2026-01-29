using ContatosApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace ContatosApp.Pages.Contacts;

[Authorize]
public class DeleteModel : PageModel
{
    private readonly AppDbContext _context;

    public DeleteModel(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var contact = await _context.Contacts
            .IgnoreQueryFilters() // para conseguir excluir também se já estiver "sumido"
            .FirstOrDefaultAsync(c => c.Id == id);

        if (contact is null)
        {
            return NotFound();
        }

        if (!contact.IsDeleted)
        {
            contact.IsDeleted = true;
            contact.DeletedAtUtc = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("/Index");
    }
}
