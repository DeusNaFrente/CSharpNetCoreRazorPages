using ContatosApp.Data; 
using ContatosApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;


namespace ContatosApp.Pages.Contacts;

[Authorize]
public class EditModel : PageModel
{
    private readonly AppDbContext _context;

    public EditModel(AppDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Contact Contact { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var contact = await _context.Contacts.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        if (contact is null)
        {
            return NotFound();
        }

        Contact = contact;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
	{
		if (!ModelState.IsValid)
		{
			return Page();
		}

		Contact.Email = Contact.Email.Trim();
		Contact.Phone = Contact.Phone.Trim();
		Contact.Name = Contact.Name.Trim();

		var existing = await _context.Contacts.FirstOrDefaultAsync(c => c.Id == Contact.Id);
		if (existing is null)
		{
			return NotFound();
		}

		//Unicidade
		var phoneExists = _context.Contacts.Any(c => c.Phone == Contact.Phone && c.Id != Contact.Id);
		if (phoneExists)
		{
			ModelState.AddModelError("Contact.Phone", "Já existe um contato com este telefone.");
			return Page();
		}

		var emailExists = _context.Contacts.Any(c => c.Email == Contact.Email && c.Id != Contact.Id);
		if (emailExists)
		{
			ModelState.AddModelError("Contact.Email", "Já existe um contato com este email.");
			return Page();
		}

		existing.Name = Contact.Name;
		existing.Phone = Contact.Phone;
		existing.Email = Contact.Email;

		try
		{
			await _context.SaveChangesAsync();
		}
		catch (DbUpdateException)
		{
			ModelState.AddModelError(string.Empty, "Não foi possível salvar. Verifique se telefone e email já existem.");
			return Page();
		}

		return RedirectToPage("/Contacts/Details", new { id = existing.Id });
	}

}

