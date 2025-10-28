# NexusMods App - Agent Development Guide

## Build & Test Commands

**Build entire solution:**
```bash
dotnet build --nologo -v q --property WarningLevel=0 /clp:ErrorsOnly
```

**Run single test project:**
```bash
dotnet test tests/NexusMods.DataModel.Tests/NexusMods.DataModel.Tests.csproj --nologo -v q -p:WarningLevel=0 /clp:ErrorsOnly
```

**Run specific test method:**
```bash
dotnet test --filter "TestMethodName" --nologo -v q -p:WarningLevel=0 /clp:ErrorsOnly
```

**Lint/Format:**
```bash
# DO NOT FORMAT - No formatting is enforced in this repository
# NEVER RUN dotnet format commands
```

When testing, only run tests in the project where the code has changed.
Never run all tests.

## Code Style Guidelines

### General
- **Indentation:** 4 spaces (configured in .editorconfig)
- **Nullable reference types:** Enabled
- **Implicit usings:** Enabled

### Naming Conventions
- **Classes:** PascalCase (e.g., `GameInstaller`)
- **Methods/Properties:** PascalCase (e.g., `InstallMod()`)
- **Fields:** _camelCase with underscore prefix for private fields
- **Constants:** PascalCase
- **Namespaces:** PascalCase, hierarchical (e.g., `NexusMods.Games.Generic`)

### Error Handling
- Never swallow exceptions without logging
- Use structured logging with proper context
- Async methods should handle cancellation tokens

### Testing
- **Frameworks:** xUnit, TUnit, Verify for snapshot testing
- **Test projects:** End with `.Tests` and mirror source structure
- **Naming:** `[MethodName]_[Scenario]_[ExpectedResult]`
- Use `[Fact]` for unit tests, `[Theory]` for parameterized tests
- Use `NSubstitute` and `FluentAssertions`.

### Architecture Patterns
- **DI:** Use Microsoft.Extensions.DependencyInjection
- **Async:** Prefer async/await, avoid .Result/.Wait
- **Collections:** Use ObservableCollections for reactive UI
- **Database:** MnemonicDB for data persistence

### Package Management
- Central package management via Directory.Packages.props
- Use explicit package versions only when overriding central version
- Prefer abstractions over concrete implementations

### UI (Avalonia)
- Follow MVVM pattern with ReactiveUI
- Use reactive extensions (R3) for event handling
- Separate view models into dedicated files
- Use data binding extensively, avoid code-behind logic
- Use `ObserveDatoms` rather than `Datoms`, prefer real time Database updates
