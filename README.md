# Survey Management API

A .NET 10 Web API for creating surveys, submitting responses and viewing submitted responses.
Built with **Clean Architecture**, **CQRS** with **MediatR**, **FluentValidation** and **EF Core** (SQL Server LocalDB).

## Tech stack

| Area | Choice |
|---|---|
| Framework | ASP.NET Core Web API (.NET 10) |
| Database | SQL Server LocalDB via Entity Framework Core 10 (migrations included) |
| CQRS | MediatR 12.5.0 (commands for writes, queries for reads) |
| Validation | FluentValidation 12, run automatically through a MediatR pipeline behavior |
| API docs | Swagger / OpenAPI (Swashbuckle) |
| Tests | xUnit with the EF Core in-memory provider |

## Project structure

```
SurveyManagement.slnx
src/
├── SurveyManagement.Domain          Entities and enums. Depends on nothing.
├── SurveyManagement.Application     Commands, queries, handlers, validators, DTOs.
├── SurveyManagement.Infrastructure  EF Core DbContext, entity configurations, migrations.
└── SurveyManagement.API             Controllers, Swagger, global exception handling.
tests/
└── SurveyManagement.UnitTests       Validator and handler tests.
```

Dependencies point inward: `API → Infrastructure → Application → Domain`.
Handlers depend on the `IAppDbContext` interface, so the Application layer never references Infrastructure.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server LocalDB (installed with Visual Studio's "ASP.NET and web development" workload)
- EF Core CLI tool (only needed for the command-line setup):

```bash
dotnet tool install --global dotnet-ef
```

## How to run

### 1. Create the database

From the solution folder:

```bash
dotnet ef database update --project src/SurveyManagement.Infrastructure --startup-project src/SurveyManagement.API
```

This creates the `SurveyManagementDb` database on `(localdb)\MSSQLLocalDB`.
To use a different SQL Server, change `ConnectionStrings:DefaultConnection` in `src/SurveyManagement.API/appsettings.json`.

### 2. Start the API

**Visual Studio:** open `SurveyManagement.slnx`, set `SurveyManagement.API` as the startup project, choose the `http` profile and press **F5**.

**Command line:**

```bash
dotnet run --project src/SurveyManagement.API --launch-profile http
```

### 3. Open Swagger

Go to **http://localhost:5088/swagger**. All endpoints can be tried from there.

## Endpoints

| Operation | Method & route | Type | Success | Errors |
|---|---|---|---|---|
| Create survey | `POST /api/surveys` | Command | 201 | 400 |
| Get all surveys | `GET /api/surveys` | Query | 200 | |
| Get survey by id | `GET /api/surveys/{id}` | Query | 200 | 404 |
| Submit response | `POST /api/surveys/{id}/submit` | Command | 201 | 400, 404 |
| View responses | `GET /api/surveys/{id}/responses` | Query | 200 | 404 |

Question types: `SingleSelect`, `MultiSelect`, `Text`.

### Create survey

```json
POST /api/surveys
{
  "title": "Customer Satisfaction Survey",
  "description": "Please provide your feedback.",
  "questions": [
    { "text": "How satisfied are you?", "type": "SingleSelect", "options": ["Excellent", "Good", "Average", "Poor"] },
    { "text": "Which channels did you use?", "type": "MultiSelect", "options": ["Website", "Mobile App", "Call Center"] },
    { "text": "Additional comments", "type": "Text" }
  ]
}
```

### Submit response

Use the question and option ids returned by `GET /api/surveys/{id}`.

```json
POST /api/surveys/1/submit
{
  "answers": [
    { "questionId": 1, "selectedOptionIds": [2] },
    { "questionId": 2, "selectedOptionIds": [5, 6] },
    { "questionId": 3, "textAnswer": "The service was very good." }
  ]
}
```

### Error responses

Errors use the standard `ProblemDetails` format. Validation errors list every failing field:

```json
{
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Answers[0].SelectedOptionIds": ["Single select questions accept only one option."]
  }
}
```

## Validation rules

**Creating a survey** (`CreateSurveyCommandValidator`)
- Title is required (max 200 characters).
- At least one question is required, and each question needs text.
- The question type must be valid.
- Single and multiple select questions need at least two options.
- Text questions must not have options.

**Submitting a response** (`SubmitSurveyResponseCommandValidator` + handler)
- The survey must exist (404 otherwise).
- Each question must belong to the survey, and can only be answered once.
- Selected options must belong to their question.
- Single select accepts exactly one option; multiple select accepts one or more.
- Text questions require a text answer and accept no options.

Rules that only check the request shape live in the FluentValidation validator. Rules that need the database live in the handler, which returns the same 400 format.

## Database design

| Table | Purpose |
|---|---|
| `Surveys` | Title, description, creation date |
| `Questions` | Question text, type and display order, linked to a survey |
| `QuestionOptions` | Selectable options for a question |
| `SurveyResponses` | One row per submission |
| `Answers` | One row per answered question in a submission, holds the text answer |
| `AnswerOptions` | Join table: one row per selected option (composite key `AnswerId + QuestionOptionId`) |

Storing selected options in a join table keeps foreign keys on every option, so the database itself guarantees that each selected option exists.

## Running the tests

```bash
dotnet test
```

Or use **Test → Test Explorer → Run All** in Visual Studio. The tests use an in-memory database, so SQL Server isn't needed.
