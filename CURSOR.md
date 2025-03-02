# Cursor Session Guidelines

## Project Structure
- Solution file: `VideoBatch.sln` in root directory
- Main project: `VideoBatchStarter/VideoBatch.csproj`
- Documentation: `VideoBatchStarter/README.md` (copied to output directory)

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
   - Runtime behavior
   - Documentation accessibility
   - Proper file locations

## Version Management
1. Version format: `MAJOR.MINOR.PATCH[-PRERELEASE]`
   - MAJOR: Breaking changes
   - MINOR: New features (backward compatible)
   - PATCH: Bug fixes (backward compatible)
   - PRERELEASE: e.g., '-beta', '-alpha'

2. To update version:
   - Edit `Version` in `VideoBatchStarter/VideoBatch.csproj`
   - Commit and push to master
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

## Best Practices
1. Always test locally before pushing
2. Check file paths and dependencies
3. Update version numbers appropriately
4. Keep documentation in sync with changes
5. Clean up build artifacts before committing 