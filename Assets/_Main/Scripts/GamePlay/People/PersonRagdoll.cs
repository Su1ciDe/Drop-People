using UnityEngine;

namespace GamePlay.People
{
	public class PersonRagdoll : MonoBehaviour
	{
		[SerializeField] private Rigidbody[] ragdolls;
		public Rigidbody[] Ragdolls => ragdolls;
		

		public void EnableRagdoll()
		{
			SetRagdoll(true);
		}

		public void DisableRagdoll()
		{
			SetRagdoll(false);
		}

		private void SetRagdoll(bool enable)
		{
			for (var i = 1; i < ragdolls.Length; i++)
				ragdolls[i].isKinematic = !enable;
		}
	}
}