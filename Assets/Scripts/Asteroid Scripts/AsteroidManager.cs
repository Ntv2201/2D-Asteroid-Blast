using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using System.Drawing;
using System.ComponentModel;
using Unity.VisualScripting;

public class AsteroidManager : MonoBehaviour
{
    [SerializeField] private int m_startingAsteroids;
    [SerializeField] private int m_maxAsteroids;
    [SerializeField] private float m_asteroidSpawnDelay;
    [SerializeField] private int m_currentAteroidCounts;
    [SerializeField] private Rect m_spawnArea;
    [SerializeField] private List<GameObject> m_asteroidPrefabs;
    [SerializeField] private int[] m_numberOfAsteroidSpawnOnDeath;

    private void OnEnable()
    {
        GameEvents.Instance.onGameOver += OnGameOver;
        GameEvents.Instance.onRetry += OnRetry;
    }
    private void OnDisable()
    {
        GameEvents.Instance.onRetry -= OnRetry;
        GameEvents.Instance.onGameOver -= OnGameOver;
    }

    private void Start()
    {
        OnRetry();
    }

    private IEnumerator SpawnInitialAsteroidCoroutine()
    {
        for(int i = 0; i < m_startingAsteroids; i++)
        {
            yield return new WaitForSeconds(.1f);

            SpawnAsteroidsAtRandomPosition(3, GetRandomSpawnPoint());
        }
        StartCoroutine(AsteroidSpawnerCoroutine());
    }

    private IEnumerator AsteroidSpawnerCoroutine()
    {
        while(m_currentAteroidCounts >= m_maxAsteroids)
        {
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(m_asteroidSpawnDelay);

        SpawnAsteroidsAtRandomPosition(3, GetRandomSpawnPoint());

        StartCoroutine(AsteroidSpawnerCoroutine());
    }

    private Vector2 GetRandomSpawnPoint()
    {
        Vector2 spawnPoint = new Vector2(
            Random.Range(m_spawnArea.xMin, m_spawnArea.xMax),
            Random.Range(m_spawnArea.yMin, m_spawnArea.yMax)
        );
        return spawnPoint;
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
        
        if(size == 3)
        {
            m_currentAteroidCounts++;
        }
    }


    private void OnAsteroidDie(AsteroidController asteroid)
    {
        int size = asteroid.Size;
        Vector2 asteroidSpawnPoint = asteroid.transform.position;

        Destroy(asteroid.gameObject);

        GameEvents.Instance.AddToScore(1);

        if(size == 3)
        {
            m_currentAteroidCounts--;
        }
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

    private void OnRetry()
    {
        foreach(Transform t in transform)
        {
            Destroy(t.gameObject);
        }
        StartCoroutine(SpawnInitialAsteroidCoroutine());
    }

    private void OnGameOver()
    {
        StopAllCoroutines();
        m_currentAteroidCounts = 0;
    }

}
