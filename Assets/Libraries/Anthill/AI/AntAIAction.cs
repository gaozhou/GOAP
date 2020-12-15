namespace Anthill.AI
{
	public class AntAIAction
	{
		public int Cost;            // Цена действия.
		public readonly string Name;         // Имя действия.
		public string State;        // Имя состояния связанного с этим действием.
		public readonly AntAICondition Pre;  // Предстоящие условия.
		public readonly AntAICondition Post; // Последующие условия.

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