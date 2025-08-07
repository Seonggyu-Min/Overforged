using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EditorAttributes;
using Zenject;
using Unity.AppUI.UI;

namespace SHG
{
  public class BotPresenter : MonoBehaviour
  {
    IAudioLibrary audioLibrary;
    IBot bot;
    [SerializeField]
    [Required]
    Animator animator;
    [SerializeField]
    [Required]
    Transform hammer;
    [SerializeField]
    [Required]
    Transform saw;
    bool isPlayingWalkSound;
    string sfxToPlay;

    void Awake()
    {
      this.hammer.gameObject.SetActive(false);
      this.saw.gameObject.SetActive(false);
    }

    void Start()
    {
      this.audioLibrary = SingletonAudio.Instance;
      this.bot = this.GetComponent<IBot>();
      this.bot.OnWork += this.OnWork;
      this.bot.OnRoot += this.OnRoot;
      this.bot.OnFinishWork += this.OnFinishWork;
      this.bot.OnAttack += this.OnAttack;
      this.bot.OnHit += this.OnHit;
      this.bot.OnDied += this.OnDied;
    }

    void Update()
    {
      this.animator.SetBool("IsMoving", this.bot.NavMeshAgent.velocity.magnitude > 0.1f);
    }

    void OnWork(IInteractableTool tool)
    {
      if (tool is AnvilComponent smithingTool)
      {
        this.animator.SetTrigger("Hammering");
        this.hammer.gameObject.SetActive(true);
        this.sfxToPlay = "hammering";
        this.Invoke(nameof(PlaySfx), 0.5f);
      }
      else
      {
        this.animator.SetTrigger("Working");
      }
      if (tool is TableComponent table)
      {
        this.saw.gameObject.SetActive(true);
        this.sfxToPlay = "sawing";
        this.Invoke(nameof(PlaySfx), 0.5f);
      }
      if (tool is Component component)
      {
        this.transform.LookAt(component.transform.position);
      }
      if (this.sfxToPlay == null)
      {
        this.sfxToPlay = "interacting";
        this.PlaySfx();
      }
    }

    void OnRoot(Item item)
    {
      this.transform.LookAt(item.transform.position);
      this.animator.SetTrigger("Rooting");
      this.sfxToPlay = "grab";
      this.PlaySfx();
    }

    void PlaySfx()
    {
      if (this.sfxToPlay != null)
      {
        this.audioLibrary.PlayRandomSfx(this.sfxToPlay,
        position: this.transform.position +
        (Camera.main.transform.position - CameraController.Instance.CameraLookPos))
          .SetDistance(max: 5f)
          .Set3dBlend(0.8f);
        this.sfxToPlay = null;
      }
    }

    void OnAttack()
    {
      this.animator.SetTrigger("Attack");
      this.sfxToPlay = "attack";
      this.PlaySfx();
    }

    void OnFinishWork()
    {
      if (this.hammer.gameObject.activeSelf)
      {
        this.hammer.gameObject.SetActive(false);
      }
      if (this.saw.gameObject.activeSelf)
      {
        this.saw.gameObject.SetActive(false);
      }
    }

    void OnHit()
    {
      this.animator.SetTrigger("Hit");
      this.sfxToPlay = "getHit";
      this.PlaySfx();
    }

    void OnDied()
    {
      this.animator.SetBool("Dead", true);
      this.sfxToPlay = "botDead";
      this.PlaySfx();
    }
  }
}
