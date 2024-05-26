using Microsoft.JSInterop;

namespace Framework.Sound;

public interface ISoundService
{
	Task PlaySound(string path);
	Task PlayMusic(string path);
	Task StopMusic();
}


public class SoundService(IJSRuntime jsRuntime)
{
	private readonly IJSRuntime jsRuntime = jsRuntime;
	private DotNetObjectReference<SoundService> objRef = null!;

	public async Task Init(bool disableContextMenu = true)
	{
		objRef = DotNetObjectReference.Create(this);
		await jsRuntime.InvokeVoidAsync("sound.init", objRef);
		// Console.WriteLine("soundservice initialized correctly");
	}

	public async Task PlaySound(string path)
	{
		await jsRuntime.InvokeVoidAsync("sound.playSound", path);
	}
	public async Task PlayMusic(string path)
	{
		await jsRuntime.InvokeVoidAsync("sound.playMusic", path);
	}
	public async Task StopMusic()
	{
		await jsRuntime.InvokeVoidAsync("sound.stopMusic");
	}
}