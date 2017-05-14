using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour {
  private bool stepSaved = false;

  void Update()
  {
    // Checks if the ball has hit the ground
    bool ballGrounded = GameObject.Find("GamePlay").GetComponent<GamePlay>().groundEntered;
    if (ballGrounded) {
      Debug.Log("STEP RESET");
      stepSaved = false;
    }
  } 

  void OnCollisionEnter(Collision col)
  {
    if(!stepSaved) {
      GameObject.Find("GamePlay").GetComponent<GamePlay>().tag = gameObject.tag;
      Debug.Log("STEP SAVED");
      stepSaved = true;
    }
  }
}
