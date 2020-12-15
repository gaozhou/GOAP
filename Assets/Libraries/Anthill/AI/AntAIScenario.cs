using System;
using UnityEngine;

namespace Anthill.AI
{
	[CreateAssetMenuAttribute(fileName = "Scenario", menuName = "New AI Scenario", order = 1)]
	public class AntAIScenario : ScriptableObject
	{
		public AntAIScenarioCondition conditions;
		[HideInInspector] public AntAIScenarioAction[] actions = new AntAIScenarioAction[0];
		[HideInInspector] public AntAIScenarioGoal[] goals = new AntAIScenarioGoal[0];
	}

	[Serializable]
	public class AntAIScenarioGoal
	{
		public string name;
		public bool isOpened;
		public AntAIScenarioItem[] conditions;

		public AntAIScenarioGoal()
		{
			name = "<Unnamed>";
			isOpened = true;
			conditions = new AntAIScenarioItem[0];
		}
	}

	[Serializable]
	public class AntAIScenarioAction
	{
		public string name;
		public string state;
		public int cost;
		public bool isOpened;
		public AntAIScenarioItem[] pre;
		public AntAIScenarioItem[] post;

		public AntAIScenarioAction()
		{
			name = "<Unnamed>";
			state = name;
			cost = 0;
			isOpened = true;
			pre = new AntAIScenarioItem[0];
			post = new AntAIScenarioItem[0];
		}
	}

	[Serializable]
	public struct AntAIScenarioItem
	{
		public int id;
		public bool value;
	}

	[Serializable]
	public class AntAIScenarioCondition
	{
		public AntAIScenarioConditionItem[] list = new AntAIScenarioConditionItem[0];
		public int serialId = -1;

		public AntAIScenarioCondition Clone()
		{
			var clone = new AntAIScenarioCondition
			{
				list = new AntAIScenarioConditionItem[list.Length], serialId = serialId
			};
			for (int i = 0, n = list.Length; i < n; i++)
			{
				clone.list[i] = list[i];
			}
			return clone;
		}

		public string GetName(int aIndex)
		{
			var index = Array.FindIndex(list, aX => aX.id == aIndex);
			return (index >= 0 && index < list.Length) ? list[index].name : null;
		}

		public int GetID(string aConditionName)
		{
			var index = Array.FindIndex(list, aX => aX.name.Equals(aConditionName));
			return (index >= 0 && index < list.Length) ? list[index].id : -1;
		}

		public string this[int aIndex]
		{
			get { return (aIndex >= 0 && aIndex < list.Length) ? list[aIndex].name : null; }
		}

		public int Count
		{
			get { return list.Length; }
		}
	}

	[Serializable]
	public struct AntAIScenarioConditionItem
	{
		public int id;
		public string name;
	}
}