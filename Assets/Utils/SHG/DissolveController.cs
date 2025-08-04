using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EditorAttributes;

namespace SHG
{
  [RequireComponent(typeof(Renderer))]
  public class DissolveController : MonoBehaviour
  {
    [SerializeField]
    Renderer dissolveRenderer;
    Coroutine dissolveRoutine;
    [SerializeField]
    int dissolveMaterialIndex = 0;
    [SerializeField]
    public float dissolveSpeed;
    const string DISSOLVE_VALUE = "_DissolveValue";
    MaterialPropertyBlock materialPropertyBlock;


    public IEnumerator StartAppear()
    {
      return (this.StartDissovle(-this.dissolveSpeed * Time.deltaTime));
    }

    public IEnumerator StartDisappear()
    {
      return (this.StartDissovle(this.dissolveSpeed * Time.deltaTime));
    }

    [Button ("Appear")]
    public void Appear()
    {
      this.dissolveRoutine = this.StartCoroutine(this.StartAppear());
    }

    [Button ("Disapper")]
    public void Disappear()
    {
      this.dissolveRoutine = this.StartCoroutine(this.StartDisappear());
    }

    IEnumerator StartDissovle(float delta) 
    {
      float destValue = delta < 0 ? 0f: 1f;
      float currentValue = this.materialPropertyBlock.GetFloat(DISSOLVE_VALUE);
      if (delta > 0) {
        while (currentValue < destValue) {
          currentValue += delta;
          this.materialPropertyBlock.SetFloat(DISSOLVE_VALUE, currentValue);
          this.dissolveRenderer.SetPropertyBlock(this.materialPropertyBlock, this.dissolveMaterialIndex);
          yield return (null);
        }
      }
      else {
        while (currentValue > destValue) {
          currentValue += delta;
          this.materialPropertyBlock.SetFloat(DISSOLVE_VALUE, currentValue);
          this.dissolveRenderer.SetPropertyBlock(this.materialPropertyBlock, this.dissolveMaterialIndex);
          yield return (null);
        }
      }
      this.dissolveRoutine = null;
    }

    public void AppearImmediately()
    {
      this.ResetTo(0f);
    }

    public void DisappearImmediately()
    {
      this.ResetTo(1f);
    }

    [Button ("Reset to")]
    void ResetTo(float dissolveValue)
    {
      #if UNITY_EDITOR
      if (dissolveValue < 0 || dissolveValue > 1) {
        throw (new ArgumentException($"ResetTo: {dissolveValue}"));
      }
      #endif
      if (this.dissolveRoutine != null) {
        this.StopCoroutine(this.dissolveRoutine);
        this.dissolveRoutine = null;
      }
      this.materialPropertyBlock.SetFloat(
        DISSOLVE_VALUE, dissolveValue);
      this.dissolveRenderer.SetPropertyBlock(this.materialPropertyBlock, this.dissolveMaterialIndex);
    }
    
    void Awake()
    {
      if (this.dissolveRenderer == null) {
        this.dissolveRenderer = this.GetComponent<Renderer>();
      }
      if (this.dissolveRenderer == null) {
        Debug.LogError($"{nameof(DissolveController)} Awake: No renderer");
      }
      else {
        this.materialPropertyBlock = new MaterialPropertyBlock();
        this.dissolveRenderer.SetPropertyBlock(this.materialPropertyBlock, this.dissolveMaterialIndex);
      }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
  }
}

