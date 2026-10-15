using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using System.Drawing;
using System.ComponentModel;
using Unity.VisualScripting;

public class AsteroidManager : MonoBehaviour
{
    [SerializeField] private Rect m_spawnArea;
    [SerializeField] private int m_maxAsteroids;
    [SerializeField] private List<GameObject> m_asteroidPrefabs;
    [SerializeField] private int[] m_numberOfAsteroidSpawnOnDeath;

    private void Start()
    {
        StartCoroutine(SpawnAsteroidCoroutine());
    }

    private IEnumerator SpawnAsteroidCoroutine()
    {
        for(int i = 0; i < m_maxAsteroids; i++)
        {
            yield return new WaitForSeconds(.1f);

            Vector2 spawnPoint = new Vector2(
                Random.Range(m_spawnArea.xMin, m_spawnArea.xMax),
                Random.Range(m_spawnArea.yMin, m_spawnArea.yMax)
            );

            SpawnAsteroidsAtRandomPosition(3, spawnPoint);
        }
    }

    private void SpawnAsteroidsAtRandomPosition(int size, Vector2 spawnPoint)
    {
        IEnumerable<GameObject> sizePrefabs = m_asteroidPrefabs.Where((x) => x.GetComponent<AsteroidController>().Size == size);

        if(sizePrefabs == null || sizePrefabs.Count() <= 0) return;

        int index = Random.Range(0, sizePrefabs.Count());
        GameObject asteroidToSpawn = Instantiate(sizePrefabs.ElementAt(index), transform);

        asteroidToSpawn.transform.position = spawnPoint;

        AsteroidController controller = asteroidToSpawn.GetComponent<AsteroidController>();
        controller.onAsteroidDie += OnAsteroidDie;
    }


    private void OnAsteroidDie(AsteroidController asteroid)
    {
        int size = asteroid.Size;
        Vector2 asteroidSpawnPoint = asteroid.transform.position;

        Destroy(asteroid.gameObject);
        size--;

        int numToSpawn = m_numberOfAsteroidSpawnOnDeath[size];

        if(size > 0)
        {
            for(int i = 0; i < numToSpawn; i++)
            {
                SpawnAsteroidsAtRandomPosition(size, (Random.insideUnitCircle * 5f) + asteroidSpawnPoint);

            }
        }

    }

}
