using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class AsteroidManager : MonoBehaviour
{
    [SerializeField] private int m_maxAsteroids;
    [SerializeField] private List<GameObject> m_asteroidPrefabs;
    [SerializeField] private Rect m_spawnArea;

    private void Start()
    {
        StartCoroutine(SpawnAsteroidCoroutine());
    }

    private void SpawnAsteroidsAtRandomPosition()
    {
        int index = Random.Range(0, m_asteroidPrefabs.Count);
        GameObject asteroidToSpawn = Instantiate(m_asteroidPrefabs[index], transform);

        Vector2 spawnPoint = new Vector2(
            Random.Range(m_spawnArea.xMin, m_spawnArea.xMax),
            Random.Range(m_spawnArea.yMin, m_spawnArea.yMax)
        );

        asteroidToSpawn.transform.position = spawnPoint;
    }

    private IEnumerator SpawnAsteroidCoroutine()
    {
        for(int i = 0; i< m_maxAsteroids; i++)
        {
            yield return new WaitForSeconds(.1f);
            SpawnAsteroidsAtRandomPosition();
        }
    }

}
