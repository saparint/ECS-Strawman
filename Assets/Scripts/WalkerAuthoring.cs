using Unity.Entities;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public struct Walker : IComponentData
{
    public float ForwardSpeed;
    public float AngularSpeed;

    public static Walker Random(uint seed)
    {
        var random = new Random(seed);
        return new Walker(){ ForwardSpeed = random.NextFloat(0.1f, 0.8f),
                             AngularSpeed = random.NextFloat(0.5f, 4) };
    }
}

public class WalkerAuthoring : MonoBehaviour
{
    public float _forwardSpeed = 1;
    public float _angularSpeed = 1;

    class Baker : Baker<WalkerAuthoring>
    {
        public override void Bake(WalkerAuthoring src)
        {
            var data = new Walker()
            {
                ForwardSpeed = src._forwardSpeed,
                AngularSpeed = src._angularSpeed
            };
            AddComponent(GetEntity(TransformUsageFlags.Dynamic), data);
        }
    }
}
