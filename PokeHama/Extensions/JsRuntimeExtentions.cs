using Microsoft.JSInterop;

namespace PokeHama.Extensions;

public static class JsRuntimeExtentions
{
    public static async Task<bool> OpenFullscreen(this IJSRuntime jsRuntime)
    {
        return await jsRuntime.InvokeAsync<bool>("openFullscreen");
    }

    public static async Task<bool> CloseFullscreen(this IJSRuntime jsRuntime)
    {
        return await jsRuntime.InvokeAsync<bool>("closeFullscreen");
    }
}