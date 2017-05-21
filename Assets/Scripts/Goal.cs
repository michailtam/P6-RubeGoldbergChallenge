using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
  public GamePlay gamePlay;
  public bool hasEnteredGoal = false;
  
  private void OnCollisionEnter(Collision col)
  {
    if (col.gameObject.CompareTag("Throwable"))
      if(!hasEnteredGoal) {
        hasEnteredGoal = true;  // Prevents from checking multiple times cause of a bouncing ball
        gamePlay.HasPlayerCheated(); // Checks if the player has cheated
      }
  }
}
