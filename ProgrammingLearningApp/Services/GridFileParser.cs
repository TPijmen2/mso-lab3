using System;
using System.IO;
using System.Linq;
using ProgrammingLearningApp.Models;

namespace ProgrammingLearningApp.Services
{
	public class GridFileParser
	{
		public PathfindingExercise ParseFromFile(string filePath)
		{
			if (!File.Exists(filePath))
				throw new FileNotFoundException($"Grid file not found: {filePath}");

			string[] lines = File.ReadAllLines(filePath)
				.Where(l => !string.IsNullOrWhiteSpace(l))
				.ToArray();

			if (lines.Length == 0)
				throw new FormatException("Grid file is empty");

			// Determine grid dimensions
			int height = lines.Length;
			int width = lines[0].Length;

			// Validate all lines have same length
			if (lines.Any(line => line.Length != width))
				throw new FormatException("All grid rows must have the same length");

			var grid = new Grid(width, height);
			Position endPosition = null;

			// Parse grid: 'o' = open, '+' = blocked, 'x' = end
			// Note: File format has Y=0 at top, but our coordinate system has Y=0 at bottom
			for (int fileY = 0; fileY < height; fileY++)
			{
				for (int x = 0; x < width; x++)
				{
					char c = lines[fileY][x];
					int gridY = height - 1 - fileY; // Flip Y coordinate

					switch (c)
					{
						case 'o':
							// Open cell, already default
							break;
						case '+':
							grid.SetCellBlocked(x, gridY, true);
							break;
						case 'x':
							endPosition = new Position(x, gridY);
							grid.SetEndPosition(x, gridY);
							break;
						default:
							throw new FormatException($"Invalid character '{c}' in grid file");
					}
				}
			}

			if (endPosition == null)
				throw new FormatException("Grid must contain an end position marked with 'x'");

			string exerciseName = Path.GetFileNameWithoutExtension(filePath);
			return new PathfindingExercise(exerciseName, grid);
		}
	}
}