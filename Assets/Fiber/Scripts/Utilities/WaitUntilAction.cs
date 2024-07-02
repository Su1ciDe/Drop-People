using UnityEngine;
using UnityEngine.Events;

namespace Fiber.Utilities
{
	/// <summary>
	/// <para>Suspends the coroutine execution until the referenced action triggered.</para>
	/// </summary>
	public sealed class WaitUntilAction : CustomYieldInstruction
	{
		private bool actionTriggered = false;

		public override bool keepWaiting => !actionTriggered;

		/// <summary>
		/// Suspends the coroutine execution until the referenced action triggered.
		/// </summary>
		/// <param name="action">Action to be referenced</param>
		public WaitUntilAction(ref UnityAction action)
		{
			action -= WaitAction;
			action += WaitAction;
		}

		private void WaitAction()
		{
			actionTriggered = true;
		}
	}
}