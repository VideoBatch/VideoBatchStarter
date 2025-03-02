# Docking System Specification

## Overview
The docking system is built on top of AcrylicUI, a Windows Forms UI framework that provides modern, acrylic-style interface components. The system allows for a flexible, multi-panel interface with dockable windows that can be shown/hidden through menu options.

## VideoBatch Implementation Notes
This specification describes how to enhance the existing VideoBatch application, which currently has:
- A Project panel on the left
- File navigation menu
- Status bar

The goal is to add the following panels while preserving existing functionality:
1. Media Inspector Panel (for video file details)
2. Batch Processing Panel (similar to Canvas Chain)
3. Output Panel (for processing results and logs)

## Dependencies
```xml
<PackageReference Include="AcrylicUI" Version="1.0.*" />
```

## Core Components

### 1. Main Form (frmMain)
The main form serves as the container for all docked panels and implements the following key features:

- DockPanel: Main container for all dockable windows
- Menu system for toggling panel visibility
- Status bar for application messages
- Event handling for panel management

#### Basic Structure
```csharp
public partial class frmMain : AcrylicForm
{
    private readonly AcrylicUI.Docking.DockPanel DockPanel;
    private readonly List<ToolWindow> _toolWindows = new();
    
    // Injected dependencies
    private readonly ILogger<frmMain> logger;
    private readonly IVideoProcessingService videoProcessingService;
    private readonly IWorkAreaFactory workAreaFactory;
    
    // Dock Windows
    private readonly ProjectTree projectTree;           // Existing VideoBatch panel
    private readonly MediaInspectorDock mediaInspector; // For video file details
    private readonly BatchProcessingDock batchDock;     // For processing chain
    private readonly OutputDock outputDock;            // For results and logs
}
```

### 2. Tool Windows
Each dockable panel inherits from `ToolWindow` and implements specific functionality:

#### ProjectTree (Existing)
- Default dock area: Left
- Displays video project structure
- Handles video file selection and management

#### MediaInspectorDock (New)
- Default dock area: Bottom
- Displays video file metadata and properties
- Shows preview frames and technical details
- Implementation focus: Video file analysis

#### BatchProcessingDock (New)
- Default dock area: Right
- Manages video processing chain
- Handles processing task configuration
- Implementation focus: Video processing workflow

#### OutputDock (New)
- Default dock area: Bottom
- Displays processing logs and results
- Shows progress and error information
- Implementation focus: Processing feedback

### 3. View Management

#### Menu Structure
```csharp
// View menu items (extend existing menu)
mnuView.DropDownItems.AddRange(new ToolStripItem[] {
    mnuViewProjectExplorer,     // Existing
    mnuViewMediaInspector,      // New
    mnuViewBatchProcessing,     // New
    mnuViewOutput              // New
});
```

#### Panel Toggle Implementation
```csharp
private void ToggleToolWindow(ToolWindow toolWindow)
{
    if (toolWindow.DockPanel == null)
        DockPanel.AddContent(toolWindow);
    else
        DockPanel.RemoveContent(toolWindow);
}
```

#### Menu Click Handlers
```csharp
private void Project_Click(object sender, EventArgs e)
{
    ToggleToolWindow(projectTree);
}

private void MediaInspector_Click(object sender, EventArgs e)
{
    ToggleToolWindow(mediaInspector);
}

private void MnuViewBatchProcessing_Click(object sender, EventArgs e)
{
    ToggleToolWindow(batchDock);
}

private void MnuViewOutput_Click(object sender, EventArgs e)
{
    ToggleToolWindow(outputDock);
}
```

## Implementation Steps for VideoBatch

1. **Preserve Existing Functionality**
   - Document current ProjectTree implementation
   - Ensure existing menu structure is preserved
   - Map current event handlers

2. **Create New Tool Windows**
   ```csharp
   public class MediaInspectorDock : ToolWindow
   {
       public MediaInspectorDock()
       {
           DefaultDockArea = DockArea.Bottom;
           DockText = "Media Inspector";
       }
       
       public void UpdateVideoDetails(string filePath)
       {
           // Implement video metadata display
       }
   }
   
   public class BatchProcessingDock : ToolWindow
   {
       public BatchProcessingDock()
       {
           DefaultDockArea = DockArea.Right;
           DockText = "Batch Processing";
       }
       
       public void ConfigureProcessingChain()
       {
           // Implement processing chain configuration
       }
   }
   
   public class OutputDock : ToolWindow
   {
       public OutputDock()
       {
           DefaultDockArea = DockArea.Bottom;
           DockText = "Output";
       }
       
       public void LogProcessingStatus(string message)
       {
           // Implement processing log display
       }
   }
   ```

3. **Integrate with Existing Code**
   ```csharp
   // In main form constructor after existing initialization
   mediaInspector = new MediaInspectorDock();
   batchDock = new BatchProcessingDock();
   outputDock = new OutputDock();
   
   _toolWindows.Add(mediaInspector);
   _toolWindows.Add(batchDock);
   _toolWindows.Add(outputDock);
   ```

4. **Add Event Handlers**
   ```csharp
   // Wire up project tree selection to media inspector
   projectTree.FileSelected += (sender, file) => 
   {
       mediaInspector.UpdateVideoDetails(file.Path);
   };
   
   // Wire up batch processing to output
   batchDock.ProcessingStarted += (sender, args) =>
   {
       outputDock.LogProcessingStatus("Processing started...");
   };
   ```

## Implementation Steps

1. **Setup Project Dependencies**
   - Add AcrylicUI NuGet package
   - Reference required project dependencies

2. **Create Base Windows**
   - Implement main form inheriting from `AcrylicForm`
   - Create tool windows inheriting from `ToolWindow`

3. **Configure DockPanel**
   ```csharp
   // In main form constructor
   DockPanel = new DockPanel();
   DockPanel.Dock = DockStyle.Fill;
   Controls.Add(DockPanel);
   
   // Add message filters
   Application.AddMessageFilter(DockPanel.DockContentDragFilter);
   Application.AddMessageFilter(DockPanel.DockResizeFilter);
   ```

4. **Initialize Tool Windows**
   ```csharp
   // In main form constructor
   _toolWindows.Add(projectTree);
   _toolWindows.Add(mediaInspector);
   _toolWindows.Add(batchDock);
   _toolWindows.Add(outputDock);
   
   foreach (var toolWindow in _toolWindows)
   {
       DockPanel.AddContent(toolWindow);
   }
   ```

5. **Setup Event Handling**
   ```csharp
   // Panel events
   DockPanel.ContentAdded += DockPanel_ContentAdded;
   DockPanel.ContentRemoved += DockPanel_ContentRemoved;
   DockPanel.ActiveContentChanged += DockPanel_ActiveContentChanged;
   
   // Menu events
   mnuViewProjectExplorer.Click += Project_Click;
   mnuViewMediaInspector.Click += MediaInspector_Click;
   mnuViewBatchProcessing.Click += MnuViewBatchProcessing_Click;
   mnuViewOutput.Click += MnuViewOutput_Click;
   ```

## Panel Customization

Each tool window can be customized by setting properties in their constructor:

```csharp
public class CustomDock : ToolWindow
{
    public CustomDock()
    {
        DefaultDockArea = DockArea.Left; // or Right, Bottom, etc.
        DockText = "Window Title";
        // Additional initialization
    }
}
```

## Event Handling

### Content Added
```csharp
private void DockPanel_ContentAdded(object sender, DockContentEventArgs e)
{
    DockPanel panel = sender as DockPanel;
    var docs = panel.GetDocuments();
    DockContent doc = e.Content;
    
    logger.LogInformation($"Panel {panel.Name}: Content Added. Documents: {docs.Count}");
    
    // Additional handling for specific content types
    if (doc is ProjectWorkArea project)
    {
        // Wire up project-specific events
    }
}
```

### Content Removed
```csharp
private void DockPanel_ContentRemoved(object sender, DockContentEventArgs e)
{
    DockPanel panel = sender as DockPanel;
    var docs = panel.GetDocuments();
    DockContent doc = e.Content;
    
    // Clean up and handle removal
    if (doc is WorkArea w && w.IsDirty)
    {
        // Handle dirty state
    }
}
```

## Migration Notes

When migrating this system to a new project:

1. Ensure all required dependencies are properly referenced
2. Maintain the hierarchy of panels and their relationships
3. Implement proper dependency injection for services
4. Consider the state management of panels (dirty state, content updates)
5. Implement proper event handling for panel interactions
6. Maintain the menu system for panel visibility control

## Required Interfaces

The following interfaces should be implemented in the new project:

```csharp
public interface IWorkAreaFactory
{
    Task<WorkArea> CreateAsync(Guid id);
}

public interface IProjectServices
{
    void Update();
    // Additional project-related methods
}
```

## Required Interfaces for VideoBatch

```csharp
public interface IVideoProcessingService
{
    Task<VideoMetadata> GetVideoMetadataAsync(string filePath);
    Task ProcessVideoAsync(string inputPath, string outputPath, ProcessingOptions options);
}

public interface IWorkAreaFactory
{
    Task<WorkArea> CreateAsync(Guid id);
}

public class VideoMetadata
{
    public TimeSpan Duration { get; set; }
    public string Codec { get; set; }
    public string Resolution { get; set; }
    public double Framerate { get; set; }
    // Add other relevant properties
}

public class ProcessingOptions
{
    public string OutputFormat { get; set; }
    public string EncodingSettings { get; set; }
    public Dictionary<string, string> Filters { get; set; }
    // Add other processing options
}
```

## Migration Steps for VideoBatch

1. Back up existing Project panel implementation
2. Add AcrylicUI NuGet package if not already present
3. Create new dock window classes
4. Integrate dock windows with existing project structure
5. Test panel interactions with video files
6. Implement video processing chain functionality
7. Add logging and output display features 

## Panel Layout Configuration

The docking system uses a tabbed interface for panels sharing the same dock area. Here's how the panels should be configured:

```csharp
// Configure panel sizes after adding tool windows
DockPanel.Regions[DockArea.Bottom].Size = new System.Drawing.Size(0, 150); // Height for tabbed panels
DockPanel.Regions[DockArea.Left].Size = new System.Drawing.Size(218, 0);   // Width for Project panel
DockPanel.Regions[DockArea.Right].Size = new System.Drawing.Size(250, 0);  // Width for Batch Processing
```

Panel layout specifications:
1. Project Panel
   - Dock Area: Left
   - Width: 218 pixels
   - Full height
   - Can share space with other left-docked panels

2. Batch Processing Panel
   - Dock Area: Right
   - Width: 250 pixels
   - Full height

3. Media Inspector Panel
   - Dock Area: Bottom
   - Height: 150 pixels
   - Appears as a tab in the bottom panel region

4. Output Panel
   - Dock Area: Bottom
   - Height: 150 pixels
   - Appears as another tab alongside Media Inspector

Note: When multiple panels share the same dock area (like Media Inspector and Output panels in the bottom area), they will appear as tabs that can be selected to bring each panel to the front. This is the standard behavior of the AcrylicUI docking system and provides a clean way to switch between panels in the same region. 