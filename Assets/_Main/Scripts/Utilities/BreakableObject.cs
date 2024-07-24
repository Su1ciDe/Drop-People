using DG.Tweening;
using UnityEngine;

namespace Utilities
{
	public class BreakableObject : MonoBehaviour
	{
		[SerializeField] private Rigidbody[] rigidbodies;
		[SerializeField] private float explosionForce = 100;
		[SerializeField] private float explosionRadius = 1;
		[SerializeField] private float upwardsModifier = 1;

		public void Break()
		{
			foreach (var rb in rigidbodies)
			{
				rb.isKinematic = false;
				rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, upwardsModifier);
				rb.AddTorque(new Vector3(Random.Range(0, 180), Random.Range(0, 180), Random.Range(0, 180)));
				rb.transform.DOScale(0, 1).SetDelay(2).SetEase(Ease.OutSine).OnComplete(() => Destroy(gameObject));
			}
		}
	}
}