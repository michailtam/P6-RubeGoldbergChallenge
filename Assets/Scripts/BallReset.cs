using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallReset : MonoBehaviour {

  public ParticleSystem particle;
  private bool hasBallEneteredGround = false;

  private void OnCollisionEnter(Collision col) {
    if (col.gameObject.CompareTag("Ground")) {
      if(!hasBallEneteredGround) {
        hasBallEneteredGround = true;

        // Indicate that the ball has hit the ground
        GameObject.Find("GamePlay").GetComponent<GamePlay>().groundEntered = true;
        Instantiate(particle, transform.position, Quaternion.Euler(-90, 0, 0));
        hasBallEneteredGround = false;
      }
    }
  }
}
