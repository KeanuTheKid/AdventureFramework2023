using Framework.Items;
using Framework.State;
using Framework.Slides.JsonClasses;


namespace Framework.Slides;

// Couldn't resist my urge to overengineer again
// basically just for the name
public class SlidesJsonException : Exception
{
	public SlidesJsonException() { }
	public SlidesJsonException(string message) : base(message) { }
	public SlidesJsonException(string message, Exception inner) : base(message, inner) { }
}

public class SlidesVerifier(GameState gameState, ItemService items)
{
	private readonly GameState gameState = gameState;
	private readonly ItemService items = items;

	private Dictionary<string, JsonSlide>? CurrentState { get; set; }

	private static readonly string[] SetGameStateOptions = ["true", "false", "toggle"];
	private static readonly string[] ButtonTypeOptions = ["rect", "polygon", "image", "circle", "preset"];
	private static readonly string[] PresetOptions = ["left", "right", "top", "bottom"];

	private string? StartSlide { get; set; } = null;


	public void Init(Dictionary<string, JsonSlide> slides)
	{
		CurrentState = slides;
		StartSlide = null;
	}


	public void VerifySlides(Dictionary<string, JsonSlide> slides)
	{
		CurrentState = slides;
		foreach (var kvp in slides)
		{
			try
			{
				VerifySlide(kvp.Key, kvp.Value);
			}
			catch (SlidesJsonException e)
			{
				throw new SlidesJsonException($"Error in Slides.json: ", e);
			}
		}
		// if there is no slide with the Tag "START", throw exception
		if (StartSlide is null)
		{
			throw new SlidesJsonException("No slide with tag \"START\" found");
		}
	}

	public void VerifySlide(string id, JsonSlide slide)
	{
		// tags need to be checked for both minigames and normal slides
		if (slide.Tags is not null)
		{
			if (slide.Tags.Contains("START"))
			{
				if (StartSlide != null)
				{
					throw new SlidesJsonException(
						$"At Slide \"{id}\": Slide with Tag \"START\" already exists at \"{StartSlide}\"");
				}
				else
				{
					StartSlide = id;
				}
			}
		}
		// check if is minigame, if yes, other stuff applies
		if (slide.Type == "Minigame")
		{
			if (slide.MinigameDefClassName is null)
			{
				throw new SlidesJsonException($"At Slide \"{id}\": \"MinigameDefClassName\" undefined");
			}
			// // if (slide.FallbackSlide is null)
			// // {
			// // 	throw new SlidesJsonException($"At Slide \"{id}\": \"FallbackSlide\" undefined");
			// // }
		}
		else
		{
			// check important things
			// image can't be null
			if (slide.Image is null)
			{
				throw new SlidesJsonException($"At Slide \"{id}\": \"Image\" undefined");
			}
			// slide buttons can't be null, but can be empty
			if (slide.Buttons is null)
			{
				throw new SlidesJsonException($"At Slide \"{id}\": \"Buttons\" undefined");
			}
			// if buttons is empty, OnEnter can't be null or empty
			if (slide.Buttons.Count == 0)
			{
				if (slide.OnEnter is null)
				{
					throw new SlidesJsonException($"At Slide \"{id}\": \"Buttons\" emtpy and \"OnEnter\" undefined");
				}
				else
				{
					if (slide.OnEnter.Count == 0)
					{
						throw new SlidesJsonException($"At Slide \"{id}\": \"Buttons\" emtpy and \"OnEnter\" empty");
					}
				}
			}

			// iterate over buttons and pass on exceptions thrown in button verifier method
			foreach (var idAndButton in slide.Buttons)
			{
				try
				{
					VerifyButton(idAndButton.Key, idAndButton.Value);
				}
				catch (SlidesJsonException e)
				{
					throw new SlidesJsonException($"At Slide \"{id}\", in buttons: ", e);
				}
			}
		}

	}

	public void VerifyButton(string id, JsonButton button)
	{
		// type can't be null
		if (button.Type is null)
		{
			throw new SlidesJsonException($"At Button \"{id}\": \"Type\" undefined");
		}
		if (!ButtonTypeOptions.Contains(button.Type))
		{
			throw new SlidesJsonException($"At Button \"{id}\": \"{button.Type}\" is not a valid type option");
		}

		if (button.Type == "preset")
		{
			if (button.Image is null)
			{
				throw new SlidesJsonException($"At Button \"{id}\": \"Type\" is \"preset\" and \"Image\" undefined");
			}
			if (!PresetOptions.Contains(button.Image))
			{
				throw new SlidesJsonException($"At Button \"{id}\": \"{button.Image}\" is not a valid preset option");
			}
		}
		// presets are a bit special, so check other stuff only if not preset
		else
		{
			if (button.Points is null)
			{
				throw new SlidesJsonException($"At Button \"{id}\": \"Points\" undefined");
			}
			if (button.Type == "image")
			{
				if (button.Image is null)
				{
					throw new SlidesJsonException($"At Button \"{id}\": \"Type\" is \"image\" and \"Image\" undefined");
				}
			}
		}
		//? check with Andrii if no actions is very frequent, if yes, make them nullable
		if (button.Actions is null)
		{
			throw new SlidesJsonException($"At Button \"{id}\": \"Actions\" undefined");
		}
		try
		{
			VerifyActions(button.Actions);
		}
		catch (SlidesJsonException e)
		{
			throw new SlidesJsonException($"At button \"{id}\" in actions: ", e);
		}
	}

	public void VerifyActions(List<List<string>> actions)
	{
		// to make sure that current state is always set
		if (CurrentState is null)
		{
			throw new Exception("State must be set before calling VerifyActions. Consider calling Init before");
		}
		List<string> blockStarts = [];
		List<string> blockEnds = [];
		for (int i = 0; i < actions.Count; i++)
		{
			var action = actions[i];
			// there is no action that takes different number than 2/3 params
			if (action.Count > 3 || action.Count < 2)
			{
				throw new SlidesJsonException($"At action {i}: To many or to few params");
			}

			// check for the actions that require 1 param, throw ex if more params required
			if (action.Count == 2)
			{
				if (action[0] == "Route")
				{
					// if Slide to route to doesn't exist
					if (!CurrentState.ContainsKey(action[1]))
					{
						throw new SlidesJsonException($"At action {i}: Route: No Slide with id \"{action[1]}\" found");
					}
					continue;
				}
				else if (action[0] == "AddItem" || action[0] == "RemoveItem")
				{
					// check if item exists
					if (!items.DoesItemExist(action[1]))
					{
						throw new SlidesJsonException(
							$"At action {i}: {(action[0] == "AddItem" ? "AddItem" : "RemoveItem")}: "
							+ $"No item with id \"{action[1]}\" found"
						);
					}
					continue;
				}
				else if (action[0] == "RequireItem")
				{
					var x = action[1].StartsWith('!') ? action[1][1..] : action[1];
					if (!items.DoesItemExist(x))
					{
						throw new SlidesJsonException($"At action {i}: RequireItem: No item with id \"{x}\" found");
					}
					continue;
				}
				else if (action[0] == "RequireGameState")
				{
					var x = action[1].StartsWith('!') ? action[1][1..] : action[1];
					if (!gameState.CheckForState(x))
					{
						throw new SlidesJsonException($"At action {i}: RequireGameState: No GameState with key \"{x}\" found");
					}
					continue;
				}
				else if (action[0] == "StartBlock")
				{
					blockStarts.Add(action[1]);
					continue;
				}
				else if (action[0] == "EndBlock")
				{
					blockEnds.Add(action[1]);
					continue;
				}
				else if (action[0] == "Exit")
				{
					// do nothing here, as the params don't actually matter
				}
				else if (action[0] == "Sleep")
				{
					if (!int.TryParse(action[1], out _))
					{
						throw new SlidesJsonException($"At action {i}: Sleep: \"{action[1]}\" is not a valid number");
					}

				}
				else if (action[0] == "PlaySound")
				{
					// can't really do anything, as it's a file path
				}
				else if (action[0] == "PlayMusic")
				{
					// can't really do anything, as it's a file path
				}
				else if (action[0] == "StopMusic")
				{
					// do nothing here, as the params don't actually matter
				}
				else
				{
					// if not found yet, it has to be invalid action
					throw new SlidesJsonException($"At action {i}: Unknown Action \"{action[0]}\"");
				}
			}
			// acitons with 3 params
			else if (action.Count == 3)
			{
				if (action[0] == "SetGameState")
				{
					// check if gamestate exists
					// var x = action[1].StartsWith('!') ? action[1][1..] : action[1];
					if (!gameState.CheckForState(action[1]))
					{
						throw new SlidesJsonException($"At action {i}: SetGameState: No GameState with key \"{action[1]}\" found");
					}
					if (!SetGameStateOptions.Contains(action[2]))
					{
						throw new SlidesJsonException($"At action {i}: SetGameState: \"{action[2]}\" is not a possible param");
					}
					continue;
				}
				else
				{
					throw new SlidesJsonException($"At action {i}: Unknown Action \"{action[0]}\"");
				}
			}
		}
		// check if the StartBlock match the EndBlock
		if (!(blockStarts.Count == blockEnds.Count))
		{
			throw new SlidesJsonException($"Block mismatch: Not the same amount of StartBlock and EndBlock");
		}
		foreach (var x in blockStarts)
		{
			if (!blockEnds.Contains(x))
			{
				throw new SlidesJsonException($"Block mismatch: StartBlock \"{x}\" has no matching EndBlock");
			}
		}
		foreach (var x in blockEnds)
		{
			if (!blockStarts.Contains(x))
			{
				throw new SlidesJsonException($"Block mismatch: EndBlock \"{x}\" has no matching StartBlock");
			}
		}
	}
}
