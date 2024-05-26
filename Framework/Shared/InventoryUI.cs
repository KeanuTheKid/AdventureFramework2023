using Microsoft.AspNetCore.Components;
using GameStateInventory;
using FrameworkItems;
using JsonUtilities;
using Microsoft.JSInterop;
using static InventoryEvent;
using Blazored.Video;
namespace Framework.InventoryUI;


public class InventoryUIBase : ComponentBase
{
	[Inject]
	protected GameState? GameState { get; set; }

	[Inject]
	protected JsonUtility? JsonUtility { get; set; }

	[Inject]
	protected Items? Items { get; set; }

	public Dictionary<string, Item> InvItems = new();
	protected override void OnInitialized()
	{
		// base.OnInitialized();
		// Subscribe to the ItemAdded event

		//! This resets the game state. Don't ever do this except at the beginning
		// await GameState.LoadGameStateAndItemsAsync();

		InventoryEvent.ItemAdded += HandleItemAdded;



		if (GameState == null)
		{
			// Log an error, throw an exception, or handle the null GameState case appropriately.
			Console.WriteLine("GameState is null!");
			return;
		}

		//! Please remove tests after not using them anymore
		// // Console.WriteLine("frog and key in Inventory");
		// GameState.AddItem("frog");
		// GameState.AddItem("goldkey");

		// // Console.WriteLine(GameState.GetSaveString());
		// GameState.RemoveItem("frog");
		// GameState.RemoveItem("goldkey");
		// // Console.WriteLine("remove all items");

		// // Console.WriteLine(GameState.GetSaveString());


		InvItems = GameState.GetItemObjects();

		StateHasChanged();

		//! This also resets the GameState. Don't do this at unforseeable places
		// GameState.SetFromSaveString("7B224974656D73223A5B2266726F67222C22676F6C646B6579225D2C2267616D655374617465223A7B22627574746F6E31223A747275652C224C65427574746F6E223A66616C73652C22436F64655465726D696E616C223A747275652C22446F6F72223A747275657D7D");

		// InvItems = GameState.GetItemObjects();

		StateHasChanged();

		return;

	}

	private void HandleItemAdded(object sender, InventoryEvent.ItemAddedEventArgs e)
	{
		// Handle the item added event here
		// For example, update the UI, perform some action, etc.

		InvItems = GameState!.GetItemObjects();

		StateHasChanged(); // Update the UI if needed
	}

}