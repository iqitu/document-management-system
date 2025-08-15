# Document Management System

Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.

## Repository Status and Limitations

**CRITICAL**: This repository is in early development. The solution file references 9 projects but only 1 project currently exists and can be built.

**Working Projects:**
- `DocumentManagement.Models` - .NET 7.0 class library (builds successfully)

**Missing Projects (referenced in solution but do not exist):**
- `DocumentManagement.WPF` - WPF desktop application
- `DocumentManagement.Core` - Core business logic
- `DocumentManagement.Data` - Data access layer 
- `DocumentManagement.Services` - Service layer
- `DocumentManagement.WordParser` - Word document parser
- `DocumentManagement.Core.Tests` - Core unit tests
- `DocumentManagement.Services.Tests` - Services unit tests
- `DocumentManagement.WordParser.Tests` - Parser unit tests

## Working Effectively

### Environment Requirements
- .NET SDK 8.0.118+ is available and working
- Target framework: .NET 7.0
- OS: Ubuntu 24.04 Linux

### Building the Repository

**NEVER build at solution level** - it will fail due to missing projects. Always work with individual projects.

#### Build Commands (VALIDATED)
```bash
# Navigate to the only working project
cd src/DocumentManagement.Models

# Restore packages (takes ~1 second)
dotnet restore

# Build Debug configuration (takes ~2-3 seconds)
dotnet build

# Build Release configuration (takes ~3-4 seconds) 
dotnet build -c Release

# Clean build artifacts
dotnet clean

# Full clean rebuild (takes ~3-4 seconds total)
dotnet clean && dotnet build
```

**Timing Expectations:**
- Package restore: ~1 second
- Debug build: ~2-3 seconds  
- Release build: ~3-4 seconds
- Clean: ~0.5 seconds

### Commands That FAIL
```bash
# DO NOT run these - they will fail due to missing projects:
dotnet build                    # (from solution root)
dotnet restore                  # (from solution root) 
dotnet test                     # (anywhere - no test projects exist)
dotnet run                      # (no executable projects)
```

### Project Structure
```
DocumentManagement.sln          # Solution file (references missing projects)
src/
  DocumentManagement.Models/    # ONLY working project
    DocumentManagement.Models.csproj
    Document.cs                 # Sample model class
    bin/Debug/net7.0/          # Build output (gitignored)
    obj/                       # Build temp files (gitignored)
```

## Validation Scenarios

### Always validate changes by:
1. **Build Test**: Navigate to `src/DocumentManagement.Models` and run `dotnet build`
2. **Clean Build Test**: Run `dotnet clean && dotnet build` 
3. **Release Build Test**: Run `dotnet build -c Release`
4. **Assembly Verification**: Check that `bin/Debug/net7.0/DocumentManagement.Models.dll` is created

### Manual Validation Steps
```bash
cd src/DocumentManagement.Models

# Verify project can be built
dotnet build

# Verify output assembly exists and is valid
ls -la bin/Debug/net7.0/DocumentManagement.Models.dll
file bin/Debug/net7.0/DocumentManagement.Models.dll

# Should show: "PE32 executable (DLL) (console) Intel 80386 Mono/.Net assembly"
```

## Development Guidelines

### Working with Models Project
- Add new model classes to `src/DocumentManagement.Models/`
- Models use .NET 7.0 features including nullable reference types
- Available packages: `System.ComponentModel.DataAnnotations`, `Newtonsoft.Json`
- Always build after making changes to verify syntax

### Git Workflow
- Build artifacts are gitignored (bin/, obj/, *.dll, etc.)
- Always run `dotnet build` before committing to ensure code compiles
- Do not attempt to build missing projects

### Creating New Projects
If creating missing projects referenced in the solution:
1. Use `dotnet new` templates appropriate to project type
2. Ensure target framework is `net7.0`  
3. Add project references as needed
4. Update build instructions once working

## Common Tasks

### Repository Root Contents
```
ls -la
total 24
drwxr-xr-x 4 runner docker 4096 Aug 15 15:09 .
drwxr-xr-x 3 runner docker 4096 Aug 15 15:09 ..
drwxr-xr-x 7 runner docker 4096 Aug 15 15:09 .git
-rw-r--r-- 1 runner docker 5829 Aug 15 15:09 DocumentManagement.sln
drwxr-xr-x 3 runner docker 4096 Aug 15 15:09 src
-rw-r--r-- 1 runner docker  436 Aug 15 15:16 .gitignore
```

### .NET Version Info
```bash
dotnet --version
# Output: 8.0.118

dotnet --info | head -10
# .NET SDK:
#  Version:           8.0.118
#  Commit:            f4db1f6abf
#  Workload version:  8.0.100-manifests.1f3a291a
# Runtime Environment:
#  OS Name:     ubuntu
#  OS Version:  24.04
#  OS Platform: Linux
#  RID:         ubuntu.24.04-x64
```

### Models Project Structure
```bash
ls -la src/DocumentManagement.Models/
# DocumentManagement.Models.csproj
# Document.cs
# bin/                 (after build)
# obj/                 (after build)
```

### Sample Build Output
```bash
cd src/DocumentManagement.Models && dotnet build
# MSBuild version 17.8.31+af11d4f25 for .NET
#   Determining projects to restore...
#   All projects are up-to-date for restore.
#   DocumentManagement.Models -> /path/to/bin/Debug/net7.0/DocumentManagement.Models.dll
# 
# Build succeeded.
#     0 Warning(s)
#     0 Error(s)
# Time Elapsed 00:00:02.44
```

## Architecture Overview

Based on the solution structure, this appears to be designed as a layered .NET application:

- **Models**: Data transfer objects and entities
- **Core**: Business logic and domain services  
- **Data**: Entity Framework/database access
- **Services**: Application services and APIs
- **WordParser**: Document processing functionality
- **WPF**: Desktop user interface
- **Tests**: Unit test projects

**Current Status**: Only the Models layer exists and is functional.