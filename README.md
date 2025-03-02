# VideoBatch

## Known Issues

### AcrylicUI Issues
- Unwanted border appears around the hover button panel in message dialogs

These issues are related to the AcrylicUI library itself and will need to be addressed upstream.

## Creating Releases

The project uses GitHub Actions to automatically build and publish releases based on the version specified in the project file. When you push to the master branch, it will:

1. Read the version from VideoBatch.csproj (currently set to 1.0.0-beta)
2. Build the application in Release configuration
3. Create a self-contained single-file executable
4. Package it into a ZIP file named VideoBatch-[version].zip
5. Create a GitHub Release with the version tag

To create a new release:

1. Update the version in VideoBatch.csproj:
   ```xml
   <PropertyGroup>
     <Version>X.Y.Z</Version>
   </PropertyGroup>
   ```

2. Commit and push your changes to the master branch:
   ```bash
   git add VideoBatch.csproj
   git commit -m "chore: Bump version to X.Y.Z"
   git push origin master
   ```

3. The GitHub Actions workflow will automatically:
   - Build the Windows Forms app
   - Create a self-contained executable (no .NET runtime required)
   - Package it in a versioned ZIP file
   - Create a GitHub Release
   - Set prerelease flag automatically for versions containing a hyphen (e.g., 1.0.0-beta)

4. Once complete, you can find the release in the "Releases" section of the repository
   - The ZIP file contains a single executable with all dependencies
   - No additional installation required
   - Optimized for Windows x64