using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;

public class ThingSpawner : MonoBehaviour
{
    private const string TWEEN_ID_SEQ_GAME_OVER = "TweenIdSeqGameOver";

    #region Public Variables

    public Thing[] ThingPrefabs;
    public PlayerController.Controls Controls;
    public float SpawnVariance = 0.2f;
    public float SpawnTime = 1.0f;
    public int NumberToSpawn = 2;

    #endregion

    #region Private Variables

    private bool _isRunning;
    private IEnumerator _spawnCoroutine;
    private List<Thing> _things;

    #endregion

    private void Awake()
    {
        _things = new List<Thing>();
        Subscribe();
    }

    private void Start()
    {
        _isRunning = true;
        _spawnCoroutine = Spawn();
        StartCoroutine(_spawnCoroutine);
    }

    #region Private Methods

    private void Subscribe()
    {
        Messenger.AddListener<Thing>(Constants.EVENT_DESTROY_THING, OnDestroyThing);
        Messenger.AddListener(Constants.EVENT_GAME_OVER, OnGameOver);
    }

    private void Unsubscribe()
    {
        Messenger.RemoveListener<Thing>(Constants.EVENT_DESTROY_THING, OnDestroyThing);
        Messenger.RemoveListener(Constants.EVENT_GAME_OVER, OnGameOver);
    }

    private IEnumerator Spawn()
    {
        while (_isRunning)
        {
            CreateThings();
            yield return new WaitForSeconds(SpawnTime + Random.Range(-SpawnVariance, SpawnVariance));
        }
    }

    private void CreateThings()
    {
        for (int i = 0; i < NumberToSpawn; i++)
        {
            Thing thing = Instantiate(
                ThingPrefabs[Random.Range(0, ThingPrefabs.Length)],
                new Vector2(transform.position.x,
                    Random.Range(WorldController.WORLD_BOUNDS.z,
                    WorldController.WORLD_BOUNDS.w)), Quaternion.identity);
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
            if (thing is BadThing badThing)
            {
                badThing.Init(Controls, dir);
            }
            else
            {
                thing.Init(dir);
            }

            _things.Add(thing);
        }
    }

    #endregion

    #region Event Handlers

    private void OnDestroyThing(Thing thing)
    {
        if (thing == null)
        {
            throw new ArgumentNullException(nameof(thing));
        }

        // There are multipler spawners. When a destroy event happens, we need to check to makes sure this spawner
        // has the item we want to destroy.
        if (_things.Remove(thing))
        {
            Destroy(thing.transform.root.gameObject);
        }
    }

    private void OnGameOver()
    {
        _isRunning = false;
        StopCoroutine(_spawnCoroutine);

        var sequence = DOTween.Sequence();

        foreach (Thing thing in _things)
        {
            sequence.Insert(0f,
                DOTween.To(() => thing.Velocity, v => thing.Velocity = v, Vector2.zero, 1.0f)
                       .SetEase(Ease.OutSine)
            );

            sequence.Insert(0f,
                DOTween.To(() => thing.AngularVelocity, av => thing.AngularVelocity = av, 0f, 1.0f)
                       .SetEase(Ease.OutSine)
            );

            sequence.Insert(0.25f, thing.transform.root.DOShakePosition(1.0f, 0.25f));
            sequence.Insert(0.25f, thing.transform.root.DOShakeScale(1.0f, 0.4f));
            sequence.Insert(0.25f, thing.transform.root.DOShakeRotation(1.0f));
            sequence.Insert(1.0f, thing.transform.root.DOScale(0f, 0.5f)
                                                      .SetEase(Ease.InExpo));
            sequence.OnComplete(OnGameOverAnimationComplete);
            sequence.SetId(TWEEN_ID_SEQ_GAME_OVER);
        }
    }

    private void OnGameOverAnimationComplete()
    {
        Messenger.Broadcast(Constants.EVENT_GAME_OVER_ANIMATION_COMPLETE, MessengerMode.REQUIRE_LISTENER);
    }

    private void OnDestroy()
    {
        _isRunning = false;
        DOTween.Kill(TWEEN_ID_SEQ_GAME_OVER);
        StopCoroutine(_spawnCoroutine);
        Unsubscribe();
    }

    #endregion
}
