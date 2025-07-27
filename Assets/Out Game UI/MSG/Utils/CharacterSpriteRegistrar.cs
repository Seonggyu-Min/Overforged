using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MIN
{
    public class CharacterSpriteRegistrar : MonoBehaviour
    {
        [SerializeField] private Sprite[] characterSprites;

        private void Start()
        {
            CustomPropertyDatabase.RegisterCharacterSprite(CharacterId.Bunny, characterSprites[0]);
            CustomPropertyDatabase.RegisterCharacterSprite(CharacterId.Cat, characterSprites[1]);
            CustomPropertyDatabase.RegisterCharacterSprite(CharacterId.Dog, characterSprites[2]);
            CustomPropertyDatabase.RegisterCharacterSprite(CharacterId.Duck, characterSprites[3]);
        }
    }
}
