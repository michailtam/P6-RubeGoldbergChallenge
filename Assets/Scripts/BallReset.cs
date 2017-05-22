using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallReset : MonoBehaviour {
  public LayerMask goalLayer;
  public ParticleSystem particle;
  public AudioClip[] audioClip;

  private void OnCollisionEnter(Collision col) {
    if (col.gameObject.CompareTag("Ground")) {
      AudioSource.PlayClipAtPoint(audioClip[1], transform.position);

      // Resets the level and indicates that the ball has hit the ground
      Instantiate(particle, transform.position, Quaternion.Euler(-90, 0, 0));
      GameObject.Find("GamePlay").GetComponent<GamePlay>().ResetLevel();
    }
    else
      AudioSource.PlayClipAtPoint(audioClip[0], transform.position);
  }
}
