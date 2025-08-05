using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{

    public static TutorialManager Instance;

    [SerializeField] TMP_Text guideMessage;

    void Awake()
    {
        Instance = this;
    }

}
