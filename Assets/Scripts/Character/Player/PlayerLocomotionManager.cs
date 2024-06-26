using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace MSA
{
    public class PlayerLocomotionManager : CharacterLocomotionManager
    {
        PlayerManager player;

        [HideInInspector] public float verticalMovement;
        [HideInInspector] public float horizontalMovement;
        [HideInInspector] public float moveAmount;

        [Header("Movement Settings")]
        private Vector3 moveDirection;
        private Vector3 targetRotationDirection;
        [SerializeField] float walkingSpeed = 2;
        [SerializeField] float runningSpeed = 5;
        [SerializeField] float sprintingSpeed = 6.5f;
        [SerializeField] float rotationSpeed = 15;


        [Header("Jump")]
        [SerializeField] float jumpStaminaCost = 8;
        [SerializeField] float jumpHeight = 4;
        [SerializeField] float jumpForwardSpeed = 5;
        [SerializeField] float freeFallSpeed = 2;
        private Vector3 jumpDirection;

        [Header("Dodge")]
        private Vector3 rollDirection;


        protected override void Awake()
        {
            base.Awake();

            player = GetComponent<PlayerManager>();
        }

        protected override void Update()
        {
            base.Update();

            if (player.IsOwner)
            {
                player.characterNetworkManager.verticalMovement.Value = verticalMovement;
                player.characterNetworkManager.horizontalMovement.Value = horizontalMovement;
                player.characterNetworkManager.moveAmount.Value = moveAmount;
            }
            else
            {
                verticalMovement = player.characterNetworkManager.verticalMovement.Value;
                horizontalMovement = player.characterNetworkManager.horizontalMovement.Value;
                moveAmount = player.characterNetworkManager.moveAmount.Value;

                //  IF NOT LOCKED ON, PASS MOVE AMOUNT
                player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount, player.playerNetworkManager.isSprinting.Value);

                //  IF LOCKED ON, PASS HORZ AND VERT
            }
        }



        public void HandleAllMovement()
        {
            HandleGroundedMovement();
            HandleRotation();
            HandleJumpingMovement();
            HandleFreeFallMovement();
        }

        private void GetMovementValues()
        {
            verticalMovement = PlayerInputManager.instance.verticalInput;
            horizontalMovement = PlayerInputManager.instance.horizontalInput;
            moveAmount = PlayerInputManager.instance.moveAmount;
        }
        private void HandleGroundedMovement()
        {
            GetMovementValues();
            if (!player.canMove)
                return;
            moveDirection = PlayerCamera.instance.transform.forward * verticalMovement;
            moveDirection = moveDirection + PlayerCamera.instance.transform.right * horizontalMovement;
            moveDirection.Normalize();
            moveDirection.y = 0;

            if (player.playerNetworkManager.isSprinting.Value)
            {
                player.characterController.Move(moveDirection * sprintingSpeed * Time.deltaTime);
            }
            else
            {
                if (PlayerInputManager.instance.moveAmount > 0.5f)
                {
                    player.characterController.Move(moveDirection * runningSpeed * Time.deltaTime);
                }
                else if (PlayerInputManager.instance.moveAmount >= 0.5f)
                {
                    player.characterController.Move(moveDirection * walkingSpeed * Time.deltaTime);
                }
            }
           

            
        }

        private void HandleJumpingMovement()
        {
            if (player.playerNetworkManager.isJumping.Value)
            {
                player.characterController.Move(jumpDirection * jumpForwardSpeed * Time.deltaTime);
            }
        }
        private void HandleFreeFallMovement()
        {
            if (!player.isGrounded)
            {
                Vector3 freeFallDirection;

                freeFallDirection = PlayerCamera.instance.transform.forward * PlayerInputManager.instance.verticalInput;
                freeFallDirection = freeFallDirection + PlayerCamera.instance.transform.right * PlayerInputManager.instance.horizontalInput;
                freeFallDirection.y = 0;

                player.characterController.Move(freeFallDirection * freeFallSpeed * Time.deltaTime);
            }
        }



        private void HandleRotation()
        {
            if (!player.canRotate)
                return;
            Vector3 targetRotationDirection = Vector3.zero;
            targetRotationDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
            targetRotationDirection = targetRotationDirection + PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;
            targetRotationDirection.Normalize();
            targetRotationDirection.y = 0;

            if (targetRotationDirection == Vector3.zero )
            {
                targetRotationDirection = transform.forward;
            }

            Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
            transform.rotation = targetRotation;
        }

        public void HandleSprinting()
        {
            if(player.isPerformingAction)
            {
                player.playerNetworkManager.isSprinting.Value = false;
            }

            if (moveAmount >= 0.5)
            {
                player.playerNetworkManager.isSprinting.Value = true;
            }
            else
            {
                player.playerNetworkManager.isSprinting.Value = false;
            }
        }

        public void AttemptToPerformDodge()
        {
            if (player.isPerformingAction)
                return;

            //  IF WE ARE MOVING WHEN WE ATTEMPT TO DODGE, WE PERFORM A ROLL
            if (PlayerInputManager.instance.moveAmount > 0)
            {
                rollDirection = PlayerCamera.instance.cameraObject.transform.forward * PlayerInputManager.instance.verticalInput;
                rollDirection += PlayerCamera.instance.cameraObject.transform.right * PlayerInputManager.instance.horizontalInput;
                rollDirection.y = 0;
                rollDirection.Normalize();

                Quaternion playerRotation = Quaternion.LookRotation(rollDirection);
                player.transform.rotation = playerRotation;

                player.playerAnimatorManager.PlayTargetActionAnimation("Roll_Forward_01", true, true);
            }
            //  IF WE ARE STATIONARY, WE PERFORM A BACKSTEP
            else
            {
                player.playerAnimatorManager.PlayTargetActionAnimation("Back_Step_01", true, true);
            }
        }

        public void AttemptToPerformJump()
        {
            //  IF WE ARE PERFORMING A GENERAL ACTION, WE DO NOT WANT TO ALLOW A JUMP (WILL CHANGE WHEN COMBAT IS ADDED)
            if (player.isPerformingAction)
                return;

            //  IF WE ARE OUT OF STAMINA, WE DO NOT WISH TO ALLOW A JUMP
            if (player.playerNetworkManager.currentStamina.Value <= 0)
                return;

            //  IF WE ARE ALREADY IN A JUMP, WE DO NOT WANT TO ALLOW A JUMP AGAIN UNTIL THE CURRENT JUMP HAS FINISHED
            if (player.playerNetworkManager.isJumping.Value)
                return;

            //  IF WE ARE NOT GROUNDED, WE DO NOT WANT TO ALLOW A JUMP
            if (!player.isGrounded)
                return;

            //  IF WE ARE TWO HANDING OUR WEAPON, PLAY THE TWO HANDED JUMP ANIMATION, OTHERWISE PLAY THE ONE HANDED ANIMATION ( TO DO )
            player.playerAnimatorManager.PlayTargetActionAnimation("Main_Jump_Start", false);

            player.playerNetworkManager.isJumping.Value = true;

            player.playerNetworkManager.currentStamina.Value -= jumpStaminaCost;

            jumpDirection = PlayerCamera.instance.cameraObject.transform.forward * PlayerInputManager.instance.verticalInput;
            jumpDirection += PlayerCamera.instance.cameraObject.transform.right * PlayerInputManager.instance.horizontalInput;
            jumpDirection.y = 0;

            if (jumpDirection != Vector3.zero)
            {
                //  IF WE ARE SPRINTING, JUMP DIRECTION IS AT FULL DISTANCE
                if (player.playerNetworkManager.isSprinting.Value)
                {
                    jumpDirection *= 1;
                }
                //  IF WE ARE RUNNING, JUMP DIRECTION IS AT HALF DISTANCE
                else if (PlayerInputManager.instance.moveAmount > 0.5)
                {
                    jumpDirection *= 0.5f;
                }
                //  IF WE ARE WALKING, JUMP DIRECTION IS AT QUARTER DISTANCE
                else if (PlayerInputManager.instance.moveAmount <= 0.5)
                {
                    jumpDirection *= 0.25f;
                }
            }
        }

        public void ApplyJumpingVelocity()
        {
            yVelocity.y = Mathf.Sqrt(jumpHeight * -2 * gravityForce);
        }

    }

}
