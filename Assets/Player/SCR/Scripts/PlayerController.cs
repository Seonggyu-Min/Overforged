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
        private bool workingMode;

        private void Awake()
        {
            player = GetComponent<Player>();
            inputdirection = new();
            canMove = true;
            workingMode = false;
        }

        private void Update()
        {
            if (player.Animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                canMove = true;
            }
        }

        private void FixedUpdate()
        {
            if (canMove)
                SetMove();
        }

        private void SetMove()
        {
            Vector3 direction = new Vector3(-inputdirection.y, 0, inputdirection.x);
            transform.position += player.PlayerPhysical.Speed * Time.deltaTime * direction;

        }

        #region 키 입력
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
            if (player.PlayerPhysical.IsDash) return;
            StartCoroutine(Dash());
        }

        private void OnChangeState()
        {
            workingMode = !workingMode;
            ChangeState(workingMode);
        }

        private void OnAction()
        {
            if (!canMove) return;
            if (workingMode) Work();
            else PutOrTake();
        }

        private void OnThrow()
        {
            if (!canMove) return;
            Throw();
            if (player.PlayerPhysical.IsHold)
            {
                LayDownObject(true);
            }
        }

        private void OnShowOff()
        {
            if (!canMove) return;
            ShowOff();
        }
        #endregion

        #region 애니메이션
        private IEnumerator Dash()
        {
            player.PlayerPhysical.IsDash = true;
            player.Rigidbody.AddForce(transform.forward
                * player.PlayerPhysical.DashForce, ForceMode.Impulse);
            player.SendPlayAnimationEvent(photonView.ViewID, "Dash", "Trigger");
            float time = 0.8333f;
            while (time > 0.0f)
            {
                time -= Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }
            player.Rigidbody.velocity = Vector3.zero;
            player.PlayerPhysical.IsDash = false;

        }
        private void Walk(bool move)
        {
            player.Animator.SetBool("Walk", move);

        }

        private void Holding(bool hold)
        {
            player.Animator.SetBool("Hold", hold);
        }

        private void ChangeState(bool work)
        {
            player.Animator.SetBool("Work", work);
            player.SendPlayAnimationEvent(photonView.ViewID, "ChangeState", "Trigger");
            player.Hammer.SetActive(work);
            StartCoroutine(TurnAround());
        }

        private IEnumerator TurnAround()
        {
            float time = 0.5f;
            while (time > 0.0f)
            {
                time -= Time.deltaTime;
                yield return new WaitForFixedUpdate();
                transform.Rotate(Vector3.up * 700 * Time.deltaTime);
            }

        }

        private void Throw()
        {
            player.SendPlayAnimationEvent(photonView.ViewID, "Throw", "Trigger");
        }


        private void Hammering()
        {
            player.SendPlayAnimationEvent(photonView.ViewID, "Hammering", "Trigger");
        }


        private void Tempering()
        {
            canMove = false;
            player.SendPlayAnimationEvent(photonView.ViewID, "Tempering", "Trigger");
        }


        private void ShowOff()
        {
            canMove = false;
            player.SendPlayAnimationEvent(photonView.ViewID, "ShowOff", "Trigger");
        }
        #endregion

        #region 아이템, 오브젝트와의 상호작용
        /// <summary>
        /// 집게 사용 함수
        /// </summary>
        /// <param name="useTong">사용 여부</param>
        /// <param name="tongs">들어온 게임 오브젝트</param>
        private void UseTongs(bool useTong, GameObject tongs = null)
        {

            player.PlayerPhysical.UseTongs = useTong;
            player.Animator.SetBool("UseTongs", useTong);
            if (useTong)
            {
                tongs.SetActive(!useTong);
                player.Tongs = tongs;
                player.Tongs.transform.SetParent(player.HoldingPos);
                player.Tongs.transform.localPosition = new Vector3(0, 0, 0);
                player.Tongs.transform.rotation = Quaternion.identity;
            }
            else
            {
                player.Tongs.SetActive(!useTong);
                player.Tongs.transform.SetParent(null);
                player.Tongs = null;
            }
        }

        /// <summary>
        /// 물건을 들었을 때의 함수
        /// </summary>
        /// <param name="pickUpObject"></param>
        private void PickUpObject(GameObject pickUpObject)
        {
            if (player.HoldObject == null)
            {
                player.HoldObject = pickUpObject;
                player.HoldObject.transform.SetParent(player.HoldingPos);
                player.HoldObject.transform.localPosition = new Vector3(0, 0, 0);
                player.HoldObject.transform.rotation = Quaternion.identity;
                player.HoldObject.GetComponent<Collider>().isTrigger = true;
                player.HoldObject.GetComponent<Rigidbody>().useGravity = false;
                player.PlayerPhysical.IsHold = true;
                Holding(true);
            }
        }

        /// <summary>
        /// 물건을 내려 놓을 때의 함수
        /// </summary>
        private void LayDownObject(bool isThrow = false)
        {
            if (player.HoldObject != null)
            {
                player.HoldObject.transform.SetParent(null);
                player.HoldObject.GetComponent<Collider>().isTrigger = false;
                player.HoldObject.GetComponent<Rigidbody>().useGravity = true;
                if (isThrow)
                {
                    float throwPower = 5f;
                    if (player.PlayerPhysical.IsDash) throwPower *= 2;
                    player.HoldObject.GetComponent<Rigidbody>().AddForce(transform.forward * throwPower, ForceMode.Impulse);
                }

                player.HoldObject = null;
                player.PlayerPhysical.IsHold = false;
                Holding(false);
            }
        }


        /// <summary>
        /// 상호 작용 오브젝트에 물건을 넣거나 뺄 때 쓰는 함수
        /// </summary>
        private void PutOrTake()
        {
            GameObject ActionObj = player.PlayerPhysical.GetActionObj();

            // 집게를 들고 있을 때
            if (player.PlayerPhysical.UseTongs)
            {
                // 아이템을 들고 있다면
                if (player.PlayerPhysical.IsHold)
                {
                    if (ActionObj == null)
                    {
                        // 아이템 내려놓기
                        LayDownObject();
                    }
                    else if (ActionObj.CompareTag("InteractionObj"))
                    {
                        // 해당 오브젝트와 상호작용 아이템이고 사용 중이 아니라면 거기에 넣기
                    }

                }
                // 아이템을 들고 있지 않다면 
                else if (!player.PlayerPhysical.IsHold)
                {
                    if (ActionObj == null)
                    {
                        // 집게 내려놓기
                        UseTongs(false);
                    }
                    else if (ActionObj.CompareTag("InteractionObj"))
                    {
                        // 해당 오브젝트에 집게로 집을 수 있는 아이템이 있다면
                        // 아이템 들기
                        PickUpObject(ActionObj/*후에 바꿔야 됨*/);
                    }
                    else if (ActionObj.CompareTag("Item"))
                    {
                        // 아이템 들기
                        PickUpObject(ActionObj);
                    }

                }
            }
            // 집게를 들고 있지 않을 때
            else
            {
                // 아이템을 들고 있다면
                if (player.PlayerPhysical.IsHold)
                {
                    if (ActionObj == null)
                    {
                        // 아무것도 없다면 아이템 내려놓기
                        LayDownObject();
                    }
                    else if (ActionObj.CompareTag("InteractionObj"))
                    {
                        // 상호 작용 오브젝트가 비어있어 해당 아이템을 넣을 수 있으면 넣기
                        // 용광로일때, 담금질할때
                        Tempering();
                    }

                }
                // 아이템을 들고 있지 않다면 
                else if (!player.PlayerPhysical.IsHold)
                {
                    if (ActionObj == null)
                    {
                        return;
                    }
                    else if (ActionObj.CompareTag("InteractionObj"))
                    {
                        // 상호 작용 오브젝트에 집게 없이 들 수 있는 아이템이 있다면 먹기
                        Holding(true);
                    }
                    else if (ActionObj.CompareTag("Item"))
                    {
                        // 아이템 들고 있기
                        PickUpObject(ActionObj);
                    }
                    else if (ActionObj.CompareTag("Tongs"))
                    {
                        // 집게 들기
                        UseTongs(true, ActionObj);
                    }
                }
            }
        }

        /// <summary>
        /// 상호 작용 오브젝트와 상호 작용을 할 때 쓰는 함수
        /// </summary>
        private void Work()
        {
            GameObject ActionObj = player.PlayerPhysical.GetActionObj();
            if (ActionObj == null)
            {
                return;
            }
            else if (ActionObj.CompareTag("InteractionObj"))
            {
                // 상호 작용 오브젝트와 상호작용을 할 수 있는 상태하면 상호작용하기

                if (player.PlayerPhysical.IsHold)
                {
                    // 해당 오브젝트에 아이템을 넣을 수 있으면 넣기
                }
                else
                {
                    // 모루나 나무 작업대일때 안에 아이템이 있으면 작업
                    Hammering();
                }

            }
        }
        #endregion
    }
}
