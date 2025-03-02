# Custom Toolbar Implementation Specification

## Overview
The VideoBatch application implements a custom toolbar using AcrylicUI's WindowPanel control, which provides a modern, borderless window experience with custom minimize, maximize, restore, and close buttons. The implementation includes several Windows API hacks to handle window behaviors correctly.

## Core Components

### 1. WindowPanel Control
The main container for the custom toolbar is the `WindowPanel` control from AcrylicUI:

```csharp
private AcrylicUI.Controls.WindowPanel windowPanel1;
```

Key Properties:
- `ProfileFeature`: Enables/disables the profile button
- `IsAcrylic`: Controls the acrylic effect (currently disabled)
- `Icon`: Custom application icon
- `SectionHeader`: Window title text
- `RoundCorners`: Enables/disables rounded corners (Windows 11 feature)

### 2. Window State Management

#### Fields
```csharp
private int borderSize = 0;        // Border size for window
private bool _flatBorder = true;   // Flat border style
private Size _restoreSize;         // Size for window restoration
```

#### Window States
The application handles three window states:
1. Normal (Restored)
2. Maximized
3. Minimized

### 3. Custom Window Behaviors

#### Border Removal
- Implements borderless window while maintaining snap functionality
- Uses Windows API hooks to handle window messages
- Preserves resize functionality without standard window chrome

#### Window Resizing
Custom implementation for:
- Window dragging
- Edge resizing
- Double-click maximize/restore
- Window snapping

## Implementation Details

### 1. Window Setup
```csharp
private void SetupUIDefaults()
{
    FormBorderStyle = FormBorderStyle.Sizable;
    windowPanel1.ProfileFeature = true;
    windowPanel1.IsAcrylic = false;
    BlurOpacity = 255;  // No opacity
    BackColor = Colors.MontereyDark;
}
```

### 2. Windows 11 Integration

#### Round Corners Support
```csharp
private void RoundCorners(bool isWindows11)
{
    if (isWindows11)
    {
        var attribute = DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE;
        var preference = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND;
        DwmSetWindowAttribute(Handle, attribute, ref preference, sizeof(uint));
    }
    windowPanel1.RoundCorners = isWindows11;
}
```

### 3. Window Message Handling

#### Message Processing
```csharp
protected override void WndProc(ref Message message)
{
    // Handle window resizing
    if (message.Msg == WM_NCHITTEST)
    {
        // Custom hit testing for resize areas
    }

    // Remove border while keeping snap
    if (message.Msg == WM_NCCALCSIZE)
    {
        // Custom non-client area calculation
    }

    // Handle minimize/restore
    if (message.Msg == WM_SYSCOMMAND)
    {
        // Save/restore window size
    }
}
```

### 4. Custom Button Handlers

#### Minimize
```csharp
private void BtnMin_Click(object sender, EventArgs e)
{
    _restoreSize = ClientSize;
    WindowState = FormWindowState.Minimized;
    AdjustForm();
}
```

#### Maximize/Restore
```csharp
private void BtnMaximize_Click(object sender, EventArgs e)
{
    _restoreSize = ClientSize;
    WindowState = (WindowState == FormWindowState.Normal ? 
                   FormWindowState.Maximized : 
                   FormWindowState.Normal);
    AdjustForm();
}
```

### 5. Form Adjustments

#### Padding Adjustments
```csharp
private void AdjustForm()
{
    switch (WindowState)
    {
        case FormWindowState.Maximized:
            Padding = new Padding(8, 8, 8, 8);
            break;
        case FormWindowState.Normal:
            if (Padding.Top != borderSize)
                Padding = new Padding(borderSize);
            break;
    }
}
```

## Full Screen Implementation (F11)

### Current Status
- Menu item exists with F11 shortcut
- Not yet implemented
- Event handler stub in place

### Implementation Plan
1. Store original window state before full screen
2. Handle F11 toggle:
   ```csharp
   private void ToggleFullScreen_Click(object sender, EventArgs e)
   {
       if (!_isFullScreen)
       {
           _preFullScreenState = WindowState;
           _preFullScreenBounds = Bounds;
           FormBorderStyle = FormBorderStyle.None;
           WindowState = FormWindowState.Maximized;
           windowPanel1.Visible = false;
           _isFullScreen = true;
       }
       else
       {
           FormBorderStyle = FormBorderStyle.Sizable;
           WindowState = _preFullScreenState;
           Bounds = _preFullScreenBounds;
           windowPanel1.Visible = true;
           _isFullScreen = false;
       }
   }
   ```

## Known Limitations and Hacks

1. **Window Border Hack**
   - Uses custom WndProc handling to remove borders
   - Maintains resize functionality through hit testing

2. **DPI Scaling**
   - Requires explicit DPI awareness
   - Uses GetDpiForWindow API call

3. **Dark Theme Integration**
   - Uses Win32Hacks.DarkThemeTitleBar for consistent appearance
   - Requires Windows 10 or later

4. **Drop Shadow**
   - Implements custom CreateParams for drop shadow effect
   - May have performance impact on older systems

## Future Improvements

1. **Full Screen Mode**
   - Implement F11 toggle functionality
   - Handle multi-monitor scenarios
   - Preserve window state

2. **Performance Optimization**
   - Reduce WndProc message handling
   - Optimize resize operations

3. **Windows 11 Integration**
   - Enhanced Snap Layouts support
   - Better DWM integration

4. **Accessibility**
   - Keyboard navigation in custom toolbar
   - Screen reader support 