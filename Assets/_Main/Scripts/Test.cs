using UnityEngine;
using UnityEngine.AI;

namespace DefaultNamespace
{
	public class Test : MonoBehaviour
	{
		[SerializeField] private bool stay;

		private NavMeshAgent agent;

		private void Awake()
		{
			agent = GetComponent<NavMeshAgent>();
		}

		private void Start()
		{
			if (stay)
			{
				agent.SetDestination(transform.position);
			}
		}
	}
}