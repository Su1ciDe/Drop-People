using Fiber.Managers;
using UnityEngine;

namespace GamePlay.Player
{
    /// <summary>
    /// /// Handle Player Movement, Collision Detections ...
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        private MoveBase _movementBase = null;
        
        // Awake is called before Start
        private void Awake()
        {
            _movementBase = GetComponent<MoveBase>();
        }

        // Start is called before the first frame update
        private void Start()
        {
            // TODO
        }

        // Update is called on every frame
        private void Update()
        {
            // TODO
        }
        
        // OnStart is called when click "tap to play button"
        private void OnStart()
        {
            if(_movementBase != null)
                _movementBase.StartMovement();
            else
                Debug.LogWarning("You did not implement FiberMove!");
        }

        // OnWin is called when game is completed as succeed
        private void OnWin()
        {
            if(_movementBase != null)
                _movementBase.StopMovement();
            else
                Debug.LogWarning("You did not implement FiberMove!");
        }

        // OnLose is called when game is completed as failed
        private void OnLose()
        {
            if(_movementBase != null)
                _movementBase.StopMovement();
            else
                Debug.LogWarning("You did not implement FiberMove!");
        }

        #region InputSystem

        private bool _isPressed = false;
        
        private void OnPressed()
        {
            ShouldPress(true);
        }

        private void OnReleased()
        {
            ShouldPress(false);
        }

        private void OnDrag(Vector2 direction)
        {
            
        }
        
        private void ShouldPress(bool state)
        {
            _isPressed = state;
        }

        #endregion
        
        private void OnEnable()
        {
            LevelManager.OnLevelStart += OnStart;
            LevelManager.OnLevelWin += OnWin;
            LevelManager.OnLevelLose += OnLose;

            InputBase.OnInputPressed += OnPressed;
            InputBase.OnInputReleased += OnReleased;
            InputBase.OnInputDrag += OnDrag;
        }

        private void OnDisable()
        {
            LevelManager.OnLevelStart -= OnStart;
            LevelManager.OnLevelWin -= OnWin;
            LevelManager.OnLevelLose -= OnLose;
            
            InputBase.OnInputPressed -= OnPressed;
            InputBase.OnInputReleased -= OnReleased;
            InputBase.OnInputDrag -= OnDrag;
        }
    }
}