using Fiber.Animation;
using UnityEngine;

namespace GamePlay.People
{
	public class PersonAnimations : AnimationController
	{
		private void Start()
		{
			SetFloat(AnimationType.IdleSpeed, Random.Range(0.75f, 1.25f));
		}

		public void Run()
		{
			SetBool(AnimationType.Run, true);
		}

		public void StopRunning()
		{
			SetBool(AnimationType.Run, false);
		}

		public void Sit()
		{
			SetBool(AnimationType.Sit, true);
		}

		public void StopSitting()
		{
			SetBool(AnimationType.Sit, false);
		}
	}
}