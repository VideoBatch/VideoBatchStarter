# VideoBatch

A Windows Forms application for batch video processing, built with .NET 9.0 and AcrylicUI dark theme.

## Menu Structure and Implementation Status

### File Menu

#### Project Management
- **New Project** (Ctrl+N) - ❌ Not implemented
  - Creates a new video batch processing project
  - Will support creating a new JSON project file

- **Open Project** (Ctrl+O) - ❌ Not implemented
  - Opens an existing video batch processing project
  - Supports JSON project files

- **Save Project** (Ctrl+S) - ❌ Not implemented
  - Saves the current project
  - Saves to JSON format

- **Save Project As** (Ctrl+Shift+S) - ❌ Not implemented
  - Saves the current project to a new location
  - Supports JSON format

- **Recent Projects** - ❌ Not implemented
  - Shows list of recently opened projects
  - Will maintain history in application settings

- **Exit** (Alt+X) - ✅ Implemented
  - Closes the application
  - Prompts to save if there are unsaved changes

### Edit Menu

#### Editing Operations
- **Undo** (Ctrl+Z) - ❌ Not implemented
  - Reverts the last action
  - Will maintain action history

- **Redo** (Ctrl+Y) - ❌ Not implemented
  - Reapplies previously undone action
  - Will maintain action history

#### Clipboard Operations
- **Cut** (Ctrl+X) - ❌ Not implemented
  - Cuts selected items to clipboard

- **Copy** (Ctrl+C) - ❌ Not implemented
  - Copies selected items to clipboard

- **Paste** (Ctrl+V) - ❌ Not implemented
  - Pastes items from clipboard

- **Delete** (Del) - ❌ Not implemented
  - Removes selected items

### View Menu

#### UI Components
- **Project Explorer** - ✅ Implemented
  - Toggles the Project Explorer panel visibility
  - Shows project structure and files
  - Supports docking, floating, and closing
  - Menu state syncs with panel visibility

- **Assets** - ✅ Implemented
  - Toggles the Assets panel visibility
  - Shows media file properties and metadata
  - Supports docking, floating, and closing
  - Menu state syncs with panel visibility

- **Batch Processing** - ✅ Implemented
  - Toggles the Batch Processing panel visibility
  - Shows batch processing settings and controls
  - Supports docking, floating, and closing
  - Menu state syncs with panel visibility

- **Output Window** - ✅ Implemented
  - Toggles the Output Window visibility
  - Shows processing logs and status
  - Supports docking, floating, and closing
  - Menu state syncs with panel visibility

#### Display Options
- **Full Screen** (F11) - ⏸️ Temporarily disabled
  - Full screen mode will be implemented in a future update
  - Will support F11 shortcut toggle
  - Will preserve window state and position

### Help Menu

#### Support
- **Documentation** (F1) - ✅ Implemented
  - Opens the user documentation
  - Renders README.md with dark theme styling
  - Displays in default browser

- **Check for Updates** - ❌ Not implemented
  - Checks for new versions of VideoBatch
  - Will support automatic updates

- **About VideoBatch** - ✅ Implemented
  - Shows application information
  - Displays version, credits, and license info
  - Links to website (www.videobatch.co.uk)

## Implementation Notes

- Current Version: 1.0.0-beta
- All menu items have logging implemented for tracking usage
- Dark theme implemented using AcrylicUI
- Project files use JSON format for compatibility and ease of editing
- Docking system implemented with support for:
  - Panel docking to left, right, and bottom areas
  - Floating windows
  - Tabbed interface for panels in same dock area
  - Synchronized menu state with panel visibility
  - Close button functionality for all panels

## Development Status

- Basic UI framework: ✅ Implemented
- Menu structure: ✅ Implemented
- Dark theme: ✅ Implemented
- Docking system: ✅ Implemented
- Core functionality: ❌ In progress 

## Known Issues

### UI/UX Issues
1. About Dialog
   - Button styling needs to be updated to match AcrylicUI theme
   - Button size and appearance needs adjustment
   - Need to implement proper AcrylicUI button controls instead of standard WinForms buttons 