using DG.Tweening;
using Fiber.Managers;
using Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace GamePlay.People
{
	[SelectionBase]
	public class Person : MonoBehaviour
	{
		public PersonType PersonType { get; private set; }
		public ISlot CurrentSlot { get; set; }

		[SerializeField] private NavMeshAgent agent;
		[SerializeField] private PersonAnimations personAnimations;
		[SerializeField] private SkinnedMeshRenderer meshRenderer;

		public static float SCREW_DURATION = .25f;
		public static float MOVE_DURATION = .2f;
		private const int SCREW_AMOUNT = 2;
		private const float SCREW_HEIGHT = 1.5F;

		private void OnDestroy()
		{
			this.DOKill();
		}

		public void Setup(PersonType boltType)
		{
			PersonType = boltType;
			meshRenderer.material = GameManager.Instance.PersonMaterialsSO.PersonMaterials[boltType];
		}

		public void ChangeSlot(ISlot slot, bool removeCurrentSlot = true, bool changePosition = true)
		{
			if (removeCurrentSlot)
				if (CurrentSlot.Person.Equals(this))
					CurrentSlot.SetPerson(null);

			slot.SetPerson(this, changePosition);
			// name = slot.name + " - " + BoltType;
		}

		public Tween Unscrew()
		{
			this.DOKill();

			var seq = DOTween.Sequence();
			if (transform.position.Equals(CurrentSlot.GetTransform().position)) return seq;

			seq.Join(transform.DOMove(new Vector3(transform.position.x, transform.position.y + SCREW_HEIGHT, transform.position.z), SCREW_DURATION).SetEase(Ease.Linear));
			seq.Join(transform.DOLocalRotate(-360 * Vector3.up, SCREW_DURATION / SCREW_AMOUNT, RotateMode.FastBeyond360).SetLoops(SCREW_AMOUNT, LoopType.Incremental).SetEase(Ease.Linear));
			seq.SetTarget(this);
			return seq;
		}

		public Tween MoveToSlot(bool changeRotation = false)
		{
			this.DOKill();

			var seq = DOTween.Sequence();
			if (transform.position.Equals(CurrentSlot.GetTransform().position)) return seq;

			seq.Join(transform.DOLocalMove(SCREW_HEIGHT * Vector3.up, MOVE_DURATION).SetEase(Ease.InOutSine));
			seq.Join(transform.DOScale(Vector3.one, MOVE_DURATION).SetEase(Ease.InOutSine));
			if (changeRotation)
				seq.Join(transform.DORotate(CurrentSlot.GetTransform().eulerAngles, MOVE_DURATION));

			seq.SetTarget(this);

			return seq;
		}

		public Tween Screw()
		{
			this.DOKill();

			var seq = DOTween.Sequence();
			if (transform.position.Equals(CurrentSlot.GetTransform().position)) return seq;

			seq.Join(transform.DOLocalMove(Vector3.zero, SCREW_DURATION).SetEase(Ease.Linear));
			seq.Join(transform.DOLocalRotate(360 * Vector3.up, SCREW_DURATION / SCREW_AMOUNT, RotateMode.FastBeyond360).SetLoops(SCREW_AMOUNT, LoopType.Incremental).SetEase(Ease.Linear));
			seq.SetTarget(this);

			return seq;
		}
	}
}