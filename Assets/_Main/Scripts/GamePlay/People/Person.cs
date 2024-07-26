using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
		public bool IsMoving { get; private set; }

		public PersonRagdoll PersonRagdoll { get; private set; }

		public PersonType PersonType { get; private set; }
		public ISlot CurrentSlot { get; set; }

		public NavMeshAgent Agent => agent;

		[SerializeField] private NavMeshAgent agent;
		[SerializeField] private float rotationDamping = 1;
		[SerializeField] private Transform model;
		[SerializeField] private PersonAnimations personAnimations;
		[SerializeField] private SkinnedMeshRenderer meshRenderer;

		[Space]
		[SerializeField] private ParticleSystem footPrintParticle;

		private void Awake()
		{
			PersonRagdoll = GetComponent<PersonRagdoll>();

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
			footPrintParticle.GetComponent<Renderer>().material.color = meshRenderer.material.color;
		}

		public void OnGroupMove()
		{
			var head = PersonRagdoll.Ragdolls[0];
			head.MovePosition(transform.position + 2 * Vector3.up);
		}

		public void OnGroupPickedUp()
		{
			PersonRagdoll.EnableRagdoll();
		}

		public void OnGroupDroppedDown()
		{
			PersonRagdoll.DisableRagdoll();
		}

		public void OnGroupPlaced()
		{
			agent.enabled = true;
			agent.SetDestination(transform.position);

			OnGroupDroppedDown();
		}

		public void ChangeSlot(ISlot slot, bool removeCurrentSlot = true, bool changePosition = true)
		{
			if (removeCurrentSlot)
				if (CurrentSlot.Person.Equals(this))
					CurrentSlot.SetPerson(null);

			slot.SetPerson(this, changePosition);
		}

		private readonly Queue<Vector3> movementQueue = new Queue<Vector3>();

		public void MoveToSlot(bool changeRotation, params Vector3[] moveQueue)
		{
			var slotT = CurrentSlot.GetTransform();
			if (Mathf.Abs((transform.position.xz() - slotT.position.xz()).sqrMagnitude) < .1f) return;

			foreach (var movePoint in moveQueue)
				movementQueue.Enqueue(movePoint);
			movementQueue.Enqueue(slotT.position);

			StartCoroutine(MoveCoroutine(changeRotation));
		}

		private IEnumerator MoveCoroutine(bool changeRotation)
		{
			if (!agent.enabled) yield break;

			transform.DOKill();

			IsMoving = true;

			var movePosition = movementQueue.Dequeue();
			agent.SetDestination(movePosition);
			personAnimations.Run();

			footPrintParticle.Play();

			yield return null;
			while (agent.remainingDistance - agent.stoppingDistance > .1f)
			{
				UpdateRotation();
				yield return null;
			}

			if (movementQueue.Count > 0)
			{
				StartCoroutine(MoveCoroutine(changeRotation));
				yield break;
			}

			transform.position = movePosition;
			movementQueue.Clear();

			personAnimations.StopRunning();
			IsMoving = false;

			footPrintParticle.Stop();

			if (changeRotation)
				model.DORotate(CurrentSlot.GetTransform().eulerAngles, .15f);
		}

		private void UpdateRotation()
		{
			if (agent.desiredVelocity.sqrMagnitude > agent.stoppingDistance)
				model.forward = Vector3.Lerp(model.forward, agent.velocity.normalized, Time.deltaTime * rotationDamping);
		}
	}
}