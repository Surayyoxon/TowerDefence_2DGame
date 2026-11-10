
using UnityEngine;

public class EnemyWaveMember : MonoBehaviour
{
    private WaveManager waveManager;
    private bool isRegistered = false;

    public void Initialize(WaveManager manager)
    {
        waveManager = manager;
        isRegistered = true;
    }

    private void OnDestroy()
    {
        if (isRegistered && waveManager != null)
        {
            waveManager.EnemyDefeated();
        }
    }
}
