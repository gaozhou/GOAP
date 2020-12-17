using System.Collections.Generic;
using System.Text;

namespace Anthill.AI
{
	public class AntAIPlanner
	{
		public const int MAX_ATOMS = sizeof(int) * 8;
		public const int MAX_ACTIONS = sizeof(int) * 8;

		public delegate void PlanUpdatedDelegate(AntAIPlan aNewPlan);
		public event PlanUpdatedDelegate EventPlanUpdated;

		public readonly List<string> Atoms = new List<string>();
		public readonly List<AntAIAction> Actions = new List<AntAIAction>();
		public readonly List<AntAICondition> Goals = new List<AntAICondition>();

		#if UNITY_EDITOR
		public AntAICondition DebugConditions; // Used for AI Debugger only.
		#endif

		public void LoadScenario(AntAIScenario aScenario)
		{
			// AntLog.Trace("<b>Action List</b>");
			for (int i = 0, n = aScenario.actions.Length; i < n; i++)
			{
				var scenarioAction = aScenario.actions[i];
				var action = GetAction(scenarioAction.name);
				action.State = scenarioAction.state;
				action.Cost = scenarioAction.cost;

				// AntLog.Trace("Action: {0}", action.name);

				int atomIndex;
				for (int j = 0, nj = scenarioAction.pre.Length; j < nj; j++)
				{
					atomIndex = GetAtomIndex(aScenario.conditions.GetName(scenarioAction.pre[j].id));
					action.Pre.Set(atomIndex, scenarioAction.pre[j].value);
					/*AntLog.Trace("Pre -> {0}:{1}", 
						aScenario.conditions.GetName(scenarioAction.pre[j].id),
						scenarioAction.pre[j].value);*/
				}

				for (int j = 0, nj = scenarioAction.post.Length; j < nj; j++)
				{
					atomIndex = GetAtomIndex(aScenario.conditions.GetName(scenarioAction.post[j].id));
					action.Post.Set(atomIndex, scenarioAction.post[j].value);
					/*AntLog.Trace("Post -> {0} : {1}", 
						aScenario.conditions.GetName(scenarioAction.post[j].id),
						scenarioAction.post[j].value);*/
				}
			}

			// AntLog.Trace("<b>Goal List</b>");

			for (int i = 0, n = aScenario.goals.Length; i < n; i++)
			{
				var scenarioGoal = aScenario.goals[i];
				var goal = GetGoal(scenarioGoal.name);

				//AntLog.Trace("Goal: {0}", goal.name);

				for (int j = 0, nj = scenarioGoal.conditions.Length; j < nj; j++)
				{
					goal.Set(this, aScenario.conditions.GetName(scenarioGoal.conditions[j].id),
						scenarioGoal.conditions[j].value);
					/*AntLog.Trace("Cond -> {0} : {1}", 
						aScenario.conditions.GetName(scenarioGoal.conditions[j].id),
						scenarioGoal.conditions[j].value);*/
				}
			}
		}

		public bool Pre(string aActionName, string aAtomName, bool aValue)
		{
			var action = GetAction(aActionName);
			var atomId = GetAtomIndex(aAtomName);
			if (action == null || atomId == -1)
			{
				return false;
			}
			return action.Pre.Set(atomId, aValue);
		}

		public bool Post(string aActionName, string aAtomName, bool aValue)
		{
			var action = GetAction(aActionName);
			var atomId = GetAtomIndex(aAtomName);
			if (action == null || atomId == -1)
			{
				return false;
			}
			return action.Post.Set(atomId, aValue);
		}

		public bool SetCost(string aActionName, int aCost)
		{
			var action = GetAction(aActionName);
			if (action == null) return false;
			action.Cost = aCost;
			return true;
		}

		public void SetState(string aActionName, string aStateName)
		{
			FindAction(aActionName).State = aStateName;
		}

		public string GetState(string aActionName)
		{
			var action = FindAction(aActionName);
			AntLog.Assert(action == null, $"Action \"{aActionName}\" not registered!", true);
			return action.State;
		}

		public string NameIt(bool[] aBits)
		{
			var result = "";
			for (int i = 0, n = Atoms.Count; i < n; i++)
			{
				//result += (aBits[i] ? atoms[i].ToUpper() : atoms[i].ToLower()) + " ";
				result = aBits[i] ? $"{result} <color=green>{Atoms[i]}</color>" : string.Concat(result, " ", Atoms[i]);
			}
			return result;
		}

		public void Clear()
		{
			Atoms.Clear();
			Actions.Clear();
		}

		public string Describe(string aTitle = "Result:")
		{
			var result = new StringBuilder(string.Concat(aTitle, "\n"));
			for (int i = 0, n = Actions.Count; i < n; i++)
			{
				var action = Actions[i];
				result.Append($"Action: '{action.Name}' State: '{action.State}' Cost: {action.Cost}\n");
				result.Append("  Preconditions:\n");
				var value = false;
				for (var j = 0; j < MAX_ATOMS; j++)
				{
					if (!action.Pre.GetMask(j)) 
						continue;
					value = action.Pre.GetValue(j);
					result.Append(string.Format("    '<color={2}>{0}</color>' = <color={2}>{1}</color>\n", 
						Atoms[j], value, (value) ? "green" : "red"));
				}
				result.Append("  Postconditions:\n");
				for (var j = 0; j < MAX_ATOMS; j++)
				{
					if (!action.Post.GetMask(j)) 
						continue;
					value = action.Post.GetValue(j);
					result.Append(string.Format("    '<color={2}>{0}</color>' = <color={2}>{1}</color>\n", 
						Atoms[j], value, (value) ? "green" : "red"));
				}
			}
			return result.ToString();
		}

		public int GetAtomIndex(string aAtomName)
		{
			var index = Atoms.IndexOf(aAtomName);
			if (index != -1 || Atoms.Count >= MAX_ATOMS) 
				return index;
			
			Atoms.Add(aAtomName);
			index = Atoms.Count - 1;
			return index;
		}

		public AntAICondition GetGoal(string aGoalName)
		{
			var goal = FindGoal(aGoalName);
			if (goal != null) 
				return goal;
			goal = new AntAICondition() { Name = aGoalName };
			Goals.Add(goal);
			return goal;
		}

		public AntAICondition FindGoal(string aGoalName)
		{
			return Goals.Find(a => a.Name.Equals(aGoalName));
		}

		public AntAIAction GetAction(string aActionName)
		{
			var action = FindAction(aActionName);
			if (action != null || Actions.Count >= MAX_ACTIONS)
				return action;
			action = new AntAIAction(aActionName);
			Actions.Add(action);
			return action;
		}

		public AntAIAction FindAction(string aActionName)
		{
			return Actions.Find(a => a.Name != null && a.Name.Equals(aActionName));
		}

		public void MakePlan(ref AntAIPlan aPlan, AntAICondition aCurrent, AntAICondition aGoal)
		{
			#if UNITY_EDITOR
			DebugConditions = aCurrent.Clone();
			#endif

			var opened = new List<AntAINode>();
			var closed = new List<AntAINode>();

			opened.Add(new AntAINode()
			{
				World = aCurrent,
				Parent = null,
				Cost = 0,
				Heuristic = aCurrent.Heuristic(aGoal),
				Sum = aCurrent.Heuristic(aGoal),
				Action = ""
			});

			var current = opened[0];
			while (opened.Count > 0)
			{
				// Find lowest rank
				current = opened[0];
				for (int i = 1, n = opened.Count; i < n; i++)
				{
					if (opened[i].Sum < current.Sum)
					{
						current = opened[i];
					}
				}

				opened.Remove(current);

				if (current.World.Match(aGoal))
				{
					// Plan is found!
					ReconstructPlan(ref aPlan, closed, current);
					aPlan.IsSuccess = true;
					EventPlanUpdated?.Invoke(aPlan);

					return;
				}

				closed.Add(current);

				// Get neighbors
				var neighbors = GetPossibleTransitions(current.World);
				for (int i = 0, n = neighbors.Count; i < n; i++)
				{
					var cost = current.Cost + neighbors[i].Cost;
					
					var neighbor = current.World.Clone();
					neighbor.Act(neighbors[i].Post);

					var openedIndex = FindEqual(opened, neighbor);
					var closedIndex = FindEqual(closed, neighbor);

					if (openedIndex > -1 && cost < opened[openedIndex].Cost)
					{
						opened.RemoveAt(openedIndex);
						openedIndex = -1;
					}

					if (closedIndex > -1 && cost < closed[closedIndex].Cost)
					{
						closed.RemoveAt(closedIndex);
						closedIndex = -1;
					}

					if (openedIndex == -1 && closedIndex == -1)
					{
						opened.Add(new AntAINode()
						{
							World = neighbor,
							Cost = cost,
							Heuristic = neighbor.Heuristic(aGoal),
							Sum = cost + neighbor.Heuristic(aGoal),
							Action = neighbors[i].Name,
							Parent = current.World
						});
					}
				}
			}

			// Failed plan.
			ReconstructPlan(ref aPlan, closed, current);
			aPlan.IsSuccess = false;

			EventPlanUpdated?.Invoke(aPlan);
		}

		#region Private Methods

		private List<AntAIAction> GetPossibleTransitions(AntAICondition aCurrent)
		{
			var possible = new List<AntAIAction>();
			for (int i = 0, n = Actions.Count; i < n; i++)
			{
				if (Actions[i].Pre.Match(aCurrent))
				{
					possible.Add(Actions[i]);
				}
			}
			return possible;
		}

		private static int FindEqual(IList<AntAINode> aList, AntAICondition aCondition)
		{
			for (int i = 0, n = aList.Count; i < n; i++)
			{
				if (aList[i].World.Equals(aCondition))
				{
					return i;
				}
			}
			return -1;
		}

		private void ReconstructPlan(ref AntAIPlan aPlan, IList<AntAINode> aClosed, AntAINode aGoal)
		{
			aPlan.Reset();
			var current = aGoal;
			while (current?.Parent != null)
			{
				aPlan.Insert(current.Action);
				var index = FindEqual(aClosed, current.Parent);
				current = index == -1 ? aClosed[0] : aClosed[index];
			}
		}

		#endregion
	}
}