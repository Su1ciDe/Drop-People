using GamePlay.People;
using UnityEngine;

namespace Interfaces
{
	public interface ISlot
	{
		public Person Person { get; set; }
		public int Index { get; set; }
		public void SetPerson(Person person, bool setPosition = true);
		public Transform GetTransform();
	}
}