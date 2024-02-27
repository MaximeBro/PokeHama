namespace PokeHama.Extensions;

public static class ServicesExtensions
{
    public static void UseUploading(this WebApplication @this)
    {
        @this.MapGet("/api/files/{name:required}", (HttpResponse response, string name) =>
        {
            response.Headers.ContentDisposition = "inline";
            var stream = new StreamReader($"wwwroot/_content/pictures/{name}");
            return Results.Stream(stream.BaseStream, MimeTypes.GetMimeTypeOf(Path.GetExtension(name)));
        });
    }
}