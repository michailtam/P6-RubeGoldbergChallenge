using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMenuManager : MonoBehaviour {

  public List<GameObject> objectPrefabList;  // The prefab list that contains the prefab objects
  public int[] maxInstancesForEach;
  private List<GameObject> objectList;       // The list that contains the menu objects
  private int currentMenuObjectIndex = 0;    // The current index of the menu object 

  // Use this for initialization
  void Start ()
  {
    objectList = new List<GameObject>();
    
    // Add every prefab to the list
		foreach(Transform child in transform) {
      objectList.Add(child.gameObject);
    }
	}
	
  // Shifts one menu item to the left
	public void ShiftToLeft()
  {
    objectList[currentMenuObjectIndex].SetActive(false);
    currentMenuObjectIndex--;

    if (currentMenuObjectIndex < 0)
      currentMenuObjectIndex = objectList.Count - 1;
    objectList[currentMenuObjectIndex].SetActive(true);
  }

  // Shifts one menu item to the right
  public void ShiftToRight()
  {
    objectList[currentMenuObjectIndex].SetActive(false);
    currentMenuObjectIndex++;

    if (currentMenuObjectIndex > objectList.Count-1)
      currentMenuObjectIndex = 0;
    objectList[currentMenuObjectIndex].SetActive(true);
  }

  // Spawns the selected menu item
  public void SpawnCurrentObject()
  {
    // Checks if for the current prefab it is allowed to create an additional instance
    if (maxInstancesForEach[currentMenuObjectIndex] == 0)
      return;

    maxInstancesForEach[currentMenuObjectIndex]--; // Decreases the allowed instances

    // If the current selected item is the trampoline
    if (string.Compare(objectList[currentMenuObjectIndex].transform.name, "Trampoline") == 0) {
      // Creates a vector to instantiate under the shown prefab
      Vector3 posCreation = new Vector3(
        objectList[currentMenuObjectIndex].transform.GetChild(1).transform.position.x,
        objectList[currentMenuObjectIndex].transform.GetChild(1).transform.position.y,
        objectList[currentMenuObjectIndex].transform.GetChild(1).transform.position.z);

      // Create the trampoline at the position the object menu shows and rotate it vertically
      GameObject obj = Instantiate(objectPrefabList[currentMenuObjectIndex].gameObject,
        posCreation, Quaternion.identity);
      obj.transform.Rotate(new Vector3(-90,0,0)); 
      
      // Sets the physics properties of the trampoline 
      obj.GetComponent<Collider>().isTrigger = true;
      Rigidbody rig = obj.GetComponent<Rigidbody>();
      rig.useGravity = true;
      rig.isKinematic = false;
      obj.GetComponent<Collider>().isTrigger = false;
    }
    else {
      Instantiate(objectPrefabList[currentMenuObjectIndex].gameObject,
        objectList[currentMenuObjectIndex].transform.GetChild(1).transform.position,
        objectList[currentMenuObjectIndex].transform.GetChild(1).transform.rotation);
    }
  }
}
