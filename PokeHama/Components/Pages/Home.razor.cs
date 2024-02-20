namespace PokeHama.Components.Pages;

public partial class Home
{
	private List<string> _pokemons = new();
	private int _amountToDisplay = 151;

	private static readonly string _default_url = "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/versions/generation-vii/icons/";
	
	
	protected override Task OnInitializedAsync()
	{
		for (int i = 1; i <= _amountToDisplay; i++)
		{
			_pokemons.Add($"{_default_url}{i}.png");
		}

		return Task.CompletedTask;
	}
}