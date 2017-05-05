using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fan : MonoBehaviour {

  public float fanForce = 10f;
  public float damping = 0.1f;

  private void OnTriggerEnter(Collider col) {
    
    // Check if the ball has entered the air flow of the fan
    if(col.gameObject.CompareTag("Ball")) {
      Debug.Log("BALL");
      
    }
  }
}
