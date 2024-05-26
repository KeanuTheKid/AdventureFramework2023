using Microsoft.JSInterop;

namespace Framework.Keyboard;

// defines the functions the others should call
// doesn't include ones that are more like "under the hood"
// so that the regular users don't accidentally break something
public interface IKeyboardService
{
	bool GetKeyState(string key);
	Dictionary<string, bool> GetKeyboardState();
	// Dictionary<string, bool> GetStaticKeyboardState();
	event EventHandler<KeyEventArgs> OnKeyDown;
	event EventHandler<KeyEventArgs> OnKeyUp;
}

public class KeyboardService(IJSRuntime jsRuntime) : IKeyboardService
{
	private readonly IJSRuntime jsRuntime = jsRuntime;
	private DotNetObjectReference<KeyboardService> objRef = null!;

	public async Task Init()
	{
		objRef = DotNetObjectReference.Create(this);
		await jsRuntime.InvokeVoidAsync("keyboard.init", objRef);
	}

	// this is a big dictionary of all the keys on the keyboard
	private readonly Dictionary<string, bool> keyboardState = new()
	{
		{ "Backspace", false},
		{ "Tab", false},
		{ "Enter", false},
		{ "ShiftLeft", false },
		{ "ShiftRight", false },
		{ "ControlLeft", false },
		{ "ControlRight", false },
		{ "AltLeft", false },
		{ "AltRight", false },
		{ "Pause", false },
		{ "CapsLock", false },
		{ "Escape", false },
		{ "Space", false },
		{ "PageUp", false },
		{ "PageDown", false },
		{ "End", false },
		{ "Home", false },
		{ "ArrowLeft", false },
		{ "ArrowUp", false },
		{ "ArrowRight", false },
		{ "ArrowDown", false },
		{ "PrintScreen", false },
		{ "Insert", false },
		{ "Delete", false },
		{ "Digit0", false },
		{ "Digit1", false },
		{ "Digit2", false },
		{ "Digit3", false },
		{ "Digit4", false },
		{ "Digit5", false },
		{ "Digit6", false },
		{ "Digit7", false },
		{ "Digit8", false },
		{ "Digit9", false },
		{ "KeyA", false },
		{ "KeyB", false },
		{ "KeyC", false },
		{ "KeyD", false },
		{ "KeyE", false },
		{ "KeyF", false },
		{ "KeyG", false },
		{ "KeyH", false },
		{ "KeyI", false },
		{ "KeyJ", false },
		{ "KeyK", false },
		{ "KeyL", false },
		{ "KeyM", false },
		{ "KeyN", false },
		{ "KeyO", false },
		{ "KeyP", false },
		{ "KeyQ", false },
		{ "KeyR", false },
		{ "KeyS", false },
		{ "KeyT", false },
		{ "KeyU", false },
		{ "KeyV", false },
		{ "KeyW", false },
		{ "KeyX", false },
		{ "KeyY", false },
		{ "KeyZ", false },
		{ "MetaLeft", false },
		{ "MetaRight", false },
		{ "ContextMenu", false },
		{ "Numpad0", false },
		{ "Numpad1", false },
		{ "Numpad2", false },
		{ "Numpad3", false },
		{ "Numpad4", false },
		{ "Numpad5", false },
		{ "Numpad6", false },
		{ "Numpad7", false },
		{ "Numpad8", false },
		{ "Numpad9", false },
		{ "NumpadMultiply", false },
		{ "NumpadAdd", false },
		{ "NumpadSubtract", false },
		{ "NumpadDecimal", false },
		{ "NumpadDivide", false },
		{ "F1", false },
		{ "F2", false },
		{ "F3", false },
		{ "F4", false },
		{ "F5", false },
		{ "F6", false },
		{ "F7", false },
		{ "F8", false },
		{ "F9", false },
		{ "F10", false },
		{ "F11", false },
		{ "F12", false },
		{ "NumLock", false },
		{ "ScrollLock", false },
		{ "Semicolon", false },
		{ "Equal", false },
		{ "Comma", false },
		{ "Minus", false },
		{ "Period", false },
		{ "Slash", false },
		{ "Backquote", false },
		{ "BracketLeft", false },
		{ "Backslash", false },
		{ "BracketRight", false },
		{ "Quote", false }
	};

	public event EventHandler<KeyEventArgs>? OnKeyDown;
	public event EventHandler<KeyEventArgs>? OnKeyUp;

	[JSInvokable]
	public void KeyDown(string key)
	{
		keyboardState[key] = true;
		OnKeyDown?.Invoke(this, new KeyEventArgs { Key = key, Down = true });
	}

	[JSInvokable]
	public void KeyUp(string key)
	{
		keyboardState[key] = false;
		OnKeyUp?.Invoke(this, new KeyEventArgs { Key = key, Down = false });
	}

	public bool GetKeyState(string key)
	{
		return keyboardState[key];
	}

	// public Dictionary<string, bool> GetKeyboardState() => keyboardState;

	public Dictionary<string, bool> GetKeyboardState()
	{
		Dictionary<string, bool> state = [];
		foreach (var kvp in keyboardState)
		{
			state.Add(kvp.Key, kvp.Value);
		}
		return state;
	}
}

public class KeyEventArgs : EventArgs
{
	public string Key { get; set; } = null!;
	public bool Down { get; set; }
}