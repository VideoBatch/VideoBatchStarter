# Cursor Session Guidelines

## Project Structure
- Solution file: `VideoBatch.sln` in root directory
- Main project: `VideoBatchStarter/VideoBatch.csproj`
- Documentation:
  - `README.md` and `CURSOR.md` in root directory
  - Additional documentation in `docs/` directory
  - `VideoBatchStarter/README.md` (copied to output directory)

## Build and Test Process
1. Always build and test before committing:
   ```bash
   # From the root directory:
   dotnet restore VideoBatch.sln
   dotnet build VideoBatch.sln
   dotnet run --project VideoBatchStarter/VideoBatch.csproj
   ```

2. Check for:
   - Build errors and warnings

3. Shortcut Commands
   - :issue: - the operator will follow :issue: with a bug, task or issue. First, document the issue in the `docs/ISSUES.md` file according to the template at the end of this file, then search the code base to understand what the presenting problem is, then identify the root cause of the issue, and outline the solution steps to fix in the Issue document, but don't apply the fix to the code. 
   - :fix: if the operatore approves the issue solution, then apply the changes, build and run the application and allow the operator to verify the fix, once verified, then update the Issue document and move the fixed issue to `docs/CHANGELOG.md` accord to the template outlined there
   - :shipit: - once all changes are done and tested, the operator may issue a :shipit: command which tells you to  bump the patch release version numver, then commit the code with useful commit messages, and include any issue titles if necessary, then push which will kick off a new build and release a new version using the github actions in the .github/workflow github action. 

4.  Git Commit and Publish
   - Don't commit and push until the code is tested and the :shipit: command is entered

## Version Management
1. Version format: `MAJOR.MINOR.PATCH[-PRERELEASE]`
   - MAJOR: Breaking changes
   - MINOR: New features (backward compatible)
   - PATCH: Bug fixes (backward compatible)
   - PRERELEASE: e.g., '-beta', '-alpha'

2. To update version:
   - Edit `Version` in `VideoBatchStarter/VideoBatch.csproj`
   - Commit and push to master (after ":shipit" approval)
   - GitHub Actions will automatically:
     - Create a tag (e.g., v1.0.1-beta)
     - Build and package the release
     - Create a GitHub release

## Release Process
1. GitHub Actions workflow (`release.yml`) handles:
   - Building the solution
   - Publishing as single-file executable
   - Including README.md and other assets
   - Creating ZIP archive
   - Publishing GitHub release

2. Release artifacts include:
   - Single-file executable
   - README.md
   - Configuration files
   - Required runtime components

## Common Issues and Solutions
1. README.md location:
   - Development: `VideoBatchStarter/README.md`
   - Published: Copied to executable directory
   - Documentation service checks both locations

2. Build directory:
   - Avoid committing `bin`, `obj`, and test-publish directories
   - Clean with `dotnet clean` before commits

3. Nullability warnings:
   - Use `NoWarn` in csproj for framework-related warnings
   - Properly initialize required fields in constructors
   - Use nullable annotations where appropriate

## Documentation Structure
1. Root directory:
   - `README.md` - Main project documentation
   - `CURSOR.md` - Development guidelines and commands

2. docs/ directory:
   - `ISSUES.md` - Active issues and templates
   - `CHANGELOG.md` - Release history and resolved issues
   - `spec.md` - Technical specifications
   - Additional documentation files

## Best Practices
1. Always test locally before pushing
2. Check file paths and dependencies
3. Update version numbers appropriately
4. Keep documentation in sync with changes
5. Clean up build artifacts before committing
6. Wait for explicit ":shipit" approval before committing changes 