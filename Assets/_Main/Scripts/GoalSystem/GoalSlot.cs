using GamePlay.People;
using Interfaces;
using UnityEngine;

namespace GoalSystem
{
	public class GoalSlot : MonoBehaviour, ISlot
	{
		public Person Person { get; set; }
		public int Index { get; set; }

		public void SetPerson(Person person, bool setPosition = true)
		{
			if (!person)
			{
				Person = null;
				return;
			}

			Person = person;
			Person.CurrentSlot = this;
			Person.transform.SetParent(transform);

			if (setPosition)
				Person.transform.localPosition = Vector3.zero;
		}

		public Transform GetTransform() => transform;
	}
}