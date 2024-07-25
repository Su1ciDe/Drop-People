using Fiber.Utilities;
using GamePlay.People;
using UnityEngine;
using Utilities;

namespace GamePlay.Obstacles
{
	public class BreakableObstacle : BaseObstacle
	{
		[SerializeField] private BreakableObject breakableObject;

		public override void OnGroupCompleteNear(PersonGroup personGroup)
		{
			base.OnGroupCompleteNear(personGroup);

			CurrentGridCell.CurrentNode = null;
			breakableObject.Break();

			ParticlePooler.Instance.Spawn("Breakable", transform.position);
		}
	}
}