namespace BacktraderLib;

public sealed record ClickArgs(
	MousePt MousePos,
	MouseButton MouseBtn,
	EvtKeys Keys,
	Loc Loc,
	EvtRange Xaxis,
	EvtRange Yaxis,
	int CurveNumber,
	int PointIndex,
	int PointNumber
)
{
	internal static readonly string JSCode = JS.Fmt(
		"""
		const e = evt.event;
		const pt = evt.points[0];
		return {
			mousePos: {x:e.clientX, y:e.clientY},
			mouseBtn: e.button,
			keys: {alt:e.altKey, ctrl:e.ctrlKey, meta:e.metaKey, shift:e.shiftKey},
			loc:  {x:pt.x, y:pt.y, z:pt.z},
			xaxis: {min:pt.xaxis.range[0], max: pt.xaxis.range[1]},
			yaxis: {min:pt.yaxis.range[0], max: pt.yaxis.range[1]},
			curveNumber: pt.curveNumber,
			pointIndex:  pt.pointIndex,
			pointNumber: pt.pointNumber,
		};
		"""
	);
}
