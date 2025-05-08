using System;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class PhysicsSimulation : MonoBehaviour
{
    public GameObject emitter; // Emits spheres
    public GameObject sphereGameObject; 

    // User-defined public variables.
    // These define the properties of an emitter
    public float mass = 0.1f;
    public float scale = 1;
    public float period = 0.5f;
    public Vector3 initialVelocity;
    public Vector3 constantF = new Vector3(0f, -9.8f, 0f);
    public float dragF = 0f;
    public int maxSpheres = 25;

    // Colliders in the scene
    private CustomCollider[] _colliders;

    // Emitted spheres
    private int _numSpheres;
    private int _sphereIndex;
    private List<Sphere> _spheres;
    private double _timeToEmit;
    
    // Forces
    private List<IForce> _forces;
    private ConstantForce _constantForce;
    private ViscousDragForce _viscousDragForce;

    // Initialize data
    private void Start()
    {
        _numSpheres = 0;
        _sphereIndex = 0;
        _colliders = FindObjectsByType<CustomCollider>(FindObjectsSortMode.None);
        _spheres = new List<Sphere>();

        _constantForce = new ConstantForce(constantF);
        _viscousDragForce = new ViscousDragForce(dragF);
        _forces = new List<IForce>
        {
            _constantForce,
            _viscousDragForce
        };
    }

    // Emits spheres, compute their position and velocity, and check for collisions
    private void FixedUpdate()
    {
        float deltaTime = Time.deltaTime;
        // Emit spheres
        _timeToEmit -= deltaTime;
        if (_timeToEmit <= 0.0) EmitSpheres();

        
        foreach (Sphere sphere in _spheres) // For each sphere 
        {
            // Compute their position and velocity by solving the system of forces using Euler's method
            ComputeSphereMovement(sphere, _forces);
           
            foreach (CustomCollider customCollider in _colliders) // For each collider 
            {
                // Check for and handle collisions
                OnCollision(sphere, customCollider);
            }
        }
    }

    private void EmitSpheres()
    {
        // Initialize local position of a sphere
        Vector3 localPos = new Vector3(0f, 0f, 0f);
        Vector3 localVelocity = initialVelocity;

        // Get the world position of a sphere
        Vector3 worldPos = emitter.transform.TransformPoint(localPos);
        Vector3 worldVelocity = emitter.transform.TransformDirection(localVelocity);

        // Initialize a sphere 
        Sphere sphere = new Sphere(mass, scale, worldPos, worldVelocity, sphereGameObject);

        if (_numSpheres < maxSpheres)
        {
            // Add another sphere
            _spheres.Add(sphere);
            _numSpheres++;
        }
        else
        {
            Sphere destroy = _spheres[_sphereIndex];
            Destroy(destroy.SphereGameObject);
            // Keep the number of sphere to a finite amount by just replacing the old sphere
            _spheres[_sphereIndex++] = sphere;
            // If the end is reached, reset the index to start remove the index-0 sphere
            if (_sphereIndex >= maxSpheres)
                _sphereIndex = 0;
        }

        // Reset the time
        _timeToEmit = period;
    }

    public static void ComputeSphereMovement(Sphere ball, List<IForce> forces)
    {
        // TODO: Calculate the ball's position and velocity by solving the system
        // of forces using Euler's method
        // (1) Calculate total forces

        // (2) Solve the system of forces using Euler's method,
        //     and update the ball's position and velocity.

        // Update the transform of the actual game object
        ball.SphereGameObject.transform.position = ball.Position;
    }

    public static bool OnCollision(Sphere ball, CustomCollider customCollider)
    {
        Transform colliderTransform = customCollider.transform;
        Vector3 colliderSize = colliderTransform.lossyScale; // size of collider

        // Save current localScale value, and temporarily change the collider's
        // world scale to (1,1,1) for our calculations. (Don't modify this)
        Vector3 curLocalScale = colliderTransform.localScale;
        SetWorldScale(colliderTransform, Vector3.one);

        // Position and velocity of the ball in the the local frame of the collider
        Vector3 localPos = colliderTransform.InverseTransformPoint(ball.Position);
        Vector3 localVelocity = colliderTransform.InverseTransformDirection(ball.Velocity);

        float ballRadius = ball.Scale / 2.0f;
        float colliderRestitution = customCollider.restitution;

        // TODO: In the following if conditions assign these variables appropriately.
        bool collisionOccurred = false;      // if the ball collides with the collider.
        bool isEntering = false;             // if the ball is moving towards the collider.
        Vector3 normal = Vector3.zero;       // normal of the colliding surface.

        if (customCollider.CompareTag("SphereCollider"))
        {
            // Collision with a sphere collider
            float colliderRadius = colliderSize.x / 2f;  // We assume a sphere collider has the same x,y, and z scale values

            // TODO: Detect collision with a sphere collider.
        }
        else if (customCollider.CompareTag("PlaneCollider"))
        {
            // Collision with a plane collider

            var planeHeight = colliderSize.x * 10; // height of plane, defined by the x-scale
            var planeWidth = colliderSize.z * 10; // width of plane, defined by the z-scale
            // Note: In Unity, a plane's actual size is its inspector values times 10.

            // TODO: Detect sphere collision with a plane collider


            // Generally, when the sphere is moving on the plane, the restitution alone is not enough
            // to counter gravity and the ball will eventually sink. We solve this by ensuring that
            // the ball stays above the plane.
            if (collisionOccurred && isEntering)
            {
                // TODO: Follow these steps to ensure the sphere always on top of the plane.
                //   1. Find the new localPos of the ball that is always on the plane
                //   2. Convert the localPos to worldPos
                //   3. Update the sphere's position with the new value
            }
        }


        if (collisionOccurred && isEntering)
        {
            // The sphere needs to bounce.
            // TODO: Update the sphere's velocity, remember to bring the velocity to world space
        }


        colliderTransform.localScale = curLocalScale; // Revert the collider scale back to former value
        return collisionOccurred;
    }

    // Set the world scale of an object
    public static void SetWorldScale(Transform transform, Vector3 worldScale)
    {
        transform.localScale = Vector3.one;
        Vector3 lossyScale = transform.lossyScale;
        transform.localScale = new Vector3(worldScale.x / lossyScale.x, worldScale.y / lossyScale.y,
            worldScale.z / lossyScale.z);
    }
}
