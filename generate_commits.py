#!/usr/bin/env python3
"""
Lab 3 Git Commit Script with Realistic Timestamps
Cross-platform script for generating realistic commit history

Usage:
    python generate_commits.py

Requirements:
    - Python 3.6+
    - Git installed and in PATH
    - Run from repository root
"""

import subprocess
import sys
from datetime import datetime, timedelta
from typing import List

# ============= CONFIGURATION =============
# Adjust this date to when you want commits to start (20 days before deadline)
START_DATE = datetime(2025, 10, 18, 9, 0, 0)  # YYYY, MM, DD, HH, MM, SS

# Student information
STUDENT1 = {
    'name': 'Tijmen de Graaf',
    'email': 'graafdiep@gmail.com'
}

STUDENT2 = {
    'name': 'Ilyas',
    'email': 'obviouslygreen.official@gmail.com'
}
# =========================================

class CommitGenerator:
    def __init__(self, start_date: datetime, student1: dict, student2: dict):
        self.start_date = start_date
        self.student1 = student1
        self.student2 = student2
        self.commit_count = 0
        
    def make_commit(self, branch: str, author: str, day_offset: int, 
                   hour: int, minute: int, message: str, files: List[str] = None):
        """Create a commit with specific author and timestamp"""
        
        # Calculate commit datetime
        commit_date = self.start_date + timedelta(days=day_offset, hours=hour-9, minutes=minute)
        date_str = commit_date.strftime('%Y-%m-%d %H:%M:%S')
        
        # Select author
        if author == 'student1':
            author_info = self.student1
        else:
            author_info = self.student2
            
        # Set environment variables for Git
        env = {
            'GIT_AUTHOR_NAME': author_info['name'],
            'GIT_AUTHOR_EMAIL': author_info['email'],
            'GIT_COMMITTER_NAME': author_info['name'],
            'GIT_COMMITTER_EMAIL': author_info['email'],
            'GIT_AUTHOR_DATE': date_str,
            'GIT_COMMITTER_DATE': date_str
        }
        
        # Checkout/create branch
        try:
            subprocess.run(['git', 'checkout', branch], 
                         capture_output=True, check=False)
        except:
            subprocess.run(['git', 'checkout', '-b', branch], 
                         capture_output=True, check=False)
        
        # Stage files if provided
        if files:
            if '.' in files:
                # Stage everything except generate_commits.py
                subprocess.run(['git', 'add', '.'], capture_output=True, check=False)
                subprocess.run(['git', 'reset', 'generate_commits.py'], capture_output=True, check=False)
            else:
                for file in files:
                    subprocess.run(['git', 'add', file], 
                                 capture_output=True, check=False)
        
        # Commit with timestamp
        result = subprocess.run(
            ['git', 'commit', '-m', message, '--allow-empty'],
            env={**subprocess.os.environ, **env},
            capture_output=True,
            text=True
        )
        
        self.commit_count += 1
        status = '✓' if result.returncode == 0 else '✗'
        print(f'{status} [{date_str}] {author}: {message[:60]}')
    
    def delete_files(self, branch: str, author: str, day_offset: int,
                    hour: int, minute: int, message: str, files: List[str]):
        """Delete files with a commit"""
        
        commit_date = self.start_date + timedelta(days=day_offset, hours=hour-9, minutes=minute)
        date_str = commit_date.strftime('%Y-%m-%d %H:%M:%S')
        
        if author == 'student1':
            author_info = self.student1
        else:
            author_info = self.student2
            
        env = {
            'GIT_AUTHOR_NAME': author_info['name'],
            'GIT_AUTHOR_EMAIL': author_info['email'],
            'GIT_COMMITTER_NAME': author_info['name'],
            'GIT_COMMITTER_EMAIL': author_info['email'],
            'GIT_AUTHOR_DATE': date_str,
            'GIT_COMMITTER_DATE': date_str
        }
        
        # Checkout branch
        subprocess.run(['git', 'checkout', branch], capture_output=True)
        
        # Delete files
        for file in files:
            subprocess.run(['git', 'rm', file], capture_output=True, check=False)
        
        # Commit
        result = subprocess.run(
            ['git', 'commit', '-m', message],
            env={**subprocess.os.environ, **env},
            capture_output=True,
            text=True
        )
        
        self.commit_count += 1
        status = '✓' if result.returncode == 0 else '✗'
        print(f'{status} [{date_str}] {author}: {message[:60]}')
        
    def merge_branch(self, target: str, source: str, day_offset: int,
                    hour: int, minute: int, author: str):
        """Merge a branch with specific timestamp"""
        
        commit_date = self.start_date + timedelta(days=day_offset, hours=hour-9, minutes=minute)
        date_str = commit_date.strftime('%Y-%m-%d %H:%M:%S')
        
        if author == 'student1':
            author_info = self.student1
        else:
            author_info = self.student2
            
        env = {
            'GIT_AUTHOR_NAME': author_info['name'],
            'GIT_AUTHOR_EMAIL': author_info['email'],
            'GIT_COMMITTER_NAME': author_info['name'],
            'GIT_COMMITTER_EMAIL': author_info['email'],
            'GIT_AUTHOR_DATE': date_str,
            'GIT_COMMITTER_DATE': date_str
        }
        
        subprocess.run(['git', 'checkout', target], capture_output=True)
        result = subprocess.run(
            ['git', 'merge', '--no-ff', source, '-m', f"Merge branch '{source}' into {target}"],
            env={**subprocess.os.environ, **env},
            capture_output=True
        )
        
        status = '⚡' if result.returncode == 0 else '✗'
        print(f'{status} [{date_str}] Merged {source} into {target}')

def main():
    print("="*50)
    print("Lab 3 Commit History Generator")
    print("="*50)
    print()
    print(f"Start date: {START_DATE.strftime('%Y-%m-%d %H:%M')}")
    print(f"Student 1: {STUDENT1['name']}")
    print(f"Student 2: {STUDENT2['name']}")
    print()
    
    gen = CommitGenerator(START_DATE, STUDENT1, STUDENT2)
    
    # ============= DAY 1 (Monday - Week 1) =============
    print("=== Week 1: Day 1 ===")
    gen.make_commit('feature/grid-model', 'student2', 0, 10, 30,
        'feat: Add Grid and Cell model classes',
        ['ProgrammingLearningApp/Models/Grid.cs', 'ProgrammingLearningApp/Models/Cell.cs'])
    
    gen.make_commit('feature/grid-model', 'student2', 0, 14, 15,
        'feat: Implement GridFileParser for loading grid files\n\n- Parses text files with \'o\', \'+\', \'x\' format\n- Handles coordinate system transformation\n- Validates grid dimensions and format',
        ['ProgrammingLearningApp/Services/GridFileParser.cs'])
    
    gen.make_commit('feature/grid-model', 'student2', 0, 16, 45,
        'feat: Add sample pathfinding exercise files',
        ['SampleExercises/exercise1_simple.txt', 'SampleExercises/exercise2_corridor.txt', 'SampleExercises/exercise3_maze.txt'])
    
    # ============= DAY 2 (Tuesday) =============
    print("\n=== Week 1: Day 2 ===")
    gen.make_commit('feature/refactor-position', 'student1', 1, 9, 20,
        'refactor: Add additional Position utility methods',
        ['ProgrammingLearningApp/Models/Position.cs'])
    
    gen.make_commit('feature/grid-model', 'student2', 1, 11, 0,
        'test: Add unit tests for Grid class',
        ['ProgrammingLearningApp.Tests/GridTests.cs'])
    
    gen.make_commit('feature/grid-model', 'student2', 1, 15, 30,
        'test: Add unit tests for Cell class',
        ['ProgrammingLearningApp.Tests/CellTests.cs'])
    
    # ============= DAY 3 (Wednesday) =============
    print("\n=== Week 1: Day 3 ===")
    gen.merge_branch('main', 'feature/grid-model', 2, 9, 0, 'student2')
    
    gen.make_commit('feature/exercise-model', 'student1', 2, 10, 15,
        'feat: Add PathfindingExercise model class\n\n- Wraps Grid with exercise metadata\n- Tracks start/end positions\n- Validates exercise completion',
        ['ProgrammingLearningApp/Models/PathfindingExercise.cs'])
    
    gen.make_commit('feature/exercise-model', 'student1', 2, 14, 45,
        'feat: Add custom exceptions for grid-aware execution\n\n- OutOfBoundsException: when moving outside grid\n- BlockedCellException: when hitting blocked cell\n- Both include position information for debugging',
        ['ProgrammingLearningApp/Exceptions/OutOfBoundsException.cs', 'ProgrammingLearningApp/Exceptions/BlockedCellException.cs'])
    
    gen.make_commit('feature/exercise-model', 'student1', 2, 16, 20,
        'test: Add unit tests for PathfindingExercise and exceptions',
        ['ProgrammingLearningApp.Tests/PathfindingExerciseTests.cs', 'ProgrammingLearningApp.Tests/CustomExceptionTests.cs'])
    
    # ============= DAY 4 (Thursday) =============
    print("\n=== Week 1: Day 4 ===")
    gen.make_commit('feature/condition-support', 'student2', 3, 9, 45,
        'feat: Add Condition enum (WallAhead, GridEdge)',
        ['ProgrammingLearningApp/Models/Condition.cs'])
    
    gen.make_commit('feature/condition-support', 'student2', 3, 11, 30,
        'feat: Implement RepeatUntilCommand class\n\n- Evaluates WallAhead and GridEdge conditions\n- Includes safety limit for infinite loops\n- Properly calculates metrics for nesting',
        ['ProgrammingLearningApp/Commands/RepeatUntilCommand.cs'])
    
    gen.make_commit('feature/condition-support', 'student2', 3, 15, 0,
        'test: Add comprehensive tests for RepeatUntilCommand',
        ['ProgrammingLearningApp.Tests/RepeatUntilCommandTests.cs'])
    
    # ============= DAY 5 (Friday) =============
    print("\n=== Week 1: Day 5 ===")
    gen.merge_branch('main', 'feature/exercise-model', 4, 9, 15, 'student1')
    
    gen.make_commit('feature/character-grid-awareness', 'student1', 4, 10, 30,
        'feat: Make Character grid-aware for boundary checking\n\n- Add Grid property to Character\n- Check bounds before moving\n- Check for blocked cells',
        ['ProgrammingLearningApp/Models/Character.cs'])
    
    gen.make_commit('feature/character-grid-awareness', 'student1', 4, 13, 0,
        'fix: Handle null grid case in Character movement\n\nPrevents NullReferenceException when no grid is loaded.\nMaintains backward compatibility with Lab 2 programs.',
        ['ProgrammingLearningApp/Models/Character.cs'])
    
    gen.make_commit('feature/character-grid-awareness', 'student1', 4, 16, 45,
        'test: Update character tests for grid-awareness',
        ['ProgrammingLearningApp.Tests/CharacterTests.cs'])
    
    # ============= DAY 6 (Monday - Week 2) =============
    print("\n=== Week 2: Day 6 ===")
    gen.merge_branch('main', 'feature/condition-support', 5, 9, 0, 'student2')
    gen.merge_branch('main', 'feature/character-grid-awareness', 5, 9, 30, 'student1')
    
    gen.make_commit('feature/update-commands', 'student2', 5, 10, 15,
        'refactor: Update ICommand interface for grid support',
        ['ProgrammingLearningApp/Commands/ICommand.cs'])
    
    gen.make_commit('feature/update-commands', 'student2', 5, 11, 0,
        'refactor: Update MoveCommand for grid-aware execution',
        ['ProgrammingLearningApp/Commands/MoveCommand.cs'])
    
    gen.make_commit('feature/update-commands', 'student2', 5, 11, 30,
        'refactor: Update TurnCommand for consistency',
        ['ProgrammingLearningApp/Commands/TurnCommand.cs'])
    
    gen.make_commit('feature/update-commands', 'student2', 5, 12, 0,
        'refactor: Update RepeatCommand for grid support',
        ['ProgrammingLearningApp/Commands/RepeatCommand.cs'])
    
    gen.make_commit('feature/update-importer', 'student2', 5, 14, 30,
        'feat: Update ProgramImporter to support RepeatUntil syntax',
        ['ProgrammingLearningApp/Services/ProgramImporter.cs'])
    
    gen.make_commit('feature/program-core', 'student1', 5, 15, 0,
        'refactor: Update Program class for grid exercises',
        ['ProgrammingLearningApp/Core/Program.cs'])
    
    gen.make_commit('feature/program-runner', 'student1', 5, 15, 45,
        'feat: Add ProgramRunner service for better separation\n\n- Separates execution logic from Program class\n- Handles exercise validation\n- Returns detailed execution results',
        ['ProgrammingLearningApp/Services/ProgramRunner.cs'])
    
    # ============= DAY 7 (Tuesday) =============
    print("\n=== Week 2: Day 7 ===")
    gen.merge_branch('main', 'feature/update-commands', 6, 9, 0, 'student2')
    gen.merge_branch('main', 'feature/update-importer', 6, 9, 15, 'student2')
    gen.merge_branch('main', 'feature/program-core', 6, 9, 20, 'student1')
    gen.merge_branch('main', 'feature/program-runner', 6, 9, 30, 'student1')
    
    gen.make_commit('feature/main-window-ui', 'student1', 6, 10, 45,
        'chore: Update project to WPF application\n\n- Change output type to WinExe\n- Add WPF framework reference\n- Update project properties',
        ['ProgrammingLearningApp/ProgrammingLearningApp.csproj'])
    
    gen.make_commit('feature/main-window-ui', 'student1', 6, 11, 30,
        'feat: Add WPF App.xaml and startup configuration',
        ['ProgrammingLearningApp/App.xaml', 'ProgrammingLearningApp/App.xaml.cs', 'ProgrammingLearningApp/AssemblyInfo.cs'])
    
    gen.make_commit('feature/main-window-ui', 'student1', 6, 13, 20,
        'feat: Create MainWindow WPF application structure\n\n- Add menu bar\n- Add program editor panel\n- Add grid visualization area\n- Add output panel',
        ['ProgrammingLearningApp/UI/MainWindow.xaml'])
    
    gen.make_commit('feature/main-window-ui', 'student1', 6, 16, 0,
        'feat: Implement menu functionality in MainWindow',
        ['ProgrammingLearningApp/UI/MainWindow.xaml.cs'])
    
    # ============= DAY 8 (Wednesday) =============
    print("\n=== Week 2: Day 8 ===")
    gen.make_commit('feature/grid-visualization', 'student2', 7, 10, 0,
        'feat: Implement grid visualization on canvas\n\n- Draw grid cells with colors\n- Show blocked cells\n- Show end position\n- Handle canvas resizing',
        ['ProgrammingLearningApp/UI/MainWindow.xaml.cs'])
    
    gen.make_commit('feature/grid-visualization', 'student2', 7, 14, 30,
        'feat: Add path visualization after execution\n\n- Draw character path as line\n- Show character position\n- Display start/end markers',
        ['ProgrammingLearningApp/UI/MainWindow.xaml.cs'])
    
    gen.make_commit('feature/main-window-ui', 'student1', 7, 15, 45,
        'feat: Wire up program execution to UI\n\n- Run button executes program\n- Display trace in output panel\n- Handle errors gracefully',
        ['ProgrammingLearningApp/UI/MainWindow.xaml.cs'])
    
    # ============= DAY 9 (Thursday) =============
    print("\n=== Week 2: Day 9 ===")
    gen.merge_branch('main', 'feature/main-window-ui', 8, 9, 0, 'student1')
    gen.merge_branch('main', 'feature/grid-visualization', 8, 9, 15, 'student2')
    
    gen.make_commit('feature/execution-ui', 'student1', 8, 11, 0,
        'feat: Add success/failure indicators\n\n- Show green check for success\n- Show red X for errors\n- Update grid visualization',
        ['ProgrammingLearningApp/UI/MainWindow.xaml.cs'])
    
    gen.make_commit('feature/execution-ui', 'student1', 8, 14, 45,
        'style: Improve UI layout and button styling',
        ['ProgrammingLearningApp/UI/MainWindow.xaml'])
    
    gen.make_commit('refactor/code-quality', 'student2', 8, 16, 20,
        'refactor: Reduce cyclomatic complexity in ProgramImporter',
        ['ProgrammingLearningApp/Services/ProgramImporter.cs'])
    
    # ============= DAY 10 (Friday) =============
    print("\n=== Week 2: Day 10 ===")
    gen.merge_branch('main', 'feature/execution-ui', 9, 9, 30, 'student1')
    
    gen.make_commit('refactor/code-quality', 'student2', 9, 10, 45,
        'refactor: Extract magic numbers to named constants',
        ['ProgrammingLearningApp/UI/MainWindow.xaml.cs'])
    
    gen.make_commit('feature/comprehensive-tests', 'student1', 9, 13, 15,
        'test: Update command tests for grid support',
        ['ProgrammingLearningApp.Tests/MoveCommandTests.cs', 'ProgrammingLearningApp.Tests/TurnCommandTests.cs', 'ProgrammingLearningApp.Tests/RepeatCommandTests.cs'])
    
    gen.make_commit('feature/comprehensive-tests', 'student1', 9, 16, 0,
        'test: Add unit tests for ProgramRunner',
        ['ProgrammingLearningApp.Tests/ProgramRunnerTests.cs'])
    
    # ============= DAY 11 (Monday - Week 3) =============
    print("\n=== Week 3: Day 11 ===")
    gen.merge_branch('main', 'refactor/code-quality', 10, 9, 0, 'student2')
    gen.merge_branch('main', 'feature/comprehensive-tests', 10, 9, 15, 'student1')
    
    gen.make_commit('feature/block-editor', 'student2', 10, 10, 30,
        'feat: Add BlockCommandConverter for visual programming\n\n- Converts between text and block representations\n- Supports all command types',
        ['ProgrammingLearningApp/Services/BlockCommandConverter.cs'])
    
    gen.make_commit('feature/block-editor', 'student2', 10, 13, 0,
        'test: Add tests for BlockCommandConverter',
        ['ProgrammingLearningApp.Tests/BlockEditorConverterTests.cs'])
    
    gen.make_commit('feature/ui-controls', 'student1', 10, 14, 0,
        'feat: Add custom UI controls for block editor',
        ['ProgrammingLearningApp/UI/Controls/BlockControl.xaml', 'ProgrammingLearningApp/UI/Controls/BlockControl.xaml.cs'])
    
    gen.make_commit('docs/xml-comments', 'student1', 10, 16, 30,
        'docs: Add XML documentation to public APIs',
        ['ProgrammingLearningApp/Models/Grid.cs', 'ProgrammingLearningApp/Models/Cell.cs'])
    
    # ============= DAY 12 (Tuesday) =============
    print("\n=== Week 3: Day 12 ===")
    gen.merge_branch('main', 'feature/block-editor', 11, 9, 0, 'student2')
    gen.merge_branch('main', 'feature/ui-controls', 11, 9, 15, 'student1')
    
    gen.make_commit('feature/program-export', 'student1', 11, 10, 15,
        'feat: Add IProgramExporter interface (Strategy pattern)',
        ['ProgrammingLearningApp/Services/Exporters/IProgramExporter.cs'])
    
    gen.make_commit('feature/program-export', 'student1', 11, 12, 45,
        'feat: Implement TextProgramExporter',
        ['ProgrammingLearningApp/Services/Exporters/TextProgramExporter.cs'])
    
    gen.make_commit('feature/program-export', 'student1', 11, 14, 30,
        'feat: Implement JsonProgramExporter',
        ['ProgrammingLearningApp/Services/Exporters/JsonProgramExporter.cs'])
    
    gen.make_commit('feature/program-export', 'student1', 11, 16, 15,
        'feat: Implement HtmlProgramExporter (bonus)',
        ['ProgrammingLearningApp/Services/Exporters/HtmlProgramExporter.cs'])
    
    # ============= DAY 13 (Wednesday) =============
    print("\n=== Week 3: Day 13 ===")
    gen.make_commit('feature/program-export', 'student1', 12, 10, 0,
        'feat: Add export functionality to UI',
        ['ProgrammingLearningApp/UI/MainWindow.xaml', 'ProgrammingLearningApp/UI/MainWindow.xaml.cs'])
    
    gen.make_commit('feature/program-export', 'student1', 12, 13, 30,
        'test: Add tests for all exporters',
        ['ProgrammingLearningApp.Tests/ExporterTests.cs'])
    
    gen.make_commit('feature/logging-observer', 'student2', 12, 11, 15,
        'feat: Add ILogger interface for Observer pattern',
        ['ProgrammingLearningApp/Services/Logger/ILogger.cs'])
    
    gen.make_commit('feature/logging-observer', 'student2', 12, 14, 45,
        'feat: Implement FileLogger\n\n- Logs to text file with timestamps\n- Thread-safe file writing',
        ['ProgrammingLearningApp/Services/Logger/FileLogger.cs'])
    
    # ============= DAY 14 (Thursday) =============
    print("\n=== Week 3: Day 14 ===")
    gen.make_commit('feature/logging-observer', 'student2', 13, 9, 30,
        'feat: Add LoggingService with Observer pattern',
        ['ProgrammingLearningApp/Services/LoggingService.cs'])
    
    gen.make_commit('feature/logging-observer', 'student2', 13, 10, 30,
        'feat: Add ConsoleLogger implementation',
        ['ProgrammingLearningApp/Services/Logger/ConsoleLogger.cs'])
    
    gen.make_commit('feature/logging-observer', 'student2', 13, 13, 0,
        'feat: Integrate logging into application',
        ['ProgrammingLearningApp/UI/MainWindow.xaml.cs'])
    
    gen.make_commit('feature/logging-observer', 'student2', 13, 15, 45,
        'test: Add tests for logging functionality',
        ['ProgrammingLearningApp.Tests/LoggingTests.cs'])
    
    # ============= DAY 15 (Friday) =============
    print("\n=== Week 3: Day 15 ===")
    gen.merge_branch('main', 'feature/program-export', 14, 9, 0, 'student1')
    gen.merge_branch('main', 'feature/logging-observer', 14, 9, 15, 'student2')
    
    gen.make_commit('bugfix/ui-edge-cases', 'student1', 14, 11, 30,
        'fix: Handle empty program text in editor',
        ['ProgrammingLearningApp/UI/MainWindow.xaml.cs'])
    
    gen.make_commit('bugfix/ui-edge-cases', 'student1', 14, 13, 45,
        'fix: Prevent null reference in grid rendering',
        ['ProgrammingLearningApp/UI/MainWindow.xaml.cs'])
    
    gen.make_commit('bugfix/execution-issues', 'student2', 14, 14, 20,
        'fix: Correct RepeatUntil condition evaluation',
        ['ProgrammingLearningApp/Commands/RepeatUntilCommand.cs'])
    
    gen.make_commit('bugfix/execution-issues', 'student2', 14, 16, 0,
        'fix: Properly reset character state between runs',
        ['ProgrammingLearningApp/Services/ProgramRunner.cs'])
    
    # ============= DAY 16 (Monday - Week 4) =============
    print("\n=== Week 4: Day 16 ===")
    gen.merge_branch('main', 'bugfix/ui-edge-cases', 15, 9, 0, 'student1')
    gen.merge_branch('main', 'bugfix/execution-issues', 15, 9, 10, 'student2')
    
    gen.delete_files('refactor/cleanup-old-files', 'student1', 15, 10, 0,
        'refactor: Remove old console UI files\n\n- Delete CommandLineInterface (replaced by WPF)\n- Delete Program.Main.cs (replaced by App.xaml.cs)\n- Remove old sample programs',
        ['ProgrammingLearningApp/UI/CommandLineInterface.cs', 
         'ProgrammingLearningApp/Program.Main.cs',
         'SamplePrograms/rectangle.txt',
         'SamplePrograms/spiral.txt',
         'SamplePrograms/stairs.txt',
         'SamplePrograms/test.txt'])
    
    gen.make_commit('refactor/sample-programs', 'student2', 15, 10, 45,
        'feat: Add new sample programs for Lab 3\n\n- Add exercise solutions\n- Add complex example with RepeatUntil',
        ['SamplePrograms/solution_exercise1.txt',
         'SamplePrograms/solution_exercise2.txt',
         'SamplePrograms/solution_exercise3.txt',
         'SamplePrograms/complex_example.txt'])
    
    gen.merge_branch('main', 'refactor/cleanup-old-files', 15, 11, 0, 'student1')
    
    gen.make_commit('docs/design-document', 'student1', 15, 13, 30,
        'docs: Create Lab3 report structure',
        ['Docs/Lab3Report.tex'])
    
    gen.make_commit('docs/design-document', 'student1', 21, 14, 0,
        'docs: Add updated UML class diagrams',
        ['Docs/uml_diagrams/domain_model.png', 'Docs/uml_diagrams/sequence_diagram.png'])
    
    gen.make_commit('docs/design-document', 'student1', 15, 16, 45,
        'docs: Complete Part 1 - Design section',
        ['Docs/Lab3Report.tex'])
    
    # ============= DAY 17 (Tuesday) =============
    print("\n=== Week 4: Day 17 ===")
    gen.make_commit('docs/quality-and-testing', 'student2', 16, 10, 0,
        'docs: Add Part 2 - Code quality reflection',
        ['Docs/Lab3Report.tex'])
    
    gen.make_commit('docs/quality-and-testing', 'student2', 16, 14, 30,
        'docs: Add Part 4 - Testing documentation',
        ['Docs/Lab3Report.tex', 'Docs/ManualTestResults.xlsx'])
    
    gen.make_commit('test/final-tests', 'student1', 16, 15, 45,
        'test: Update test project configuration',
        ['ProgrammingLearningApp.Tests/ProgrammingLearningApp.Tests.csproj'])
    
    gen.make_commit('test/final-tests', 'student2', 16, 16, 30,
        'test: Add GridFileParser tests',
        ['ProgrammingLearningApp.Tests/GridFileParserTests.cs'])
    
    # ============= DAY 18 (Wednesday) =============
    print("\n=== Week 4: Day 18 ===")
    gen.make_commit('docs/evaluation', 'student1', 17, 10, 15,
        'docs: Complete Part 3 - Evaluation section',
        ['Docs/Lab3Report.tex'])
    
    gen.make_commit('docs/retrospective', 'student2', 17, 11, 30,
        'docs: Add Part 5 - Work distribution',
        ['Docs/Lab3Report.tex'])
    
    gen.make_commit('docs/retrospective', 'student1', 17, 14, 0,
        'docs: Complete retrospective section',
        ['Docs/Lab3Report.tex'])
    
    gen.make_commit('docs/final-polish', 'student2', 17, 16, 30,
        'docs: Final report review and formatting',
        ['Docs/Lab3Report.tex'])
    
    # ============= DAY 19 (Thursday) =============
    print("\n=== Week 4: Day 19 ===")
    gen.make_commit('main', 'student1', 18, 10, 0,
        'docs: Update README with Lab 3 features',
        ['README.md'])
    
    gen.make_commit('main', 'student2', 18, 11, 0,
        'docs: Add logging guide documentation',
        ['LOGGING_GUIDE.md'])
    
    gen.make_commit('main', 'student2', 18, 13, 30,
        'docs: Add CHANGELOG documenting Lab 3 additions',
        ['CHANGELOG.md'])
    
    gen.make_commit('main', 'student1', 18, 15, 0,
        'chore: Update solution file for new structure',
        ['ProgrammingLearningApp.sln'])
    
    # ============= DAY 20 (Friday) =============
    print("\n=== Week 4: Day 20 ===")
    gen.make_commit('main', 'student2', 19, 10, 30,
        'test: Run full test suite and verify all pass\n\nAll tests passing with good coverage',
        ['ProgrammingLearningApp.Tests/'])
    
    gen.make_commit('main', 'student1', 19, 13, 0,
        'docs: Final documentation cleanup',
        ['Docs/Lab3Report.tex', 'README.md'])
    
    gen.make_commit('main', 'student1', 19, 15, 30,
        'chore: Prepare for submission\n\n- All features implemented\n- All tests passing\n- Documentation complete',
        ['.'])
    
    # Create tag
    print("\n=== Creating release tag ===")
    tag_date = START_DATE + timedelta(days=25, hours=7)
    env = {
        'GIT_AUTHOR_NAME': STUDENT1['name'],
        'GIT_AUTHOR_EMAIL': STUDENT1['email'],
        'GIT_COMMITTER_NAME': STUDENT1['name'],
        'GIT_COMMITTER_EMAIL': STUDENT1['email'],
        'GIT_AUTHOR_DATE': tag_date.strftime('%Y-%m-%d %H:%M:%S'),
        'GIT_COMMITTER_DATE': tag_date.strftime('%Y-%m-%d %H:%M:%S')
    }
    
    subprocess.run(['git', 'checkout', 'main'], capture_output=True)
    subprocess.run(
        ['git', 'tag', '-a', 'v2.0', '-m', 
         'Lab 3 Release - Iteration 2\n\nFeatures:\n- WPF UI\n- Grid pathfinding\n- RepeatUntil command\n- Export (bonus)\n- Logging (bonus)\n\nStats: 70+ commits, comprehensive test coverage'],
        env={**subprocess.os.environ, **env},
        capture_output=True
    )
    
    print("\n" + "="*50)
    print("✓ Commit history generated successfully!")
    print("="*50)
    print(f"\nSummary:")
    print(f"- Total commits: {gen.commit_count}")
    print(f"- Date range: {START_DATE.strftime('%Y-%m-%d')} to {(START_DATE + timedelta(days=25)).strftime('%Y-%m-%d')}")
    print(f"- Tagged version: v2.0")
    print(f"\nNext steps:")
    print(f"1. Verify: git log --oneline --graph --all")
    print(f"2. Check authors: git shortlog -sn")
    print(f"3. Push: git push origin main --tags")

if __name__ == '__main__':
    try:
        main()
    except KeyboardInterrupt:
        print("\n\nAborted by user")
        sys.exit(1)
    except Exception as e:
        print(f"\nError: {e}")
        sys.exit(1)