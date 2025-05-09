using UnityEngine;

public class Sphere {
    public float Mass;
    public float Scale;
    public Vector3 Position;
    public Vector3 Velocity;
    public GameObject SphereGameObject;
    
    public Sphere(float mass, float scale, Vector3 position, Vector3 velocity, GameObject sphere) {
        Mass = mass;
        Scale = scale;
        Position = position;
        Velocity = velocity;
        SphereGameObject = Object.Instantiate(sphere, Position, Quaternion.identity);
        PhysicsSimulation.SetWorldScale(sphere.transform, new Vector3(scale, scale, scale));
    }
}

public interface IForce {
    public abstract Vector3 GetForce(Sphere p);
}

public class ConstantForce : IForce {
    private Vector3 _force;

    public ConstantForce(Vector3 force) {
        _force = force;
    }

    public Vector3 GetForce(Sphere p) {
        return  _force;
    }

    public void SetForce(Vector3 force) {
        _force = force;
    }
}

public class ViscousDragForce : IForce {
    private float k_d;
    public ViscousDragForce(float k_d)
    {
        this.k_d = k_d;
    }

    public Vector3 GetForce(Sphere p) {
        return -k_d * p.Velocity;
    }

    public void SetDragCoefficient(float k_d)
    {
        this.k_d = k_d;
    }
}
