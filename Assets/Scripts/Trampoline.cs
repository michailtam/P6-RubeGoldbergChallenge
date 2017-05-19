using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour {
  private bool stepSaved = false;

  void OnCollisionEnter(Collision col)
  {
    if(!stepSaved) {
      // Checks if the player has cheated the step
      /*GameObject.Find("GamePlay").GetComponent<GamePlay>().
        HasPlayerCheated(gameObject.transform.FindChild("ExitPoint").tag);
      stepSaved = true;*/
    }
  }
}
