# Animation System Guide

## Overview

The CodeCat application now includes a comprehensive animation system that allows you to see your program execute step-by-step. Watch the character move through the grid one step at a time, making it easier to understand program flow and debug issues.

## Features

### Step-by-Step Execution
- View each command being executed one at a time
- See the character move and turn in real-time
- Track position and direction at each step

### Playback Controls
- **Play (▶)**: Start automatic animation playback
- **Pause (⏸)**: Pause the animation at the current step
- **Stop (⏹)**: Stop and reset to the beginning
- **Step Forward (⏭)**: Manually advance to the next step
- **Step Backward (⏮)**: Go back to the previous step

### Speed Control
- Adjust animation speed from 100ms to 2000ms per step
- Slider control for easy speed adjustment
- Real-time speed display in milliseconds

### Progress Tracking
- Visual progress bar showing current step
- Step counter (e.g., "Step: 5 / 20")
- Current command description displayed below controls

### Custom Character Sprites
- Load your own character images for each direction
- Automatic sprite rotation based on direction
- Fallback to default rendering if no sprites are found

## How to Use

### Running a Program with Animation

1. **Load an Exercise**: 
   - Click `Exercise → Load Exercise` to open a grid file
   - The grid visualization will appear on the right

2. **Write or Load a Program**:
   - Type your program in the Text editor, or
   - Use the Blocks editor for visual programming, or
   - Load an example program from the File menu

3. **Execute the Program**:
   - Click the `▶ Run` button
   - The program will execute and animation controls will appear
   - The character will be positioned at the starting location

4. **Control the Animation**:
   - Click `▶ Play` to start automatic playback
   - Use `⏸ Pause` to pause at any time
   - Click `⏭` or `⏮` to step through manually
   - Drag the progress slider to jump to any step
   - Adjust the speed slider for faster or slower playback

### Understanding Animation Steps

Each animation step represents one atomic action:
- **Move commands** are broken down into individual steps (Move 3 becomes 3 steps)
- **Turn commands** show as single rotation steps
- **Repeat loops** show each iteration separately
- **RepeatUntil loops** show each cycle until the condition is met

### Using Custom Character Sprites

#### Sprite File Requirements

Place your character sprite images in the sprites folder with these naming conventions:

```
Assets/Character/
├── Up/
│   ├── 1.png
│   ├── 2.png
│   ├── 3.png
│   └── ...
├── Down/
│   ├── 1.png
│   ├── 2.png
│   └── ...
├── Left/
│   ├── 1.png
│   ├── 2.png
│   └── ...
└── Right/
    ├── 1.png
    ├── 2.png
    └── ...
```

**Supported formats:**
- PNG (recommended for transparency)
- JPG/JPEG

#### Loading Sprites

**Method 1: Automatic Loading**
1. Place sprite files in the correct directory (see above)
2. Restart the application
3. Sprites will be automatically loaded

**Method 2: Using the Menu**
1. Click `Animation → Open Frames Folder`
2. The sprites directory will open in Windows Explorer
3. Copy your sprite images to the appropriate subfolders
4. Click `Animation → Reload Frames` to load them

**Method 3: Reload After Changes**
1. Click `Animation → Reload Frames`
2. A message will show how many frames were loaded per direction
3. No need to restart the application

#### Sprite Design Tips

- **Square images** work best (e.g., 64x64, 128x128 pixels)
- **Design directional sprites** - each direction folder should have frames facing that direction
- **Use transparency** (PNG with alpha channel) for best results
- **Keep it simple** - the sprite will be scaled to fit the grid cell
- **High contrast** makes the character easier to see on the grid
- **Sequential frames** - number your frames 1.png, 2.png, 3.png for smooth animation

**Frame orientation:**
- Up folder: Character facing upward ↑
- Down folder: Character facing downward ↓
- Left folder: Character facing left ←
- Right folder: Character facing right →

### Animation Status Indicators

The status bar at the bottom shows animation information:

- **"No animation"**: No program has been run yet
- **"Animation ready (N steps)"**: Animation loaded and ready to play
- **"Playing animation..."**: Animation is currently playing
- **"Animation paused"**: Animation paused at current step
- **"Animation stopped"**: Animation stopped and reset
- **"Animation completed"**: All steps have been played

### Viewing Without Animation

If you prefer to see the complete path without animation:
- The path will still be drawn on the grid after execution
- Animation controls will only appear after running a program with an exercise loaded
- You can hide animation controls by clicking "Clear"

## Technical Details

### Animation Architecture

The animation system consists of three main components:

1. **AnimationController** (`Services/AnimationController.cs`)
   - Manages animation state (Playing, Paused, Stopped, Completed)
   - Stores all animation steps
   - Provides events for step changes
   - Controls playback speed

2. **AnimatedProgramRunner** (`Services/AnimatedProgramRunner.cs`)
   - Extends `ProgramRunner` to capture execution steps
   - Breaks down Move commands into individual steps
   - Records position and direction at each step
   - Generates `AnimationStep` objects

3. **CharacterSpriteLoader** (`Services/CharacterSpriteLoader.cs`)
   - Loads sprite images from disk
   - Caches loaded sprites for performance
   - Supports multiple sprite naming conventions
   - Handles missing sprites gracefully

### Animation Step Structure

Each `AnimationStep` contains:
- **Position**: Grid coordinates (X, Y)
- **Direction**: Character facing direction (North, East, South, West)
- **CommandDescription**: Human-readable description of the command
- **StepNumber**: Sequential step index (0-based)

## Credits

last-tick on itch.io for the assets for the default character sprite used in this application.
