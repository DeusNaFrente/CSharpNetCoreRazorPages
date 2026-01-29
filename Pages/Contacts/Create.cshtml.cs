using ContatosApp.Data; 
using ContatosApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;


namespace ContatosApp.Pages.Contacts;

[Authorize]
public class CreateModel : PageModel
{
    private readonly AppDbContext _context;

    public CreateModel(AppDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Contact Contact { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
	{
		if (!ModelState.IsValid)
		{
			return Page();
		}

		//Normalização simples
		Contact.Email = Contact.Email.Trim();
		Contact.Phone = Contact.Phone.Trim();
		Contact.Name = Contact.Name.Trim();

		var phoneExists = _context.Contacts.Any(c => c.Phone == Contact.Phone);
		if (phoneExists)
		{
			ModelState.AddModelError("Contact.Phone", "Já existe um contato com este telefone.");
			return Page();
		}

		var emailExists = _context.Contacts.Any(c => c.Email == Contact.Email);
		if (emailExists)
		{
			ModelState.AddModelError("Contact.Email", "Já existe um contato com este email.");
			return Page();
		}

		_context.Contacts.Add(Contact);

		try
		{
			await _context.SaveChangesAsync();
		}
		catch (DbUpdateException)
		{

			ModelState.AddModelError(string.Empty, "Não foi possível salvar. Verifique se telefone e email já existem.");
			return Page();
		}

		return RedirectToPage("/Index");
	}

}

