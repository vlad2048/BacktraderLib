using BaseUtils;

namespace FeedUtils;

public interface IUrlBuilder
{
	IUrlBuilder Add<T>(string key, T val);
	IUrlBuilder AddIf<T>(string key, T? val, Func<T, string>? valFun = null) where T : struct;
}

public static class UrlBuilder
{
	public static string Build(string baseUrl, Action<IUrlBuilder> fun)
	{
		var builder = new UrlBuilderImpl();
		fun(builder);
		return builder.Map.Count switch
		{
			0 => baseUrl,
			_ => $"{baseUrl}?{builder.Map.Select(e => $"{e.Key}={e.Value}").JoinText("&")}",
		};
	}
	

	sealed class UrlBuilderImpl : IUrlBuilder
	{
		public Dictionary<string, string> Map { get; } = new();

		public IUrlBuilder Add<T>(string key, T val)
		{
			Map[key] = $"{val}";
			return this;
		}

		public IUrlBuilder AddIf<T>(string key, T? val, Func<T, string>? valFun = null) where T : struct
		{
			if (val.HasValue)
				Map[key] = valFun switch
				{
					not null => valFun(val.Value),
					null => $"{val.Value}",
				};
			return this;
		}
	}
}