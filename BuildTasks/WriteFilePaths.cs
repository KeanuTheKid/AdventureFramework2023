using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.Text.Json;

namespace BuildTasks;

public class WriteFilePaths : Microsoft.Build.Utilities.Task
{
	[Required]
	public string DirectoryPath { get; set; } = null!;

	[Required]
	public string OutputFilePath { get; set; } = null!;

	public string FilePrefix { get; set; } = "";

	public override bool Execute()
	{
		try
		{
			var files = Directory.GetFiles(DirectoryPath, "*.json", SearchOption.TopDirectoryOnly);
			var fileNames = files.Select(Path.GetFileName).Select(x => FilePrefix + x).ToArray();


			// // suppress warning cause andnoying
			// File.WriteAllLines(OutputFilePath, fileNames!);
			// return true;

			var json = JsonSerializer.Serialize(fileNames);
			File.WriteAllText(OutputFilePath, json);
			return true;
		}
		catch (Exception ex)
		{
			Log.LogErrorFromException(ex);
			return false;
		}
	}
}