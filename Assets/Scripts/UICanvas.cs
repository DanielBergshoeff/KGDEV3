using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[System.Serializable]
public class DamageEvent : UnityEvent<Vector3, float> { }

public class UICanvas : MonoBehaviour {

    public static DamageEvent DamageDone;

    public GameObject damagePrefab;
    public GameObject damageUI;

    public Text AmmoInGun;
    public Text TotalAmmo;

	// Use this for initialization
	void Start () {
        if(DamageDone == null)
            DamageDone = new DamageEvent();

        DamageDone.AddListener(UIDamage);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void UIDamage(Vector3 pos, float damage) {
        GameObject damageObject = Instantiate(damagePrefab);
        damageObject.transform.SetParent(damageUI.transform);
        damageObject.transform.position = pos;

        Text damageText = damageObject.GetComponent<Text>();
        damageText.text = damage.ToString();

        StartCoroutine(RemoveObjectAfterTime(damageObject, 3.0f));
    }

    private void RemoveText(GameObject go) {
        Destroy(go);
    }

    IEnumerator RemoveObjectAfterTime(GameObject go, float time) {
        yield return new WaitForSeconds(time);
        Destroy(go);
    }
}
