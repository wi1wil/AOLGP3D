using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ZombieSpawnerScript : MonoBehaviour
{
    public GameObject zombiePrefab;
    public GameObject[] spawnPoints;
    public TMP_Text waveText;
    public TMP_Text zombiesLeftText;
    public int maxZombies;
    public float spawnInterval = 5f;
    private float nextSpawnTime;
    private int currentZombies = 0;

    public int startZombiesPerWave = 3;
    public int zombiesPerWaveIncrease = 2;
    public float waveDelay = 3f;

    private int zombiesToSpawnThisWave;
    private int zombiesSpawnedThisWave;
    private int currentWave = 1;
    private bool waitingForNextWave = false;

    void Start()
    {
        zombiesToSpawnThisWave = startZombiesPerWave;
        maxZombies = zombiesToSpawnThisWave;
        UpdateWaveUI();
    }

    void Update()
    {
        if (!waitingForNextWave)
        {
            if (Time.time >= nextSpawnTime && currentZombies < maxZombies && zombiesSpawnedThisWave < zombiesToSpawnThisWave)
            {
                SpawnZombie();
                nextSpawnTime = Time.time + spawnInterval;
            }

            if (zombiesSpawnedThisWave == zombiesToSpawnThisWave && currentZombies == 0)
            {
                StartCoroutine(StartNextWave());
            }
        }
        UpdateWaveUI();
    }

    void SpawnZombie()
    {
        if (spawnPoints.Length == 0 || zombiePrefab == null)
            return;

        if (currentZombies >= maxZombies || zombiesSpawnedThisWave >= zombiesToSpawnThisWave)
            return;

        int idx = Random.Range(0, spawnPoints.Length);
        GameObject spawnPoint = spawnPoints[idx];
        GameObject zombie = Instantiate(zombiePrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
        currentZombies++;
        zombiesSpawnedThisWave++;
        UpdateWaveUI();
    }

    public void OnZombieDeath()
    {
        currentZombies--;
        UpdateWaveUI();
    }

    private IEnumerator StartNextWave()
    {
        waitingForNextWave = true;
        yield return new WaitForSeconds(waveDelay);

        currentWave++;
        zombiesToSpawnThisWave += zombiesPerWaveIncrease;
        maxZombies = zombiesToSpawnThisWave;
        zombiesSpawnedThisWave = 0;
        waitingForNextWave = false;

        UpdateWaveUI();
        Debug.Log("Wave " + currentWave + " started! Zombies to spawn: " + zombiesToSpawnThisWave);
    }

    private void UpdateWaveUI()
    {
        if (waveText != null)
            waveText.text = "Wave: " + currentWave;
        if (zombiesLeftText != null)
            zombiesLeftText.text = "Zombies Left: " + currentZombies + " / " + zombiesToSpawnThisWave;
    }
}
