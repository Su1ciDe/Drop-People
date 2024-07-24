using UnityEngine;

namespace GamePlay.People
{
	public class PersonRagdoll : MonoBehaviour
	{
		[SerializeField] private Rigidbody[] ragdolls;
		public Rigidbody[] Ragdolls => ragdolls;

		[SerializeField] private DynamicBone dynamicBone;
		public DynamicBone DynamicBone => dynamicBone;

		public void EnableRagdoll()
		{
			// SetRagdoll(true);
			SetDynamicBone(true);
		}

		public void DisableRagdoll()
		{
			// SetRagdoll(false);
			SetDynamicBone(false);
		}

		private void SetDynamicBone(bool enable)
		{
			dynamicBone.enabled = enable;
		}

		private void SetRagdoll(bool enable)
		{
			for (var i = 1; i < ragdolls.Length; i++)
				ragdolls[i].isKinematic = !enable;
		}
	}
}