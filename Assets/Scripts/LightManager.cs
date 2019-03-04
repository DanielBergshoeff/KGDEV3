using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour {

    public static LightManager lightManager;

    public List<GameObject> lights;
    public LayerMask layerObstruction;

	// Use this for initialization
	void Start () {
        lightManager = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// Returns true if the position is in the shadow of any light that is in range
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public static bool InShadow(Vector3 position) {
        List<GameObject> lightsInRange = lightManager.GetLightsInRange(position);

        for (int i = 0; i < lightsInRange.Count; i++) {
            RaycastHit hit;
            var heading = position - lightsInRange[i].transform.position;
            var direction = heading / heading.magnitude;
            Debug.DrawRay(lightsInRange[i].transform.position, direction * lightsInRange[i].GetComponent<Light>().range);
            if (Physics.Raycast(lightsInRange[i].transform.position, direction, out hit, lightsInRange[i].GetComponent<Light>().range, lightManager.layerObstruction)) {
                if (hit.point != position) {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Returns the lights that are in range of the position param
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    private List<GameObject> GetLightsInRange(Vector3 position) {
        List<GameObject> lightsInRange = new List<GameObject>();
        for (int i = 0; i < lights.Count; i++) {
            float lightDistance = Vector3.Distance(lights[i].transform.position, position);
            if (lights[i].GetComponent<Light>().range * 0.5f >= lightDistance) { //If the light distance is well within the range of the light
                lightsInRange.Add(lights[i]);
            }
        }

        return lightsInRange;
    }
}
