
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class WaveData
{
    public string waveName;

    [Header("Enemy Counts")]
    public int[] enemyCounts = new int[4];

    [Header("Settings")]
    public float spawnInterval = 1.5f;
    public float nextWaveDelay = 5f;
}

public class WaveManager : MonoBehaviour
{
    [Header("Wave Configuration")]
    [SerializeField] private WaveData[] waves;

    [Header("References")]
    [SerializeField] private EnemySpawner enemySpawner;

    private int currentWaveIndex = 0;
    private int aliveEnemies = 0;

    private void Start()
    {
        if (enemySpawner == null)
        {
            Debug.LogError("EnemySpawner ulanmagan!");
            enabled = false;
            return;
        }

        if (waves == null || waves.Length == 0)
        {
            Debug.LogError("Wave list bo'sh!");
            enabled = false;
            return;
        }

        StartCoroutine(WaveRoutine());
    }

    private IEnumerator WaveRoutine()
    {
        while (currentWaveIndex < waves.Length)
        {
            WaveData wave = waves[currentWaveIndex];

            Debug.Log(
                "========== " + wave.waveName + " START =========="
            );

            yield return StartCoroutine(SpawnWave(wave));

            // Barcha enemylar yo'qolishini kutish
            yield return new WaitUntil(() => aliveEnemies == 0);

            Debug.Log(
                "========== " + wave.waveName + " COMPLETED =========="
            );

            currentWaveIndex++;

            if (currentWaveIndex < waves.Length)
            {
                Debug.Log("Next Wave boshlanishiga tayyorgarlik...");

                yield return new WaitForSeconds(
                    wave.nextWaveDelay
                );
            }
        }

        Debug.Log("LEVEL COMPLETED!");
    }

    private IEnumerator SpawnWave(WaveData wave)
    {
        // Enemylarni aralash tartibda chiqarish uchun ro'yxat
        List<int> spawnQueue = new List<int>();

        int[] remaining = new int[4];

        int totalEnemies = 0;

        for (int i = 0; i < 4; i++)
        {
            remaining[i] = Mathf.Max(0, wave.enemyCounts[i]);
            totalEnemies += remaining[i];
        }

        // Har xil enemylarni navbat bilan joylashtirish
        while (spawnQueue.Count < totalEnemies)
        {
            for (int i = 0; i < 4; i++)
            {
                if (remaining[i] > 0)
                {
                    spawnQueue.Add(i);
                    remaining[i]--;
                }
            }
        }

        // Enemylarni spawn qilish
        for (int i = 0; i < spawnQueue.Count; i++)
        {
            GameObject enemy = enemySpawner.SpawnEnemy(
                spawnQueue[i]
            );

            if (enemy != null)
            {
                EnemyWaveMember member =
                    enemy.GetComponent<EnemyWaveMember>();

                if (member == null)
                    member = enemy.AddComponent<EnemyWaveMember>();

                aliveEnemies++;
                member.Initialize(this);
            }

            if (i < spawnQueue.Count - 1)
            {
                yield return new WaitForSeconds(
                    wave.spawnInterval
                );
            }
        }
    }

    public void EnemyDefeated()
    {
        aliveEnemies--;

        aliveEnemies = Mathf.Max(0, aliveEnemies);

        Debug.Log("Alive Enemies: " + aliveEnemies);
    }

    public int GetCurrentWave()
    {
        return currentWaveIndex + 1;
    }

    public int GetTotalWaves()
    {
        return waves.Length;
    }

    public int GetAliveEnemies()
    {
        return aliveEnemies;
    }
}
