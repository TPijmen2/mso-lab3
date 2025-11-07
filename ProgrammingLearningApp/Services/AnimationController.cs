using ProgrammingLearningApp.Models;

namespace ProgrammingLearningApp.Services
{
	public enum AnimationState
	{
		Stopped,
		Playing,
		Paused,
		Completed
	}

	public class AnimationStep
	{
		public Position Position { get; set; }
		public Direction Direction { get; set; }
		public string CommandDescription { get; set; }
		public int StepNumber { get; set; }

		public AnimationStep(Position position, Direction direction, string command, int stepNumber)
		{
			Position = position.Copy();
			Direction = direction;
			CommandDescription = command;
			StepNumber = stepNumber;
		}
	}

	public class AnimationController
	{
		private List<AnimationStep> steps;
		private int currentStepIndex;
		private AnimationState state;

		public AnimationState State => state;
		public int CurrentStepIndex => currentStepIndex;
		public int TotalSteps => steps?.Count ?? 0;
		public bool HasSteps => steps != null && steps.Count > 0;
		public bool CanStepForward => currentStepIndex < (steps?.Count ?? 0) - 1;
		public bool CanStepBackward => currentStepIndex > 0;
		public AnimationStep CurrentStep => steps != null && currentStepIndex >= 0 && currentStepIndex < steps.Count 
			? steps[currentStepIndex] : null;

		// Animation speed in milliseconds between steps
		public int AnimationSpeed { get; set; } = 500;

		// Events
		public event EventHandler<AnimationStep> StepChanged;
		public event EventHandler AnimationCompleted;
		public event EventHandler AnimationStarted;
		public event EventHandler AnimationPaused;
		public event EventHandler AnimationReset;

		public AnimationController()
		{
			steps = new List<AnimationStep>();
			currentStepIndex = -1;
			state = AnimationState.Stopped;
			LoggingService.Instance.LogDebug("AnimationController initialized");
		}

		public void LoadSteps(List<AnimationStep> animationSteps)
		{
			steps = animationSteps ?? new List<AnimationStep>();
			currentStepIndex = steps.Count > 0 ? 0 : -1;
			state = AnimationState.Stopped;
			
			LoggingService.Instance.LogInfo($"Animation loaded with {steps.Count} steps");
			
			if (steps.Count > 0)
			{
				OnStepChanged(steps[0]);
			}
		}

		public void Play()
		{
			if (state == AnimationState.Completed)
			{
				Reset();
			}

			if (HasSteps && state != AnimationState.Playing)
			{
				state = AnimationState.Playing;
				LoggingService.Instance.LogInfo("Animation started");
				OnAnimationStarted();
			}
		}

		public void Pause()
		{
			if (state == AnimationState.Playing)
			{
				state = AnimationState.Paused;
				LoggingService.Instance.LogInfo($"Animation paused at step {currentStepIndex}");
				OnAnimationPaused();
			}
		}

		public void Stop()
		{
			state = AnimationState.Stopped;
			currentStepIndex = 0;
			LoggingService.Instance.LogInfo("Animation stopped");
			
			if (HasSteps)
			{
				OnStepChanged(steps[0]);
			}
		}

		public void Reset()
		{
			currentStepIndex = 0;
			state = AnimationState.Stopped;
			LoggingService.Instance.LogInfo("Animation reset");
			OnAnimationReset();
			
			if (HasSteps)
			{
				OnStepChanged(steps[0]);
			}
		}

		public bool StepForward()
		{
			if (!CanStepForward)
			{
				if (state == AnimationState.Playing)
				{
					state = AnimationState.Completed;
					LoggingService.Instance.LogInfo("Animation completed");
					OnAnimationCompleted();
				}
				return false;
			}

			currentStepIndex++;
			LoggingService.Instance.LogDebug($"Stepped forward to step {currentStepIndex}");
			OnStepChanged(steps[currentStepIndex]);

			if (!CanStepForward && state == AnimationState.Playing)
			{
				state = AnimationState.Completed;
				LoggingService.Instance.LogInfo("Animation completed");
				OnAnimationCompleted();
			}

			return true;
		}

		public bool StepBackward()
		{
			if (!CanStepBackward)
				return false;

			currentStepIndex--;
			LoggingService.Instance.LogDebug($"Stepped backward to step {currentStepIndex}");
			OnStepChanged(steps[currentStepIndex]);
			return true;
		}

		public void JumpToStep(int stepIndex)
		{
			if (stepIndex < 0 || stepIndex >= steps.Count)
				return;

			currentStepIndex = stepIndex;
			LoggingService.Instance.LogDebug($"Jumped to step {currentStepIndex}");
			OnStepChanged(steps[currentStepIndex]);
		}

		public List<AnimationStep> GetAllSteps()
		{
			return new List<AnimationStep>(steps);
		}

		protected virtual void OnStepChanged(AnimationStep step)
		{
			StepChanged?.Invoke(this, step);
		}

		protected virtual void OnAnimationCompleted()
		{
			AnimationCompleted?.Invoke(this, EventArgs.Empty);
		}

		protected virtual void OnAnimationStarted()
		{
			AnimationStarted?.Invoke(this, EventArgs.Empty);
		}

		protected virtual void OnAnimationPaused()
		{
			AnimationPaused?.Invoke(this, EventArgs.Empty);
		}

		protected virtual void OnAnimationReset()
		{
			AnimationReset?.Invoke(this, EventArgs.Empty);
		}
	}
}
