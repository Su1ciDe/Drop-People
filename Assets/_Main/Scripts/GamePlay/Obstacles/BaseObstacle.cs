using GamePlay.People;
using GridSystem;
using Interfaces;
using UnityEngine;

namespace GamePlay.Obstacles
{
	public abstract class BaseObstacle : MonoBehaviour, INode
	{
		public bool CanMove => false;
		public GridCell CurrentGridCell { get; set; }

		public virtual void Place(GridCell placedCell)
		{
			CurrentGridCell = placedCell;
			CurrentGridCell.CurrentNode = this;

			transform.SetParent(placedCell.transform);
			transform.localPosition = Vector3.zero;
		}

		public virtual void OnGroupCompleteNear(PersonGroup personGroup)
		{
		}
	}
}