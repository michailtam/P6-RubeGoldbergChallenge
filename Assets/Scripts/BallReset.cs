using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallReset : MonoBehaviour {
  public LayerMask goalLayer;
  public ParticleSystem particle;

  private void OnCollisionEnter(Collision col) {
    if (col.gameObject.CompareTag("Ground")) {
      // Resets the level and indicates that the ball has hit the ground
      Instantiate(particle, transform.position, Quaternion.Euler(-90, 0, 0));
      GameObject.Find("GamePlay").GetComponent<GamePlay>().ResetLevel();
    }
  }
}
