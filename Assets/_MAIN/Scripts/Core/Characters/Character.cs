using System.Collections.Generic;
using System.Collections;
using DIALOGUE;
using TMPro;
using UnityEngine;


namespace CHARACTERS
{

//skrip ini menjadi induk semua character
public abstract class Character 
{
    //inisialisasi variable
    public const bool ENABLE_ON_START = true;
    private const float UNIGHLIGHTED_DARKEN_STRENGHT = 0.65f;
    public string name = "";
    public string displayname = "";
    public RectTransform root = null;
    //pembuatan variable untuk config character
    public CharacterConfigData config;
    public Animator animator;
    public Color color {get; protected set;} = Color.white;
    protected Color displayColor => highlighted ? highlightedColor : unhighlightedColor;
    protected Color highlightedColor => color;
    protected Color unhighlightedColor => new Color(color.r* UNIGHLIGHTED_DARKEN_STRENGHT, color.g* UNIGHLIGHTED_DARKEN_STRENGHT, color.b * UNIGHLIGHTED_DARKEN_STRENGHT, color.a);
    public bool highlighted {get; protected set;} = true;

    //pembuatan object untuk character manajer
    protected CharacterManager manager => CharacterManager.instance;
    //pembuatan object untuk dialogue system
    public DialogueSystem dialogueSystem => DialogueSystem.instance;

    //coroutine here
    protected Coroutine co_changingColor;
    protected Coroutine co_revealing, co_hiding;
    protected Coroutine co_moving;
    protected Coroutine co_highLighting;

    //boolean for coroutine
    public bool isRevealing => co_revealing != null;
    public bool isChangingColor => co_changingColor != null;
    public bool isMoving => co_moving != null;
    public bool isHiding => co_hiding != null;
    public bool isHighlighting => (highlighted && co_highLighting != null);
    public bool isUnHighLighting => (!highlighted && co_highLighting != null);
    public virtual bool isVisible {get; set;}

    //konstruktor dari character dimana membutuhkan nama,config, dan prefab
    public Character(string name, CharacterConfigData config, GameObject prefab)
    {
        //konfigurasi properti karakter 
        this.name = name;
        displayname = name;
        this.config = config;

        //jika prefabnya tidak sama dengan null
        if(prefab != null)
        {
            //spawn object prefab dalam root characterpanel atau "Characters"
            GameObject ob = Object.Instantiate(prefab, manager.characterpanel);
            //Object yang baru dibuat diberi nama berformat agar konsisten
            ob.name = manager.FormatCharacrterPath(manager.characterPrefabName, name);
            //mengaktifkan object
            ob.SetActive(true);
            //mendapatkan rect transform dari ob yang dimasukan kedalam root
            root = ob.GetComponent<RectTransform>();
            //mendapatkan animator dari root rect transform
            animator = root.GetComponentInChildren<Animator>();
        }
    }

    //coroutine untuk memulai pembicaraan dengan memasukan dialogue dalam bentuk list ke fungsi say
    public Coroutine Say(string dialogue) => Say(new List<string>{dialogue});
    public Coroutine Say(List<string> dialogue)
    {
        //memanggil fungsi dalam dialogue manager untuk menampilkan displayname
        dialogueSystem.ShowSpeakerName(displayname);
        //memanggil fungsi 
        UpdateTextCustomiazationOnScreen();
        //mengembalikan nilai system dialogue say dalam bentuk coroutine
        return dialogueSystem.Say(dialogue);
    }

    //mengubah font nama dari character
    public void SetNameFont(TMP_FontAsset font) => config.nameFont = font;
    //mengubah dialogue font dari character
    public void SetDialogueFont(TMP_FontAsset font) => config.dialogueFont = font;
    //mengubah warna dari nama character
    public void SetNameColor(Color color) => config.nameColor = color;
    //mengubah warna dari dialogue character
    public void SetDialogueColor(Color color) => config.dialogueColor = color;

    //merubah konfigurasi character ke default dengan mendapatkan nilai config yang baru dari SO
    public void ResetConfigurationData() => config = CharacterManager.instance.GetCharacterConfig(name);

    //merubah update dari config text agar ditampilkan dalam sistem
    public void UpdateTextCustomiazationOnScreen() => dialogueSystem.ApplySpeakerDataToDialogueContainer(config);

    //fungsi untuk menampilkan karakter
    public virtual Coroutine Show()
    {
        if(isRevealing)
        {
            return co_revealing;
        }

        if(isHiding)
        {
            manager.StopCoroutine(co_hiding);
        }

        //memanggil fungsi untuk membuat character muncul
        co_revealing = manager.StartCoroutine(ShowingOrHiding(true));
        return co_revealing;
    }

    public virtual Coroutine Hide()
    {
        if(isHiding)
        {
            return co_hiding;
        }

        if(isRevealing)
        {
            manager.StopCoroutine(co_revealing);
        }

         //memanggil fungsi  untuk membuat character sembunyi
        co_hiding = manager.StartCoroutine(ShowingOrHiding(false));
        return co_hiding;
    }

    //fungsi show atau hiding tidak dibuat di induk class tapi di anaknya classnya nanti
    public virtual IEnumerator ShowingOrHiding(bool Show)
    {
        Debug.Log("Show/hide cannot be called form here");
        yield return null;
    }

    //fungsi untuk mengatur posisi dari character
    public virtual void SetPosition(Vector2 position)
    {
        if(root == null)
        {
            return;
        }
        //mengubah posisi vector2 menjadi min and max anchor target
        (Vector2 minAnchorTarget, Vector2 maxAnchorTarget) = ConverUITargetPositionToRelativeCharaterAnchorTargets(position);
        //menerapkan target potition berdasarkan anchor targetnya pada root
        root.anchorMin = minAnchorTarget;
        root.anchorMax = maxAnchorTarget;
    }

    //fungsi untuk memindahkan posisi dari character
    public virtual Coroutine MoveToPosition(Vector2 position, float speed = 2f, bool smooth = false)
    {
        if(root == null)
        {
            return null;
        }

        if(isMoving)
        {
            manager.StopCoroutine(co_moving);
        }
        //memanggil fungsi moving to potition dengan memasukan parameter posisi,kecepatan,dan kesemutan
        co_moving = manager.StartCoroutine(MovingToPosition(position,speed,smooth));
        return co_moving;
    }

    //Ienumerator untuk perpindahan sprite
    private IEnumerator MovingToPosition(Vector2 position, float speed, bool smooth)
    {
        //konversi posisi tujuan vector 2 menjadi min and max anchor target
        (Vector2 minAnchorTarget, Vector2 maxAnchorTarget) = ConverUITargetPositionToRelativeCharaterAnchorTargets(position);
        //mendapatkan nilai padding dengan mengurangi antara max dengan min
        Vector2 padding = root.anchorMax - root.anchorMin;

        //ketika posisi sekarang tdak sama dengan posisi yang baru maka
        while(root.anchorMin != minAnchorTarget || root.anchorMax != maxAnchorTarget)
        {
            //nilai dari root anchor min akan bergerak dengan gradiasi karena menggunakan vector lerp ataupun movetowards, kedua tipe ini berbeda dalam kesemoothannya
            root.anchorMin = smooth ? 
                Vector2.Lerp(root.anchorMin, minAnchorTarget, speed * Time.deltaTime)
                :Vector2.MoveTowards(root.anchorMin, minAnchorTarget, speed * Time.deltaTime * 0.35f);

            //root anchor max didapat dengan menambahkan root anchor min ditambah padding, meski begitu akan tetap smooth karena anchor min nilainya naik bergradiasi
            root.anchorMax = root.anchorMin + padding;

            //jika anchor min dari root dan tujuan anchormin nya hampir sama maka while dihentikan
            if(smooth && Vector2.Distance(root.anchorMin, minAnchorTarget)<= 0.001f)
            {
                //mengupdate nilai dari anchor min dan max
                root.anchorMin = minAnchorTarget;
                root.anchorMax = maxAnchorTarget;
                break;
            }
            yield return null;
        }
        Debug.Log("Done Moving");
        co_moving = null;
    }


    //konversi nilai tujuan vector2 untuk posisi menjadi dua nilai vector2 anchor
    protected (Vector2, Vector2) ConverUITargetPositionToRelativeCharaterAnchorTargets(Vector2 position)
    {
        //mendapatkan nilai max x dan max y berdasarkan persamaan dibawah
        Vector2 padding = root.anchorMax - root.anchorMin;
        float maxX = 1f - padding.x;
        float maxY = 1f - padding.y;

        //membuat nilai min anchor target sesuai dengan persamaan dibawah
        Vector2 minAnchorTarget = new Vector2(maxX * position.x, maxY * position.y);
        //membuat nilai max anchor target sesuai dengan persamaan dibawah
        Vector2 maxAnchorTarget = minAnchorTarget + padding;
        //mengembalikan nilai minanchor dan maxanchor dalam vector2
        return(minAnchorTarget, maxAnchorTarget);
    }

    //fungsi untuk mengupdate color dari character
    public virtual void SetColor(Color color)
    {
        this.color = color;
    }

    //Fungsu untuk merubah warna dari color yang telah di set sebelumnya
    public Coroutine TransitionColor(Color color, float speed = 1f)
    {
        this.color = color;
        if(isChangingColor)
        {
            manager.StopCoroutine(co_changingColor);
        }
        //menjalankan IEnumerator changing color dengan warna dan kecepatan yang sudah di set
        co_changingColor = manager.StartCoroutine(ChangingColor(displayColor,speed));
        return co_changingColor;
    }

    //fungsi ini dilakukan dalam anak kelas
    public virtual IEnumerator ChangingColor(Color color, float speed)
    {
        Debug.Log("Color is not applicable");
        yield return null;

    }

    //fungsi untuk menghightlight character untuk mengetahui siapa yang berbicara
    public Coroutine HighLight(float speed = 1f)
    {
        if (isHighlighting)
        {
            return co_highLighting;
        }

        if(isUnHighLighting)
        {
            manager.StopCoroutine(co_highLighting);
        }
        //merubah status dan menjalankan Ienumerator higlighting
        highlighted = true;
        co_highLighting = manager.StartCoroutine(HighLighting(highlighted,speed));
        return co_highLighting;
    }

    //fungsi untuk mengUnhightlight character untuk mengetahui siapa yang berbicara
    public Coroutine UnHighLight(float speed = 1f)
    {
        if (isUnHighLighting)
        {
            return co_highLighting;
        }

        if(isHighlighting)
        {
            manager.StopCoroutine(co_highLighting);
        }

        //merubah status dan menjalankan Ienumerator higlighting
        highlighted = false;
        co_highLighting = manager.StartCoroutine(HighLighting(highlighted,speed));
        return co_highLighting;   
    }

    //fungsi ini hanya dibuat di child bukan diinduk
    public virtual IEnumerator HighLighting(bool HighLight, float speedMultiplier)
    {
        Debug.Log("highlighting is not avaible on this character type!");
        yield return null;
    }

    //character type dari character
    public enum CharacterType
    {
        Text,
        Sprite,
        SpriteSheet,
        Live2D,
        Model3D
    }
}

}