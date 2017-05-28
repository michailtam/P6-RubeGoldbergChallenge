using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformPointFrom : MonoBehaviour {

  public Transform ballPosition;

  private void OnTriggerEnter(Collider col)
  {
    if (col.gameObject.CompareTag("Throwable")) {
      // Transforms the ball to the second transform point 
      ballPosition.position = transform.parent.GetComponent<TransformPointManager>().transformPoint2.position;
    }
  }
}
