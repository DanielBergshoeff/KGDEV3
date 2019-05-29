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
            GameObject enemy = collision.gameObject;
            Debug.Log(enemy.name);
            EnemyAI enemyAI = null;
            while (true) {
                try {
                    enemyAI = enemy.GetComponent<EnemyAI>();
                    if (enemyAI == null)
                        enemy = enemy.transform.parent.gameObject;
                    else
                        break;
                }
                catch (System.NullReferenceException) {
                    enemyAI = GetComponent<EnemyAI>();
                    Debug.Log(enemyAI);
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

            if (enemyAI != null) {
                enemyAI.TakeDamage(damage);
                Debug.Log("Enemy damaged!");
            }

            UICanvas.DamageDone.Invoke(collision.contacts[0].point, damage);
        }
    }
}
