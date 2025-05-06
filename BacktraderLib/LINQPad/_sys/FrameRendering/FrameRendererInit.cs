using System.Reflection;
using BacktraderLib._sys.FrameRendering.Utils;
using Frames;

namespace BacktraderLib._sys.FrameRendering;

static class FrameRendererInit
{
	static readonly MethodInfo methodRenderSerie = typeof(RendererSerie).GetMethods(BindingFlags.Public | BindingFlags.Static).Single(e => e.Name == nameof(RendererSerie.Render));
	static readonly MethodInfo methodRenderFrame = typeof(RendererFrame).GetMethods(BindingFlags.Public | BindingFlags.Static).Single(e => e.Name == nameof(RendererFrame.Render));
	static readonly MethodInfo methodRenderFrame2 = typeof(RendererFrame2).GetMethods(BindingFlags.Public | BindingFlags.Static).Single(e => e.Name == nameof(RendererFrame2.Render));

	public static void Init()
	{
		Frame.RendererSerie = obj => GenericFunctionUtils.Call<Tag>(methodRenderSerie, obj);
		Frame.RendererFrame = obj => GenericFunctionUtils.Call<Tag>(methodRenderFrame, obj);
		Frame.RendererFrame2 = obj => GenericFunctionUtils.Call<Tag>(methodRenderFrame2, obj);
	}
}