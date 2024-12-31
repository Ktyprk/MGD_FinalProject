using UnityEngine;
using System;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
        [SerializeField] private CharacterController characterController;
    
         public int Health = 10;
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
        
        public float spawnInterval = 0.5f; // Mermi spawn aralığı (saniye)
        public float bulletForce = 20f;   // Mermiye uygulanacak ileri yönlü kuvvet

        private bool isShooting = false;  // Mouse sol tuşu basılı mı?
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

            // Mouse sol tuşu bırakıldığında atış durur
            if (Input.GetMouseButtonUp(0))
            {
                isShooting = false;
                spawnTimer = 0f; // Zamanlayıcıyı sıfırla
            }

            // Eğer atış yapılıyorsa ve zamanlama uygunsa mermi spawn et
            if (isShooting)
            {
                spawnTimer += Time.deltaTime;
                if (spawnTimer >= spawnInterval)
                {
                    SpawnBullet();
                    spawnTimer = 0f;
                }
            }

            if (Health <= 0)
            {
                PhotonNetwork.Destroy(gameObject);
            }
            
            
        }
        
        void SpawnBullet()
        {
            GameObject bullet = PhotonNetwork.Instantiate(
                Path.Combine("PhotonPrefabs", "Bullet"), 
                transform.position + transform.forward * 2f, 
                Quaternion.identity
            );

            // Mermiye ileri yönlü kuvvet uygula
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
            if (inputVector.magnitude > 0.1f)
            {
                Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y).normalized;
                
                Quaternion targetRotation = Quaternion.LookRotation(moveDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
        }
    
        public void TakeDamage(int damage, Player _player)
        {
            photonView.RPC("RPC_TakeDamage", _player, damage);
        }

        [PunRPC]
        void RPC_TakeDamage(int damage)
        {
            Health -= damage;
            if(Health <= 0)
            {
                Health = 0;
                alive = false;
            }
            Debug.Log("Taken " + damage + " damage.");
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

