using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowPlay : MonoBehaviour {

    public Camera fpsCamera;
    public LayerMask obstructionLayer;
    public LayerMask playerLayer;
    public LayerMask defaultLayer;
    public Transform rightHandTransform;
    public GameObject playerLightPrefab;
    public GameObject gunGameObject;
    public Gun Gun;
    public Transform gunPosition;
    public Canvas canvas;

    private UICanvas UICanvas;

    public GameObject ClaimedEntity = null;

    public float timePerShot = 0.1f;
    public float timer = 0.0f;

    public float timeTillReleaseLight = 3.0f;

    private GameObject playerLight;
    private bool pressingRightMouseButton = false;
    private float timerForLight = 0.0f;

	// Use this for initialization
	void Start () {
        UICanvas = canvas.GetComponent<UICanvas>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.E)) {
            TakeOverEntity();
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
        if (Gun != null) {
            if (timer < 0) {
                timer = timePerShot;
                Shoot();
            }
        }
    }

    /// <summary>
    /// If the currently equipped gun has any ammo in the magazine, shoot a bullet
    /// </summary>
    private void Shoot() {
        if (Gun.amtOfBullets > 0) {
            GameObject bullet = Instantiate(Gun.BulletPrefab, Gun.BulletInstantiatePosition.transform.position, Gun.BulletInstantiatePosition.transform.rotation);

            RaycastHit hit;
            Vector3 hitPos = fpsCamera.transform.position + fpsCamera.transform.forward * 1000.0f;
            if(Physics.Raycast(fpsCamera.transform.position, fpsCamera.transform.forward, out hit, 1000.0f)){
                Debug.Log(hit.collider.gameObject.tag + " should be hit");
                hitPos = hit.point;
            }

            Vector3 heading = hitPos - bullet.transform.position;
            float distance = heading.magnitude;
            Vector3 direction = heading / distance;

            bullet.GetComponent<Rigidbody>().AddForce(direction * 5000f);
            Gun.amtOfBullets -= 1;
            UICanvas.AmmoInGun.text = Gun.amtOfBullets.ToString();
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
    /// Sends raycast forward and checks if position is in the shadow of enemy and player
    /// </summary>
    private void TakeOverEntity() {


        RaycastHit hit;
        Vector3 rayOrigin = fpsCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
        Debug.DrawRay(rayOrigin, fpsCamera.transform.forward * 1000.0f);

        if (Physics.Raycast(rayOrigin, fpsCamera.transform.forward, out hit, 10.0f, defaultLayer)){
            if (hit.collider.CompareTag("Gun")){
                Debug.Log("GUN!");
                if(gunGameObject != null) {
                    gunGameObject.transform.parent = null;
                    gunGameObject.GetComponent<Rigidbody>().isKinematic = false;
                }

                gunGameObject = hit.collider.gameObject;
                gunGameObject.transform.parent = GetComponentInChildren<Camera>().transform;
                gunGameObject.transform.localPosition = gunPosition.localPosition;
                gunGameObject.transform.rotation = gunPosition.rotation;
                Gun = gunGameObject.GetComponent<Gun>();
                gunGameObject.GetComponent<Rigidbody>().isKinematic = true;

                if(Gun != null) {
                    UICanvas.AmmoInGun.text = Gun.amtOfBullets.ToString();
                    UICanvas.TotalAmmo.text = Gun.amtOfBulletsTotal.ToString();
                }
            }
        }

        else if (Physics.Raycast(rayOrigin, fpsCamera.transform.forward, out hit, 1000.0f, obstructionLayer)) {
            if (hit.collider.CompareTag("Goal")) {
                GameObject shadowCaster = LightManager.InShadow(hit.point, obstructionLayer);
                GameObject shadowCasterPlayerLight = InPlayerLightShadow(hit.point, obstructionLayer);
                if ((shadowCaster != null || shadowCasterPlayerLight != null) && (LightManager.InShadow(hit.point, playerLayer) || InPlayerLightShadow(hit.point, playerLayer))) {
                    //Debug.Log("In shadow!");
                    if (shadowCaster == null)
                        shadowCaster = shadowCasterPlayerLight;

                    while (true) {
                        try {
                            shadowCaster = shadowCaster.transform.parent.gameObject;
                        }
                        catch(System.NullReferenceException) {
                            break;
                        }
                    }

                    //If the player is currently in control of an entity, leave that entity behind
                    if (ClaimedEntity != null) {
                        ClaimedEntity.transform.parent = null;
                        ClaimedEntity.SetActive(true);
                    }

                    //Take control of the enemy whose shadow is overlapping with the players shadow
                    transform.position = shadowCaster.transform.position;
                    shadowCaster.transform.parent = this.gameObject.transform;
                    ClaimedEntity = shadowCaster;

                    shadowCaster.SetActive(false);
                }
            }
        }
    }

    /// <summary>
    /// Sends raycast from the player light to the position param and returns false if it reaches the position
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    private GameObject InPlayerLightShadow(Vector3 position, LayerMask layer) {
        if (playerLight != null) {
            if (!pressingRightMouseButton) {
                RaycastHit hit;
                var heading = position - playerLight.transform.position;
                var direction = heading / heading.magnitude;
                Debug.DrawRay(playerLight.transform.position, direction * 100f);
                float playerLightRange = playerLight.GetComponentInChildren<Light>().range;
                if (Vector3.Distance(position, playerLight.transform.position) < playerLightRange * 0.5f) {
                    if (Physics.Raycast(playerLight.transform.position, direction, out hit, playerLightRange * 0.5f, layer)) {
                        if (hit.point != position) {
                            return hit.collider.gameObject;
                        }
                    }
                }
            }
        }

        return null;
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
