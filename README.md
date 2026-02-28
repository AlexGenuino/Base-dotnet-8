# App Base - ASP.NET Core 9

API base em .NET 9 com Clean Architecture, autenticação JWT + Refresh Token, CQRS (MediatR) e repositórios assíncronos.

## Estrutura

- **API** – Controllers, middleware, Swagger, configuração
- **Application** – Casos de uso (Commands/Handlers), validadores, DTOs, interfaces de repositório
- **Domain** – Entidades e contratos (ex.: IAuthService)
- **Infra** – EF Core, repositórios, persistência
- **Services** – Implementações de domínio (ex.: AuthService com BCrypt e JWT)

## Pré-requisitos

- .NET 9 SDK
- Oracle ou banco configurado (connection string em `API/appsettings.json`)

## Executar

```bash
cd API
dotnet run
```

Swagger: `https://localhost:7xxx/swagger` (porta conforme launchSettings).

## Segurança (desenvolvimento)

- Use **User Secrets** para chave JWT e connection string sensíveis:
  ```bash
  cd API
  dotnet user-secrets set "JWT:Key" "sua-chave-segura-aqui"
  dotnet user-secrets set "ConnectionStrings:DefaultConnection" "sua-connection-string"
  ```
- Em produção: use variáveis de ambiente ou Azure Key Vault; não commite segredos no `appsettings.json`.

## Endpoints de autenticação

- `POST /auth/create-user` – Criar usuário e retornar tokens em cookie
- `POST /auth/login` – Login (email/senha), retorna tokens em cookie
- `POST /auth/refresh-token` – Renovar JWT usando o refresh token (cookie)

Senhas são hasheadas com **BCrypt**; tokens usam **UTC** para expiração.
