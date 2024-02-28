using System.Text.Json;

namespace PokeHama.Services;

public class MiniGamesService
{
	private List<string> _failGuessMessages = new();

	public Task InitAsync()
	{
		var stream = File.OpenRead($"{Directory.GetCurrentDirectory()}\\Data\\fail_guess_messages.json");
		_failGuessMessages = JsonSerializer.Deserialize<List<string>>(stream) ?? new List<string>();

		return Task.CompletedTask;
	}
	
	public string GetRandomFailGuessMessage() => _failGuessMessages[new Random().Next(1, _failGuessMessages.Count)];
}