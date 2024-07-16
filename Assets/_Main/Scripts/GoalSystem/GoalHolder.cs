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
		[SerializeField] private Transform cover;
		[Space]
		[SerializeField] private GoalSlot[] goalSlots;
		public GoalSlot[] GoalSlots => goalSlots;

		[SerializeField] private MMF_Player feedbacks;

		private const float MOVE_DURATION = .35F;

		private void OnDisable()
		{
			transform.DOKill();
		}

		public void Setup(PersonType personType)
		{
			PersonType = personType;

			var mat = GameManager.Instance.PersonMaterialsSO.GoalHolderMaterials[personType];
			var mats = holderMeshRenderer.materials;
			mats[0] = mat;
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
			cover.gameObject.SetActive(true);
			var seq = DOTween.Sequence();
			seq.Append(cover.DOLocalMove(cover.localPosition + 5 * Vector3.up, .35f).From().SetEase(Ease.InExpo));
			seq.AppendCallback(() =>
			{
				HapticManager.Instance.PlayHaptic(HapticPatterns.PresetType.Success);
				feedbacks.PlayFeedbacks();
			});
			seq.AppendInterval(0.2f);
			return seq;
		}
	}
}