# VideoBatch Issues Tracker

## Active Issues

### ISSUE-DOC1: Reorganize Documentation Files

**Status**: Under Investigation

**Description**:  
Need to reorganize documentation files by moving them into a dedicated `docs/` folder, while keeping `README.md` and `CURSOR.md` in the root directory. This will improve project organization and make documentation easier to maintain.

**Steps to Reproduce**:
1. N/A - This is a structural improvement issue

**Initial Analysis**:
- Several .md files are currently in the root directory
- Need to maintain easy access to README.md and CURSOR.md
- Documentation references in code need to be updated
- CURSOR.md needs updating to reflect new structure

**Root Cause**:
Documentation files have accumulated in the root directory, making the project structure less organized and harder to maintain.

**Proposed Fix**:
1. Create a new `docs/` directory in the root
2. Move all .md files except README.md and CURSOR.md to docs/
3. Update CURSOR.md to document the new structure
4. Update any code references to documentation files
5. Update documentation links if present
6. Verify documentation service still works with new structure

**Impact**:
- Improved project organization
- Better documentation maintainability
- Cleaner root directory
- No functional changes to application

**Priority**: Low

**Files Affected**:
- All .md files except README.md and CURSOR.md
- CURSOR.md (to update documentation about new structure)
- Any code files referencing documentation paths

### 1. ToolWindow Close Button Not Working on Initial Load

**Description:**  
The close button ('x') on tool windows does not function when the application first loads. The button becomes functional only after toggling the window visibility through the menu.

**Steps to Reproduce:**
1. Launch the application
2. Attempt to close any tool window using its close button
3. Observe that the close button does not respond
4. Toggle the window visibility using the View menu
5. Try the close button again - it now works

**Initial Analysis:**  
After reviewing the codebase, the issue appears to be related to how tool windows are initialized and added to the DockPanel. The current implementation in `LoadToolWindows()` directly adds content to the DockPanel without properly initializing the event handlers that manage window state.

**Root Cause:**  
1. Tool windows are added directly to the DockPanel using `DockPanel.AddContent()` during initialization
2. The menu state and event handlers for window management are not properly synchronized with the initial window state
3. The `DockContentEventArgs` events are only hooked up after the windows are already added

**Potential Impact:**
- Poor user experience when trying to manage tool windows
- Inconsistent behavior between initial load and subsequent interactions
- Menu items may not correctly reflect window states

**Proposed Fix:**
1. Modify the `LoadToolWindows()` method to use the `ToggleToolWindow()` method for initial window display
2. Update the window initialization process to ensure event handlers are registered before adding content
3. Add proper state management for tool windows during initialization

### 2. Tool Window Close Button Hover Investigation

**Description:**  
Need to investigate if the close button ('x') on tool windows is receiving mouse hover events. This will help determine if the issue is with event handling or if we need to modify the AcrylicUI library.

**Investigation Goals:**
1. Determine if mouse hover events are being triggered on the close button
2. Verify if the event handling system is properly initialized
3. Identify which component is responsible for handling the close button events

**Proposed Investigation:**
1. Add logging for mouse hover events on tool windows
2. Monitor if events are triggered differently on initial load vs after toggle
3. Determine if the issue is in our code or requires AcrylicUI library changes

**Status:** Awaiting `:fix:` command to implement logging

**Priority:** Medium

**Impact:** Diagnostic information for issue #1

## Resolved Issues

_(None yet)_

## Issue Template

### ISSUE-XXX: Title
**Status**: [Under Investigation/In Progress/Fixed/Closed]

**Description**:  
Brief description of the issue

**Steps to Reproduce**:
1. Step 1
2. Step 2
3. ...

**Initial Analysis**:
- Key observation 1
- Key observation 2

**Root Cause**:
Explanation of what causes the issue

**Fix**:
Description of how the issue was fixed 