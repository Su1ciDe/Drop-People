using DG.Tweening;
using Fiber.Managers;
using GamePlay.People;
using Lofelt.NiceVibrations;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace GoalSystem
{
	[SelectionBase]
	public class GoalHolder : MonoBehaviour
	{
		public bool Completed { get; set; }
		public PersonType PersonType { get; private set; }

		[SerializeField] private MeshRenderer holderMeshRenderer;
		[Space]
		[SerializeField] private GoalSlot[] goalSlots;
		public GoalSlot[] GoalSlots => goalSlots;

		[SerializeField] private MMF_Player feedbacks;

		private const float MOVE_DURATION = .35F;
		private const int MATERIAL_INDEX = 0;

		private void OnDisable()
		{
			transform.DOKill();
		}

		public void Setup(PersonType personType)
		{
			PersonType = personType;

			var mat = GameManager.Instance.PersonMaterialsSO.GoalHolderMaterials[personType];
			var mats = holderMeshRenderer.materials;
			mats[MATERIAL_INDEX] = mat;
			holderMeshRenderer.materials = mats;
		}

		public Tween Spawn(Transform point)
		{
			var duration = gameObject.activeSelf ? 0 : .25f;

			gameObject.SetActive(true);
			transform.position = point.position;
			transform.rotation = point.rotation;

			return transform.DOScale(0, duration).From().SetEase(Ease.OutBack);
		}

		public Tween MoveTo(Vector3 position)
		{
			return transform.DOMove(position, MOVE_DURATION).SetEase(Ease.InOutQuart);
		}

		public Tween CloseCover()
		{
			HapticManager.Instance.PlayHaptic(HapticPatterns.PresetType.Success);
			feedbacks.PlayFeedbacks();
			
			var seq = DOTween.Sequence();
			seq.AppendInterval(0.2f);
			return seq;
		}
	}
}