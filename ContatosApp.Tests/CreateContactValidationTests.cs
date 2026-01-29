using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ContatosApp.Tests;

public class CreateContactValidationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public CreateContactValidationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Create_Should_Return_Error_When_Name_Is_Too_Short()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        //GET pra pegar o antiforgery token do form
        var getResp = await client.GetAsync("/Contacts/Create");
        Assert.Equal(HttpStatusCode.OK, getResp.StatusCode);

        var getHtml = await getResp.Content.ReadAsStringAsync();
        var token = ExtractHiddenInputValue(getHtml, "__RequestVerificationToken");
        Assert.False(string.IsNullOrWhiteSpace(token));

        //POST com token
        var formData = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("__RequestVerificationToken", token),
            new KeyValuePair<string, string>("Contact.Name", "ABC"),
            new KeyValuePair<string, string>("Contact.Phone", "333333333"),
            new KeyValuePair<string, string>("Contact.Email", "testeabc@teste.com"),
        });

        var postResp = await client.PostAsync("/Contacts/Create", formData);

        //Quando validação falha, ele retorna a mesma página (200)
        Assert.Equal(HttpStatusCode.OK, postResp.StatusCode);

        var postHtml = await postResp.Content.ReadAsStringAsync();
        Assert.Contains("Nome deve ter mais de 5 caracteres", postHtml);
    }

    [Fact]
	public async Task Create_Should_Return_Error_When_Email_Is_Invalid()
	{
		var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
		{
			AllowAutoRedirect = false
		});

		var getResp = await client.GetAsync("/Contacts/Create");
		Assert.Equal(HttpStatusCode.OK, getResp.StatusCode);

		var getHtml = await getResp.Content.ReadAsStringAsync();
		var token = ExtractHiddenInputValue(getHtml, "__RequestVerificationToken");
		Assert.False(string.IsNullOrWhiteSpace(token));

		var formData = new FormUrlEncodedContent(new[]
		{
			new KeyValuePair<string, string>("__RequestVerificationToken", token),
			new KeyValuePair<string, string>("Contact.Name", "Contato Valido"),
			new KeyValuePair<string, string>("Contact.Phone", "444444444"),
			new KeyValuePair<string, string>("Contact.Email", "email_invalido"),
		});

		var postResp = await client.PostAsync("/Contacts/Create", formData);
		Assert.Equal(HttpStatusCode.OK, postResp.StatusCode);

		var postHtml = await postResp.Content.ReadAsStringAsync();
		Assert.Contains("Email inv", postHtml);

	}

	
	[Fact]
	public async Task Edit_Should_Return_Error_When_Phone_Is_Duplicated()
	{
		var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
		{
			AllowAutoRedirect = false
		});

		//Cria primeiro contato
		await CreateContact(client, "Contato Um", "555555555", "um@teste.com");

		//cria segundo contato
		await CreateContact(client, "Contato Dois", "666666666", "dois@teste.com");

		//Abre página de edição do segundo contato (id = 2)
		var getResp = await client.GetAsync("/Contacts/Edit/2");
		Assert.Equal(HttpStatusCode.OK, getResp.StatusCode);

		var html = await getResp.Content.ReadAsStringAsync();
		var token = ExtractHiddenInputValue(html, "__RequestVerificationToken");

		//Tenta salvar usando telefone do primeiro contato
		var formData = new FormUrlEncodedContent(new[]
		{
			new KeyValuePair<string, string>("__RequestVerificationToken", token),
			new KeyValuePair<string, string>("Contact.Id", "2"),
			new KeyValuePair<string, string>("Contact.Name", "Contato Dois"),
			new KeyValuePair<string, string>("Contact.Phone", "555555555"),
			new KeyValuePair<string, string>("Contact.Email", "dois@teste.com"),
		});

		var postResp = await client.PostAsync("/Contacts/Edit/2", formData);
		Assert.Equal(HttpStatusCode.OK, postResp.StatusCode);

		var postHtml = await postResp.Content.ReadAsStringAsync();
		Assert.Contains("telefone", postHtml.ToLower());
	}

	
	private static string ExtractHiddenInputValue(string html, string inputName)
    {
        var marker = $"name=\"{inputName}\"";
        var idx = html.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
        if (idx < 0) return "";

        var valueMarker = "value=\"";
        var valueIdx = html.IndexOf(valueMarker, idx, StringComparison.OrdinalIgnoreCase);
        if (valueIdx < 0) return "";

        valueIdx += valueMarker.Length;
        var endIdx = html.IndexOf("\"", valueIdx, StringComparison.OrdinalIgnoreCase);
        if (endIdx < 0) return "";

        return html.Substring(valueIdx, endIdx - valueIdx);
    }
	
	private static async Task CreateContact(
    HttpClient client,
    string name,
    string phone,
    string email)
	{
		var getResp = await client.GetAsync("/Contacts/Create");
		var html = await getResp.Content.ReadAsStringAsync();
		var token = ExtractHiddenInputValue(html, "__RequestVerificationToken");

		var formData = new FormUrlEncodedContent(new[]
		{
			new KeyValuePair<string, string>("__RequestVerificationToken", token),
			new KeyValuePair<string, string>("Contact.Name", name),
			new KeyValuePair<string, string>("Contact.Phone", phone),
			new KeyValuePair<string, string>("Contact.Email", email),
		});

		await client.PostAsync("/Contacts/Create", formData);
	}
	
	private static async Task<int> CreateContactAndGetId(HttpClient client, string name, string phone, string email)
	{
		var getResp = await client.GetAsync("/Contacts/Create");
		var html = await getResp.Content.ReadAsStringAsync();
		var token = ExtractHiddenInputValue(html, "__RequestVerificationToken");

		var formData = new FormUrlEncodedContent(new[]
		{
			new KeyValuePair<string, string>("__RequestVerificationToken", token),
			new KeyValuePair<string, string>("Contact.Name", name),
			new KeyValuePair<string, string>("Contact.Phone", phone),
			new KeyValuePair<string, string>("Contact.Email", email),
		});

		var postResp = await client.PostAsync("/Contacts/Create", formData);

		// Se criou com sucesso, o Create faz RedirectToPage("/Index") => 302
		if (postResp.StatusCode != HttpStatusCode.Redirect)
		{
			// Falhou, devolve -1 só pra estourar no teste e você ver o HTML
			var body = await postResp.Content.ReadAsStringAsync();
			throw new Exception($"Falhou ao criar contato. Status={postResp.StatusCode}. Body={body}");
		}

		// pega o Id do contato recém criado pelo email
		var indexHtml = await (await client.GetAsync("/")).Content.ReadAsStringAsync();

		// forma simples: procura o link de Details contendo o id (ex: /Contacts/Details/12)
		// e assume que o último criado aparece na lista
		var marker = "/Contacts/Details/";
		var lastIdx = indexHtml.LastIndexOf(marker, StringComparison.OrdinalIgnoreCase);
		if (lastIdx < 0) throw new Exception("Não consegui achar link de Details no Index.");

		lastIdx += marker.Length;
		var endIdx = indexHtml.IndexOf("\"", lastIdx, StringComparison.OrdinalIgnoreCase);
		if (endIdx < 0) throw new Exception("Não consegui extrair o Id do link de Details.");

		var idStr = indexHtml.Substring(lastIdx, endIdx - lastIdx);

		if (!int.TryParse(idStr, out var id))
			throw new Exception($"Id inválido extraído: {idStr}");

		return id;
	}


}
