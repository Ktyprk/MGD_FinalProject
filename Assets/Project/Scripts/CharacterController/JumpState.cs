using UnityEngine;


    public class JumpState : PlayerState
    {
        public override void EnterState(PlayerController player)
        {
            player.verticalVelocity = Mathf.Sqrt(player.jumpHeight * 2f * player.gravity);
            //player.playerAC.JumpStart();
        }
        public override void ExitState(PlayerController player)
        {
            
            //player.playerAC.JumpFinish();
        }

        public override void UpdateState(PlayerController player)
        {
            //player.playerAC.JumpIdle();
            player.HandleMovement();
            player.HandleRotation();
        }
    }

