using UnityEngine;
using Photon.Pun;

public class PlayerAnimatorController : MonoBehaviourPun, IPunObservable
{
    [SerializeField] Animator animator;

    private bool isIdling;

    private void Start()
    {
        if (photonView.IsMine) // Only the owner controls animations
        {
            GetComponent<PlayerController>().OnStateChange += HandleStateChange;
        }
    }

    private void HandleStateChange(PlayerState obj)
    {
        if (!photonView.IsMine) return; // Only the owner updates the Animator Controller
        animator.runtimeAnimatorController = obj.animatorOverrideController;
    }

    public void PlayAnim(string name)
    {
        if (!gameObject.activeInHierarchy || !photonView.IsMine) return;

        photonView.RPC(nameof(PlayAnimRPC), RpcTarget.All, name); // Sync animation across clients
    }

    [PunRPC]
    private void PlayAnimRPC(string name)
    {
        animator.CrossFadeInFixedTime(name, 0.1f, 0, 0);
    }

    public void Walk()
    {
        isIdling = false;
        PlayAnim("Walk");
    }

    public void Idle()
    {
        if (isIdling) return;
        isIdling = true;
        PlayAnim("Idle");
    }

    public void JumpStart()
    {
        PlayAnim("JumpStart");
    }

    public void JumpIdle()
    {
        PlayAnim("JumpIdle");
    }

    public void JumpFinish()
    {
        PlayAnim("JumpFinish");
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Send isIdling state to others
            stream.SendNext(isIdling);
        }
        else
        {
            // Receive isIdling state from others
            isIdling = (bool)stream.ReceiveNext();
        }
    }
}