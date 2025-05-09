using System.Reactive;
using System.Text;
using BacktraderLib._sys;
using BaseUtils;
using LINQPad;
using RxLib;

namespace BacktraderLib;

public class Tag
{
	readonly AsyncSubject<Unit> whenRender = new AsyncSubject<Unit>().D(D);
	readonly string tagName;
	readonly string? text;

	public string Id { get; }

	public Tag(string tagName, string? id = null, string? text = null)
	{
		(this.tagName, Id, this.text) = (tagName, id ?? IdGen.Make(), text);


		var whenRenderEvtName = $"{Id}_OnRender";
		Events.Listen(whenRenderEvtName, _ =>
		{
			whenRender.OnNext(Unit.Default);
			whenRender.OnCompleted();
		});
		JS.Run(
			"""
			(async () => {
				const elt = await window.waitForElement(____0____);
				window.dispatch(____1____, '');
			})();
			""",
			e => e
				.JSRepl_Val(0, Id)
				.JSRepl_Val(1, whenRenderEvtName)
		);
	}





	public Tag Dyna<T>(IObservable<T>? obs, Func<T, ITagMutator, ITagMutator> action)
	{
		if (obs != null)
			whenRender
				.CombineLatest(obs)
				.Select(t => t.Second)
				.Subscribe(val =>
				{
					var mutator = new TagMutator(this);
					action(val, mutator);
					JS.RunOn(
						Id,
						"""
						elt => {
							____0____
						}
						""",
						e => e
							.JSRepl_Obj(0, mutator.JSCode)
					);
				}).D(D);
		return this;
	}




	public string? Class { get; init; }
	public string[]? Style { get; set; }
	public Dictionary<string, string?> Attributes { get; init; } = new();

	
	
	public Tag AddStyle(params string[] styles)
	{
		Style = Style switch
		{
			null => styles,
			not null => Style.ConcatA(styles),
		};
		return this;
	}

	public string OnRenderJS
	{
		init =>
			JS.RunOn(
				Id,
				"""
				elt => {
					____0____
				}
				""",
				e => e
					.JSRepl_Obj(0, value)
			);
	}

	public Action OnRender
	{
		init => whenRender.Subscribe(_ => value());
	}

	public Action? OnClick
	{
		init
		{
			if (value == null) return;
			var evtName = $"{Id}_OnClick";
			Events.ListenFast(evtName, value);
			JS.Run(
				"""
				(async () => {
					const elt = await window.waitForElement(____0____);
					elt.addEventListener('click', () => {
						window.dispatch(____1____, '');
					});
				})();
				""",
				e => e
					.JSRepl_Val(0, Id)
					.JSRepl_Val(1, evtName)
			);
		}
	}



	public Tag[] Kids { get; init; } = [];


	public object ToDump()
	{
		if (Kids.Length > 0 && text != null) throw new ArgumentException("Cannot use both Kids and text");
		return Util.RawHtml($"{this}");
	}
	

	public override string ToString()
	{
		var sb = new StringBuilder();

		sb.Append($"<{tagName}");

		sb.Append($" id='{Id}'");

		if (Class != null)
			sb.Append($" class='{Class}'");

		if (Style != null)
			sb.Append($" style='{string.Join(';', Style)}'");

		foreach (var (key, val) in Attributes)
		{
			if (val == null)
			{
			}
			else if (val.Equals("true", StringComparison.OrdinalIgnoreCase))
			{
				sb.Append($" {key}");
			}
			else if (val.Equals("false", StringComparison.OrdinalIgnoreCase))
			{
			}
			else
			{
				sb.Append($" {key}='{val}'");
			}
		}

		sb.Append(">");

		if (text != null)
			sb.Append(text);

		foreach (var kid in Kids)
			sb.Append($"{kid}");

		sb.Append($"</{tagName}>");
		return sb.ToString();
	}
}
