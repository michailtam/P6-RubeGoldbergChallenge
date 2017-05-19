using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallReset : MonoBehaviour {
  public ParticleSystem particle;
  private Vector3 startPosition;

  void Start()
  {
    // Saves the init position in the current level
    startPosition = transform.position;
  }

  private void OnCollisionEnter(Collision col) {
    if (col.gameObject.CompareTag("Ground")) {
      // Resets the level and indicates that the ball has hit the ground
      GameObject.Find("GamePlay").GetComponent<GamePlay>().ResetLevel();
      Instantiate(particle, transform.position, Quaternion.Euler(-90, 0, 0));

      // Resets ball position (returns to the pedastal) and velocities
      transform.position = startPosition;
      Rigidbody rig = GetComponent<Rigidbody>();
      rig.velocity = Vector3.zero;
      rig.angularVelocity = Vector3.zero;
      rig.isKinematic = true;
    }
  }
}
