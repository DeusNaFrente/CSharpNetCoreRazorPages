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


🚀 Como executar o projeto:

📌 Pré-requisitos

Antes de começar, é necessário ter instalado na máquina

Docker Desktop
https://www.docker.com/products/docker-desktop/

O Docker Compose já vem incluído no Docker Desktop.

📥 Baixar o projeto

Você pode baixar o projeto diretamente deste repositório:
https://github.com/DeusNaFrente/CSharpNetCoreRazorPages

Clique em Code → Download ZIP

Extraia o arquivo em uma pasta local

⚙️ Configuração do ambiente

Na raiz do projeto, crie um arquivo chamado .env com o seguinte conteúdo:

DB_HOST=contatos_db
DB_PORT=3306
DB_NAME=contatosdb
DB_USER=app
DB_PASSWORD=app123


⚠️ O arquivo .env não está versionado por motivos de segurança.

🐳 Subir os containers com Docker

Dentro da pasta do projeto, execute:

docker compose up -d


Esse comando irá:

subir o banco de dados MariaDB

subir a aplicação .NET

🗄️ Restaurar o banco de dados

Após os containers estarem rodando, importe o dump do banco:

docker exec -i contatos_db mariadb -uapp -papp123 contatosdb < db/contatosdb_dump.sql

🌐 Acessar a aplicação

Após subir tudo corretamente, acesse no navegador:

http://localhost:5000


Caso a porta seja diferente, verifique o arquivo docker-compose.yml.

🔁 Parar os containers

Para parar a aplicação:

docker compose down

Autor

Junior
