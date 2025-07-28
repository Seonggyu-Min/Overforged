using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MIN
{
    public enum CharacterId
    {
        Bunny,
        Cat,
        Dog,
        Duck
    }

    public enum TeamColorId
    {
        Red,
        Orange,
        Yellow,
        Green,
        Blue,
        DarkBlue,
        Purple,
        Pink
    }

    public static class CustomPropertyDatabase
    {
        private static Dictionary<CharacterId, Sprite> _characterSprites = new();

        private static Dictionary<TeamColorId, Color> _teamColors = new()
        {
            { TeamColorId.Red, Color.red },
            { TeamColorId.Orange, new Color (1f, 0.5f, 0f, 1f) },
            { TeamColorId.Yellow, Color.yellow },
            { TeamColorId.Green, Color.green },
            { TeamColorId.Blue, Color.blue },
            { TeamColorId.DarkBlue, new Color(0f, 0f, 0.5f, 1f) },
            { TeamColorId.Purple, new Color(0.5f, 0f, 0.5f, 1f) },
            { TeamColorId.Pink, new Color(1f, 0.75f, 0.8f, 1f) }
        };

        public static void RegisterCharacterSprite(CharacterId id, Sprite sprite)
        {
            _characterSprites[id] = sprite;
        }

        public static Color GetColorById(TeamColorId id)
        {
            return _teamColors.TryGetValue(id, out Color color) ? color : Color.white;
        }
        public static Sprite GetSpriteById(CharacterId id)
        {
            return _characterSprites.TryGetValue(id, out Sprite sprite) ? sprite : null;
        }
    }
}
