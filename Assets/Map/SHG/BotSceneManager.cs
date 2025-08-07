using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EditorAttributes;

namespace SHG
{
  public class BotSceneManager : MonoBehaviour
  {
    public static BotSceneManager Instance => instance;
    static BotSceneManager instance;
    public LocalPlayerController player;
    public EnemyBotController bot;

    [Button]
    void StartPlay()
    {
      this.player.gameObject.SetActive(true);
      this.bot.gameObject.SetActive(true);
    }
    
    void Awake()
    {
      if (instance == null) {
        instance = this;
      }
      else {
        Destroy(this.gameObject);
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
