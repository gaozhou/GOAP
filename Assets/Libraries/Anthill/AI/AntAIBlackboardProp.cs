using System;
using UnityEngine;

namespace Anthill.AI
{
	public class AntAIBlackboardProp
	{
		public enum ValueType
		{
			String,
			Float,
			Int,
			Bool,
			Vec2,
			Vec3
		}

		public delegate void ValueUpdateDelegate(AntAIBlackboardProp aProperty);
		public event ValueUpdateDelegate EventChanging;
		public event ValueUpdateDelegate EventChanged; 

		private string _strValue;
		private float _floatValue;
		private int _intValue;
		private bool _boolValue;
		private Vector2 _vec2Value;
		private Vector3 _vec3Value;

		public AntAIBlackboardProp()
		{
			Type = ValueType.String;
		}

		#region Public Methods

		public override string ToString()
		{
			var result = "";
			switch (Type)
			{
				case ValueType.String :
					result = _strValue;
				break;

				case ValueType.Float :
					result = _floatValue.ToString("0.00");
				break;

				case ValueType.Int :
					result = _intValue.ToString();
				break;

				case ValueType.Bool :
					result = _boolValue.ToString();
				break;

				case ValueType.Vec2 :
					result = string.Concat(
						_vec2Value.x.ToString("0.00"), ", ",
						_vec2Value.y.ToString("0.00"));
				break;

				case ValueType.Vec3 :
					result = string.Concat(
						_vec3Value.x.ToString("0.00"), ", ",
						_vec3Value.y.ToString("0.00"), ", ",
						_vec3Value.z.ToString("0.00"));
				break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			return result;
		}

		#endregion
		#region Getters/Setters

		public ValueType Type { get; private set; }

		public string Value
		{
			get => _strValue;
			set
			{
				EventChanging?.Invoke(this);

				Type = ValueType.String;
				_strValue = value;
				EventChanged?.Invoke(this);
			}
		}

		public float AsFloat
		{
			get => _floatValue;
			set
			{
				EventChanging?.Invoke(this);

				Type = ValueType.Float;
				_floatValue = value;
				EventChanged?.Invoke(this);
			}
		}

		public int AsInt
		{
			get => _intValue;
			set 
			{
				EventChanging?.Invoke(this);

				Type = ValueType.Int;
				_intValue = value;
				EventChanged?.Invoke(this);
			}
		}

		public bool AsBool
		{
			get => _boolValue;
			set
			{
				EventChanging?.Invoke(this);

				Type = ValueType.Bool;
				_boolValue = value;
				EventChanged?.Invoke(this);
			}
		}

		public Vector2 AsVector2
		{
			get => _vec2Value;
			set
			{
				EventChanging?.Invoke(this);

				Type = ValueType.Vec2;
				_vec2Value = value;
				EventChanged?.Invoke(this);
			}
		}

		public Vector3 AsVector3
		{
			get => _vec3Value;
			set
			{
				EventChanging?.Invoke(this);

				Type = ValueType.Vec3;
				_vec3Value = value;
				EventChanged?.Invoke(this);
			}
		}
		#endregion
	}
}