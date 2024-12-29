using UnityEngine;

namespace CHARACTERS
{
//Untuk tipe character live2d
public class Character_Live2D : Character
{
    public Character_Live2D(string name,CharacterConfigData config, GameObject prefab,string rootAssetFolder) : base(name,config,prefab)
    {
        Debug.Log($"Create live2D character : '{name}'");
    }
}
}

