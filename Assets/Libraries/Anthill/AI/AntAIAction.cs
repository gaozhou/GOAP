namespace Anthill.AI
{
	public class AntAIAction
	{
		public int Cost;            		 // 消耗
		public readonly string Name;         // 名字
		public string State;        		 // 关联的状态
		public readonly AntAICondition Pre;  // 前置条件
		public readonly AntAICondition Post; // 后置条件

		public AntAIAction(string aName, int aCost = 1)
		{
			Cost = aCost;
			Name = aName;
			State = aName;
			Pre = new AntAICondition();
			Post = new AntAICondition();
		}
	}
}