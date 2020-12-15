using System.Collections.Generic;

namespace Anthill.AI
{
	public class AntAIState
	{
		public readonly string Name;

		private readonly List<string> _interruptions;
		protected bool Finished;
		
		public bool AllowForceInterrupting { get; protected set; }

		public AntAIState(string aName)
		{
			Name = aName;
			_interruptions = new List<string>();
			Finished = false;
			AllowForceInterrupting = true;
		}

		public void AddInterrupt(string aConditionName)
		{
			_interruptions.Add(aConditionName);
		}

		public virtual void Start()
		{
			// Вызывается перед началом выполнения задачи.
		}

		public virtual void Update(float aDeltaTime)
		{
			// Вызывается каждый игровой цикл пока задача активна.
		}

		public virtual void Stop()
		{
			// Вызывается после завершения выполнения задачи (даже если задача была прервана).
		}

		public virtual void Reset()
		{
			Finished = false;
		}

		public bool IsFinished(AntAIAgent aAgent, AntAICondition aWorldState)
		{
			return (Finished || OverlapInterrupts(aAgent.Planner, aWorldState));
		}

		public bool OverlapInterrupts(AntAIPlanner aPlanner, AntAICondition aConditions)
		{
			for (int i = 0, n = _interruptions.Count; i < n; i++)
			{
				var index = aPlanner.GetAtomIndex(_interruptions[i]);
				if (aConditions.GetValue(index))
				{
					return true;
				}
			}
			return false;
		}
	}
}