namespace Anthill.AI
{
	public class AntAICondition
	{
		public string Name;
		private int _values;
		private int _mask;

		private AntAIPlanner _currentPlanner;

		public void Clear()
		{
			_values = 0;
			_mask = 0;
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
			return  (index >= 0 && index < AntAIPlanner.MAX_ATOMS) && (_values & 1 << index) > 0;
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
			
			_values &= ~(1 << aIndex);
			_values |= aValue ? 1 << aIndex : 0;
			_mask |= 1 << aIndex;
			return true;
		}

		public int Heuristic(AntAICondition aOther)
		{
			var dist = 0;
			for (var i = 0; i < AntAIPlanner.MAX_ATOMS; i++)
			{
				if ((aOther._mask & (1 << i)) > 0 && (_values & (1 << i)) != (aOther._values & (1 << i)))
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
				if ((_mask & (1 << i)) > 0 && (aOther._mask & (1 << i)) > 0 && (_values & (1 << i)) != (aOther._values & (1 << i)))
				{
					return false;
				}
			}
			return true;
		}

		public bool GetMask(int aIndex)
		{
			return (aIndex >= 0 && aIndex < AntAIPlanner.MAX_ATOMS) && (_mask & (1 << aIndex)) > 0;
		}

		public bool GetValue(int aIndex)
		{
			return (aIndex >= 0 && aIndex < AntAIPlanner.MAX_ATOMS) && (_values & (1 << aIndex)) > 0;
		}

		public AntAICondition Clone()
		{
			return new AntAICondition
			{
				_values = _values,
				_mask = _mask,
			};
		}

		public void Act(AntAICondition aPost)
		{
			_mask |= aPost._mask;
			_values &= ~aPost._mask;
			_values |= aPost._mask & aPost._values;
		}

		public bool Equals(AntAICondition aCondition)
		{
			return _values == aCondition._values;
		}

		public bool[] Description()
		{
			var result = new bool[AntAIPlanner.MAX_ATOMS];
			for (var i = 0; i < AntAIPlanner.MAX_ATOMS; i++)
			{
				result[i] = (_mask & _values & (1 << i)) > 0;
			}
			return result;
		}
	}
}