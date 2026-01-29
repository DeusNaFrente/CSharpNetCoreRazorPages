using ContatosApp.Data; 
using ContatosApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ContatosApp.Pages.Contacts;

public class DetailsModel : PageModel
{
    private readonly AppDbContext _context;

    public DetailsModel(AppDbContext context)
    {
        _context = context;
    }

    public Contact Contact { get; private set; } = default!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var contact = await _context.Contacts.FirstOrDefaultAsync(c => c.Id == id);
        if (contact is null)
        {
            return NotFound();
        }

        Contact = contact;
        return Page();
    }
}

