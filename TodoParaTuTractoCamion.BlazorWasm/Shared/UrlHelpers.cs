namespace TodoParaTuTractoCamion.BlazorWasm.Shared;

public static class UrlHelpers
{
    public static string GetSafeImageUrl(string? url, string? baseAddress = null)
    {
        if (string.IsNullOrEmpty(url)) return "";
        
        // Si ya es una URL completa (http/https), la dejamos como está
        if (url.StartsWith("http", System.StringComparison.OrdinalIgnoreCase)) 
            return url;
            
        // Si tenemos un baseAddress y la url es relativa
        if (!string.IsNullOrEmpty(baseAddress))
        {
            var cleanBase = baseAddress.TrimEnd('/');
            var cleanUrl = url.StartsWith("/") ? url : "/" + url;
            return $"{cleanBase}{cleanUrl}";
        }

        // Fallback: Si empieza con /, quitamos la barra inicial para que sea relativa al <base href>
        if (url.StartsWith("/")) 
            return url.Substring(1);
            
        return url;
    }
}
