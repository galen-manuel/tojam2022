using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThingSpawner : MonoBehaviour
{
    public Thing ThingPrefab;
    public PlayerController.Controls Controls;
    public float SpawnVariance = 0.2f;
    public float SpawnTime = 1.0f;
    public int NumberToSpawn = 2;

    private bool _running;
    private IEnumerator _spawnCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        _running = true;
        _spawnCoroutine = Spawn();
        StartCoroutine(_spawnCoroutine);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator Spawn()
    {
        while (_running)
        {
            CreateThings();
            yield return new WaitForSeconds(SpawnTime + Random.Range(-SpawnVariance, SpawnVariance));
        }
    }

    private void CreateThings()
    {
        for (int i = 0; i < NumberToSpawn; i++)
        {
            Thing thing = Instantiate(ThingPrefab, new Vector2(transform.position.x,
            Random.Range(WorldController.WORLD_BOUNDS.z, WorldController.WORLD_BOUNDS.w)), Quaternion.identity);
            var dir = Random.insideUnitCircle;
            switch (Controls)
            {
                case PlayerController.Controls.WASD:
                    if (dir.x < 0)
                    {
                        dir.x *= -1;
                    }
                    break;
                case PlayerController.Controls.Arrow:
                    if (dir.x > 0)
                    {
                        dir.x *= -1;
                    }
                    break;
                default:
                    break;
            }
            thing.Init(dir);
        }
    }

    private void OnDestroy()
    {
        _running = false;
        StopCoroutine(_spawnCoroutine);
    }
}
