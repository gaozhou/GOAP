namespace Anthill.AI
{
	public class AntAICondition
	{
		public string Name;
		public int Values;
		public int Mask;
		// public bool[] values;
		// public bool[] mask;

		private AntAIPlanner _currentPlanner;

		public void Clear()
		{
			Values = 0;
			Mask = 0;
		}

		public void BeginUpdate(AntAIPlanner aPlanner)
		{
			_currentPlanner = aPlanner;
		}

		public void EndUpdate()
		{
			_currentPlanner = null;
		}

		public bool Has(string aAtomName)
		{
			return Has(_currentPlanner, aAtomName);
		}

		public bool Has(AntAIPlanner aPlanner, string aAtomName)
		{
			var index = aPlanner.GetAtomIndex(aAtomName);
			return  (index >= 0 && index < AntAIPlanner.MAX_ATOMS) && (Values & 1 << index) > 0;
		}

		public bool Set(string aAtomName, bool aValue)
		{
			return Set(_currentPlanner.GetAtomIndex(aAtomName), aValue);
		}

		public bool Set(AntAIPlanner aPlanner, string aAtomName, bool aValue)
		{
			return Set(aPlanner.GetAtomIndex(aAtomName), aValue);
		}

		public bool Set(int aIndex, bool aValue)
		{
			if (aIndex < 0 || aIndex >= AntAIPlanner.MAX_ATOMS)
				return false;
			
			Values &= ~(1 << aIndex);
			Values |= aValue ? 1 << aIndex : 0;
			Mask |= 1 << aIndex;
			return true;
		}

		public int Heuristic(AntAICondition aOther)
		{
			var dist = 0;
			for (var i = 0; i < AntAIPlanner.MAX_ATOMS; i++)
			{
				if ((aOther.Mask & (1 << i)) > 0 && (Values & (1 << i)) != (aOther.Values & (1 << i)))
				{
					dist++;
				}
			}
			return dist;
		}

		public bool Match(AntAICondition aOther)
		{
			for (var i = 0; i < AntAIPlanner.MAX_ATOMS; i++)
			{
				if ((Mask & (1 << i)) > 0 && (aOther.Mask & (1 << i)) > 0 && (Values & (1 << i)) != (aOther.Values & (1 << i)))
				{
					return false;
				}
			}
			return true;
		}

		public bool GetMask(int aIndex)
		{
			return (aIndex >= 0 && aIndex < AntAIPlanner.MAX_ATOMS) && (Mask & (1 << aIndex)) > 0;
		}

		public bool GetValue(int aIndex)
		{
			return (aIndex >= 0 && aIndex < AntAIPlanner.MAX_ATOMS) && (Values & (1 << aIndex)) > 0;
		}

		public AntAICondition Clone()
		{
			return new AntAICondition
			{
				Values = Values,
				Mask = Mask,
			};
		}

		public void Act(AntAICondition aPost)
		{
			Mask |= aPost.Mask;
			Values &= ~aPost.Mask;
			Values |= aPost.Mask & aPost.Values;
		}

		public bool Equals(AntAICondition aCondition)
		{
			return Values == aCondition.Values;
		}

		public bool[] Description()
		{
			var result = new bool[AntAIPlanner.MAX_ATOMS];
			for (var i = 0; i < AntAIPlanner.MAX_ATOMS; i++)
			{
				result[i] = (Mask & Values & (1 << i)) > 0;
			}
			return result;
		}
	}
}