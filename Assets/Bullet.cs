using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Bullet : MonoBehaviour {


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision) {
        Debug.Log(collision.collider.gameObject.tag + " hit");
        if (collision.gameObject.CompareTag("Enemy")) {
            Debug.Log("Enemy!");
            GameObject enemy = collision.gameObject;
            while (true) {
                try {
                    enemy = enemy.transform.parent.gameObject;
                }
                catch (System.NullReferenceException) {
                    break;
                }
            }

            float damage = 0.0f;

            switch (collision.gameObject.name) {
                case "Head":
                    damage = 100.0f;
                    break;
                case "Body":
                    damage = 20.0f;
                    break;
                case "LeftArm":
                case "RightArm":
                    damage = 10.0f;
                    break;
            }

            UICanvas.DamageDone.Invoke(collision.contacts[0].point, damage);
        }
        else {
            UICanvas.DamageDone.Invoke(collision.contacts[0].point, 0);
        }
    }
}
