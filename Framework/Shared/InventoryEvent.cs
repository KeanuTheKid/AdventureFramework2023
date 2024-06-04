// using System;

// public static class InventoryEvent
// {
//     // Define an event handler delegate
//     public delegate void ItemAddedEventHandler(object sender, ItemAddedEventArgs e);

//     // Define an event args class
//     public class ItemAddedEventArgs : EventArgs
//     {
//         public string? ItemId { get; set; }
//     }

//     // Define a static event
//     public static event ItemAddedEventHandler? ItemAdded;

//     // Method to trigger the ItemAdded event
//     public static void OnItemAdded(object sender, ItemAddedEventArgs e)
//     {
//         // Console.WriteLine("OnItemAdded called");
//         ItemAdded?.Invoke(sender, e);
//     }
// }
