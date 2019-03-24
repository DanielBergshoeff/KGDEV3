using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour {

    public static LightManager lightManager;

    public List<GameObject> lights;

	// Use this for initialization
	void Start () {
        lightManager = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// Returns the object casting the shadow if the position is in its shadow
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public static GameObject InShadow(Vector3 position, LayerMask layerToCheck) {
        Debug.Log("Checking shadow");
        List<GameObject> lightsInRange = lightManager.GetLightsInRange(position);

        for (int i = 0; i < lightsInRange.Count; i++) {
            RaycastHit hit;
            var heading = position - lightsInRange[i].transform.position;
            var direction = heading / heading.magnitude;
            Debug.DrawRay(lightsInRange[i].transform.position, direction * lightsInRange[i].GetComponent<Light>().range);
            if (Physics.Raycast(lightsInRange[i].transform.position, direction, out hit, lightsInRange[i].GetComponent<Light>().range, layerToCheck)) {
                if (hit.point != position) {
                    return hit.collider.gameObject;
                }
            }
        }
        
        return null;
    }

    /// <summary>
    /// Will return the light that shines the brighest at the position
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="lightsNotToChoose"></param>
    /// <returns></returns>
    public static GameObject GetClosestLightInRange(Vector3 pos, List<GameObject> lightsNotToChoose, GameObject go) {
        List<GameObject> lightsInRange = lightManager.GetLightsInRange(pos);

        float distTimesRange = 0.0f;
        GameObject light = null;

        for (int i = 0; i < lightsInRange.Count; i++) {
            if (!lightsNotToChoose.Contains(lightsInRange[i])) {
                var heading = pos - lightsInRange[i].transform.position;
                var direction = heading / heading.magnitude;
                var distance = heading.magnitude;
                RaycastHit hit;
                if (Physics.Raycast(lightsInRange[i].transform.position, direction, out hit, distance)) {
                    if (distance * lightsInRange[i].GetComponent<Light>().range > distTimesRange) {
                        if (hit.transform.parent.gameObject == go) {
                            distTimesRange = distance * lightsInRange[i].GetComponent<Light>().range;
                            light = lightsInRange[i];
                        }
                    }
                }
            }
        }

        return light;
    }


    /// <summary>
    /// Will return the light that shines the brighest at the position
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="lightsNotToChoose"></param>
    /// <returns></returns>
    public static GameObject GetClosestLightInRange(Vector3 pos, List<GameObject> lightsNotToChoose) {
        List<GameObject> lightsInRange = lightManager.GetLightsInRange(pos);

        float distTimesRange = 0.0f;
        GameObject light = null;

        for (int i = 0; i < lightsInRange.Count; i++) {
            if (!lightsNotToChoose.Contains(lightsInRange[i])) {
                var heading = pos - lightsInRange[i].transform.position;
                var direction = heading / heading.magnitude;
                var distance = heading.magnitude;
                RaycastHit hit;
                if (!Physics.SphereCast(lightsInRange[i].transform.position, 0.6f, direction, out hit, distance)) {
                    if(distance * lightsInRange[i].GetComponent<Light>().range > distTimesRange) {
                        distTimesRange = distance * lightsInRange[i].GetComponent<Light>().range;
                        light = lightsInRange[i];
                    }
                }
            }
        }

        return light;
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
