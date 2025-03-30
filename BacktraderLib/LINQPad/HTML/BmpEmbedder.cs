using System.Drawing;
using System.Drawing.Imaging;

namespace BacktraderLib;

public static class BmpEmbedder
{
	public static Tag ToEmbeddedImageTag(this Bitmap bmp) => new("img")
	{
		Attributes =
		{
			{ "src", $"data:image/png;base64,{bmp.ToBase64(ImageFormat.Png)}" },
		},
		Style = [
			"object-fit: none",
		],
	};


	static string ToBase64(this Bitmap bmp, ImageFormat imageFormat)
	{
		var ms = new MemoryStream();
		bmp.Save(ms, imageFormat);
		ms.Position = 0;
		var buffer = ms.ToArray();
		ms.Close();
		return Convert.ToBase64String(buffer);
	}
}