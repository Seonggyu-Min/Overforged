using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCR
{
    [CreateAssetMenu(fileName = "Image Data", menuName = "Scriptable Object/Image List", order = int.MaxValue)]
    public class ImageList : ScriptableObject
    {
        public List<Sprite> sprites;
    }
}
