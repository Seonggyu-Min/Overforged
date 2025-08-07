using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EditorAttributes;
using UnityEngine.UI;
using TMPro;
using System;

namespace SHG
{
  public class BotSceneManager : MonoBehaviour
  {
    const int PLAY_TIME_IN_SECONDS = 120;
    [SerializeField] [Required]
    TMP_Text timerLabel;
    [SerializeField] [Required]
    BotSceneRecipeUI[] recipeUIs;
    public static BotSceneManager Instance => instance;
    static BotSceneManager instance;
    public LocalPlayerController player;
    public EnemyBotController bot;
    int remaingTimeInSeconds;
    Coroutine timerRoutine;
    WaitForSeconds oneSecondWait = new WaitForSeconds(1);

    [Button]
    public void StartPlay()
    {
      this.remaingTimeInSeconds = PLAY_TIME_IN_SECONDS;
      this.player.gameObject.SetActive(true);
      this.bot.gameObject.SetActive(true);
      this.bot.IsOwner = true;
      this.bot.StartCreateProduct();
      this.timerRoutine = this.StartCoroutine(this.TimerRoutine());
      for (int i = 0; i < Math.Min(3, this.recipeUIs.Length); ++i) {
        var craftData = BotContext.Instance.GetCraftDataAt(i);
        var recipe = BotContext.Instance.Recipes[i];
        if (craftData != null)
        {
          this.recipeUIs[i].gameObject.SetActive(true);
          this.recipeUIs[i].SetUp(
            craftData, recipe.WoodType, recipe.OreType
          );
        }
        else
        {
          Debug.LogWarning($"no craftData");
        }
      }
    }

    IEnumerator TimerRoutine()
    {
      while (this.remaingTimeInSeconds > 0) {
        this.remaingTimeInSeconds -= 1;
        int min = this.remaingTimeInSeconds / 60;
        int second = this.remaingTimeInSeconds % 60;
        string secondString = second > 10 ? second.ToString() : $"0{second}";
        if (min > 0)
        {
          this.timerLabel.text = $"{min}:{secondString}";
        }
        else
        {
          this.timerLabel.text = $"{secondString}";
        }
        yield return (this.oneSecondWait);
      }
      this.OnTimeOut();
    }

    void Awake()
    {
      if (instance == null)
      {
        instance = this;
      }
      else
      {
        Destroy(this.gameObject);
      }
    }

    void OnTimeOut()
    {

    }
  }
}
