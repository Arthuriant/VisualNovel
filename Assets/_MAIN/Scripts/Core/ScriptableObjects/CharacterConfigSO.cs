using UnityEngine;

namespace CHARACTERS
{
[CreateAssetMenu(fileName = "Character Configuration Asset",menuName = "Dialogue System/Character Configuration Asset")]
public class CharacterConfigSO : ScriptableObject
{
    //meminta data untuk setiap character sesuai dengan character config data yang dimasukan di SO
    public CharacterConfigData[] characters;


    //meminta data untuk setiap character sesuai dengan skrip sekarang
    public CharacterConfigData GetConfig(string characterName)
    {
        characterName = characterName.ToLower();

        for(int i=0;i<characters.Length;i++)
        {
            //memecah characters menjadi character
            CharacterConfigData data = characters[i];
            //jika characternya memiliki nama yang sama antara nama dan alias maka
            if(string.Equals(characterName, data.name.ToLower()) || string.Equals(characterName, data.alias.ToLower()))
            {
                //mengembalikan nilai data config  dari karakternya namun copy artinya tidak menimpa konfigurasi data sebelumnya melainkan baru

                return data.Copy();
            }
        }
        //jika tidak ada nama yang cocok maka dijadikan default
        return CharacterConfigData.Default;
    }
}

}
