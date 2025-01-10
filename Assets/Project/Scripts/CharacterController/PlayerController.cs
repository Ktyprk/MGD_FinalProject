using UnityEngine;
using System;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using Unity.VisualScripting;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
        [SerializeField] private CharacterController characterController;
    
         public int enemydamage = 5;
         public bool alive = true;
    
        public PhotonView photonView;

        public PlayerAnimatorController playerAC;

        [Header("Settings")]
        [SerializeField] private float rotationSpeed = 10f;
        [SerializeField] private float walkSpeed = 5f;
        public float jumpHeight = 2f;
        public float gravity = 9.81f;
        
        public float verticalVelocity;

        private PlayerState currentState;
        public System.Action<PlayerState> OnStateChange;

        public PlayerState idleState, walkState, jumpState;

        [Header("Ground Check")]
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float groundCheckRadius = 0.2f; 
        [SerializeField] private Vector3 groundCheckOffset = new Vector3(0, -1f, 0); 
        
        private Vector3 velocity;
        private float jumpStartTime;
        
        [Header("Input")]
        private Vector2 inputVector;


        public Camera cam;
        
        public int MaxHealth = 100;
        public int Health;
        public Image HealthBar; 
    
        public void SetCamera(Camera assignedCam)
        {
            if (assignedCam == null)
            {
                Debug.LogError("Kamera atanamadı!");
                return;
            }

            cam = assignedCam;
        }
        
        public bool isJumping
        {
            get
            {
                return Input.GetKeyDown(KeyCode.Space);
            }
        }

        public bool isWalking
        {
            get
            {
                Vector2 inputVector = ControlsManager.Instance.controls.Player.Move.ReadValue<Vector2>();
                return inputVector.magnitude > 0.1f;
            }
        }

        public bool isGrounded
        {
            get
            {
                return Physics.CheckSphere(transform.position + groundCheckOffset, groundCheckRadius, groundLayer);
            }
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red; 
            Gizmos.DrawWireSphere(transform.position + groundCheckOffset, groundCheckRadius); 
        }

        private void Start()
        {
            photonView = GetComponent<PhotonView>();
            SetState(idleState);
        }

        public void SetState(PlayerState newState)
        {
            if (currentState != null)
            {
                currentState.ExitState(this);
            }

            currentState = newState;
            currentState.EnterState(this);
            OnStateChange?.Invoke(currentState);
        }
        
        public float spawnInterval = 0.5f; 
        public float bulletForce = 20f;   

        private bool isShooting = false;  
        private float spawnTimer = 0f; 

        private void Update()
        {
            if (!photonView.IsMine) return; 
                    InputHandler();
                
            if (currentState != null)
            {
                currentState.UpdateState(this);
            }
            
            if (isGrounded && ControlsManager.Instance.controls.Player.Jump.triggered && currentState != jumpState)
            {
                jumpStartTime = Time.time;
                SetState(jumpState);
            }else if (isWalking && isGrounded && currentState != walkState )
            {
                SetState(walkState);
            }
            else if (!isWalking && isGrounded && currentState != idleState )
            {
                SetState(idleState);
            }
            
            if (Input.GetMouseButtonDown(0))
            {
                isShooting = true;
            }
            
            if (Input.GetMouseButtonUp(0))
            {
                isShooting = false;
                spawnTimer = 0f; 
            }
            
            if (isShooting)
            {
                spawnTimer += Time.deltaTime;
                if (spawnTimer >= spawnInterval)
                {
                    SpawnBullet();
                    spawnTimer = 0f;
                }
            }
            
        }
        
        void SpawnBullet()
        {
            GameObject bullet = PhotonNetwork.Instantiate(
                Path.Combine("PhotonPrefabs", "Bullet"), 
                transform.position + transform.forward * 2f, 
                Quaternion.identity
            );
            
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                bulletRb.AddForce(transform.forward * bulletForce, ForceMode.Impulse);
            }
        }

        public void HandleMovement()
        {
            Vector3 move = new Vector3(inputVector.x, 0, inputVector.y).normalized;
            
            move *= walkSpeed;
            
            characterController.Move(move * Time.deltaTime);
        }

        public void HandleRotation()
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                Vector3 targetPosition = hit.point;
                targetPosition.y = transform.position.y; 
                
                Vector3 direction = (targetPosition - transform.position).normalized;
                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
                }
            }
        }

    
        public void TakeDamage(int damage, Player player)
        {
            photonView.RPC("RPC_TakeDamage", player, damage);
        }

        [PunRPC]
        void RPC_TakeDamage(int damage)
        {
            Health -= damage;
            UpdateHealthBar();
            if(Health <= 0)
            {
                Health = 0;
                photonView.RPC("RPC_DestroyBullet", RpcTarget.All);
            }
            Debug.Log("Taken " + damage + " damage.");
            
        }
        
        void UpdateHealthBar()
        {
            if (HealthBar != null)
            {
                HealthBar.fillAmount = (float)Health / MaxHealth;
            }
        }
        
        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                TakeDamage(enemydamage, photonView.Owner);
            }
        }
        
        [PunRPC]
        void RPC_DestroyBullet()
        {
            Destroy(gameObject);
        }
        
        private float VerticalForceCalculation()
        {
            if (isGrounded && jumpStartTime < Time.time - 0.1f)
            {
                verticalVelocity = 0f;
            }
            else
            {
                verticalVelocity -= gravity * Time.deltaTime;
            }

            return verticalVelocity;
        }
        
        private void InputHandler()
        {
            inputVector = ControlsManager.Instance.controls.Player.Move.ReadValue<Vector2>();
        }

        
}

