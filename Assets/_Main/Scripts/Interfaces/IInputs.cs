using UnityEngine;
using UnityEngine.Events;

namespace Interfaces
{
	public interface IInputs
	{
		public bool CanInput { get; set; }
		public event UnityAction<Vector3> OnDown;
		public event UnityAction<Vector3> OnMove;
		public event UnityAction<Vector3> OnUp;
	}
}