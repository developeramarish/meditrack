# Tests — Domain Context

## Testing Strategy

**Two tiers:**
1. **Unit tests** (`*.UnitTests`) — fast, no infrastructure, mock external dependencies with NSubstitute
2. **Integration tests** (`*.IntegrationTests`) — real PostgreSQL via WebApplicationFactory, use Testcontainers when Docker available

## Project Map

| Test Project | Source Project(s) | Focus |
|---|---|---|
| `Clara.UnitTests` | Clara.API | MCP tool handlers, session services, validators, AI pipeline logic |
| `MedicalRecords.UnitTests` | MedicalRecords.Domain, .Infrastructure, .API | Domain entities, value objects, aggregates, CQRS handlers |
| `Clara.IntegrationTests` | Clara.API | HTTP endpoints, SignalR hub, DB queries, full request pipeline |

## Conventions

- **Naming:** `{ClassName}Tests.cs` mirrors `{ClassName}.cs`
- **Method naming:** `MethodName_Scenario_ExpectedResult`
- **Arrange-Act-Assert** pattern in every test
- **One assertion per test** (prefer multiple focused tests over one test with many assertions)
- **NSubstitute** for mocking interfaces — never mock concrete classes
- **FluentAssertions** for all assertions — never use `Assert.*`
- **Bogus** for test data generation when needed

## Running Tests

```bash
dotnet test --filter "FullyQualifiedName~UnitTests"       # Fast — no Docker needed
dotnet test --filter "FullyQualifiedName~IntegrationTests" # Needs PostgreSQL running
dotnet test                                                # Everything
```

## TDD Cycle

1. **RED** — Write a failing test for the behavior you want
2. **GREEN** — Write the minimum code to make it pass
3. **REFACTOR** — Clean up while keeping tests green
4. Commit after each GREEN step
