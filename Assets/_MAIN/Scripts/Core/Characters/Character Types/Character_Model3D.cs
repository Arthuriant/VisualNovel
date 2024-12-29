using UnityEngine;

namespace CHARACTERS
{
//Untuk tipe character model 3d
public class Character_Model3D : Character
{
    public Character_Model3D(string name,CharacterConfigData config, GameObject prefab,string rootAssetFolder) : base(name,config,prefab)
    {
        Debug.Log($"Create model3D character : '{name}'");
    }
}

}
