# MiniATS

API REST para gestão de vagas e candidaturas, construída para praticar
EF Core, autenticação JWT e boas práticas de API em .NET 10.

## Stack

- .NET 10 / ASP.NET Core
- Entity Framework Core 10 + SQL Server
- JWT Bearer Authentication
- MiniProfiler
- Docker

## Como rodar

```bash
# banco
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=SuaSenha@123" \
  -p 1433:1433 --name sqlserver-ats -d mcr.microsoft.com/mssql/server:2022-latest

# segredos
cd MiniAts
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost,1433;Database=MiniAts;User Id=sa;Password=SuaSenha@123;TrustServerCertificate=True"
dotnet user-secrets set "Jwt:Chave" "chave-com-no-minimo-32-caracteres-aqui!!"

# migrations e execução
dotnet ef database update
dotnet run
```

A base é populada automaticamente no primeiro start (10 empresas, 200 vagas, ~700 candidaturas).

## Endpoints

| Método | Rota | Auth |
|---|---|---|
| POST | `/api/auth/login` | — |
| GET | `/api/vagas` | pública |
| GET | `/api/vagas/{id}` | JWT |
| POST | `/api/vagas` | JWT |
| PUT | `/api/vagas/{id}` | JWT |
| DELETE | `/api/vagas/{id}` | JWT |

A listagem aceita `titulo`, `empresaId`, `salarioMinimo`, `pagina` e `tamanhoPagina`.

## Decisões técnicas

- **Projeção com `Select` para DTO** em vez de `Include` nas leituras: o SQL
  traz só as colunas necessárias e não passa pelo change tracker.
- **`AsNoTracking`** em todas as consultas de leitura.
- **Paginação com desempate por `Id`** no `ORDER BY`, para que a ordem seja
  determinística entre páginas.
- **Índice composto** em `(Ativa, DataCadastro)`, que é o filtro mais comum.
- **Índice único** em `(VagaId, CandidatoEmail)`: a regra de "uma candidatura
  por e-mail por vaga" é garantida pelo banco, não por condicional no código.
- **`IExceptionHandler` global**: todos os erros saem em ProblemDetails,
  com o detalhe técnico no log e um `traceId` na resposta.
- **Segredos fora do repositório** via User Secrets.
