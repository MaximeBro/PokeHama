using PokeHama.Extensions;

namespace PokeHama.Components.Pages;

public partial class Home
{
	private List<string> _pokemons = new();
	private bool _loading;
	private int _amountToDisplay = 807;
	
	
	protected override Task OnInitializedAsync()
	{
		_loading = true;
		
		for (int i = 1; i <= _amountToDisplay; i++)
		{
			_pokemons.Add($"{Hardcoded.IconUrl}{i}.png");
		}

		_loading = false;
		return Task.CompletedTask;
	}
}