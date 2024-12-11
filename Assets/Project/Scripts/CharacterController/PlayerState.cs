using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class PlayerState : MonoBehaviour
{
         public AnimatorOverrideController animatorOverrideController;
    
         public abstract void EnterState(PlayerController player);
         public abstract void ExitState(PlayerController player);
         public abstract void UpdateState(PlayerController player);
    
    
}

