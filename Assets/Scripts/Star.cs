using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour {

  public ParticleSystem particle;
  public AudioClip audioClip;

  private void OnTriggerEnter(Collider col) {
    if(col.gameObject.CompareTag("Throwable")) {
      //soundSource.Play();
      AudioSource.PlayClipAtPoint(audioClip, transform.position);
      
      // Decreases the amount of the remaining collectables (stars)
      GameObject.Find("GamePlay").GetComponent<GamePlay>().decreaseCollectibles(gameObject);
      Instantiate(particle, transform.position, Quaternion.identity);
      gameObject.SetActive(false);
    }
  }
}
