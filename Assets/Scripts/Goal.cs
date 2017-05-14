using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
  public ParticleSystem particle;
  public GamePlay gamePlay;

  private void OnCollisionEnter(Collision col)
  {
    if (col.gameObject.CompareTag("Throwable")) {
      // Checks if the player has cheated the steps
      if(gamePlay.HasPlayerCheated()) {
        Debug.Log("PLAYER HAS CHEATED THE STEPS");
      }
      else {
        Debug.Log("GOAL!!!");

        // Create a particle system for 5 sec to indicate that the ball is in the goal
        Instantiate(particle, transform.position, Quaternion.Euler(-90, 0, 0));
        Destroy(col.gameObject);    // Destroy the ball after entering the goal
      }
    }
  }
}
