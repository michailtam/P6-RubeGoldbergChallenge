using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fan : MonoBehaviour {

  public float fanBladeForceFactor = 300f;  // Force factor of the fanblade to multiply with
  private float fanBladeForce;  // The calculated force of the fanblades

  private void OnTriggerEnter(Collider col) {
    
    // Check if the ball has entered the air flow of the fan
    if(col.gameObject.CompareTag("Throwable")) {
      Rigidbody rig = col.GetComponent<Rigidbody>();
      rig.isKinematic = true;

      // Calculates the direction vector
      Vector3 direction = col.transform.position - transform.position;
      // Calculates the distance between the two vectors (the force of the blades).
      // IMPORTANT: The distance between the two vectors is bigger when throwing
      // with a greater force then throwing with a lesser force.
      fanBladeForce = direction.sqrMagnitude;

      Debug.Log("BLADES: " + transform.position + "    BLADE COLLIDER: " + col.transform.position + "   FORCE: " + fanBladeForce);
      rig.isKinematic = false;

      // Apply force to the ball (airflow)
      rig.AddForce(direction * fanBladeForce * fanBladeForceFactor);
    }
  }
}
