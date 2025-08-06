using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCR
{
    public class MapManager : MonoBehaviour
    {
        public List<MapData> MapDatas { get => mapDatas; }

        [SerializeField] List<MapData> mapDatas;

        public MapData getMap(int index)
        {
            return mapDatas[index];
        }
    }
}

