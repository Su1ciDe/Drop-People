using System.Collections;
using DG.Tweening;
using Fiber.AudioSystem;
using Fiber.Managers;
using Fiber.Utilities.Extensions;
using Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace GamePlay.People
{
	[SelectionBase]
	[RequireComponent(typeof(NavMeshAgent))]
	public class Person : MonoBehaviour
	{
		public bool IsMoving { get; set; }

		public PersonType PersonType { get; private set; }
		public ISlot CurrentSlot { get; set; }

		public NavMeshAgent Agent => agent;

		[SerializeField] private NavMeshAgent agent;
		[SerializeField] private float rotationDamping = 1;
		[SerializeField] private Transform model;
		[SerializeField] private PersonAnimations personAnimations;
		[SerializeField] private SkinnedMeshRenderer meshRenderer;

		public static float SCREW_DURATION = .25f;
		public static float MOVE_DURATION = .2f;

		private void Awake()
		{
			agent.updateRotation = false;
		}

		private void OnDestroy()
		{
			this.DOKill();
		}

		public void Setup(PersonType boltType)
		{
			PersonType = boltType;
			meshRenderer.material = GameManager.Instance.PersonMaterialsSO.PersonMaterials[boltType];
		}

		public void OnGroupPlaced()
		{
			agent.enabled = true;
			agent.SetDestination(transform.position);
		}

		public void ChangeSlot(ISlot slot, bool removeCurrentSlot = true, bool changePosition = true)
		{
			if (removeCurrentSlot)
				if (CurrentSlot.Person.Equals(this))
					CurrentSlot.SetPerson(null);

			slot.SetPerson(this, changePosition);
		}

		public void MoveToSlot(ISlot slot, bool changeRotation = false)
		{
			if (Mathf.Abs((transform.position.xz() - CurrentSlot.GetTransform().position.xz()).sqrMagnitude) < .1f) return;

			StartCoroutine(MoveCoroutine(slot, changeRotation));
		}

		private IEnumerator MoveCoroutine(ISlot slot, bool changeRotation)
		{
			if (!agent.enabled) yield break;

			transform.DOKill();

			IsMoving = true;
			var slotT = slot.GetTransform();

			agent.SetDestination(slotT.position);
			personAnimations.Run();

			yield return null;
			while (agent.remainingDistance > agent.stoppingDistance)
			{
				UpdateRotation();
				yield return null;
			}

			// HapticManager.Instance.PlayHaptic(0.65f, 1f);
			// AudioManager.Instance.PlayAudio(AudioName.Person);

			personAnimations.StopRunning();
			IsMoving = false;

			if (changeRotation)
				model.DORotate(CurrentSlot.GetTransform().eulerAngles, .15f);
		}

		private void UpdateRotation()
		{
			if (agent.desiredVelocity.sqrMagnitude > agent.stoppingDistance)
			{
				model.forward = Vector3.Lerp(model.forward, agent.velocity.normalized, Time.deltaTime * rotationDamping);
			}
			else
			{
				// model.forward = Vector3.Lerp(model.forward, Helper.MainCamera.transform.position.xz() - transform.position.xz(), Time.deltaTime);
			}
		}
	}
}