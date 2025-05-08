using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;

namespace MazeGameScripts
{
    public class MazeGameController : MonoBehaviour
    {        
        public GameObject playerGameObject; // Player sphere
        public float mazeRotationSpeed; // how much we want to rotate the maze
        public GameObject rotator;
        public GameObject winText;
        public Button againButton;
        
        public float mass;
        public Vector3 constantF;
        public float dragF; 

        private CustomCollider[] _colliders; 
        
        // Forces
        private List<IForce> _forces;
        private ConstantForce _constantForce;
        private ViscousDragForce _viscousDragForce;

        // Collectibles
        private List<CustomCollider> _collectibles;

        // Player sphere
        private Sphere _ball;
        private readonly Vector3 _startPos = new Vector3(2.23f, 0.5f, -0.7f);

        private Vector3 rotation;
        // Start is called before the first frame update
        private void Start()
        {            
            // Find all Collectible objects (SphereCollider)
            _collectibles = new List<CustomCollider>();
            var sphereColliders = GameObject.FindGameObjectsWithTag("SphereCollider");
            foreach (var sc in sphereColliders) {
                _collectibles.Add(sc.GetComponent<CustomCollider>());
            }
            
            // Initialize forces
            _constantForce = new ConstantForce(constantF);
            _viscousDragForce = new ViscousDragForce(dragF);
            _forces = new List<IForce>
            {
                _constantForce,
                _viscousDragForce
            };
            
            // Get all colliders
            _colliders = Object.FindObjectsByType<CustomCollider>(FindObjectsSortMode.None);
            
            // Initialize player
            _ball = new Sphere(mass, 1, _startPos, Vector3.zero, playerGameObject);
            Reset();
        }

        
        // Update is called once per frame
        private void Update()
        {
            UpdatePlayerValues();
            InputSystem();
            if (Vector3.Dot(_ball.Position, rotator.transform.up) <= -5) // Reset the game if the sphere falls off the maze
                Reset();


            // TODO: Use our physics engine to simulate one time step

            // Detect all colliders in scene
            foreach (var customCollider in _colliders)
            {
                // TODO: Use our physics engine to determine collision
                bool collided = false;

                if (collided && customCollider.transform.name == "Collectible")
                {
                    customCollider.gameObject.SetActive(false);
                }

                if (collided && customCollider.transform.parent.parent.name == "EndWall")
                {
                    winText.SetActive(true);
                    againButton.gameObject.SetActive(true);
                }
            }
        }

        private void UpdatePlayerValues()
        {
            _constantForce.SetForce(constantF);
            _viscousDragForce.SetDragCoefficient(dragF);
            _ball.Mass = mass;
        }

        void InputSystem()
        {
            // Input system for PC
            if (SystemInfo.deviceType == DeviceType.Desktop)
            {
                var moveHorizontal = Input.GetAxis("Horizontal");
                var moveVertical = Input.GetAxis("Vertical");
                if (moveHorizontal != 0 || moveVertical != 0)
                {
                    // Building of force vector
                    var movement = new Vector3(moveVertical, 0.0f, -moveHorizontal);
                    rotation += movement;
                    rotator.transform.rotation = Quaternion.Euler(rotation * mazeRotationSpeed);
                }
            }
        }
        
        void Reset()
        {
            winText.SetActive(false);
            againButton.gameObject.SetActive(false);
            // Restore the play area and player to original position
            rotation = Vector3.zero;
            rotator.transform.rotation = Quaternion.Euler(rotation);
            _ball.Position = _startPos;
            _ball.Velocity = Vector3.zero;

            
            // Find all Collectible objects and restore them 
            foreach (var collectible in _collectibles)
            {
                collectible.gameObject.SetActive(true);
            }
        }
    }
}