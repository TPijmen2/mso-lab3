using System.IO;
using System.Windows.Media.Imaging;
using ProgrammingLearningApp.Models;

namespace ProgrammingLearningApp.Services
{
	public class CharacterSpriteLoader
	{
		private Dictionary<Direction, List<BitmapImage>> animationFrames;
		private Dictionary<Direction, int> currentFrameIndex;
		private readonly string spritesDirectory;

		public CharacterSpriteLoader()
		{
			animationFrames = new Dictionary<Direction, List<BitmapImage>>();
			currentFrameIndex = new Dictionary<Direction, int>();
			
			// Look for sprites in Assets directory
			spritesDirectory = FindSpritesDirectory();
			
			LoggingService.Instance.LogInfo($"CharacterSpriteLoader initialized, sprites directory: {spritesDirectory}");
			
			// Load all animation frames
			LoadAllAnimationFrames();
		}

		private string FindSpritesDirectory()
		{
			// Look for Assets/Character directory
			var possiblePaths = new[]
			{
				Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Character"),
				Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Character"),
				Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Assets", "Character"),
			};

			foreach (var path in possiblePaths)
			{
				if (Directory.Exists(path))
				{
					LoggingService.Instance.LogDebug($"Found sprites directory: {path}");
					return path;
				}
			}

			// Default to first option
			var defaultPath = possiblePaths[0];
			LoggingService.Instance.LogWarning($"No sprites directory found, will use: {defaultPath}");
			return defaultPath;
		}

		private void LoadAllAnimationFrames()
		{
			// Map Direction enum to folder names
			var directionFolders = new Dictionary<Direction, string>
			{
				{ Direction.North, "Up" },
				{ Direction.East, "Right" },
				{ Direction.South, "Down" },
				{ Direction.West, "Left" }
			};

			foreach (var kvp in directionFolders)
			{
				var direction = kvp.Key;
				var folderName = kvp.Value;
				var folderPath = Path.Combine(spritesDirectory, folderName);

				animationFrames[direction] = new List<BitmapImage>();
				currentFrameIndex[direction] = 0;

				if (!Directory.Exists(folderPath))
				{
					LoggingService.Instance.LogWarning($"Animation folder not found: {folderPath}");
					continue;
				}

				// Load all numbered PNG files (1.png, 2.png, 3.png, etc.)
				var frameFiles = Directory.GetFiles(folderPath, "*.png")
					.Where(f => 
					{
						var fileName = Path.GetFileNameWithoutExtension(f);
						return int.TryParse(fileName, out _);
					})
					.OrderBy(f => int.Parse(Path.GetFileNameWithoutExtension(f)))
					.ToList();

				foreach (var filePath in frameFiles)
				{
					try
					{
						var bitmap = new BitmapImage();
						bitmap.BeginInit();
						bitmap.UriSource = new Uri(filePath, UriKind.Absolute);
						bitmap.CacheOption = BitmapCacheOption.OnLoad;
						bitmap.EndInit();
						bitmap.Freeze(); // Make it thread-safe

						animationFrames[direction].Add(bitmap);
					}
					catch (Exception ex)
					{
						LoggingService.Instance.LogException(ex, $"Error loading animation frame: {filePath}");
					}
				}

				LoggingService.Instance.LogInfo($"Loaded {animationFrames[direction].Count} animation frames for direction {direction} ({folderName})");
			}
		}

		// Gets the current animation frame for the specified direction
		public BitmapImage GetCurrentFrame(Direction direction)
		{
			if (!animationFrames.ContainsKey(direction) || animationFrames[direction].Count == 0)
			{
				return null;
			}

			var frames = animationFrames[direction];
			var frameIndex = currentFrameIndex[direction];
			
			return frames[frameIndex];
		}

		// Advances to the next animation frame for the specified direction
		public BitmapImage GetNextFrame(Direction direction)
		{
			if (!animationFrames.ContainsKey(direction) || animationFrames[direction].Count == 0)
			{
				return null;
			}

			var frames = animationFrames[direction];
			
			// Advance frame index
			currentFrameIndex[direction] = (currentFrameIndex[direction] + 1) % frames.Count;
			
			return frames[currentFrameIndex[direction]];
		}

		// Gets a specific frame index for the specified direction
		public BitmapImage GetFrame(Direction direction, int frameIndex)
		{
			if (!animationFrames.ContainsKey(direction) || animationFrames[direction].Count == 0)
			{
				return null;
			}

			var frames = animationFrames[direction];
			frameIndex = frameIndex % frames.Count; // Wrap around
			
			return frames[frameIndex];
		}

		// Resets the frame index for a specific direction to 0
		public void ResetFrameIndex(Direction direction)
		{
			if (currentFrameIndex.ContainsKey(direction))
			{
				currentFrameIndex[direction] = 0;
			}
		}

		// Resets all frame indices to 0
		public void ResetAllFrameIndices()
		{
			foreach (var direction in currentFrameIndex.Keys.ToList())
			{
				currentFrameIndex[direction] = 0;
			}
		}

		// Gets the number of frames available for a direction
		public int GetFrameCount(Direction direction)
		{
			if (!animationFrames.ContainsKey(direction))
			{
				return 0;
			}
			return animationFrames[direction].Count;
		}

		// Checks if animation frames are available for the specified direction
		public bool HasFrames(Direction direction)
		{
			return animationFrames.ContainsKey(direction) && animationFrames[direction].Count > 0;
		}

		// Checks if any animation frames are loaded
		public bool HasAnyFrames()
		{
			return animationFrames.Values.Any(frames => frames.Count > 0);
		}

		// Gets the sprites directory path
		public string GetSpritesDirectory()
		{
			return spritesDirectory;
		}

		// Checks if sprites directory exists and creates it if not
		public void EnsureSpritesDirectoryExists()
		{
			if (!Directory.Exists(spritesDirectory))
			{
				try
				{
					Directory.CreateDirectory(spritesDirectory);
					LoggingService.Instance.LogInfo($"Created sprites directory: {spritesDirectory}");
					
					// Create subdirectories
					foreach (var folder in new[] { "Up", "Down", "Left", "Right" })
					{
						var subDir = Path.Combine(spritesDirectory, folder);
						Directory.CreateDirectory(subDir);
						LoggingService.Instance.LogInfo($"Created sprites subdirectory: {subDir}");
					}
				}
				catch (Exception ex)
				{
					LoggingService.Instance.LogException(ex, $"Error creating sprites directory: {spritesDirectory}");
				}
			}
		}

		// Reloads all animation frames from disk
		public void ReloadSprites()
		{
			animationFrames.Clear();
			currentFrameIndex.Clear();
			LoadAllAnimationFrames();
			LoggingService.Instance.LogInfo("All animation frames reloaded");
		}
	}
}
