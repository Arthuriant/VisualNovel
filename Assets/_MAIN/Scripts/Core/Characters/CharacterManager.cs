using System.Collections.Generic;
using DIALOGUE;
using Mono.Cecil;
using UnityEngine;

namespace CHARACTERS
{
public class CharacterManager : MonoBehaviour
{
    public static CharacterManager instance {get; private set;}
    private Dictionary<string, Character> characters = new Dictionary<string, Character>();
    private CharacterConfigSO config => DialogueSystem.instance.config.characterConfigurationAsset;
    private const string CHARACTER_CASTING_ID = " as ";
    private const string CHARACTERNAME_ID = "<charname>";
    public string characterRootPath => $"Characters/{CHARACTERNAME_ID}";
    public string characterPrefabName => $"Character - [{CHARACTERNAME_ID}]";
    public string characterPrefabPath => $"{characterRootPath}/Prefab/{characterPrefabName}";
    [SerializeField] private RectTransform _characterpanel = null;
    public RectTransform characterpanel => _characterpanel;
    public void  Awake()
    {
        instance = this;
    }

    public CharacterConfigData GetCharacterConfig(string characterName)
    {
        return config.GetConfig(characterName);
    }
    public Character GetCharacter(string characterName, bool createIfDoesNotExist = false)
    {
        if(characters.ContainsKey(characterName.ToLower()))
        {
            return characters[characterName.ToLower()];
        }else if (createIfDoesNotExist)
        {
            return CreateCharacter(characterName);
        }
        return null;
    }
    public Character CreateCharacter(string characterName)
    {
        if (characters.ContainsKey(characterName.ToLower()))
        {
            Debug.LogWarning($"A Character called '{characterName}' alredy exist");
            return null;
        }

        CHARACTER_INFO info = GetCharacterInfo(characterName);
        Character character = CreateCharacterFromInfo(info);

        characters.Add(characterName.ToLower(), character);
        
        return character;
    }

    private CHARACTER_INFO GetCharacterInfo(string characterName)
    {
        CHARACTER_INFO result = new CHARACTER_INFO();
        string [] nameData = characterName.Split(CHARACTER_CASTING_ID, System.StringSplitOptions.RemoveEmptyEntries);
        result.name = nameData[0];
        result.castingName = nameData.Length > 1 ? nameData[1] : result.name;
        result.config = config.GetConfig(result.castingName);
        result.prefab = GetPrefabForCharacter(result.castingName);
        result.rootCharacterFolder = FormatCharacrterPath(characterRootPath, result.castingName);
        return result;
    }

    private GameObject GetPrefabForCharacter(string characterName)
    {
        string prefabPath = FormatCharacrterPath(characterPrefabPath, characterName);
        return Resources.Load<GameObject>(prefabPath);
    }

    public string FormatCharacrterPath(string path, string characterName)=> path.Replace(CHARACTERNAME_ID, characterName);

    private Character CreateCharacterFromInfo(CHARACTER_INFO info)
    {
        CharacterConfigData config = info.config;
        if(config.characterType == Character.CharacterType.Text)
        {
            return new Character_Text(info.name, config);
        }
        if(config.characterType == Character.CharacterType.Sprite || config.characterType == Character.CharacterType.SpriteSheet)
        {
            return new Character_Sprite(info.name, config, info.prefab, info.rootCharacterFolder);
        }
        if(config.characterType == Character.CharacterType.Live2D)
        {
            return new Character_Live2D(info.name, config, info.prefab, info.rootCharacterFolder);
        }
        if(config.characterType == Character.CharacterType.Model3D)
        {
            return new Character_Model3D(info.name, config,info.prefab, info.rootCharacterFolder);
        }

        return null;
    }

    private class CHARACTER_INFO
    {
        public string name = "";
        public string castingName = "";

        public string rootCharacterFolder = "";

        public CharacterConfigData config = null;

        public GameObject prefab = null;
    }
}
}
