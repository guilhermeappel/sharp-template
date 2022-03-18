
# Sharp Template

C# .NET Core Template repository developed by using **SOLID** principles.

### Techical Stack

- ASP.NET Core 6
- .NET Core 6
- EF Core 6
- Razor Pages
- PostgreSQL
- Swagger UI
- GraphQL (with HotChocolate)
- CircleCI (CI/CD)
- JWT Authentication
- Docker
- Docker Compose
- MailKit
- AutoMapper
- FluentValidation
- Argon2 Hash
- xUnit

### Design Patterns

- Domain Driven Design
- Repository & Generic Repository

## Live Example

### Swagger UI

http://sharp-template-api.herokuapp.com/swagger/index.html

### GraphQL Playground

http://sharp-template-api.herokuapp.com/graphql

## Deploy Docker

- Run the command `docker-compose up -d --build`inside the "docker" folder. It will compile the application for release with all the necessary files and run at http://localhost:7701/. (using the settings contained in the appsettings.json file)

## Deploy Test

- Run file `test-sharp-template.bat`, it will compile the application for release with all necessary files and run at http://localhost:5000/. (using the settings contained in the appsettings.json file)

## Environment settings

- To run the project on your machine, using Visual Studio, it is necessary to configure the appsettings.json file, adjusting the connection string with the data of your SQL Server installation.

Sample file:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Pooling=true;Database=SharpTemplate;User Id=postgres;Password=postgres;"
  },
  "AppSettings": {
    "SmtpHost": "smtp.office365.com",
    "SmtpPort": "587",
    "EmailUser": "no-reply@sharptemplate.com",
    "EmailPassword": "!@#123qwe",
    "AuthTokenSecretKey": "489cd5dbc708c7e541de4d7cd91ce6d0f1613573b7fc5b40d3942ccb9555cf35",
    "EmailTokenSecretKey": "962012d09b8170d912f0669f6d7d9d07",
    "Argon2PasswordKey": "a946ce40cf936def"
  }
}
```
