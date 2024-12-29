using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace DIALOGUE
{
    [System.Serializable]
    public class NameContainer  
{
    [SerializeField] private GameObject root;
    [SerializeField] private TextMeshProUGUI nameText;

    //fungsi untuk menampilkan nama
    public void show(string nameToShow = "")
    {
        //jika tidak ada nama maka secara default akan diisi oleh nametext
        root.SetActive(true);
        if(nameToShow != string.Empty)
        {
            nameText.text = nameToShow;
        }
    }

    //fungsi untuk tidak menampilkan nama
    public void Hide()
    {
        root.SetActive(false);
    }

    //fungsi untuk mengganti warna dan font dari nama
    public void SetNameColor (Color color) => nameText.color = color;
    public void SetNameFont(TMP_FontAsset font) => nameText.font = font;
}
}

