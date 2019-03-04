using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class TestMovement : MonoBehaviour {
    public GameObject EntityPrefab;

    private GeneticAlgorithm<EntityValues> ga;
    private System.Random random;

    private int population = 10;
    private int minValueJumpDistance = 0;
    private int maxValueJumpDistance = 2000;
    private int minValueMovementSpeed = 1000;
    private int maxValueMovementSpeed = 3000;

    private Entity[] entities;

	// Use this for initialization
	void Start () {
        Debug.Log("Start");

        random = new System.Random();
        ga = new GeneticAlgorithm<EntityValues>(population, 1, random, GetRandomEntityValues, FitnessFunction, 5, 0.01f);
        entities = new Entity[population];

        for (int i = 0; i < population; i++) {
            Debug.Log("Instantiate child");
            Entity entity = Instantiate(EntityPrefab, new Vector3(-48 + i * 2, 0.5f, -45f), Quaternion.identity).GetComponent<Entity>();
            entities[i] = entity;
            entity.entityValues = ga.Population[i].Genes[0];
        }
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Space)) {
            ga.NewGeneration();

            for (int i = 0; i < entities.Length; i++) {
                Destroy(entities[i].gameObject);
                entities[i] = null;
            }

            for (int i = 0; i < population; i++) {
                Debug.Log("Instantiate child");
                Entity entity = Instantiate(EntityPrefab, new Vector3(-48 + i * 2, 0.5f, -45f), Quaternion.identity).GetComponent<Entity>();
                entities[i] = entity;
                entity.entityValues = ga.Population[i].Genes[0];
            }
        }
	}

    private EntityValues GetRandomEntityValues() {
        EntityValues entityValues = new EntityValues();
        float i = random.Next(minValueJumpDistance, maxValueJumpDistance);
        entityValues.distanceForJump = i / 100f;
        entityValues.speed = random.Next(minValueMovementSpeed, maxValueMovementSpeed);
        return entityValues;
    }

    private float FitnessFunction(int index) {
        float score = 0;
        DNA<EntityValues> dna = ga.Population[index];

        for (int i = 0; i < dna.Genes.Length; i++) {
            float fitnessTime = 1 - Mathf.Clamp(Mathf.InverseLerp(0f, 30f, entities[index].timer), 0f, 1f);
            float fitnessDistance = Mathf.Clamp(Mathf.InverseLerp(0f, 100f, Mathf.Abs(entities[i].gameObject.transform.position.z - Entity.goal.transform.position.z)), 0f, 1f);
            score += fitnessTime + fitnessDistance;
        }

        return score / 2;
    }
}
