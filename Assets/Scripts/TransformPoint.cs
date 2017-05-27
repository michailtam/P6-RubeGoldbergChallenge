using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformPoint : MonoBehaviour {

  private void OnTriggerEnter(Collider col)
  {
    if (col.gameObject.CompareTag("Throwable")) {

    }
  }
}
