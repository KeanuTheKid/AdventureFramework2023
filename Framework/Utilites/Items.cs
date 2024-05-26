using JsonUtilities; // For Fetching Json. Async Functions
using Microsoft.AspNetCore.Components; // For Injecting.

//TODO rename this to Framework.Items
namespace FrameworkItems
{
    public class Item
    {
        //* Declare Item Object to use as Type in Json file loading *//
        public string Name { get; }
        public string Description { get; }
        public string Image { get; }

        public Item(string name, string description, string image)
        {
            Name = name;
            Description = description;
            Image = image;
        }
    }

    public class Items
    {
        protected JsonUtility JsonUtility { get; set; } = null!;

        public Items(JsonUtility jsonUtility)
        {
            JsonUtility = jsonUtility;
        }

        public static Dictionary<string, Item> items = new();

        public async Task LoadItemsAsync(string path = "items.json")
        {
            //* Load Items from items.json *

            try
            {
                items = await JsonUtility.LoadFromJsonAsync<Dictionary<string, Item>>(path);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Items.LoadItemsAsync: {ex.Message}");
            }
        }

        //Seems kinda useless
        public Item? GetPropertiesByName(string ItemName)
        {
            //*Returns Item Object With: Name, Desc. and Image Path*// 
            try
            {
                // Make sure items.json has been read
                if (items.Count == 0)
                {
                    throw new Exception("No items read, Call LoadItemsAsync() first.");
                }

                // Console.WriteLine(items[ItemName].Description);
                // Console.WriteLine(items[ItemName].Image);
                return items[ItemName];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Items.GetPropertiesByName: {ex?.Message}");
                return null; // or handle the error accordingly
            }
        }

        public bool DoesItemExist(string ItemName)
        {
            string p = "";

            foreach (KeyValuePair<string, Item> kvp in items)
            {
                //textBox3.Text += ("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
                p += string.Format("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
            }

            // Console.WriteLine(p);

            return items.ContainsKey(ItemName);
        }

        public Dictionary<string, Item> GetAllItems()
        {
            return items;
        }
    }
}
