# Programming Learning App - CodeCat

A WPF-based visual programming environment designed to teach basic programming concepts through **dual-mode programming** (text and visual blocks) and pathfinding exercises.

## Overview

CodeCat is an educational application that allows users to create, execute, and visualize programs that control a character navigating through grid-based exercises. The app supports both **text-based** and **block-based programming**, making it accessible for beginners while powerful enough for advanced users.

## Key Features

- **🎨 Dual Programming Modes**: Switch seamlessly between text-based and visual block-based programming
- **🧩 Visual Block Editor**: Drag-and-drop interface for creating programs without syntax errors
- **📝 Text Code Editor**: Traditional text-based programming with real-time syntax validation
- **🎯 Visual Grid-Based Exercises**: Load and solve pathfinding challenges on customizable grids
- **▶️ Real-Time Execution**: See the character navigate the grid as the program executes
- **📊 Program Metrics**: Calculate complexity metrics (command count, nesting level, repeat count)
- **💾 Multiple Export Formats**: Export programs to Text, JSON, or HTML formats
- **📚 Built-in Examples**: Pre-loaded basic, advanced, and expert example programs
- **🔍 Comprehensive Logging**: All actions are logged for debugging and analysis
- **🖥️ Interactive UI**: Full WPF interface with tabbed editor, menu system, and grid visualization

## Project Structure

```
ProgrammingLearningApp/
├── ProgrammingLearningApp/
│   ├── Models/
│   │   ├── Character.cs                    # Character with position, direction, and path tracking
│   │   ├── Position.cs                     # 2D position with X, Y coordinates
│   │   ├── Direction.cs                    # Cardinal directions (North, East, South, West)
│   │   ├── Cell.cs                         # Individual grid cell (walkable, blocked, end position)
│   │   ├── Grid.cs                         # Grid system with validation and navigation
│   │   ├── PathfindingExercise.cs          # Exercise with grid, start/end positions
│   │   ├── Condition.cs                    # Conditions for RepeatUntil (WallAhead, GridEdge)
│   │   ├── ExecutionResult.cs              # Result of program execution (legacy)
│   │   └── ProgramMetrics.cs               # Program complexity metrics
│   ├── Commands/
│   │   ├── ICommand.cs                     # Command interface
│   │   ├── MoveCommand.cs                  # Move forward N steps
│   │   ├── TurnCommand.cs                  # Turn left or right
│   │   ├── RepeatCommand.cs                # Repeat commands N times
│   │   └── RepeatUntilCommand.cs           # Repeat until condition is met
│   ├── Core/
│   │   └── Program.cs                      # Program container with commands and metrics
│   ├── Services/
│   │   ├── ProgramFactory.cs               # Factory for creating example programs
│   │   ├── ProgramImporter.cs              # Import programs from text files
│   │   ├── ProgramRunner.cs                # Execute programs with or without grids
│   │   ├── GridFileParser.cs               # Parse grid exercise files
│   │   ├── IProgramExporter.cs             # Exporter interface
│   │   ├── Exporters/
│   │   │   ├── TextProgramExporter.cs      # Export to .txt format
│   │   │   ├── JsonProgramExporter.cs      # Export to .json format
│   │   │   └── HtmlProgramExporter.cs      # Export to .html format (styled)
│   │   └── Logger/
│   │       ├── ILogger.cs                  # Logger interface
│   │       ├── FileLogger.cs               # File-based logging implementation
│   │       └── LoggingService.cs           # Singleton logging service
│   ├── Exceptions/
│   │   ├── OutOfBoundsException.cs         # Thrown when character moves outside grid
│   │   └── BlockedCellException.cs         # Thrown when character moves into blocked cell
│   ├── UI/
│   │   ├── MainWindow.xaml                 # WPF main window layout with tabbed editor
│   │   ├── MainWindow.xaml.cs              # Main window code-behind with full UI logic
│   │   └── Controls/
│   │       ├── BlockEditor.xaml            # Visual block-based programming editor
│   │       ├── BlockEditor.xaml.cs         # Block editor logic and conversion
│   │       ├── CommandBlockControl.xaml    # Individual command block UI component
│   │       └── CommandBlockControl.xaml.cs # Command block logic and drag-drop
│   ├── App.xaml                            # WPF application definition
│   ├── App.xaml.cs                         # Application startup and error handling
│   └── ProgrammingLearningApp.csproj       # .NET 9 WPF project file
├── ProgrammingLearningApp.Tests/
│   ├── CharacterTests.cs                   # Character movement and rotation tests
│   ├── GridTests.cs                        # Grid validation and navigation tests
│   ├── CellTests.cs                        # Cell state tests
│   ├── PathfindingExerciseTests.cs         # Exercise validation tests
│   ├── RepeatUntilCommandTests.cs          # Conditional repeat tests
│   ├── CustomExceptionTests.cs             # Exception handling tests
│   ├── ProgramRunnerTests.cs               # Program execution tests
│   ├── GridFileParserTests.cs              # Grid file parsing tests
│   ├── ExporterTests.cs                    # Export functionality tests
│   ├── LoggingTests.cs                     # Logging system tests
│   ├── MoveCommandTests.cs                 # Move command tests
│   ├── TurnCommandTests.cs                 # Turn command tests
│   ├── RepeatCommandTests.cs               # Repeat command tests
│   ├── ProgramTests.cs                     # Program metrics tests
│   └── ProgramFactoryTests.cs              # Factory pattern tests
├── SampleExercises/
│   ├── exercise1_simple.txt                # Simple L-shaped path
│   ├── exercise2_corridor.txt              # Straight corridor navigation
│   ├── exercise3_maze.txt                  # Complex maze challenge
│   ├── exercise4_spiral.txt                # Spiral pattern navigation
│   └── exercise5_zigzag.txt                # Zigzag pattern navigation
├── SamplePrograms/
│   ├── solution_exercise1.txt              # Solution for exercise 1
│   ├── solution_exercise2.txt              # Solution for exercise 2
│   ├── solution_exercise3.txt              # Solution for exercise 3
│   └── complex_example.txt                 # Complex nested repeat example
├── Docs/
│   ├── Lab3Report.tex                      # LaTeX report document
│   ├── ManualTestResults.xlsx              # Manual testing documentation
│   ├── TestScenarios.md                    # Test scenario descriptions
│   └── uml_diagrams/
│       ├── domain_model.png                # Domain model UML diagram
│       ├── ui_components.png               # UI architecture diagram
│       └── services_diagram.png            # Services layer diagram
├── LOGGING_GUIDE.md                        # Comprehensive logging documentation
└── README.md                               # This file
```

## Technology Stack

- **.NET 9** (net9.0-windows)
- **WPF** (Windows Presentation Foundation)
- **xUnit** (Testing framework)
- **C# 12** with nullable reference types enabled

## How to Build and Run

### Prerequisites
- Visual Studio 2022 (17.8 or later) OR .NET 9 SDK
- Windows OS (WPF requirement)

### Building the Project

#### Using Command Line
```bash
# Navigate to the solution directory
cd ProgrammingLearningApp

# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run the application
dotnet run --project ProgrammingLearningApp
```

#### Using Visual Studio
1. Open the solution file (`ProgrammingLearningApp.sln`)
2. Press `F5` or click "Start" to build and run
3. Or right-click the project and select "Build"

### Running Tests

```bash
# Run all tests
dotnet test

# Run tests with detailed output
dotnet test --verbosity normal

# Run tests with coverage (requires coverage tool)
dotnet test /p:CollectCoverage=true
```

## Usage Guide

### Main Window Interface

The application features a modern WPF interface with:

1. **Menu Bar**
   - **File Menu**: Load Program, Load Examples (Basic/Advanced/Expert), Export Program, Exit
   - **Exercise Menu**: Load Exercise, Clear Exercise
   - **Help Menu**: About

2. **Editor Panel** (Left Side) - **NEW: Tabbed Interface**
   - **Text Tab**: Traditional text editor with syntax validation and real-time feedback
   - **Blocks Tab**: Visual drag-and-drop block programming interface
   - Seamless switching between programming modes

3. **Control Panel** (Below Editor)
   - **▶ Run**: Execute the program from either text or blocks
   - **📊 Metrics**: Calculate program complexity metrics
   - **🗑 Clear**: Clear the editor and output

4. **Grid Canvas** (Center Right)
   - Visual representation of the exercise grid
   - Shows character position and path
   - Color-coded cells:
     - White: Walkable cells
     - Dark Gray: Blocked cells
     - Light Green: End position
     - Blue marker: Start position (S)
     - Green marker: End position (E)

5. **Output Panel** (Bottom Right)
   - Execution trace
   - Final character state
   - Success/failure messages
   - Error messages

### Getting Started

#### Option 1: Use Visual Block Programming (Perfect for Beginners!)
1. Switch to the **Blocks** tab in the Program Editor
2. Drag command blocks from the left palette to the work area on the right:
   - 🟢 **Move** - Move forward N steps (editable)
   - 🔵 **Turn Left** - Rotate 90° counterclockwise
   - 🟠 **Turn Right** - Rotate 90° clockwise
   - 🟣 **Repeat** - Repeat commands N times (drag blocks inside)
   - 🟪 **Repeat Until** - Repeat until condition is met
3. For container blocks (Repeat), drag child blocks into the container area
4. Click parameter values to edit them (e.g., number of steps or repetitions)
5. Click **▶ Run** to execute your block program
6. Switch to the **Text** tab anytime to see the text representation!

#### Option 2: Run a Pre-built Example
1. Click **File → Load Basic Example**
2. Click **▶ Run** to execute the program
3. View the execution trace in the output panel
4. Switch between **Text** and **Blocks** tabs to see both representations

#### Option 3: Load and Solve an Exercise
1. Click **Exercise → Load Exercise**
2. Select an exercise file from `SampleExercises/` (e.g., `exercise1_simple.txt`)
3. The grid will be displayed on the canvas
4. Create your solution using **Text** or **Blocks** mode
5. Click **▶ Run** to execute and see the character navigate the grid
6. Program succeeds if the character reaches the green end position

#### Option 4: Write Text-Based Programs
1. Switch to the **Text** tab
2. Type your program directly in the editor
3. The status bar shows real-time syntax validation
4. Click **▶ Run** to execute

### Block-Based Programming

#### Available Command Blocks

The visual block editor provides color-coded, draggable blocks:

1. **🟢 Move Block** (Green)
   - Move forward N steps in the current direction
   - Click the number to change the step count
   - Default: 1 step

2. **🔵 Turn Left Block** (Blue)
   - Rotate 90 degrees counterclockwise
   - No parameters needed

3. **🟠 Turn Right Block** (Orange)
   - Rotate 90 degrees clockwise
   - No parameters needed

4. **🟣 Repeat Block** (Purple)
   - Repeat commands N times
   - **Container block** - drag other blocks inside
   - Click the number to change repetition count
   - Supports nested repeats
   - Default: 2 times

5. **🟪 Repeat Until Block** (Dark Purple)
   - Repeat commands until a condition is met
   - **Container block** - drag other blocks inside
   - Currently uses "WallAhead" condition
   - Supports nested loops

#### Using the Block Editor

**Creating Programs:**
1. Drag blocks from the left **palette** to the right **work area**
2. Blocks snap into place vertically
3. Each block shows an **✕** button to delete it

**Working with Container Blocks:**
1. Drag a Repeat or Repeat Until block to the work area
2. The block shows a shaded container area with "Drop blocks here"
3. Drag child blocks into this container area
4. Child blocks can include other Repeat blocks (nesting)
5. Delete child blocks individually with their ✕ buttons

**Editing Parameters:**
- Click on any number in a block to edit it
- Type a new value and press Enter or click away
- Changes take effect immediately

**Managing Your Program:**
- Use the **Clear All** button to remove all blocks
- Delete individual blocks with their ✕ button
- Blocks are automatically organized vertically

**Switching Modes:**
- Switch to the **Text** tab to see the code representation
- Switch back to **Blocks** to continue visual editing
- Programs automatically sync between modes!

### Text-Based Programming Syntax

Programs use a simple text-based format with indentation for nested commands.

#### Basic Commands

**Move Command**
```
Move 5
```
Moves the character forward 5 steps in the current direction.

**Turn Command**
```
Turn left
Turn right
```
Rotates the character 90 degrees.

#### Repeat Command
```
Repeat 3 times
    Move 2
    Turn right
```
Repeats the indented commands 3 times. Can be nested.

#### RepeatUntil Command (Conditional)
```
RepeatUntil WallAhead
    Move 1

RepeatUntil GridEdge
    Move 1
    Turn right
```
Repeats commands until a condition is met:
- **WallAhead**: Continues until there's a wall or blocked cell ahead
- **GridEdge**: Continues until reaching the edge of the grid

#### Formatting Rules
- Commands must be capitalized: `Move`, `Turn`, `Repeat`, `RepeatUntil`
- Use **4 spaces** for each indentation level (not tabs)
- Nested commands must be indented under their parent
- Empty lines are ignored

### Example Programs

#### Simple Rectangle (Text)
```
Move 10
Turn right
Move 10
Turn right
Move 10
Turn right
Move 10
Turn right
```

**In Blocks:** Drag 4 Move blocks and 4 Turn Right blocks alternating.

#### Rectangle with Repeat (Text)
```
Repeat 4 times
    Move 10
    Turn right
```

**In Blocks:** Drag one Repeat block, set it to 4, then drag Move (10) and Turn Right into it.

#### Nested Repeats (Text)
```
Repeat 3 times
    Move 5
    Turn right
    Repeat 2 times
        Move 2
        Turn left
```

**In Blocks:** Drag a Repeat block, add Move and Turn Right to it, then drag another Repeat block inside, and add Move and Turn Left to the inner repeat.

#### Conditional Movement (Text)
```
RepeatUntil WallAhead
    Move 1
Turn right
RepeatUntil GridEdge
    Move 1
```

**In Blocks:** Drag a Repeat Until block, add Move (1) to it, then drag Turn Right, then another Repeat Until with Move (1).

### Grid Exercise Format

Grid files use simple ASCII characters:

```
oo+++
+oo++
++o++
++o++
++oox
```

- `o`: Walkable path
- `+`: Blocked cell (wall)
- `x`: End position (goal)
- Start position is always top-left corner by default

The character always starts facing **North** (up).

### Exporting Programs

1. Load or create a program (in either Text or Blocks mode)
2. Click **File → Export Program**
3. In the Save dialog, choose your desired format from the "Save as type" dropdown:
   - **Text files (*.txt)**: Original program format
   - **JSON files (*.json)**: Structured JSON with nested commands
   - **HTML files (*.html)**: Styled HTML with syntax highlighting
4. Enter a filename and click Save

The exporter will automatically use the appropriate format based on the file extension you select.

### Program Metrics

Click the **📊 Metrics** button (works from both Text and Blocks tabs) to see:

1. **Total Commands**: Number of all commands (including Repeat itself)
2. **Max Nesting Level**: Deepest level of nested Repeat commands
3. **Repeat Count**: Total number of Repeat/RepeatUntil commands

Example:
```
Repeat 2 times          ← Repeat #1, Level 1
    Move 1
    Repeat 3 times      ← Repeat #2, Level 2
        Turn left
```
- Total Commands: 4 (2 Repeats + 1 Move + 1 Turn)
- Max Nesting Level: 2
- Repeat Count: 2

## Design Patterns

The application demonstrates several software design patterns:

### 1. **Composite Pattern** (Commands)
- `ICommand` interface allows uniform treatment of simple and composite commands
- `MoveCommand`, `TurnCommand` are leaf nodes
- `RepeatCommand`, `RepeatUntilCommand` are composite nodes containing other commands
- Enables recursive execution and metrics calculation

### 2. **Factory Pattern** (Program Creation)
- `ProgramFactory` encapsulates creation of example programs
- Provides `CreateBasicProgram()`, `CreateAdvancedProgram()`, `CreateExpertProgram()`
- Centralizes program creation logic

### 3. **Strategy Pattern** (Exporters)
- `IProgramExporter` interface defines export contract
- Multiple implementations: `TextProgramExporter`, `JsonProgramExporter`, `HtmlProgramExporter`
- Easy to add new export formats without modifying existing code

### 4. **Singleton Pattern** (Logging)
- `LoggingService` uses singleton to provide global access to logging
- Ensures single log file and consistent logging throughout application

### 5. **Template Method Pattern** (Command Execution)
- Base command execution pattern defined in `ICommand`
- Each command implements its own execution logic
- Metrics calculation follows same pattern across all commands

### 6. **Model-View-ViewModel (MVVM) Elements**
- Separation between UI (`BlockEditor`, `CommandBlockControl`) and logic
- Bidirectional data binding between text and block representations
- Event-driven architecture for UI updates

## Block-Based Programming Benefits

### Educational Advantages
- **No Syntax Errors**: Drag-and-drop eliminates typing mistakes
- **Visual Learning**: See program structure at a glance
- **Immediate Feedback**: Visual blocks update in real-time
- **Scaffolded Learning**: Start with blocks, graduate to text
- **Concept Focus**: Learn programming logic without syntax overhead

### Technical Implementation
- **Bidirectional Conversion**: Seamlessly switch between text and blocks
- **Real-time Synchronization**: Changes in one mode reflect in the other
- **Full Feature Parity**: All commands available in both modes
- **Nested Structure Support**: Container blocks support unlimited nesting
- **Type Safety**: Block-based editing prevents invalid command structures

### Workflow Flexibility
- Beginners can start with visual blocks and never touch code
- Intermediate users can switch between modes as needed
- Advanced users can prototype visually then refine in text
- Teachers can demonstrate concepts in the mode that works best
- Programs can be shared and opened in either mode

## Logging System

The application includes comprehensive logging throughout:

### Log Locations

Logs are automatically saved to:
```
%AppData%\ProgrammingLearningApp\Logs\app_YYYYMMDD.log
```

### What Gets Logged

- **Application lifecycle**: Startup, shutdown, crashes
- **User actions**: All menu clicks, button presses, dialog interactions, tab switches
- **File operations**: Loading/exporting programs and exercises
- **Program execution**: Execution start/end, commands, success/failure
- **Block operations**: Block creation, deletion, parameter changes
- **Mode switching**: Text-to-blocks and blocks-to-text conversions
- **Errors**: All exceptions with full stack traces

### Log Levels

- `DEBUG`: Detailed diagnostic information
- `INFO`: General informational messages
- `WARNING`: Warning conditions (e.g., user error)
- `ERROR`: Error conditions with exceptions

See [LOGGING_GUIDE.md](LOGGING_GUIDE.md) for complete documentation.

## Error Handling

The application gracefully handles various error conditions:

### Custom Exceptions

1. **OutOfBoundsException**: Thrown when character tries to move outside grid boundaries
2. **BlockedCellException**: Thrown when character tries to move into a blocked cell

### Execution Results

- **Success**: Program completed and reached end position (if exercise loaded)
- **Failure**: Program completed but didn't reach end position
- **RuntimeError**: Exception occurred during execution (out of bounds, blocked cell, infinite loop)

### Safety Features

- Maximum iteration limit (10,000) for RepeatUntil to prevent infinite loops
- Grid boundary validation before movement
- Blocked cell detection
- Real-time syntax validation in text editor
- Block-based mode prevents invalid command structures
- Graceful handling of conversion errors between modes

## Testing

The project includes comprehensive unit tests:

- **15 test files** covering all major components
- **xUnit** testing framework
- Tests for:
  - All command types and execution
  - Grid navigation and validation
  - Character movement and rotation
  - Program metrics calculation
  - File parsing and importing
  - Export functionality
  - Exception handling
  - Logging system

Run tests with:
```bash
dotnet test
```

## Sample Files

### Exercises

Located in `SampleExercises/`:
- `exercise1_simple.txt`: Basic L-shaped path
- `exercise2_corridor.txt`: Long corridor
- `exercise3_maze.txt`: Complex maze with multiple turns
- `exercise4_spiral.txt`: Spiral pattern
- `exercise5_zigzag.txt`: Zigzag navigation

### Programs

Located in `SamplePrograms/`:
- `solution_exercise1.txt`: Solution for first exercise
- `solution_exercise2.txt`: Solution for corridor exercise
- `solution_exercise3.txt`: Solution for maze exercise
- `complex_example.txt`: Advanced nested repeat example

All sample programs can be loaded in either Text or Blocks mode!

## Future Enhancements

Potential areas for expansion:
- **Block Editor Enhancements**:
  - Condition selector for RepeatUntil blocks (dropdown UI)
  - Block categories and search functionality
  - Color themes for blocks
  - Block animations during execution
  - Save/load block layouts as images
- **Programming Features**:
  - Additional conditions for RepeatUntil (e.g., EndReached, ColorDetected)
  - Variable support for storing values
  - Conditional if/else blocks
  - Function/procedure definition blocks
- **UI Improvements**:
  - Visual step-by-step debugger with breakpoints
  - Animated execution with speed control
  - Split-view showing both text and blocks simultaneously
  - Undo/redo functionality for block operations
  - Block palette customization
- **Content**:
  - More built-in exercises with difficulty levels
  - Custom grid editor for creating exercises
  - Achievement system for completed exercises
  - Tutorial mode with guided lessons
- **Export/Import**:
  - More export formats (XML, YAML, Python, Scratch)
  - Import from other block-based languages
  - Share programs via QR code or URL

## Authors

- Tijmen de Graaf
- Ilyas Mallah