using System;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public enum Side
    {
        Left,
        Right
    }

    [SerializeField] private Collider2D _collider;

    public Side WorldSide;
    public Action<Side, Thing> Scored;
    public ParticleSystem GoodSystemToSpawnOnScore;
    public ParticleSystem BadSystemToSpawnOnScore;

    private void Awake()
    {
        GameHelper.IsNull(GoodSystemToSpawnOnScore);
        GameHelper.IsNull(BadSystemToSpawnOnScore);

        Subscribe();
    }

    private void Subscribe()
    {
        Messenger.AddListener(Events.GAME_OVER, OnGameOver);
    }

    private void Unsubscribe()
    {
        Messenger.RemoveListener(Events.GAME_OVER, OnGameOver);
    }

    private void SpawnParticles(ParticleSystem systemToSpawn, Vector3 position)
    {
        ParticleSystem system = Instantiate(systemToSpawn, position, Quaternion.identity);
        var vel = system.velocityOverLifetime;
        vel.x = WorldSide == Side.Right ? new ParticleSystem.MinMaxCurve(vel.x.constantMin * -1, vel.x.constantMax * -1) : vel.x;
    }

    #region Event Handlers

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var thing = collision.transform.root.GetComponent<Thing>();
        if (thing)
        {
            SpawnParticles(thing is BadThing ? BadSystemToSpawnOnScore : GoodSystemToSpawnOnScore, collision.ClosestPoint(thing.transform.position));
            thing.SpawnOut();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var thing = collision.transform.root.GetComponent<Thing>();
        if (thing)
        {
            Messenger.Broadcast(Events.PORTAL_SCORED, WorldSide, thing, MessengerMode.REQUIRE_LISTENER);
            Messenger.Broadcast(Events.DESTROY_THING, thing, MessengerMode.REQUIRE_LISTENER);

            SpawnParticles(thing is BadThing ? BadSystemToSpawnOnScore : GoodSystemToSpawnOnScore, thing.transform.position);
        }
    }

    private void OnGameOver()
    {
        _collider.enabled = false;
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    #endregion
}
