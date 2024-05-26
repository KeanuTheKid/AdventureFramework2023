using Microsoft.AspNetCore.Components;

using JsonUtilities;
using FrameworkItems;
using static InventoryEvent;
using Microsoft.JSInterop;
using ObjectEncoding;
using Framework.Minigames;
//Notifications
using Blazored.Toast.Services;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace GameStateInventory;

public interface IGameState
{
	// if value exists, set it, else create new entry
	void SetState(string name, bool value);
	bool GetState(string name);
	bool CheckForState(string name);
	bool TryGetState(string name, out bool value);
	void ToggleState(string name);
	bool TryToggleState(string name);

	void AddItem(string id);
	void RemoveItem(string id);
	bool CheckForItem(string id);

	//* Scrapped idea for minigame saving in GameState

	string CurrentSlide { get; set; }

	void SetFromSaveString(string saveString);
	string GetSaveString();
}

// interface for the minigames so that they don't accidentally overwrite everything
// they can still brick everything tho, but they have to try harder
public interface IMinigameGameState
{
	void SetState(string name, bool value);
	bool GetState(string name);
	bool CheckForState(string name);
	bool TryGetState(string name, out bool value);
	void ToggleState(string name);
	bool TryToggleState(string name);
	void AddItem(string id);
	void RemoveItem(string id);
	bool CheckForItem(string id);
}

public class GameState(JsonUtility jsonUtility, Items items, IToastService toastService) : IGameState
{
	// dependencies
	private readonly IToastService ToastService = toastService;
	private readonly JsonUtility JsonUtility = jsonUtility;
	private readonly Items Items = items;


	// initialize with empty data
	private GameStateData Data = new();

	// they just point to the data, so you can swap it out by just changing the data, not the properties
	private List<string> ItemsInInventory { get { return Data.Items; } set { Data.Items = value; } }
	private Dictionary<string, bool> State { get { return Data.GameState; } set { Data.GameState = value; } }
	// should be set by the slide service
	public string CurrentSlide { get { return Data.CurrentSlide; } set { Data.CurrentSlide = value; } }


	public async Task LoadGameStateAndItemsAsync(string path = "gamestate.json")
	{
		State = await JsonUtility.LoadFromJsonAsync<Dictionary<string, bool>>(path);
		await Items.LoadItemsAsync();
	}

	public void SetState(string name, bool value)
	{
		if (!State.TryAdd(name, value))
		{
			State[name] = value;
		}
	}

	public void ToggleState(string name)
	{
		State[name] = !State[name];
	}

	public bool GetState(string name)
	{
		try
		{
			return State[name];
		}
		catch (KeyNotFoundException)
		{
			// Everything is visible by default
			return true;
		}
	}
	public bool CheckForState(string name) => State.ContainsKey(name);
	// kinda pointless, but I had the urge to overengineer
	public bool TryGetState(string name, out bool value) => State.TryGetValue(name, out value);
	public bool TryToggleState(string name)
	{
		if (State.TryGetValue(name, out bool value))
		{
			State[name] = !value;
			return true;
		}
		return false;
	}

	public void RemoveItem(string id)
	{
		bool removed = ItemsInInventory.Remove(id);

		if (!removed)
		{
			throw new ArgumentException($"Element {id} is not in Inventory");
		}
		// Console.WriteLine($"Successfully removed {id} from inventory");
	}

	public void AddItem(string id)
	{

		if (Items.DoesItemExist(id) == false)
		{
			throw new Exception("Item doesn't exist in items.json Dictionary");
		}
		ItemsInInventory.Add(id);

		ToastService.ShowSuccess($"Added {id} to inventory");

		//Event handler for updateing inventory images
		InventoryEvent.OnItemAdded(this, new ItemAddedEventArgs { ItemId = id });
	}

	public bool CheckForItem(string id)
	{
		return ItemsInInventory.Contains(id);
	}

	public List<string> GetItemStrings()
	{
		return ItemsInInventory;
	}
	public Dictionary<string, Item> GetItemObjects()
	{
		Dictionary<string, Item> ItemObjects = new();

		foreach (string id in ItemsInInventory)
		{
			if (Items.items.TryGetValue(id, out Item? value))
			{
				ItemObjects.Add(id, value);
			}
		}
		return ItemObjects;
	}

	public string GetSaveString()
	{
		return ObjectEncoder.EncodeObject(Data);
	}

	public void SetFromSaveString(string hex)
	{
		GameStateData data = ObjectEncoder.DecodeObject<GameStateData>(hex) ??
		throw new Exception("GameStateData is null");
		Data = data;
	}
}

public class GameStateData
{
	public List<string> Items { get; set; } = [];
	public Dictionary<string, bool> GameState { get; set; } = [];
	public string CurrentSlide { get; set; } = "";

	//public Dictionary<string, Dictionary<string, > DialogueProgress {get; set;} = [];
}

public class DialogueProgress
{

	public string QuestName { get; set; }
	public List<List<string>> Messages { get; set; }
	public int Progress { get; set; }
	public DialogueProgress(string questName, List<List<string>> messages, int progress)
	{
		QuestName = questName;
		Messages = messages;
		Progress = progress;
	}

}

