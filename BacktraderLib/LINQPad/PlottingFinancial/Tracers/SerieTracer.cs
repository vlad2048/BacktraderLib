using Frames;

namespace BacktraderLib;

public static class SerieTracer
{
	public static ScatterTrace ToTrace<N>(this Serie<N> serie, string? name = null) => new()
	{
		X = serie.Index,
		Y = serie.Values,
		Name = name switch
		{
			not null => name,
			null => $"{serie.Name}",
		},
	};
}