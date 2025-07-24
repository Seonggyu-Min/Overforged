using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using Photon.Realtime;

namespace SCR
{
    public class PlayerController : MonoBehaviourPun
    {
        private Player player;
        private Vector2 inputdirection;
        private bool canMove;

        private void Awake()
        {
            player = GetComponent<Player>();
            inputdirection = new();
            canMove = true;
        }

        private void Update()
        {
            if (canMove)
                SetMove();
        }

        private void SetMove()
        {
            Vector3 direction = new Vector3(-inputdirection.y, 0, inputdirection.x);
            transform.position += player.PlayerPhysical.Speed * Time.deltaTime * direction;

        }


        private void OnMove(InputValue value)
        {
            if (!canMove) return;
            inputdirection = value.Get<Vector2>().normalized;

            if (inputdirection == Vector2.zero) Walk(false);
            else
            {
                Vector3 direction = new Vector3(-inputdirection.y, 0, inputdirection.x);
                transform.rotation = Quaternion.LookRotation(direction);
                Walk(true);
            }
        }

        private void OnDash()
        {
            if (!canMove) return;
            if (!player.PlayerPhysical.IsDash)
            {
                StartCoroutine(Dash());
            }
        }

        private void OnAction()
        {
            if (!canMove) return;
            GameObject ActionObj = player.PlayerPhysical.GetActionObj();

            // 집게를 들고 있을 때
            if (player.PlayerPhysical.UseTongs)
            {
                // 아이템을 들고 있다면
                if (player.PlayerPhysical.IsHold)
                {

                    if (ActionObj.CompareTag("상호작용 오브젝트"))
                    {
                        // 해당 오브젝트와 상호작용 아이템이고 사용 중이 아니라면 거기에 넣기
                        player.PlayerPhysical.IsHold = false;
                    }
                    else if (ActionObj == null)
                    {
                        // 아이템 내려놓기
                        player.PlayerPhysical.IsHold = false;
                    }
                }
                // 아이템을 들고 있지 않다면 
                else if (!player.PlayerPhysical.IsHold)
                {
                    if (ActionObj.CompareTag("상호작용 오브젝트"))
                    {
                        // 해당 오브젝트에 집게로 집을 수 있는 아이템이 있다면
                        // 아이템 들기
                        player.PlayerPhysical.IsHold = true;
                    }
                    else if (ActionObj.CompareTag("집게로 들어야 되는 아이템"))
                    {
                        // 아이템 들기
                        player.PlayerPhysical.IsHold = true;
                    }
                    else if (ActionObj == null)
                    {
                        // 집게 내려놓기
                        UseTongs(false);
                    }
                }
            }
            // 집게를 들고 있지 않을 때
            else
            {
                // 아이템을 들고 있다면
                if (player.PlayerPhysical.IsHold)
                {
                    if (ActionObj.CompareTag("상호작용 오브젝트"))
                    {
                        // 상호 작용 오브젝트가 비지 않고 해당 아이템을 넣을 수 있으면 넣기
                    }
                    else if (ActionObj == null)
                    {
                        // 아무것도 없다면 아이템 내려놓기
                        Holding(false);
                    }
                }
                // 아이템을 들고 있지 않다면 
                else if (!player.PlayerPhysical.IsHold)
                {
                    if (ActionObj.CompareTag("상호작용 오브젝트"))
                    {
                        // 상호 작용 오브젝트에 집게 없이 들 수 있는 아이템이 있다면 먹기
                        Holding(true);
                        // 상호 작용 오브젝트와 상호작용을 할 수 있는 상태하면 상호작용하기
                        // 용광로일때
                        StartCoroutine(Tempering());
                        // 모루일때
                        StartCoroutine(Hammering());
                        // 담금질할때
                        StartCoroutine(Tempering());
                    }
                    else if (ActionObj.CompareTag("아이템"))
                    {
                        // 아이템 들고 있기
                        Holding(true);
                    }
                    else if (ActionObj.CompareTag("집게"))
                    {
                        // 집게 들기
                        UseTongs(true);
                    }
                }
            }
        }

        private void OnThrow()
        {
            if (!canMove) return;
            StartCoroutine(Throw());
            if (player.PlayerPhysical.IsHold)
            {
                // 들고 있는 아이템에 AddForce 해주기
            }
        }

        private void OnShowOff()
        {
            if (!canMove) return;
            StartCoroutine(ShowOff());
        }

        private IEnumerator Dash()
        {
            player.PlayerPhysical.IsDash = true;
            player.Rigidbody.AddForce(transform.forward
                * player.PlayerPhysical.DashForce, ForceMode.Impulse);
            player.Animator.SetBool("Dash", true);
            float time = 0.8333f;
            while (time > 0.0f)
            {
                time -= Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }
            player.Animator.SetBool("Dash", false);
            player.Rigidbody.velocity = Vector3.zero;
            player.PlayerPhysical.IsDash = false;
        }
        private void Walk(bool move)
        {
            player.Animator.SetBool("Walk", move);
        }

        private void Holding(bool hold)
        {
            player.PlayerPhysical.IsHold = hold;
            player.Animator.SetBool("Hold", hold);
        }


        private IEnumerator Throw()
        {
            player.Animator.SetBool("Throw", true);
            float time = 0.417f;
            while (time > 0.0f)
            {
                time -= Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }
            player.Animator.SetBool("Throw", false);
        }


        private IEnumerator Hammering()
        {
            canMove = false;
            player.Animator.SetBool("Hammering", true);
            float time = 0.5f;
            while (time > 0.0f)
            {
                time -= Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }
            player.Animator.SetBool("Hammering", false);
            canMove = true;
        }


        private IEnumerator Tempering()
        {
            canMove = false;
            player.Animator.SetBool("Tempering", true);
            float time = 0.667f;
            while (time > 0.0f)
            {
                time -= Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }
            player.Animator.SetBool("Tempering", false);
            canMove = true;
        }


        private IEnumerator ShowOff()
        {
            canMove = false;
            player.Animator.SetBool("ShowOff", true);
            float time = 1f;
            while (time > 0.0f)
            {
                time -= Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }
            player.Animator.SetBool("ShowOff", false);
            canMove = true;
        }

        private void UseTongs(bool useTong)
        {
            player.PlayerPhysical.UseTongs = useTong;
            player.Animator.SetBool("UseTongs", useTong);
        }
    }
}
