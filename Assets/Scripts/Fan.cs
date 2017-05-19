using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fan : MonoBehaviour {

  public float fanBladeForce = 100f;  // Force factor of the fanblade to multiply with
  
  private void OnTriggerEnter(Collider col) {
    
    // Check if the ball has entered the air flow of the fan
    if(col.gameObject.CompareTag("Throwable")) {
      Rigidbody rig = col.GetComponent<Rigidbody>();
      rig.isKinematic = true;

      // Calculates the direction vector
      Vector3 direction = col.transform.position - transform.position;
      rig.isKinematic = false;

      // Apply force to the ball (airflow)
      rig.AddForce(direction * 10f * fanBladeForce);
    }
  }
}
