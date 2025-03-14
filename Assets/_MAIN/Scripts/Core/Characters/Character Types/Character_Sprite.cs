using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CHARACTERS
{ 
//Untuk tipe character spritesheet
public class Character_Sprite : Character
{

    public override bool isVisible
    {   
        get {return isRevealing || RootCG.alpha == 1;}
        set { RootCG.alpha = value ? 1 : 0;}
    }
    private const string SPRITE_RENDERER_PARENT_NAME = "Renderers";
    private const string SPRITESHEET_DEFAULT_SHEETNAME = "Default";
    private const char SPRITESHEET_TEX_SPRITE_DELIMITER = '-';
    //Dia bukan monobheaviour dan tidak di attach gimana caranya bisa get componenet?

    private CanvasGroup RootCG => root.GetComponent<CanvasGroup>();

    //membuat sistem layers karakter
    public List<CharacterSpriteLayer> layers = new List<CharacterSpriteLayer>();
    private string artAssetDirectory = "";
    
    //konstruktor dari character sprite yang berbasis dari induknya
     public Character_Sprite(string name,CharacterConfigData config, GameObject prefab, string rootAssetFolder) : base(name, config, prefab)
    {
        //mengetahui status auto ditampilkana tau tidak
        RootCG.alpha = ENABLE_ON_START ? 1:0;
        //root asset folder dari sprite
        artAssetDirectory = rootAssetFolder + "/Images";
        //memanggil fungsi get layers
        getLayers();
        Debug.Log($"Create sprite character : '{name}'");
    }

    //mengecheck root yang dibuat
    public void CekRoot()
    {
        Debug.Log(artAssetDirectory);
    }

    //mendapatkan layers untuk sprtie
    private void getLayers()
    {

        //mencari transform dari induk sprite renderernya
        Transform rendererRoot = animator.transform.Find(SPRITE_RENDERER_PARENT_NAME);
        if(rendererRoot == null)
        {
            return;
        }

        for (int i = 0; i< rendererRoot.transform.childCount; i++)
        {
            Transform child = rendererRoot.transform.GetChild(i);
            Image rendererImage = child.GetComponentInChildren<Image>();
            if(rendererImage != null)
            {
                CharacterSpriteLayer layer = new CharacterSpriteLayer(rendererImage, i);
                layers.Add(layer);
                child.name = $"Layer : {i}";
            }
        }
    }

    //fungsi untuk menerapkan sprite baru ke character
    public void SetSprite(Sprite sprite, int layer = 0)
    {
        layers[layer].SetSprite(sprite);
    }

    //fungsi untuk mendapatkan sprite baru dari folder
    public Sprite GetSprite(string spriteName)
    {
        if(config.characterType == CharacterType.SpriteSheet)
        {
            string[] data = spriteName.Split(SPRITESHEET_TEX_SPRITE_DELIMITER);
            Sprite[] spriteArray = new Sprite[0];
            if(data.Length == 2)
            {
                string texturename = data[0];
                spriteName = data[1];
                spriteArray = Resources.LoadAll<Sprite>($"{artAssetDirectory}/{texturename}");

            }
            else{
                spriteArray = Resources.LoadAll<Sprite>($"{artAssetDirectory}/{SPRITESHEET_DEFAULT_SHEETNAME}");
    
            }
                if(spriteArray.Length == 0)
                {
                    Debug.LogWarning($"Character '{name}' does not have a default art asset called '{SPRITESHEET_DEFAULT_SHEETNAME}'");
                }
                return Array.Find(spriteArray, sprite => sprite.name == spriteName);

            
        }
        else
        {
            return Resources.Load<Sprite>($"{artAssetDirectory}/{spriteName}");
        }
    }   
    
    //fungsi untuk mengganti sprite dengan transisisi
    public Coroutine TransitionSprite (Sprite sprite, int layer = 0, float speed = 1)
    {
        CharacterSpriteLayer spriteLayer = layers[layer];

        return spriteLayer.TransitionSprite(sprite,speed);
    }

    //fungsi untuk menampilkan dan menghilankan sprite
        public override IEnumerator ShowingOrHiding(bool Show)
        {
            float targetAlpha = Show ? 1f : 0;
            CanvasGroup self = RootCG;

            while(self.alpha != targetAlpha)
            {
                self.alpha = Mathf.MoveTowards (self.alpha, targetAlpha, 3f*Time.deltaTime);
                yield return null;
            }

            co_revealing = null;
            co_hiding = null;
        }

        //fungsi untuk mengganti warna sprite
        public override void SetColor(Color color)
        {
        
            base.SetColor(color);
            color = displayColor;
            foreach(CharacterSpriteLayer layer in layers)
            {
                layer.StopChangingColor();
                layer.SetColor(color);
            }
        }

        //Coroutine untuk mengganti warna sprite
        public override IEnumerator ChangingColor(Color color, float speed)
        {
            foreach(CharacterSpriteLayer layer in layers)
            {
                layer.TransitionColor(color,speed);

            }
            yield return null;

            while(layers.Any(l => l.isChangingColor))
            {
                yield return null;
            }
            co_changingColor = null;
        }

        //fungsu untuk higlighting sprite
        public override IEnumerator HighLighting(bool HighLight, float speedMultiplier)
        {
            Color targetColor = displayColor;
            foreach(CharacterSpriteLayer layer in layers)
            {
                layer.TransitionColor(targetColor, speedMultiplier);
            }

            yield return null;

             while(layers.Any(l => l.isChangingColor))
            {
                yield return null;
            }
            co_highLighting = null;

        }

        public override IEnumerator FaceDirection(bool FaceLeft, float speedMultiplier, bool immediate)
        {
            foreach(CharacterSpriteLayer layer in layers)
            {
                if(FaceLeft)
                    layer.FaceLeft(speedMultiplier, immediate);
                else
                    layer.FaceRight(speedMultiplier, immediate);  
            }
            yield return null;

            while(layers.Any(l => l.isFacingLeft))
                yield return null;

            co_flipping = null;  
        }

        public override void OnReceiveCastingExpression(int layer, string expression)
        {
            Debug.Log($"Layer: {layer}, Expression: {expression}");
            Sprite sprite = GetSprite(expression);
            if(sprite == null)
            {
                Debug.LogWarning($"Sprite '{expression} could not be found for character '{name}'");
                return;
            }
            TransitionSprite(sprite,layer);
        }
    }

}
