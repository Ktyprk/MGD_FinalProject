using UnityEngine;


    public class WalkState : PlayerState
    {
            public override void EnterState(PlayerController player)
            {
               player.playerAC.Walk();
            }

            public override void ExitState(PlayerController player)
            {

            }

            public override void UpdateState(PlayerController player)
            {
                player.HandleMovement();
                player.HandleRotation();
                
            }
    }


