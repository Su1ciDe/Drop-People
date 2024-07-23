using GamePlay.People;
using UnityEngine;

namespace GamePlay.Obstacles
{
	public class BreakableObstacle : BaseObstacle
	{
		public override void OnGroupCompleteNear(PersonGroup personGroup)
		{
			base.OnGroupCompleteNear(personGroup);
			
			//TODO: feedbacks (particles)
			CurrentGridCell.CurrentNode = null;
			gameObject.SetActive(false);
		}
	}
}