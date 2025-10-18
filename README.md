# Programming Learning App - Lab 2

## Project Structure

```
ProgrammingLearningApp/
├── ProgrammingLearningApp/           # Main project
│   ├── Models/
│   │   ├── Character.cs
│   │   ├── Position.cs
│   │   ├── Direction.cs
│   │   ├── ExecutionResult.cs
│   │   └── ProgramMetrics.cs
│   ├── Commands/
│   │   ├── ICommand.cs
│   │   ├── MoveCommand.cs
│   │   ├── TurnCommand.cs
│   │   └── RepeatCommand.cs
│   ├── Core/
│   │   └── Program.cs
│   ├── Services/
│   │   ├── ProgramFactory.cs
│   │   └── ProgramImporter.cs
│   ├── UI/
│   │   └── CommandLineInterface.cs
│   └── Program.Main.cs               # Entry point
├── ProgrammingLearningApp.Tests/     # Test project
│   ├── CharacterTests.cs
│   ├── MoveCommandTests.cs
│   ├── TurnCommandTests.cs
│   ├── RepeatCommandTests.cs
│   ├── ProgramTests.cs
│   └── ProgramFactoryTests.cs
├── SamplePrograms/                    # Sample program files
│   ├── rectangle.txt
│   ├── spiral.txt
│   └── stairs.txt
└── README.md
```

## How to Build and Run

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
1. Open the solution file (`.sln`)
2. Press F5 or click "Start" to build and run
3. Or right-click the project and select "Build"

### Running Tests

```bash
# Run all tests
dotnet test

# Run tests with detailed output
dotnet test --verbosity normal

# Run tests with coverage
dotnet test /p:CollectCoverage=true
```

## Usage

When you run the application, you'll be presented with a command-line interface:

### Step 1: Select a Program
1. **Basic example** - Simple rectangle without repeats
2. **Advanced example** - Rectangle using repeat command
3. **Expert example** - Complex program with nested repeats
4. **Import from file** - Load a program from a text file

### Step 2: Choose Action
1. **Execute** - Run the program and see the trace + final state
2. **Calculate metrics** - View command count, nesting level, and repeat count

## Sample Program Format

Programs should follow this exact syntax:

```
Move 10
Turn right
Move 5
Turn left
Repeat 3 times
    Move 2
    Turn right
    Repeat 2 times
        Move 1
        Turn left
```

**Important formatting rules:**
- Commands must be capitalized exactly as shown
- Use 4 spaces for each indentation level
- "Repeat X times" must be followed by indented commands
- Valid commands: `Move [number]`, `Turn left`, `Turn right`, `Repeat [number] times`

## Sample Program Files

### rectangle.txt
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

### spiral.txt
```
Repeat 8 times
    Move 5
    Turn right
    Move 5
    Turn right
    Move 5
    Turn right
    Move 5
    Turn right
    Move 2
```

### stairs.txt
```
Repeat 5 times
    Move 3
    Turn left
    Move 2
    Turn right
```

## Design Patterns Used

1. **Composite Pattern**
   - Allows treating individual commands and command groups uniformly
   - `ICommand` interface with `MoveCommand`, `TurnCommand` (leaves) and `RepeatCommand` (composite)

2. **Factory Pattern**
   - Encapsulates creation of example programs
   - `ProgramFactory` provides methods for basic, advanced, and expert programs

3. **Strategy Pattern (prepared for future)**
   - `ProgramImporter` can be extended to support multiple file formats
   - Easy to add XML, JSON, or other importers

## Metrics Calculation

The system calculates three metrics:

1. **Command Count**: Total number of commands (including the repeat itself)
2. **Max Nesting Level**: Deepest level of nested repeats (0 = no repeats)
3. **Repeat Count**: Total number of repeat commands in the program

## Testing

The project includes comprehensive unit tests covering:
- Character movement and rotation
- All command types
- Program execution
- Metrics calculation
- Factory methods
- Edge cases and error handling

## Team Members

- Tijmen de Graaf
- Ilyas Mallah