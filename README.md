Contatos – ASP.NET Core 6

Aplicação web desenvolvida teste utilizando ASP.NET Core 6, Razor Pages, Entity Framework Core e MariaDB 10.6.

Tecnologias

ASP.NET Core 6

Razor Pages

Entity Framework Core

MariaDB 10.6

Autenticação por Cookies

xUnit + Microsoft.AspNetCore.Mvc.Testing

Docker (ambiente local)

Estrutura

Projeto com uma única aplicação web, sem separação entre API e frontend.

Autenticação

Usuário fixo para testes manuais:

Usuário: admin
Senha:  admin123


Nos testes automatizados, a autenticação é simulada via TestAuthHandler.

Banco de dados

MariaDB 10.6

Gerenciado por Entity Framework Core

Estrutura criada automaticamente via migrations no startup

db.Database.Migrate();

Testes

Testes de integração cobrindo:

Criação de contatos

Validação de email duplicado

Validação de telefone duplicado

Edição de contatos

Execução:

dotnet test

Deploy (local)

Autor

Junior
