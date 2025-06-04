using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ProjectileScript : MonoBehaviour
{
    private bool collided;
    public GameObject impactEffectPrefab;

    [SerializeField] private ScoringScript scoringScript;
    private ZombieSpawnerScript zombieSpawnerScript;

    void Awake()
    {
        scoringScript = FindObjectOfType<ScoringScript>();
        zombieSpawnerScript = FindObjectOfType<ZombieSpawnerScript>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Bullet" && collision.gameObject.tag != "Player" && !collided)
        {
            if ((collision.gameObject.tag == "Zombie" || collision.gameObject.name == "Zombie") && !collided)
            {
                var zombie = collision.gameObject.GetComponent<ZombieScript>();
                if (zombie != null)
                {
                    zombie.hp -= 10;
                    Debug.Log("Zombie hit! Remaining HP: " + zombie.hp);
                    zombie.tookDamaged();
                    if (zombie.hp <= 0)
                    {
                        zombie.tookDamaged();
                        if (scoringScript != null)
                            scoringScript.AddScore(100);
                        if (zombieSpawnerScript != null)
                            zombieSpawnerScript.OnZombieDeath();
                        Debug.Log("Zombie killed! Score added.");
                        zombie.agent.isStopped = true;
                        Destroy(collision.gameObject);
                    }
                }
            }
            collided = true;

            var impact = Instantiate(impactEffectPrefab, collision.contacts[0].point, Quaternion.identity) as GameObject;

            Destroy(impact, 2f);
            Destroy(gameObject);
        }
    }
}
