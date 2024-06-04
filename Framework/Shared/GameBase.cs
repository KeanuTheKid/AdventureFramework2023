using Framework.Slides;
using Microsoft.AspNetCore.Components;
using Framework.Game.Parameters;
using Framework.State;
using Framework.Sound;


namespace Framework.Game;

public partial class GameBase : ComponentBase
{
	[Inject]
	protected SlideService SlideService { get; set; } = null!;

	[Inject]
	protected GameState GameState { get; set; } = null!;

	[Inject]
	protected SoundService SoundService { get; set; } = null!;

	// // private readonly TaskCompletionSource<bool> _tcs = new();
	// // protected Task InitTask => _tcs.Task;

	protected string SlideId => Parameters.SlideId;

	protected SlideComponentParameters Parameters { get; set; } = null!;
	// protected Dictionary<string, object?> ParametersDictionary { get; set; } = null!;


	protected override void OnInitialized()
	{
		// string slideId = SlideService.GetStartSlideId();
		// string slideId = GameState.CurrentSlide;
		Parameters = new SlideComponentParameters()
		{
			SlideId = GameState.CurrentSlide,
			OnButtonClick = EventCallback.Factory.Create<List<List<string>>>(this, EvaluateActions)
		};
		// // _tcs.SetResult(true);
	}

	protected static void HandlePolygonClick(string thing)
	{
		Console.WriteLine(thing);
	}

	protected void ChangeSlide(string slideId)
	{
		// for debug, throw an exception if the slide does not exist
		if (Debug)
		{
			SlideService.GetSlide(slideId);
		}


		Parameters.SlideId = slideId;
		// still a bit hacky, but I guess
		GameState.CurrentSlide = slideId;
		// Console.WriteLine(SlideId);
		StateHasChanged();
	}

	protected async Task FinishMinigame(List<List<string>> actions)
	{
		await EvaluateActions(actions);
	}

	protected struct Block
	{
		public List<string> Stack { get; set; }
		public bool Skipping { get; set; }
		public string SkippingTo { get; set; }
	}

	protected async Task EvaluateActions(List<List<string>> actions)
	{
		Block block = new()
		{
			Stack = new()
		};
		foreach (List<string> action in actions)
		{
			if (block.Skipping)
			{
				if (block.SkippingTo == action[1] && action[0] == "EndBlock")
				{
					block.Skipping = false;
					// block.Stack.RemoveAt(block.Stack.Count - 1);
					// removed this cause it gets removed in the switch
					// block.Stack.Remove(block.Stack.Last());
					block.SkippingTo = "";
				}
				else
				{
					continue;
				}
			}

			switch (action[0])
			{
				case "Route":
					ChangeSlide(action[1]);
					break;
				case "AddItem":
					GameState.AddItem(action[1]);
					break;
				case "RemoveItem":
					GameState.RemoveItem(action[1]);
					break;
				case "SetGameState":
					switch (action[2])
					{
						case "true":
							GameState.SetState(action[1], true);
							break;
						case "false":
							GameState.SetState(action[1], false);
							break;
						case "toggle":
							GameState.ToggleState(action[1]);
							break;
						default:
							break;
					}
					break;

				case "RequireItem":
					// if the check is negated
					if (action[1].StartsWith('!'))
					{
						// remove leading "!"
						if (!GameState.CheckForItem(action[1][1..]))
						{
							// if it is true, continue with executing
							// Console.WriteLine($"Required Item: {action[1]}");
							break;
						}
					}
					// if the check if not negated
					else
					{
						if (GameState.CheckForItem(action[1]))
						{
							// if it is true, continue with executing
							// Console.WriteLine($"Required Item: {action[1]}");
							break;
						}
					}
					// if the checks have been false

					// if there are blocks on the stack
					if (block.Stack.Count > 0)
					{
						// start skipping to corresponding EndBlock statement
						block.Skipping = true;
						block.SkippingTo = block.Stack.Last();
						break;
					}
					// if no block is on the stack, just exit entirely
					else
					{
						return;
					}

				case "RequireGameState":
					// if the check is negated
					if (action[1].StartsWith('!'))
					{
						// remove leading "!"
						if (!GameState.GetState(action[1][1..]))
						{
							// if it is true, continue with executing
							// Console.WriteLine($"Required GameState: {action[1]}");
							break;
						}
					}
					// if the check if not negated
					else
					{
						if (GameState.GetState(action[1]))
						{
							// if it is true, continue with executing
							// Console.WriteLine($"Required GameState: {action[1]}");
							break;
						}
					}
					// if the checks have been false

					// if there are blocks on the stack
					if (block.Stack.Count > 0)
					{
						// start skipping to corresponding EndBlock statement
						block.Skipping = true;
						block.SkippingTo = block.Stack.Last();
						// Console.WriteLine($"Skipping to {block.SkippingTo}");
						break;
					}
					// if no block is on the stack, just exit entirely
					else
					{
						// Console.WriteLine("return");
						return;
					}

				case "PlaySound":
					await SoundService.PlaySound(action[1]);
					break;

				case "PlayMusic":
					await SoundService.PlayMusic(action[1]);
					break;

				case "StopMusic":
					await SoundService.StopMusic();
					break;

				case "StartBlock":
					block.Stack.Add(action[1]);
					// Console.WriteLine($"StartBlock {action[1]}");
					break;
				case "EndBlock":
					block.Stack.Remove(block.Stack.Last());
					// Console.WriteLine($"EndBlock {action[1]}");
					break;

				case "Exit":
					// Console.WriteLine("return");
					return;

				case "Sleep":
					await Task.Delay(int.Parse(action[1]));
					break;

				case "ChangeRedBull":
					GameState.ChangeRedBull(int.Parse(action[1]));
					break;

				default:
					break;
			}
		}
	}
}