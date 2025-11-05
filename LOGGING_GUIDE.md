# Logging Guide

## Where Logging is Active

### 1. **Application Lifecycle** (`App.xaml.cs`)
- Application startup
- Application shutdown
- Unhandled exceptions (both app domain and dispatcher)

### 2. **Program Execution** (`ProgramRunner.cs`)
- Program execution start/end
- Character initialization
- Exercise completion/failure
- Command execution (Repeat, RepeatUntil)
- Runtime errors (OutOfBounds, BlockedCell, etc.)
- Execution status

### 3. **File Operations** (`ProgramImporter.cs`)
- Program file import
- File not found errors
- Parsing each command
- Format errors

### 4. **User Interface** (`MainWindow.xaml.cs`)
- Window initialization
- All menu actions (Load, Export, Exit)
- Exercise loading
- Program execution from UI
- Metrics calculation
- All errors and exceptions

## Log File Location

Your logs are automatically saved to:
```
%AppData%\ProgrammingLearningApp\Logs\app_YYYYMMDD.log
```

## Log Format

Each log entry includes:
- **Timestamp**: Down to milliseconds
- **Log Level**: DEBUG, INFO, WARNING, ERROR
- **Message**: What happened

Example:
```
[2024-01-15 14:23:45.123] [INFO   ] === Application Starting ===
[2024-01-15 14:23:45.456] [INFO   ] MainWindow initialized
[2024-01-15 14:23:50.789] [INFO   ] User clicked Load Program
[2024-01-15 14:23:51.012] [INFO   ] Successfully loaded program: BasicExample
[2024-01-15 14:23:55.345] [INFO   ] User clicked Run Program
[2024-01-15 14:23:55.678] [INFO   ] Starting program execution: BasicExample
[2024-01-15 14:23:55.890] [INFO   ] Program execution completed with status: Success
```

## Log Levels

- **DEBUG**: Detailed information for troubleshooting
- **INFO**: General informational messages
- **WARNING**: Warning messages (e.g., user tried to run with no program loaded)
- **ERROR**: Error messages with exception details

## What Gets Logged

### Application Events
- Startup/shutdown
- Version information
- Unhandled exceptions

### User Actions
- Menu clicks
- Button clicks
- File dialogs (opened/cancelled)

### File Operations
- Loading programs
- Exporting programs
- Loading exercises
- File parsing

### Program Execution
- Execution start/end
- Character initialization
- Command execution
- Success/failure status
- Runtime errors
- Exception details

### Metrics
- Command count
- Nesting level
- Repeat count