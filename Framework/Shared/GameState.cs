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
	void SetVisibility(string name, bool value);
	bool CheckVisibility(string name);

	void AddItem(string id);
	void RemoveItem(string id);
	bool CheckForItem(string id);

	// minigames probably should be seperated from other stuff
	// if value exists, set it, else create new entry

	//TODO Implement This:
	void SetMinigame(string name, MinigameDefBase minigame);
	void GetMinigame(string name);

	string CurrentSlide { get; set; }

	// need to think about the name
	//TODO Implement this:
	void SetFromSaveString(string saveString);
	string GetSaveString();
}

public class GameState
{
	private readonly IToastService ToastService;

	protected JsonUtility JsonUtility { get; set; } = null!;

	protected Items Items { get; set; }
	//Initialize Inventory
	private static List<string> ItemsInInventory = new();

	private static Dictionary<string, bool> State = new();

	public GameState(JsonUtility jsonUtility, Items items, IToastService toastService)
	{
		JsonUtility = jsonUtility;
		Items = items;
		ToastService = toastService;

	}

	public async Task LoadGameStateAndItemsAsync(string path = "gamestate.json")
	{
		State = await JsonUtility.LoadFromJsonAsync<Dictionary<string, bool>>(path);
		await Items.LoadItemsAsync();

	}

	public void SetVisibility(string name, bool value)
	{
		State[name] = value;
	}

	public void ChangeVisibility(string name)
	{
		State[name] = !State[name];
	}

	public bool CheckVisibility(string name)
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
	public bool CheckForVisibility(string name) => State.ContainsKey(name);


	public void AddVisibility(string name, bool value)
	{
		State.Add(name, value);
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
			if (Items.items.ContainsKey(id))
			{
				ItemObjects.Add(id, Items.items[id]);
			}

		}

		return ItemObjects;
	}

	public string GetSaveString()
	{
		// Console.WriteLine("GetSaveString called");
		GameStateData data = new GameStateData(ItemsInInventory, State);
		// foreach (var item in data.Items)
		// {
		// 	Console.WriteLine(item);
		// }
		return ObjectEncoder.EncodeObject(data);
	}

	public void SetFromSaveString(string hex)
	{
		// Console.WriteLine("SetFromSaveString called");
		GameStateData? data = ObjectEncoder.DecodeObject<GameStateData>(hex) ?? throw new Exception("GameStateData is null");
		GameState.State = data.gameState;

		// foreach (var item in data.Items)
		// {
		// 	Console.WriteLine(item);
		// }

		GameState.ItemsInInventory = data.Items;
	}
	public class GameStateData
	{

		public List<string> Items { get; }
		public Dictionary<string, bool> gameState { get; }
		//public Dictionary<string, object> Minigames => Minigames;
		public GameStateData(List<string> items, Dictionary<string, bool> gamestate)
		{

			Items = items;
			gameState = gamestate;
		}
	}

}


