# NetCoreSsrTest

## Run
1. dotnet ef database update
2. dotnet run
3. https://localhost:44376/swagger

## Credenciales seed
- admin@local / Admin123!
- regular@local / Regular123!

## Endpoints
- POST /auth/signup
- POST /auth/login
- GET /movies
- GET /movies/{id} (Regular)
- POST /movies (Admin)
- PUT /movies/{id} (Admin)
- PATCH /movies/{id} (Admin)
- DELETE /movies/{id} (Admin)
- POST /admin/sync-swapi (Admin)

## Notas
- JWT Issuer/Audience: NetCoreSsrTest
- DB: SQLite (app.db)
