namespace TodoParaTuTractoCamion.BlazorWasm.Shared;

public static class UrlHelpers
{
    public static string GetSafeImageUrl(string? url)
    {
        if (string.IsNullOrEmpty(url)) return "";
        
        // Si ya es una URL completa (http/https), la dejamos como está
        if (url.StartsWith("http", System.StringComparison.OrdinalIgnoreCase)) 
            return url;
            
        // Si empieza con /, quitamos la barra inicial para que sea relativa al <base href>
        // Esto es crucial para GitHub Pages (/TodoParaTuTractoCamion/)
        if (url.StartsWith("/")) 
            return url.Substring(1);
            
        return url;
    }
}
