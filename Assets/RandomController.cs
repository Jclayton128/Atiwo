using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomController : MonoBehaviour
{
    public static RandomController Instance;

    [SerializeField] int _randomSeed = 1234;

    public int CurrentSeed => _randomSeed;

    private void Awake()
    {
        Instance = this;
        GenerateNewRandomSeed();
    }

    [ContextMenu("Generate New Random Seed")]
    public void GenerateNewRandomSeed()
    {
        _randomSeed = Random.Range(0, int.MaxValue);
    }
}
