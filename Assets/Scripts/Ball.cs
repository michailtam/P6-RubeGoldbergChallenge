using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {

  private void OnCollisionEnter(Collision col) {
    if(col.gameObject.CompareTag("Ground")) {
      // Wait 2 sec until the ball gets destroyed and
      // a new one gets respawn onto the pedastal
      StartCoroutine(DelayTillDestroy()); 
    }
  }

  IEnumerator DelayTillDestroy() {
    yield return new WaitForSeconds(2f);  
    GameObject.Find("GamePlay").GetComponent<GamePlay>().SpawnBall();
    Destroy(gameObject);
  }
}
