using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn_Manager : MonoBehaviour
{
    [SerializeField]
    private float _spawnDelay = 5.0f;
    [SerializeField]
    private GameObject[] _asteroids;
    private bool _stopSpawning = false;

    private int[] _screenEdgesCount = {0, 1 ,2 ,3};
    private float _screenEdgeRight;
    private float _screenEdgeLeft;
    private float _screenEdgeTop;
    private float _screenEdgeBottom;

    public void StartSpawning()
    {
        _screenEdgeRight = Camera.main.ViewportToWorldPoint(new Vector3(1.0f, 0, 0.0f)).x;
        _screenEdgeLeft = -Camera.main.ViewportToWorldPoint(new Vector3(1.0f, 0, 0.0f)).x;

        _screenEdgeTop = -Camera.main.ViewportToWorldPoint(new Vector3(1.0f, 0, 0.0f)).y;
        _screenEdgeBottom = Camera.main.ViewportToWorldPoint(new Vector3(1.0f, 0, 0.0f)).y;
        StartCoroutine(SpawnAsteroid());
    }

    IEnumerator SpawnAsteroid()
    {
        yield return new WaitForSeconds(_spawnDelay);
        while (_stopSpawning == false)
        {
            int randomAsteroid = Random.Range(0, _asteroids.Length);
            Vector3 spawnPos = SetSpawnPosition();
            Instantiate(_asteroids[randomAsteroid], spawnPos, Quaternion.identity);
            yield return new WaitForSeconds(_spawnDelay);
        }
    }

    private Vector3 SetSpawnPosition()
    {
        Vector3 PosToSpawn = new Vector3(1.0f, 0.5f, 0);
        float offset = 2.0f;
        int edgeNo = Random.Range(0, _screenEdgesCount.Length);
        //Top
        if(edgeNo == 0)
        {
            PosToSpawn = new Vector3(Random.Range(_screenEdgeLeft - offset, _screenEdgeRight + offset), Random.Range(_screenEdgeTop + offset, _screenEdgeTop + 4), 0);
        }
        //Bottom
        else if (edgeNo == 1)
        {
            PosToSpawn = new Vector3(Random.Range(_screenEdgeLeft - offset, _screenEdgeRight + offset), Random.Range(_screenEdgeBottom - offset, _screenEdgeBottom - 4), 0);
        }
        //Left
        else if (edgeNo == 2)
        {
            PosToSpawn = new Vector3(Random.Range(_screenEdgeLeft - offset, _screenEdgeLeft - 4), Random.Range(_screenEdgeBottom - offset, _screenEdgeTop + offset), 0);
        }
        //Right
        else if (edgeNo == 3)
        {
            PosToSpawn = new Vector3(Random.Range(_screenEdgeRight + offset, _screenEdgeRight + 4), Random.Range(_screenEdgeBottom - offset, _screenEdgeTop + offset), 0);
        }
        return PosToSpawn;
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}
