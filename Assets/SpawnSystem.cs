using Unity.Entities;
using UnityEngine;
using Unity.Transforms;
using Unity.Collections;
using Random = Unity.Mathematics.Random;
using Unity.Mathematics;
using Unity.Burst;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct SpawnCount : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();

        var instances = new NativeArray<Entity>(config.SpawnCount, Allocator.Temp);
        state.EntityManager.Instantiate(config.Prefab, instances);

        var rand = new Random(config.RandomSeed);

        foreach (var entity in instances)
        {
            if (!state.EntityManager.HasComponent<Dancer>(entity))
            {
                state.EntityManager.AddComponent<Dancer>(entity);
            }

            if (!state.EntityManager.HasComponent<Walker>(entity))
            {
                state.EntityManager.AddComponent<Walker>(entity);
            }

            var xform = SystemAPI.GetComponentRW<LocalTransform>(entity);

            xform.ValueRW = LocalTransform.FromPositionRotation(
                rand.NextFloat3Direction() * config.SpawnRadius, 
                quaternion.RotateY(rand.NextFloat(0, math.PI * 2))
            );

            var dancer = SystemAPI.GetComponentRW<Dancer>(entity);
            var walker = SystemAPI.GetComponentRW<Walker>(entity);

            dancer.ValueRW = Dancer.Random(rand.NextUInt());
            walker.ValueRW = Walker.Random(rand.NextUInt());
        }

        state.Enabled = false;
        instances.Dispose();
    }
}
