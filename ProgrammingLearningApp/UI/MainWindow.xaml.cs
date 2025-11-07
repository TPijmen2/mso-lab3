using Microsoft.Win32;
using ProgrammingLearningApp.Models;
using ProgrammingLearningApp.Services;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ProgrammingLearningApp
{
	public partial class MainWindow : Window
	{
		private Program currentProgram;
		private PathfindingExercise currentExercise;
		private const int CellSize = 40;
		private const int CharacterRadius = 15;
		private bool isUpdatingFromTab = false;

		// Animation system
		private AnimationController animationController;
		private CharacterSpriteLoader spriteLoader;
		private DispatcherTimer animationTimer;
		private bool useSprites = false;

		public MainWindow()
		{
			InitializeComponent();
			InitializeAnimation();
			UpdateUI();
			LoggingService.Instance.LogInfo("MainWindow initialized");
		}

		// ===== Animation System Initialization =====

		private void InitializeAnimation()
		{
			animationController = new AnimationController();
			spriteLoader = new CharacterSpriteLoader();
			
			// Set up animation timer
			animationTimer = new DispatcherTimer();
			animationTimer.Interval = TimeSpan.FromMilliseconds(animationController.AnimationSpeed);
			animationTimer.Tick += AnimationTimer_Tick;

			// Subscribe to animation events
			animationController.StepChanged += AnimationController_StepChanged;
			animationController.AnimationCompleted += AnimationController_AnimationCompleted;
			animationController.AnimationStarted += AnimationController_AnimationStarted;
			animationController.AnimationPaused += AnimationController_AnimationPaused;

			// Try to load sprites
			TryLoadSprites();

			LoggingService.Instance.LogInfo("Animation system initialized");
		}

		private void TryLoadSprites()
		{
			try
			{
				spriteLoader.EnsureSpritesDirectoryExists();
				
				// Check if we have any animation frames loaded
				if (spriteLoader.HasAnyFrames())
				{
					useSprites = true;
					int totalFrames = 0;
					foreach (Direction dir in Enum.GetValues(typeof(Direction)))
					{
						totalFrames += spriteLoader.GetFrameCount(dir);
					}
					LoggingService.Instance.LogInfo($"Character animation frames loaded successfully - {totalFrames} total frames");
				}
				else
				{
					useSprites = false;
					LoggingService.Instance.LogInfo("No character animation frames found, using default rendering");
				}
			}
			catch (Exception ex)
			{
				useSprites = false;
				LoggingService.Instance.LogException(ex, "Error loading animation frames");
			}
		}

		// ===== Animation Event Handlers =====

		private void AnimationTimer_Tick(object sender, EventArgs e)
		{
			if (animationController.State == AnimationState.Playing)
			{
				bool hasNext = animationController.StepForward();
				if (!hasNext)
				{
					animationTimer.Stop();
				}
			}
		}

		private void AnimationController_StepChanged(object sender, AnimationStep step)
		{
			// Update UI on main thread
			Dispatcher.Invoke(() =>
			{
				UpdateAnimationUI(step);
			});
		}

		private void AnimationController_AnimationCompleted(object sender, EventArgs e)
		{
			Dispatcher.Invoke(() =>
			{
				animationTimer.Stop();
				PlayButton.IsEnabled = true;
				PauseButton.IsEnabled = false;
				AnimationStatusText.Text = "Animation completed";
				LoggingService.Instance.LogInfo("Animation playback completed");
			});
		}

		private void AnimationController_AnimationStarted(object sender, EventArgs e)
		{
			Dispatcher.Invoke(() =>
			{
				animationTimer.Start();
				PlayButton.IsEnabled = false;
				PauseButton.IsEnabled = true;
				AnimationStatusText.Text = "Playing animation...";
			});
		}

		private void AnimationController_AnimationPaused(object sender, EventArgs e)
		{
			Dispatcher.Invoke(() =>
			{
				animationTimer.Stop();
				PlayButton.IsEnabled = true;
				PauseButton.IsEnabled = false;
				AnimationStatusText.Text = "Animation paused";
			});
		}

		private void UpdateAnimationUI(AnimationStep step)
		{
			if (step == null) return;

			// Update progress display
			AnimationProgressText.Text = $"Step: {step.StepNumber + 1} / {animationController.TotalSteps}";
			AnimationProgressSlider.Value = step.StepNumber;
			CurrentStepText.Text = step.CommandDescription;

			// Redraw grid with character at current position
			if (currentExercise != null)
			{
				DrawGrid();
				DrawCharacterAtStep(step);
			}
		}

		// ===== Animation Control Handlers =====

		private void PlayButton_Click(object sender, RoutedEventArgs e)
		{
			LoggingService.Instance.LogInfo("User clicked Play Animation");
			animationController.Play();
		}

		private void PauseButton_Click(object sender, RoutedEventArgs e)
		{
			LoggingService.Instance.LogInfo("User clicked Pause Animation");
			animationController.Pause();
		}

		private void StopButton_Click(object sender, RoutedEventArgs e)
		{
			LoggingService.Instance.LogInfo("User clicked Stop Animation");
			animationController.Stop();
			AnimationStatusText.Text = "Animation stopped";
		}

		private void StepForwardButton_Click(object sender, RoutedEventArgs e)
		{
			LoggingService.Instance.LogDebug("User clicked Step Forward");
			animationController.StepForward();
		}

		private void StepBackButton_Click(object sender, RoutedEventArgs e)
		{
			LoggingService.Instance.LogDebug("User clicked Step Backward");
			animationController.StepBackward();
		}

		private void AnimationProgressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (animationController != null && !animationTimer.IsEnabled)
			{
				int stepIndex = (int)e.NewValue;
				animationController.JumpToStep(stepIndex);
			}
		}

		private void AnimationSpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (animationController != null)
			{
				animationController.AnimationSpeed = (int)e.NewValue;
				animationTimer.Interval = TimeSpan.FromMilliseconds(animationController.AnimationSpeed);
				SpeedText.Text = $"{(int)e.NewValue} ms";
			}
		}

		// ===== Menu Handlers for Sprites =====

		private void LoadSprites_Click(object sender, RoutedEventArgs e)
		{
			LoggingService.Instance.LogInfo("User clicked Load Sprites");
			
			// Reload animation frames from disk
			spriteLoader.ReloadSprites();
			TryLoadSprites();
			
			if (useSprites)
			{
				int totalFrames = 0;
				foreach (Direction dir in Enum.GetValues(typeof(Direction)))
				{
					int frameCount = spriteLoader.GetFrameCount(dir);
					totalFrames += frameCount;
				}
				
				MessageBox.Show($"Animation frames loaded successfully!\n\n" +
					$"Total frames: {totalFrames}\n" +
					$"North (Up): {spriteLoader.GetFrameCount(Direction.North)} frames\n" +
					$"East (Right): {spriteLoader.GetFrameCount(Direction.East)} frames\n" +
					$"South (Down): {spriteLoader.GetFrameCount(Direction.South)} frames\n" +
					$"West (Left): {spriteLoader.GetFrameCount(Direction.West)} frames",
					"Animation Frames Loaded",
					MessageBoxButton.OK,
					MessageBoxImage.Information);
			}
			else
			{
				MessageBox.Show($"No animation frames found!\n\n" +
					$"Frames should be located in:\n{spriteLoader.GetSpritesDirectory()}\n\n" +
					$"Expected folder structure:\n" +
					$"  Up/    (1.png, 2.png, 3.png, ...)\n" +
					$"  Down/  (1.png, 2.png, 3.png, ...)\n" +
					$"  Left/  (1.png, 2.png, 3.png, ...)\n" +
					$"  Right/ (1.png, 2.png, 3.png, ...)\n",
					"No Frames Found",
					MessageBoxButton.OK,
					MessageBoxImage.Warning);
			}
		}

		private void OpenSpritesFolder_Click(object sender, RoutedEventArgs e)
		{
			LoggingService.Instance.LogInfo("User clicked Open Sprites Folder");
			
			try
			{
				spriteLoader.EnsureSpritesDirectoryExists();
				string path = spriteLoader.GetSpritesDirectory();
				System.Diagnostics.Process.Start("explorer.exe", path);
				
				MessageBox.Show($"Animation frames folder opened!\n\n" +
					$"Place your animation frames in subfolders:\n\n" +
					$"  Up/    - Character facing up (1.png, 2.png, ...)\n" +
					$"  Down/  - Character facing down (1.png, 2.png, ...)\n" +
					$"  Left/  - Character facing left (1.png, 2.png, ...)\n" +
					$"  Right/ - Character facing right (1.png, 2.png, ...)\n\n" +
					$"Then use Animation → Reload Frames to load them.",
					"Animation Frames Folder",
					MessageBoxButton.OK,
					MessageBoxImage.Information);
			}
			catch (Exception ex)
			{
				LoggingService.Instance.LogException(ex, "Error opening sprites folder");
				MessageBox.Show($"Error opening folder: {ex.Message}", "Error",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		// ===== Tab Synchronization =====

		private void EditorTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (isUpdatingFromTab || EditorTabControl.SelectedIndex == -1)
				return;

			isUpdatingFromTab = true;

			try
			{
				if (EditorTabControl.SelectedIndex == 0) // Text tab selected
				{
					// Update text from blocks - get the current program from blocks first
					try
					{
						currentProgram = BlockEditorControl.GetProgram();
						if (currentProgram != null)
						{
							ProgramTextBox.Text = currentProgram.GetTextRepresentation();
							LoggingService.Instance.LogInfo("Switched to Text tab - updated text from blocks");
						}
					}
					catch (Exception ex)
					{
						LoggingService.Instance.LogException(ex, "Error updating text from blocks");
					}
				}
				else if (EditorTabControl.SelectedIndex == 1) // Blocks tab selected
				{
					// Update blocks from text - parse the text first
					try
					{
						string programText = ProgramTextBox.Text;
						if (!string.IsNullOrWhiteSpace(programText))
						{
							// Create temporary file to parse
							string tempFile = System.IO.Path.GetTempFileName();
							File.WriteAllText(tempFile, programText);

							var importer = new ProgramImporter();
							currentProgram = importer.ImportFromFile(tempFile);

							File.Delete(tempFile);
							
							LoggingService.Instance.LogInfo($"Parsed program from text: {currentProgram.Commands.Count} commands");
						}
						
						if (currentProgram != null)
						{
							BlockEditorControl.LoadProgram(currentProgram);
							LoggingService.Instance.LogInfo("Switched to Blocks tab - updated blocks from text");
						}
					}
					catch (Exception ex)
					{
						LoggingService.Instance.LogException(ex, "Error updating blocks from text");
						StatusText.Text = "Error loading blocks from text";
						StatusText.Foreground = new SolidColorBrush(Colors.Red);
					}
				}
			}
			finally
			{
				isUpdatingFromTab = false;
			}
		}

		private void BlockEditor_ProgramChanged(object sender, EventArgs e)
		{
			if (isUpdatingFromTab)
				return;

			try
			{
				currentProgram = BlockEditorControl.GetProgram();
				StatusText.Text = "Program updated from blocks";
				StatusText.Foreground = new SolidColorBrush(Colors.Green);
				LoggingService.Instance.LogInfo($"Block editor changed - program has {currentProgram.Commands.Count} commands");
			}
			catch (Exception ex)
			{
				StatusText.Text = "Invalid block configuration";
				StatusText.Foreground = new SolidColorBrush(Colors.Orange);
				LoggingService.Instance.LogException(ex, "Error converting blocks to program");
			}
		}

		// ===== File Menu =====

		private void LoadProgram_Click(object sender, RoutedEventArgs e)
		{
			LoggingService.Instance.LogInfo("User clicked Load Program");
			
			var dialog = new OpenFileDialog
			{
				Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
				Title = "Load Program"
			};

			if (dialog.ShowDialog() == true)
			{
				try
				{
					var importer = new ProgramImporter();
					currentProgram = importer.ImportFromFile(dialog.FileName);
					
					// Update the active tab
					if (EditorTabControl.SelectedIndex == 0)
					{
						ProgramTextBox.Text = currentProgram.GetTextRepresentation();
					}
					else
					{
						BlockEditorControl.LoadProgram(currentProgram);
					}
					
					StatusText.Text = $"Loaded program: {currentProgram.Name}";
					OutputTextBox.Clear();
					
					LoggingService.Instance.LogInfo($"Successfully loaded program: {currentProgram.Name}");
				}
				catch (Exception ex)
				{
					LoggingService.Instance.LogException(ex, "Error loading program from file");
					MessageBox.Show($"Error loading program: {ex.Message}", "Error",
								  MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
			else
			{
				LoggingService.Instance.LogDebug("Load Program dialog cancelled");
			}
		}

		private void LoadBasicExample_Click(object sender, RoutedEventArgs e)
		{
			LoggingService.Instance.LogInfo("User clicked Load Basic Example");
			currentProgram = ProgramFactory.CreateBasicProgram();
			
			if (EditorTabControl.SelectedIndex == 0)
			{
				ProgramTextBox.Text = currentProgram.GetTextRepresentation();
			}
			else
			{
				BlockEditorControl.LoadProgram(currentProgram);
			}
			
			StatusText.Text = "Loaded basic example";
			OutputTextBox.Clear();
			LoggingService.Instance.LogInfo("Basic example program loaded");
		}

		private void LoadAdvancedExample_Click(object sender, RoutedEventArgs e)
		{
			LoggingService.Instance.LogInfo("User clicked Load Advanced Example");
			currentProgram = ProgramFactory.CreateAdvancedProgram();
			
			if (EditorTabControl.SelectedIndex == 0)
			{
				ProgramTextBox.Text = currentProgram.GetTextRepresentation();
			}
			else
			{
				BlockEditorControl.LoadProgram(currentProgram);
			}
			
			StatusText.Text = "Loaded advanced example";
			OutputTextBox.Clear();
			LoggingService.Instance.LogInfo("Advanced example program loaded");
		}

		private void LoadExpertExample_Click(object sender, RoutedEventArgs e)
		{
			LoggingService.Instance.LogInfo("User clicked Load Expert Example");
			currentProgram = ProgramFactory.CreateExpertProgram();
			
			if (EditorTabControl.SelectedIndex == 0)
			{
				ProgramTextBox.Text = currentProgram.GetTextRepresentation();
			}
			else
			{
				BlockEditorControl.LoadProgram(currentProgram);
			}
			
			StatusText.Text = "Loaded expert example";
			OutputTextBox.Clear();
			LoggingService.Instance.LogInfo("Expert example program loaded");
		}

		private void ExportProgram_Click(object sender, RoutedEventArgs e)
		{
			LoggingService.Instance.LogInfo("User clicked Export Program");
			
			if (currentProgram == null)
			{
				LoggingService.Instance.LogWarning("Export attempted with no program loaded");
				MessageBox.Show("No program to export", "Warning",
							  MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			var dialog = new SaveFileDialog
			{
				Filter = "Text files (*.txt)|*.txt|JSON files (*.json)|*.json|HTML files (*.html)|*.html",
				FilterIndex = 1, // Default to Text files
				Title = "Export Program",
				FileName = currentProgram.Name
			};

			if (dialog.ShowDialog() == true)
			{
				try
				{
					string extension = System.IO.Path.GetExtension(dialog.FileName).ToLower();
					IProgramExporter exporter;
					string formatName;

					// Determine exporter based on file extension
					switch (extension)
					{
						case ".txt":
							exporter = new TextProgramExporter();
							formatName = "text";
							break;
						case ".json":
							exporter = new JsonProgramExporter();
							formatName = "JSON";
							break;
						case ".html":
							exporter = new HtmlProgramExporter();
							formatName = "HTML";
							break;
						default:
							// Default to text if unknown extension
							exporter = new TextProgramExporter();
							formatName = "text";
							LoggingService.Instance.LogWarning($"Unknown file extension '{extension}', defaulting to text format");
							break;
					}

					exporter.Export(currentProgram, dialog.FileName);
					
					StatusText.Text = "Program exported successfully";
					LoggingService.Instance.LogInfo($"Program exported to {formatName} file: {dialog.FileName}");
					
					MessageBox.Show($"Program exported successfully to {formatName} format", "Success",
								  MessageBoxButton.OK, MessageBoxImage.Information);
				}
				catch (Exception ex)
				{
					LoggingService.Instance.LogException(ex, $"Error exporting program to {dialog.FileName}");
					MessageBox.Show($"Error exporting program: {ex.Message}", "Error",
								  MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
			else
			{
				LoggingService.Instance.LogDebug("Export Program dialog cancelled");
			}
		}

		private void Exit_Click(object sender, RoutedEventArgs e)
		{
			LoggingService.Instance.LogInfo("User clicked Exit");
			animationTimer?.Stop();
			System.Windows.Application.Current.Shutdown();
		}

		// ===== Exercise Menu =====

		private void LoadExercise_Click(object sender, RoutedEventArgs e)
		{
			LoggingService.Instance.LogInfo("User clicked Load Exercise");
			
			var dialog = new OpenFileDialog
			{
				Filter = "Grid files (*.txt)|*.txt|All files (*.*)|*.*",
				Title = "Load Exercise"
			};

			if (dialog.ShowDialog() == true)
			{
				try
				{
					var parser = new GridFileParser();
					currentExercise = parser.ParseFromFile(dialog.FileName);

					ExerciseNameText.Text = $"Exercise: {currentExercise.Name}";
					ExerciseStatusText.Text = $"Exercise: {currentExercise.Name}";
					StatusText.Text = $"Loaded exercise: {currentExercise.Name}";

					DrawGrid();
					OutputTextBox.Clear();
					
					LoggingService.Instance.LogInfo($"Successfully loaded exercise: {currentExercise.Name}");
					LoggingService.Instance.LogDebug($"Grid size: {currentExercise.Grid.Width}x{currentExercise.Grid.Height}");
				}
				catch (Exception ex)
				{
					LoggingService.Instance.LogException(ex, "Error loading exercise from file");
					MessageBox.Show($"Error loading exercise: {ex.Message}", "Error",
								  MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
			else
			{
				LoggingService.Instance.LogDebug("Load Exercise dialog cancelled");
			}
		}

		private void ClearExercise_Click(object sender, RoutedEventArgs e)
		{
			LoggingService.Instance.LogInfo("User clicked Clear Exercise");
			currentExercise = null;
			ExerciseNameText.Text = "";
			ExerciseStatusText.Text = "No exercise loaded";
			StatusText.Text = "Exercise cleared";
			GridCanvas.Children.Clear();
			OutputTextBox.Clear();
			
			// Hide animation controls
			AnimationControlsPanel.Visibility = Visibility.Collapsed;
			AnimationStatusText.Text = "No animation";
		}

		// ===== Help Menu =====

		private void About_Click(object sender, RoutedEventArgs e)
		{
			LoggingService.Instance.LogInfo("User clicked About");
			MessageBox.Show(
				"CodeCat - Programming Learning App\n\n" +
				"Lab Assignment 3\n" +
				"A block-based programming environment with animation support.\n\n" +
				"Version 2.3",
				"About CodeCat",
				MessageBoxButton.OK,
				MessageBoxImage.Information);
		}

		// ===== Editor Events =====

		private void ProgramTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (isUpdatingFromTab)
				return;

			try
			{
				// Try to parse the program
				string programText = ProgramTextBox.Text;
				if (!string.IsNullOrWhiteSpace(programText))
				{
					// Create temporary file to parse
					string tempFile = System.IO.Path.GetTempFileName();
					File.WriteAllText(tempFile, programText);

					var importer = new ProgramImporter();
					currentProgram = importer.ImportFromFile(tempFile);

					File.Delete(tempFile);

					StatusText.Text = "Program valid";
					StatusText.Foreground = new SolidColorBrush(Colors.Green);
				}
			}
			catch
			{
				// Invalid program, but don't show error while typing
				StatusText.Text = "Invalid program syntax";
				StatusText.Foreground = new SolidColorBrush(Colors.Orange);
			}
		}

		// ===== Control Buttons =====

		private void RunButton_Click(object sender, RoutedEventArgs e)
		{
			LoggingService.Instance.LogInfo("User clicked Run Program");
			
			// Get program from active tab
			if (EditorTabControl.SelectedIndex == 1) // Blocks tab
			{
				try
				{
					currentProgram = BlockEditorControl.GetProgram();
				}
				catch (Exception ex)
				{
					MessageBox.Show($"Error building program from blocks: {ex.Message}", "Error",
								  MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}
			}
			
			if (currentProgram == null)
			{
				LoggingService.Instance.LogWarning("Run attempted with no program loaded");
				MessageBox.Show("No program loaded", "Warning",
							  MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			try
			{
				// Use animated runner to build animation steps
				var animatedRunner = new AnimatedProgramRunner(currentProgram, currentExercise);
				var result = animatedRunner.Execute();

				// Display trace
				OutputTextBox.Text = "=== Execution Trace ===\n";
				OutputTextBox.Text += string.Join("\n", result.Trace);
				OutputTextBox.Text += $"\n\n=== Final State ===\n";
				OutputTextBox.Text += $"Position: {result.FinalPosition}\n";
				OutputTextBox.Text += $"Direction: {result.FinalDirection}\n";

				// Display result status
				OutputTextBox.Text += $"\n=== Status ===\n";
				switch (result.Status)
				{
					case ExecutionStatus.Success:
						OutputTextBox.Text += "✓ SUCCESS";
						if (currentExercise != null)
						{
							OutputTextBox.Text += " - Exercise completed!";
						}
						StatusText.Text = "Program executed successfully";
						StatusText.Foreground = new SolidColorBrush(Colors.Green);
						break;

					case ExecutionStatus.Failure:
						OutputTextBox.Text += "✗ FAILURE\n";
						OutputTextBox.Text += result.ErrorMessage;
						StatusText.Text = "Program failed";
						StatusText.Foreground = new SolidColorBrush(Colors.Red);
						break;

					case ExecutionStatus.RuntimeError:
						OutputTextBox.Text += "✗ RUNTIME ERROR\n";
						OutputTextBox.Text += result.ErrorMessage;
						StatusText.Text = "Runtime error occurred";
						StatusText.Foreground = new SolidColorBrush(Colors.Red);
						break;
				}

				// Set up animation
				var steps = animatedRunner.GetAnimationSteps();
				if (steps.Count > 0)
				{
					animationController.LoadSteps(steps);
					AnimationControlsPanel.Visibility = Visibility.Visible;
					AnimationProgressSlider.Maximum = steps.Count - 1;
					AnimationProgressText.Text = $"Step: 1 / {steps.Count}";
					AnimationStatusText.Text = $"Animation ready ({steps.Count} steps)";
					
					LoggingService.Instance.LogInfo($"Animation ready with {steps.Count} steps");

					// Draw initial state
					if (currentExercise != null)
					{
						DrawGrid();
						DrawCharacterAtStep(steps[0]);
					}
				}
				else
				{
					// No animation, just draw final result
					if (currentExercise != null)
					{
						DrawGrid();
						DrawPath(result);
					}
				}
			}
			catch (Exception ex)
			{
				LoggingService.Instance.LogException(ex, "Error during program execution in UI");
				OutputTextBox.Text = $"Error executing program:\n{ex.Message}";
				MessageBox.Show($"Error executing program: {ex.Message}", "Error",
							  MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void MetricsButton_Click(object sender, RoutedEventArgs e)
		{
			LoggingService.Instance.LogInfo("User clicked Calculate Metrics");
			
			// Get program from active tab
			if (EditorTabControl.SelectedIndex == 1) // Blocks tab
			{
				try
				{
					currentProgram = BlockEditorControl.GetProgram();
				}
				catch (Exception ex)
				{
					MessageBox.Show($"Error building program from blocks: {ex.Message}", "Error",
								  MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}
			}
			
			if (currentProgram == null)
			{
				LoggingService.Instance.LogWarning("Metrics calculation attempted with no program loaded");
				MessageBox.Show("No program loaded", "Warning",
							  MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			try
			{
				var metrics = currentProgram.CalculateMetrics();

				OutputTextBox.Text = "=== Program Metrics ===\n\n";
				OutputTextBox.Text += $"Total Commands: {metrics.CommandCount}\n";
				OutputTextBox.Text += $"Max Nesting Level: {metrics.MaxNestingLevel}\n";
				OutputTextBox.Text += $"Number of Repeats: {metrics.RepeatCount}\n";

				StatusText.Text = "Metrics calculated";
				StatusText.Foreground = new SolidColorBrush(Colors.Blue);
				
				LoggingService.Instance.LogInfo($"Metrics calculated - Commands: {metrics.CommandCount}, Nesting: {metrics.MaxNestingLevel}, Repeats: {metrics.RepeatCount}");
			}
			catch (Exception ex)
			{
				LoggingService.Instance.LogException(ex, "Error calculating program metrics");
				OutputTextBox.Text = $"Error calculating metrics:\n{ex.Message}";
				MessageBox.Show($"Error calculating metrics: {ex.Message}", "Error",
							  MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void ClearButton_Click(object sender, RoutedEventArgs e)
		{
			LoggingService.Instance.LogInfo("User clicked Clear");
			ProgramTextBox.Clear();
			OutputTextBox.Clear();
			currentProgram = null;
			StatusText.Text = "Ready";
			StatusText.Foreground = new SolidColorBrush(Colors.Black);

			// Stop and hide animation
			animationTimer.Stop();
			AnimationControlsPanel.Visibility = Visibility.Collapsed;
			AnimationStatusText.Text = "No animation";

			if (currentExercise != null)
			{
				DrawGrid(); // Redraw grid without path
			}
		}

		// ===== Grid Drawing =====

		private void GridCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (currentExercise != null)
			{
				DrawGrid();
				
				// Redraw character at current animation step if animating
				if (animationController.HasSteps && animationController.CurrentStep != null)
				{
					DrawCharacterAtStep(animationController.CurrentStep);
				}
			}
		}

		private void DrawGrid()
		{
			if (currentExercise == null)
				return;

			GridCanvas.Children.Clear();

			var grid = currentExercise.Grid;
			int width = grid.Width;
			int height = grid.Height;

			// Calculate cell size to fit canvas
			double canvasWidth = GridCanvas.ActualWidth;
			double canvasHeight = GridCanvas.ActualHeight;

			if (canvasWidth == 0 || canvasHeight == 0)
				return;

			double cellWidth = Math.Min(CellSize, canvasWidth / width);
			double cellHeight = Math.Min(CellSize, canvasHeight / height);
			double cellSize = Math.Min(cellWidth, cellHeight);

			// Center the grid
			double offsetX = (canvasWidth - (width * cellSize)) / 2;
			double offsetY = (canvasHeight - (height * cellSize)) / 2;

			// Draw cells
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					var cell = grid.GetCell(x, y);

					// Note: Canvas Y increases downward, but our grid Y increases upward
					double canvasX = offsetX + x * cellSize;
					double canvasY = offsetY + (height - 1 - y) * cellSize;

					var rect = new System.Windows.Shapes.Rectangle
					{
						Width = cellSize,
						Height = cellSize,
						Stroke = Brushes.Black,
						StrokeThickness = 1
					};

					if (cell.IsBlocked)
					{
						rect.Fill = Brushes.DarkGray;
					}
					else if (cell.IsEndPosition)
					{
						rect.Fill = Brushes.LightGreen;
					}
					else
					{
						rect.Fill = Brushes.White;
					}

					Canvas.SetLeft(rect, canvasX);
					Canvas.SetTop(rect, canvasY);
					GridCanvas.Children.Add(rect);
				}
			}

			// Draw start position marker
			DrawCharacterAt(currentExercise.StartPosition, Colors.Blue, "S");

			// Draw end position marker
			DrawCharacterAt(grid.EndPosition, Colors.Green, "E");
		}

		private void DrawPath(ProgramExecutionResult result)
		{
			if (currentExercise == null || result == null)
				return;

			var grid = currentExercise.Grid;
			int width = grid.Width;
			int height = grid.Height;

			double canvasWidth = GridCanvas.ActualWidth;
			double canvasHeight = GridCanvas.ActualHeight;
			double cellWidth = Math.Min(CellSize, canvasWidth / width);
			double cellHeight = Math.Min(CellSize, canvasHeight / height);
			double cellSize = Math.Min(cellWidth, cellHeight);

			double offsetX = (canvasWidth - (width * cellSize)) / 2;
			double offsetY = (canvasHeight - (height * cellSize)) / 2;

			// Get path from character
			// We need to re-execute to get the path
			var runner = new ProgramRunner(currentProgram, currentExercise);
			var character = new Character(currentExercise.Grid);
			
			// Set position and initialize path with starting position
			character.Position = currentExercise.StartPosition.Copy();
			character.Path = new List<Position> { character.Position.Copy() };

			try
			{
				// Execute to build path
				foreach (var command in currentProgram.Commands)
				{
					command.Execute(character);
				}

				// Draw path
				if (character.Path != null && character.Path.Count > 1)
				{
					var pathGeometry = new PathGeometry();
					var pathFigure = new PathFigure();

					for (int i = 0; i < character.Path.Count; i++)
					{
						var pos = character.Path[i];
						double x = offsetX + pos.X * cellSize + cellSize / 2;
						double y = offsetY + (height - 1 - pos.Y) * cellSize + cellSize / 2;

						if (i == 0)
						{
							pathFigure.StartPoint = new System.Windows.Point(x, y);
						}
						else
						{
							pathFigure.Segments.Add(new LineSegment(new System.Windows.Point(x, y), true));
						}
					}

					pathGeometry.Figures.Add(pathFigure);

					var path = new System.Windows.Shapes.Path
					{
						Stroke = Brushes.Blue,
						StrokeThickness = 3,
						Data = pathGeometry
					};

					GridCanvas.Children.Add(path);
				}

				// Draw character at final position
				DrawCharacterAt(result.FinalPosition,
					result.Status == ExecutionStatus.Success ? Colors.Green : Colors.Red,
					"C");
			}
			catch
			{
				// If execution fails, just show final position
				DrawCharacterAt(result.FinalPosition, Colors.Red, "C");
			}
		}

		private void DrawCharacterAt(Position position, System.Windows.Media.Color color, string label)
		{
			if (currentExercise == null || position == null)
				return;

			var grid = currentExercise.Grid;
			int width = grid.Width;
			int height = grid.Height;

			double canvasWidth = GridCanvas.ActualWidth;
			double canvasHeight = GridCanvas.ActualHeight;
			double cellWidth = Math.Min(CellSize, canvasWidth / width);
			double cellHeight = Math.Min(CellSize, canvasHeight / height);
			double cellSize = Math.Min(cellWidth, cellHeight);

			double offsetX = (canvasWidth - (width * cellSize)) / 2;
			double offsetY = (canvasHeight - (height * cellSize)) / 2;

			double x = offsetX + position.X * cellSize + cellSize / 2;
			double y = offsetY + (height - 1 - position.Y) * cellSize + cellSize / 2;

			// Draw circle
			var ellipse = new Ellipse
			{
				Width = CharacterRadius * 2,
				Height = CharacterRadius * 2,
				Fill = new SolidColorBrush(color),
				Stroke = Brushes.Black,
				StrokeThickness = 2
			};

			Canvas.SetLeft(ellipse, x - CharacterRadius);
			Canvas.SetTop(ellipse, y - CharacterRadius);
			GridCanvas.Children.Add(ellipse);

			// Draw label
			var textBlock = new TextBlock
			{
				Text = label,
				FontSize = 14,
				FontWeight = FontWeights.Bold,
				Foreground = Brushes.White
			};

			Canvas.SetLeft(textBlock, x - 5);
			Canvas.SetTop(textBlock, y - 8);
			GridCanvas.Children.Add(textBlock);
		}

		private void UpdateUI()
		{
			// Initialize UI state
			StatusText.Text = "Ready";
			ExerciseStatusText.Text = "No exercise loaded";
			AnimationStatusText.Text = "No animation";
		}

		private void DrawCharacterAtStep(AnimationStep step)
		{
			if (currentExercise == null || step == null)
				return;

			var grid = currentExercise.Grid;
			int width = grid.Width;
			int height = grid.Height;

			double canvasWidth = GridCanvas.ActualWidth;
			double canvasHeight = GridCanvas.ActualHeight;
			double cellWidth = Math.Min(CellSize, canvasWidth / width);
			double cellHeight = Math.Min(CellSize, canvasHeight / height);
			double cellSize = Math.Min(cellWidth, cellHeight);

			double offsetX = (canvasWidth - (width * cellSize)) / 2;
			double offsetY = (canvasHeight - (height * cellSize)) / 2;

			double x = offsetX + step.Position.X * cellSize + cellSize / 2;
			double y = offsetY + (height - 1 - step.Position.Y) * cellSize + cellSize / 2;

			// Try to use animated sprite frames if available
			BitmapImage sprite = null;
			if (useSprites && spriteLoader.HasFrames(step.Direction))
			{
				// Use step number to determine which frame to show
				// This creates a walking animation effect
				int frameIndex = step.StepNumber % spriteLoader.GetFrameCount(step.Direction);
				sprite = spriteLoader.GetFrame(step.Direction, frameIndex);
			}
			
			if (sprite != null)
			{
				// Draw animated sprite frame (no rotation needed - frames are already directional)
				var image = new Image
				{
					Source = sprite,
					Width = cellSize * 0.8,
					Height = cellSize * 0.8
				};

				Canvas.SetLeft(image, x - image.Width / 2);
				Canvas.SetTop(image, y - image.Height / 2);
				GridCanvas.Children.Add(image);
			}
			else
			{
				// Use default rendering (circle with direction indicator)
				var ellipse = new Ellipse
				{
					Width = CharacterRadius * 2,
					Height = CharacterRadius * 2,
					Fill = new SolidColorBrush(Colors.Blue),
					Stroke = Brushes.Black,
					StrokeThickness = 2
				};

				Canvas.SetLeft(ellipse, x - CharacterRadius);
				Canvas.SetTop(ellipse, y - CharacterRadius);
				GridCanvas.Children.Add(ellipse);

				// Draw direction indicator
				var dirLine = new Line
				{
					Stroke = Brushes.White,
					StrokeThickness = 3,
					X1 = x,
					Y1 = y
				};

				switch (step.Direction)
				{
					case Direction.North:
						dirLine.X2 = x;
						dirLine.Y2 = y - CharacterRadius;
						break;
					case Direction.East:
						dirLine.X2 = x + CharacterRadius;
						dirLine.Y2 = y;
						break;
					case Direction.South:
						dirLine.X2 = x;
						dirLine.Y2 = y + CharacterRadius;
						break;
					case Direction.West:
						dirLine.X2 = x - CharacterRadius;
						dirLine.Y2 = y;
						break;
				}

				GridCanvas.Children.Add(dirLine);

				// Add label
				var textBlock = new TextBlock
				{
					Text = "C",
					FontSize = 14,
					FontWeight = FontWeights.Bold,
					Foreground = Brushes.White
				};

				Canvas.SetLeft(textBlock, x - 5);
				Canvas.SetTop(textBlock, y - 8);
				GridCanvas.Children.Add(textBlock);
			}
		}
	}
}