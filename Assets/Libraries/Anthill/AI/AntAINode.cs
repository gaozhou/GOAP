namespace Anthill.AI
{
	/// <summary>
	/// Узел данных сети мировых состояний.
	/// </summary>
	public class AntAINode
	{
		public AntAICondition Parent; // Состояние из которого мы пришли.
		public AntAICondition World;  // Состояние мира для этого узла.
		public string Action;         // Действие которое привело к этому узлу.
		public int Heuristic;         // Остаточная стоимость.
		public int Cost;              // Стоимость узла.
		public int Sum;               // Сумма heruistic и cost.
	}
}