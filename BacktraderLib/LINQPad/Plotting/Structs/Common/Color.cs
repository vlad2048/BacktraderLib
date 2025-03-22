namespace BacktraderLib;

public abstract record Color
{
	public static Color Hex(uint v) => new ColorRGBA
	{
		R = (byte)((v >> 16) & 0xff),
		G = (byte)((v >> 8) & 0xff),
		B = (byte)((v >> 0) & 0xff),
	};
	public static Color HexA(uint v) => new ColorRGBA
	{
		R = (byte)((v >> 16) & 0xff),
		G = (byte)((v >> 8) & 0xff),
		B = (byte)((v >> 0) & 0xff),
		A = (byte)((v >> 24) & 0xff),
	};
}

public sealed record ColorRGBA : Color
{
	public byte R { get; init; }
	public byte G { get; init; }
	public byte B { get; init; }
	public byte A { get; init; } = 255;
}