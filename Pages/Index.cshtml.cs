using ContatosApp.Data;  
using ContatosApp.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ContatosApp.Pages;

public class IndexModel : PageModel
{
    private readonly AppDbContext _context;

    public IndexModel(AppDbContext context)
    {
        _context = context;
    }

    public IList<Contact> Contacts { get; set; } = new List<Contact>();

    public async Task OnGetAsync()
    {
        Contacts = await _context.Contacts
            .OrderBy(c => c.Name)
            .ToListAsync();
    }
}
