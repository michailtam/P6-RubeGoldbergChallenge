using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
  public GamePlay gamePlay;

  private void OnCollisionEnter(Collision col)
  {
    if (col.gameObject.CompareTag("Throwable")) 
      gamePlay.CheckPlayer(); // Checks if the player can go to next level
  }
}
