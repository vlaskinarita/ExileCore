using ExileCore.Shared.Attributes;
using ExileCore.Shared.Nodes;

namespace ExileCore;

[Submenu]
public class CorePerformanceSettings
{
	[Menu("Added entities multi-threading", "Just for test, most of plugin dont have expensive logic for turn on that option.")]
	public ToggleNode AddedMultiThread { get; set; } = new ToggleNode(value: false);


	public ToggleNode CoroutineMultiThreading { get; set; } = new ToggleNode(value: false);


	public ToggleNode ParseEntitiesInMultiThread { get; set; } = new ToggleNode(value: false);


	[Menu("Debug information", "With this option you can check how much every plugin works.")]
	public ToggleNode CollectDebugInformation { get; set; } = new ToggleNode(value: true);


	[Menu("Threads count", "How much threads to use for prepare work.")]
	public RangeNode<int> Threads { get; set; } = new RangeNode<int>(2, 0, 4);


	[Menu("Target FPS")]
	public RangeNode<int> TargetFps { get; set; } = new RangeNode<int>(60, 5, 200);


	public RangeNode<int> TargetParallelCoroutineFps { get; set; } = new RangeNode<int>(60, 30, 500);


	[Menu(null, "How often to update entities. You can see time spent on this in DebugWindow->Coroutines.")]
	public RangeNode<int> EntitiesFps { get; set; } = new RangeNode<int>(60, 5, 200);


	[Menu("Parse server entities")]
	public ToggleNode ParseServerEntities { get; set; } = new ToggleNode(value: false);


	[Menu("Collect entities in parallel when more than X")]
	public ToggleNode CollectEntitiesInParallelWhenMoreThanX { get; set; } = new ToggleNode(value: false);


	[Menu("Limit draw plot in ms", "Don't put small value, because plot need a lot triangles and DebugWindow with a lot plot will be broke.")]
	public RangeNode<float> LimitDrawPlot { get; set; } = new RangeNode<float>(0.2f, 0.05f, 20f);

}
