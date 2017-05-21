using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMenuManager : MonoBehaviour {

  public List<GameObject> objectPrefabList;  // The prefab list that contains the prefab objects
  public int[] countInstances;
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
    if (countInstances[currentMenuObjectIndex] == 0)
      return;

    countInstances[currentMenuObjectIndex]--; // Decreases the allowed instances

    // If it is a game object with rigidbody properties (i.e. trampoline)
    if (string.Compare(objectPrefabList[currentMenuObjectIndex].transform.GetChild(1).transform.name, "Trampoline") == 0) 
    {
      // Creates a vector to instantiate under the shown prefab
      Vector3 posCreation = new Vector3(
        objectList[currentMenuObjectIndex].transform.GetChild(1).transform.position.x,
        objectList[currentMenuObjectIndex].transform.GetChild(1).transform.position.y - 0.2f,
        objectList[currentMenuObjectIndex].transform.GetChild(1).transform.position.z);

      Transform obj = Instantiate(objectPrefabList[currentMenuObjectIndex].transform.GetChild(1),
      posCreation,
      objectList[currentMenuObjectIndex].transform.GetChild(1).transform.rotation);

      // Sets the physics properties of the trampoline 
      objectList[currentMenuObjectIndex].transform.GetChild(1).GetComponent<Collider>().isTrigger = true;
      Rigidbody rig = obj.gameObject.GetComponent<Rigidbody>();
      rig.useGravity = true;
      rig.isKinematic = false;
      objectList[currentMenuObjectIndex].transform.GetChild(1).GetComponent<Collider>().isTrigger = false;
    }
    else 
    {
      Instantiate(objectPrefabList[currentMenuObjectIndex].transform.GetChild(1),
        objectList[currentMenuObjectIndex].transform.GetChild(1).transform.position,
        objectList[currentMenuObjectIndex].transform.GetChild(1).transform.rotation);
    }
  }
}
