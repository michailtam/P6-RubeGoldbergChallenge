using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformPointManager : MonoBehaviour {
  public LineRenderer line;
  public Transform transformPoint1;
  public Transform transformPoint2;
  
	// Use this for initialization
	void Start () {
    line = Instantiate(line, transformPoint1.position, transformPoint1.rotation);
    line.gameObject.SetActive(true);
    DrawTransformLine();
  }
	
  private void DrawTransformLine()
  {
    var distance = Vector3.Distance(transformPoint1.position, transformPoint2.position);
    line.materials[0].mainTextureScale = new Vector3(distance, 1, 1);

    line.SetPosition(0, transformPoint1.position);
    line.SetPosition(1, transformPoint2.position);
  }
}
