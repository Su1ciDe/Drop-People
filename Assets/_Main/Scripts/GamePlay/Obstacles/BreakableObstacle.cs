using Fiber.Utilities;
using GamePlay.People;
using UnityEngine;
using UnityEngine.AI;
using Utilities;

namespace GamePlay.Obstacles
{
	public class BreakableObstacle : BaseObstacle
	{
		[SerializeField] private BreakableObject breakableObject;
		[SerializeField] private NavMeshObstacle navMeshObstacle;

		public override void OnGroupCompleteNear(PersonGroup personGroup)
		{
			base.OnGroupCompleteNear(personGroup);

			CurrentGridCell.CurrentNode = null;
			breakableObject.Break();

			navMeshObstacle.enabled = false;

			ParticlePooler.Instance.Spawn("Breakable", transform.position);
		}
	}
}