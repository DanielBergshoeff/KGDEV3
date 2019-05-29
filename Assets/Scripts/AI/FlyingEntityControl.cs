using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FlyingEntityControl : MonoBehaviour {
    public GameObject FlyingEntityPrefab;
    public Transform spawnPosition;

    private MyGeneticAlgorithm<float> ga;
    private System.Random random;

    private int population = 20;

    private FlyingEntity[] flyingEntities;

    // Use this for initialization
    void Start () {
        random = new System.Random();
        ga = new MyGeneticAlgorithm<float>(population, 3, random, GetRandomEntityValues, FitnessFunction, 5, 0.01f);
        flyingEntities = new FlyingEntity[population];

        for (int i = 0; i < population; i++) {
            FlyingEntity flyingEntity = Instantiate(FlyingEntityPrefab, spawnPosition.position, Quaternion.identity).GetComponent<FlyingEntity>();
            flyingEntities[i] = flyingEntity;
            flyingEntities[i].entityValues = new FlyingEntityValues();
            flyingEntity.entityValues.DistanceForCollisionCheck = ga.Population[i].Genes[0];
            flyingEntity.entityValues.DistanceForCollisionLeftAndRight = ga.Population[i].Genes[1];
            flyingEntity.entityValues.Speed = ga.Population[i].Genes[2];
        }
    }

    private float FitnessFunction(int index) {
        float score = 0;
        FlyingEntity flyingEntity = flyingEntities[index];
        
        float fitnessTime = 1 - Mathf.Clamp(Mathf.InverseLerp(0f, 30f, flyingEntities[index].timer), 0f, 1f);
        float fitnessDistance = 1 - Mathf.Clamp(Mathf.InverseLerp(0f, 100f, Vector3.Distance(flyingEntities[index].gameObject.transform.position, FlyingEntity.target.transform.position)), 0f, 1f);
        float crashed = flyingEntities[index].crashed ? 0.0f : 1.0f;
        score += fitnessTime + fitnessDistance + crashed;

        return score / 3;
    }

    private float GetRandomEntityValues(int index) {
        float returnFloat = 0.0f;
        switch (index) {
            case 0:
                returnFloat = random.Next(500, 2000) / 100f;
                break;
            case 1:
                returnFloat = random.Next(1500, 6000) / 100f;
                break;
            case 2:
                returnFloat = random.Next(100000, 300000) / 100f;
                break;
        }
        return returnFloat;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            ga.NewGeneration();

            for (int i = 0; i < flyingEntities.Length; i++) {
                Destroy(flyingEntities[i].gameObject);
                flyingEntities[i] = null;
            }

            for (int i = 0; i < population; i++) {
                FlyingEntity flyingEntity = Instantiate(FlyingEntityPrefab, spawnPosition.position, Quaternion.identity).GetComponent<FlyingEntity>();
                flyingEntities[i] = flyingEntity;
                flyingEntity.entityValues.DistanceForCollisionCheck = ga.Population[i].Genes[0];
                flyingEntity.entityValues.DistanceForCollisionLeftAndRight = ga.Population[i].Genes[1];
                flyingEntity.entityValues.Speed = ga.Population[i].Genes[2];
            }
        }
    }
}
