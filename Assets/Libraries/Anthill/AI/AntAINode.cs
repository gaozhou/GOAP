namespace Anthill.AI
{
	/// <summary>
	/// 世界状态节点。
	/// </summary>
	public class AntAINode
	{
		public AntAICondition Parent; // 前置状态
		public AntAICondition World;  // 此节点的世界状态
		public string Action;         // 该节点的动作
		public int Heuristic;         // 节点价值
		public int Cost;              // 节点成本
		public int Sum;               // 当前节点成本的总和
	}
}