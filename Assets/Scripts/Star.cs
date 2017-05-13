using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour {

  public ParticleSystem particle;

  private void OnTriggerEnter(Collider col) {
    if(col.gameObject.CompareTag("Throwable")) {
      Instantiate(particle, transform.position, Quaternion.identity);
      Destroy(gameObject);
    }
  }
}
