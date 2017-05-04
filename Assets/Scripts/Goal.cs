using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour {

  public ParticleSystem particle;

  private void OnCollisionEnter(Collision col) {
    if (col.gameObject.CompareTag("Ball")) {
      // Create a particle system for 5 sec to indicate that the ball is in the goal
      ParticleSystem p = Instantiate(particle, transform.position, Quaternion.Euler(-90,0,0));
      Destroy(col.gameObject);    // Destroy the ball after entering the goal
      Destroy(p, 3f);
    }
  }
}
