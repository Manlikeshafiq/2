using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MSA
{
    public class PlayerInputManager : MonoBehaviour
    {
        public static PlayerInputManager instance;
        public PlayerManager player;

        PlayerControls playerControls;

        [Header("PLAYER MOVEMENT INPUT")]
        [SerializeField] Vector2 movementInput;

        public float verticalInput;
        public float horizontalInput;
        public float moveAmount;

        [Header("PLAYER ACTION INPUT")]
        [SerializeField] bool dodgeInput = false;


        [Header("CAMERA MOVEMENT INPUT")]
        [SerializeField] Vector2 cameraInput;
        public float cameraVerticalInput;
        public float cameraHorizontalInput;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }



        }

        private void Start()
        {

            DontDestroyOnLoad(gameObject);

            SceneManager.activeSceneChanged += OnSceneChange;

            instance.enabled = false;
        }

        private void OnSceneChange(Scene oldScene, Scene newScene)
        {
            if (newScene.buildIndex == WorldSaveGameManager.instance.GetWorldSceneIndex())
            {
                instance.enabled = true;
            }
            else {
                instance.enabled = false;
            }
        }

        private void OnEnable()
        {
            if (playerControls == null)
            {
                playerControls = new PlayerControls();

                playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
                playerControls.PlayerCamera.CameraControls.performed += i => cameraInput = i.ReadValue<Vector2>();
                playerControls.PlayerActions.Dodge.performed += i => dodgeInput = true;
            }

            playerControls.Enable();
        }

        private void OnDestroy()
        {
            SceneManager.activeSceneChanged -= OnSceneChange;
        }

        private void OnApplicationFocus(bool focus)
        {
            if (enabled)
            {
                if (focus)
                {
                    playerControls.Enable();

                }
                else
                {
                    playerControls.Disable();
                }
            }
        }

        private void Update()
        {
            HandleAllInputs();
        }

        private void HandleAllInputs()
        {
            HandleMovementInput();
            HandleCameraMovementInput();
            HandleDodgeInput();
        }
        private void HandleMovementInput()
        {
            verticalInput = movementInput.y;
            horizontalInput = movementInput.x;

            moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));

            if (moveAmount <= 0.5 && moveAmount > 0)
            {
                moveAmount = 0.5f;
            }
            else if (moveAmount > 0.5 && moveAmount <= 1)
            {
                moveAmount = 1;
            }





            if (player == null)
                return;



            player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount);
            

        }
        private void HandleCameraMovementInput()
        {
            cameraVerticalInput = cameraInput.y;
            cameraHorizontalInput = cameraInput.x;
        }

        private void HandleDodgeInput()
        {
            if (dodgeInput)
            {
                dodgeInput = false;

                //  FUTURE NOTE: RETURN (DO NOTHING) IF MENU OR UI WINDOW IS OPEN

                player.playerLocomotionManager.AttemptToPerformDodge();
            }
        }




    }
}