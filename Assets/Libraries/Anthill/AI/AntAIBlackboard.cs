using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Anthill.AI
{
	public class AntAIBlackboard : MonoBehaviour
	{
		public delegate void PropertyUpdateDelegate(string aKey, AntAIBlackboardProp aProperty);
		public event PropertyUpdateDelegate EventPropertyAdded;
		public event PropertyUpdateDelegate EventPropertyRemoved;

		private readonly Dictionary<string, AntAIBlackboardProp> _dict = new Dictionary<string, AntAIBlackboardProp>();

		#region Public Methods

		public string GetKey(int aIndex)
		{
			return aIndex >= 0 && aIndex < _dict.Count ? _dict.ElementAt(aIndex).Key : null;
		}

		public bool Contains(string aKey)
		{
			return _dict.ContainsKey(aKey);
		}

		public AntAIBlackboardProp Remove(string aKey)
		{
			if (!_dict.ContainsKey(aKey)) 
				return null;
			
			var result = _dict[aKey];
			_dict.Remove(aKey);
			EventPropertyRemoved?.Invoke(aKey, result);
			return result;
		}

		public void Clear()
		{
			_dict.Clear();
		}

		#endregion
		#region Getter/Setters

		public AntAIBlackboardProp this[int aIndex]
		{
			get => _dict.ElementAt(aIndex).Value;
			set
			{
				var key = _dict.ElementAt(aIndex).Key;
				_dict[key] = value;
			}
		}

		public AntAIBlackboardProp this[string aKey]
		{
			get
			{
				if (_dict.ContainsKey(aKey))
				{
					return _dict[aKey];
				}

				var prop = new AntAIBlackboardProp();
				_dict.Add(aKey, prop);
				EventPropertyAdded?.Invoke(aKey, prop);

				return prop;
			}
			set
			{
				if (_dict.ContainsKey(aKey))
				{
					_dict[aKey] = value;
				}
				else
				{
					_dict.Add(aKey, value);
					EventPropertyAdded?.Invoke(aKey, value);
				}
			}
		}
		public int Count => _dict.Count;
		#endregion
	}
}