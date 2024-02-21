using PokeHama.Extensions;

namespace PokeHama.Components.Pages;

public partial class Home
{
	private List<string> _pokemons = new();
	private List<string> _filtered = new();
	private bool _loading;
	private int _amountToDisplay = 807;

	public int AmoutToDisplay
	{
		get => _amountToDisplay;
		set
		{
			_amountToDisplay = value;
			_filtered = _pokemons.Take(value).ToList();
			StateHasChanged();
		}
	}
	
	
	protected override Task OnInitializedAsync()
	{
		_loading = true;
		
		for (int i = 1; i <= _amountToDisplay; i++)
		{
			_pokemons.Add($"{Hardcoded.IconUrl}{i}.png");
		}

		_filtered = _pokemons.ToList();
		_loading = false;
		return Task.CompletedTask;
	}
}