using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    public List<GameObject> spawnpoints;

    private void Awake()
    {
        Instance = this;
    }

    public Transform GetSpawnPoint(int index)
    {
        return spawnpoints[index - 1].transform;
    }
}
