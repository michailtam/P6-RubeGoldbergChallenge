using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepExitPoint : MonoBehaviour
{

  private void OnTriggerEnter(Collider col)
  {
    GameObject.Find("GamePlay").GetComponent<GamePlay>().step = gameObject.tag;
  }
}
