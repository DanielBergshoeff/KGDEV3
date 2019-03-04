using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowPlay : MonoBehaviour {

    public Camera fpsCamera;
    public LayerMask obstructionLayer;
    public Transform rightHandTransform;
    public GameObject playerLightPrefab;
    public GameObject gunGameObject;
    public Gun Gun;

    public float timePerShot = 0.1f;
    public float timer = 0.0f;

    public float timeTillReleaseLight = 3.0f;

    private GameObject playerLight;
    private bool pressingRightMouseButton = false;
    private float timerForLight = 0.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.E)) {
            TeleportToShadow();
        }
        if (Input.GetMouseButtonDown(1)) {
            InstantiatePlayerLight();
        }

        if(Input.GetMouseButtonUp(1) && pressingRightMouseButton) {
            CancelPlayerLight();
        }

        if(timer >= 0)
            timer -= Time.deltaTime;

        if (Input.GetMouseButton(0)) {
            TryShoot();
        }

        if(pressingRightMouseButton) {
            timerForLight += Time.deltaTime;
            if(timerForLight >= timeTillReleaseLight) {
                ThrowPlayerLight();
            }
        }
	}

    /// <summary>
    /// If the time since the last shot is more than the timePerShot parameter
    /// </summary>
    private void TryShoot() {
        if(timer < 0) {
            timer = timePerShot;
            Shoot();
        }
    }

    /// <summary>
    /// If the currently equipped gun has any ammo in the magazine, shoot a bullet
    /// </summary>
    private void Shoot() {
        if (Gun.amtOfBullets > 0) {
            GameObject bullet = Instantiate(Gun.BulletPrefab, Gun.BulletInstantiatePosition.transform.position, Gun.BulletPrefab.transform.rotation);
            bullet.GetComponent<Rigidbody>().AddForce(fpsCamera.transform.forward * 5000f);
            Gun.amtOfBullets -= 1;
        }
    }

    /// <summary>
    /// Instantiate the player light
    /// </summary>
    private void InstantiatePlayerLight() {
        playerLight = Instantiate(playerLightPrefab, rightHandTransform);
        playerLight.transform.localPosition = Vector3.zero;
        pressingRightMouseButton = true;
    }

    /// <summary>
    /// Add force to the player light in a forward direction
    /// </summary>
    private void ThrowPlayerLight() {
        playerLight.transform.parent = null;
        Rigidbody playerLightRigidbody = playerLight.GetComponent<Rigidbody>();
        playerLightRigidbody.isKinematic = false;
        playerLightRigidbody.AddForce(fpsCamera.transform.forward * 250f);
        pressingRightMouseButton = false;
        timerForLight = 0.0f;
    }

    /// <summary>
    /// Undo the instantiation of the player light
    /// </summary>
    private void CancelPlayerLight() {
        if(playerLight != null)
            Destroy(playerLight);
        pressingRightMouseButton = false;
        timerForLight = 0.0f;
    }

    /// <summary>
    /// Sends raycast forward and checks if position is in the shadows
    /// </summary>
    private void TeleportToShadow() {
        RaycastHit hit;
        Vector3 rayOrigin = fpsCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
        Debug.DrawRay(rayOrigin, fpsCamera.transform.forward * 1000.0f);
        if (Physics.Raycast(rayOrigin, fpsCamera.transform.forward, out hit, 1000.0f, obstructionLayer)) {
            if (hit.collider.CompareTag("Goal")) {
                if (LightManager.InShadow(hit.point) || InPlayerLightShadow(hit.point)) {
                    //Debug.Log("In shadow!");
                    transform.position = hit.point;
                }
                else {
                    //Debug.Log("Not in shadow!");
                }
            }
        }
    }

    /// <summary>
    /// Sends raycast from the player light to the position param and returns false if it reaches the position
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    private bool InPlayerLightShadow(Vector3 position) {
        if (playerLight != null) {
            if (!pressingRightMouseButton) {
                RaycastHit hit;
                var heading = position - playerLight.transform.position;
                var direction = heading / heading.magnitude;
                Debug.DrawRay(playerLight.transform.position, direction * 100f);
                float playerLightRange = playerLight.GetComponentInChildren<Light>().range;
                if (Vector3.Distance(position, playerLight.transform.position) < playerLightRange * 0.5f) {
                    if (Physics.Raycast(playerLight.transform.position, direction, out hit, playerLightRange * 0.5f, obstructionLayer)) {
                        if (hit.point != position) {
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }

    void OnGUI() {
        DrawQuad(new Rect(Screen.width / 2, Screen.height / 2, 1, 1), Color.white);
    }

    void DrawQuad(Rect position, Color color) {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        GUI.skin.box.normal.background = texture;
        GUI.Box(position, GUIContent.none);
    }
}
