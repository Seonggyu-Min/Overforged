using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EditorAttributes;
using Zenject;

namespace SHG
{
  public class BotPresenter : MonoBehaviour
  {
    [Inject]
    IAudioLibrary audioLibrary;
    public bool IsOwner;
    IBot bot;
    [SerializeField] [Required]
    Animator animator;
    [SerializeField] [Required]
    Transform hammer;
    [SerializeField] [Required]
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
      if (this.IsOwner) {
        this.bot = this.GetComponent<IBot>();
        this.bot.OnWork += this.OnWork;
        this.bot.OnRoot += this.OnRoot;
        this.bot.OnFinishWork += this.OnFinishWork;
      }
    }

    void Update()
    {
      if (this.IsOwner)
      {
        this.animator.SetBool("IsMoving", this.bot.NavMeshAgent.velocity.magnitude > 0.1f);
      }
    } 

    void OnWork(IInteractableTool tool)
    {
      if (tool is AnvilComponent smithingTool) {
        this.animator.SetTrigger("Hammering");
        this.hammer.gameObject.SetActive(true);
        this.sfxToPlay = "hammering";
        this.Invoke(nameof(PlaySfx), 0.5f);
      }
      else {
        this.animator.SetTrigger("Working");
      }
      if (tool is TableComponent table) {
        this.saw.gameObject.SetActive(true);
        this.sfxToPlay = "sawing";
        this.Invoke(nameof(PlaySfx), 0.5f);
      }
      if (tool is Component component) {
        this.transform.LookAt(component.transform.position); 
      }
      if (this.sfxToPlay == null) {
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
      if (this.sfxToPlay != null) {
        this.audioLibrary.PlayRandomSfx(this.sfxToPlay)
          .SetDistance(max: 5f)
          .Set3dBlend(0.5f);
        this.sfxToPlay = null;
      }
    }

    void OnFinishWork()
    {
      if (this.hammer.gameObject.activeSelf) {
        this.hammer.gameObject.SetActive(false);
      }
      if (this.saw.gameObject.activeSelf) {
        this.saw.gameObject.SetActive(false);
      }
    }
  }
}
