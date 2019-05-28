namespace ShiftCheck.Services;

public static class HubConnectionSettings
{
	private const string PreferenceKey = "hub_api_url";
	public const string DefaultBaseUrl = "https://hub.example.invalid/api/";

	public static string BaseUrl
	{
		get => Preferences.Default.Get(PreferenceKey, DefaultBaseUrl);
		set => Preferences.Default.Set(PreferenceKey, value);
	}

	public static bool TryNormalize(string? value, out string normalized)
	{
		normalized = string.Empty;
		if (!Uri.TryCreate(value?.Trim(), UriKind.Absolute, out var uri) ||
			(uri.Scheme != Uri.UriSchemeHttps && uri.Scheme != Uri.UriSchemeHttp) ||
			!string.IsNullOrEmpty(uri.UserInfo) ||
			!string.IsNullOrEmpty(uri.Query) ||
			!string.IsNullOrEmpty(uri.Fragment))
			return false;

#if !DEBUG
		if (uri.Scheme != Uri.UriSchemeHttps)
			return false;
#endif

		var path = uri.AbsolutePath.TrimEnd('/');
		if (path.Length > 0 && !path.EndsWith("/api", StringComparison.OrdinalIgnoreCase))
			return false;

		normalized = uri.AbsoluteUri.TrimEnd('/') + (path.Length == 0 ? "/api/" : "/");
		return true;
	}
}
