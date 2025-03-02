# VideoBatch

## Known Issues

### AcrylicUI Issues
- Unwanted border appears around the hover button panel in message dialogs

These issues are related to the AcrylicUI library itself and will need to be addressed upstream.

## Creating Releases

The project uses GitHub Actions to automatically build and publish releases. When you push a version tag, it will:
1. Build the application in Release configuration
2. Create a self-contained single-file executable
3. Package it into a ZIP file
4. Create a GitHub Release with the ZIP file attached

To create a new release:

1. Tag your release (replace X.Y.Z with your version number):
   ```bash
   git tag -a vX.Y.Z -m "Release vX.Y.Z"
   git push origin vX.Y.Z
   ```

2. The GitHub Actions workflow will automatically:
   - Build the Windows Forms app
   - Create a self-contained executable (no .NET runtime required)
   - Package it in a ZIP file
   - Create a GitHub Release

3. Once complete, you can find the release in the "Releases" section of the repository
   - The ZIP file contains a single executable with all dependencies
   - No additional installation required
   - Optimized for Windows x64