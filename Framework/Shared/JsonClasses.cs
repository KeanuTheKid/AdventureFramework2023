namespace Framework.Slides.JsonClasses;

public class JsonSlide
{
	public string? Type { get; set; }
	public List<string>? Tags { get; set; }
	// can still be null, but safety will be ensured by verifier
	// removes annoying null warnings
	public string Image { get; set; } = null!;
	// nullable, safety by verifier
	// removes annoying null warnings
	public Dictionary<string, JsonButton> Buttons { get; set; } = null!;
	// Buttons are required, but you could specify some stuff in OnEnter
	// will cause exception if both OnEnter and Buttons are empty

	public List<List<string>>? OnEnter { get; set; }

	// Temporary for Minigame Compatibility
	public string? MinigameDefClassName { get; set; }
	public string? FallbackSlide { get; set; }
}

public class JsonButton
{
	public string Type { get; set; } = null!; //*req
	public List<string>? Tags { get; set; }
	public string Points { get; set; } = null!; //*req
	public string? Image { get; set; }
	// if you don't want actions, take emtpy array
	public List<List<string>> Actions { get; set; } = null!;
	public bool? Visible { get; set; }
	public int? ZIndex { get; set; }
}