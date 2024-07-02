using UnityEngine;

namespace Fiber.Animation
{
    /// <summary>
    /// This class provides the basic animator interface.
    /// Extend this class to implement further animation logic.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public abstract class AnimationController : MonoBehaviour
    {
        private Animator animator;

        public virtual void Awake()
        {
            animator = GetComponent<Animator>();
        }

        /// <summary>
        /// Sets the value of the given trigger parameter.
        /// </summary>
        /// <param name="type">The parameter type.</param>
        public void SetTrigger(AnimationType type)
        {
            animator.SetTrigger(AnimationFactory.GetAnimation(type));
        }

        /// <summary>
        /// Sets the value of the given boolean parameter.
        /// </summary>
        /// <param name="type">The parameter type.</param>
        /// <param name="value">The new parameter value.</param>
        public void SetBool(AnimationType type, bool value)
        {
            animator.SetBool(AnimationFactory.GetAnimation(type), value);
        }

        /// <summary>
        /// Sets the value of the given integer parameter.
        /// </summary>
        /// <param name="type">The parameter type.</param>
        /// <param name="value">The new parameter value.</param>
        public void SetInt(AnimationType type, int value)
        {
            animator.SetInteger(AnimationFactory.GetAnimation(type), value);
        }

        /// <summary>
        /// Send float values to the Animator to affect transitions.
        /// </summary>
        /// <param name="type">The parameter type.</param>
        /// <param name="value">The new parameter value.</param>
        public void SetFloat(AnimationType type, float value)
        {
            animator.SetFloat(AnimationFactory.GetAnimation(type), value);
        }

        /// <summary>
        ///   <para>Returns the value of the given boolean parameter.</para>
        /// </summary>
        /// <param name="type">The parameter type.</param>
        /// <returns>The value of the parameter.</returns>
        public bool GetBool(AnimationType type) => animator.GetBool(AnimationFactory.GetAnimation(type));

        /// <summary>
        ///   <para>Returns the value of the given float parameter.</para>
        /// </summary>
        /// <param name="type">The parameter type.</param>
        /// <returns>The value of the parameter.</returns>
        public float GetFloat(AnimationType type) => animator.GetFloat(AnimationFactory.GetAnimation(type));

        /// <summary>
        ///   <para>Returns the value of the given integer parameter.</para>
        /// </summary>
        /// <param name="type">The parameter type.</param>
        /// <returns>The value of the parameter.</returns>
        public int GetInt(AnimationType type) => animator.GetInteger(AnimationFactory.GetAnimation(type));
    }
}