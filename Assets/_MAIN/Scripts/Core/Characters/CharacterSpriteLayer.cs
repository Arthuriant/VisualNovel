using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CHARACTERS
{
public class CharacterSpriteLayer 
{
    //inisialisasi chractermanager
    private CharacterManager characterManager => CharacterManager.instance;
    //inisialisasi aja
    public int layer {get; private set;} = 0;
    public Image renderer {get; private set;} = null;
    //mengambil component canvas group dalam renderer
    public CanvasGroup rendererCG => renderer.GetComponent<CanvasGroup>();
    //membuat list untuk menyimpan renderer bekas
    private List<CanvasGroup> oldRenderers = new List<CanvasGroup>();

    //Coroutine
    private const float DEFAULT_TRANSITION_SPEED = 3f;
    private float transitionSpeedMultiplier = 1;
    private Coroutine co_trantitioningLayer = null;
    private Coroutine co_levelingAlpha = null;
    private Coroutine co_changingColor = null;
    //Bool dari coroutine
    public bool isTransitioningLayer => co_trantitioningLayer != null;
    public bool isLevelingAlpha => co_levelingAlpha != null;
    public bool isChangingColor => co_changingColor != null;

    //constructor dari class ini
    public CharacterSpriteLayer (Image defaultRenderer, int layer = 0)
    {
        //memasukan renderer dengan image dan setting nilai dari layernya
        renderer = defaultRenderer;
        this.layer = layer;
    }

    //fungsi untuk mengganti gambar dari sprite
    public void SetSprite(Sprite sprite)
    {
        //mengganti renderer sprite dengan sprite yang baru
        renderer.sprite = sprite;
    }

    //fungsi untuk transisi pergantian sprite
    public Coroutine TransitionSprite(Sprite sprite, float speed = 1)
    {
        if(sprite == renderer.sprite)
        {
            return null;
        }

        if(isTransitioningLayer)
        {
            characterManager.StopCoroutine(co_trantitioningLayer);
        }

        //menjalankan coroutine trantitioning sprite 
        co_trantitioningLayer = characterManager.StartCoroutine(TransitioningSprite(sprite,speed));
        return co_trantitioningLayer;
    }

    //Ienuerator untuk trantitioning sprite
    private IEnumerator TransitioningSprite(Sprite sprite, float speedMultiplier)
    {
        //inisialisasi bahan
        transitionSpeedMultiplier = speedMultiplier;
        //membuat renderer yang baru ukurannya sesuai dengan create renderer
        Image newRenderer = CreateRenderer(renderer.transform.parent);
        newRenderer.sprite = sprite;

        //memanggil fungsi trystartleveling alphas
        yield return TryStartLevelingAlphas();

        co_trantitioningLayer = null;
    }

    //fungsi untuk membuat ukuran sprite renderer sesuai
    private Image CreateRenderer(Transform parent)
    {
        //menampilkan renderer baru dalam bentuk image dengan gambar dari renderer dan ukurannya sesuai dengan parentnya
        Image newRenderer = Object.Instantiate(renderer, parent);
        oldRenderers.Add(rendererCG);

        //mengisi nama dari new renderer dengan nama yang sama dari renderer current
        newRenderer.name = renderer.name;
        //membuat renderer yang baru menjadi render current
        renderer = newRenderer;
        //mengaktifkan gameobject dari renderer
        renderer.gameObject.SetActive(true);
        //menurunkan rendererCG menjadi 0 sehingga seperti menghilang
        rendererCG.alpha = 0;
        return newRenderer;

    }

    //transisi sprite menjadi lebih smooth
    private Coroutine TryStartLevelingAlphas()
    {
        if(isLevelingAlpha)
        {
            return co_levelingAlpha;
        }
        //menjalankan Ienumerator runAlphaleveling
        co_levelingAlpha = characterManager.StartCoroutine(RunAlphaLeveling());
        return co_levelingAlpha;
    }

    private IEnumerator RunAlphaLeveling()
    {   
        //perulangan  yang dijalankan ketika alphanya kurang dari satu atau old renderer memiliki alpha yang lebih dari 1
        while(rendererCG.alpha < 1 || oldRenderers.Any(oldCG => oldCG.alpha >0))
        {
            float speed = DEFAULT_TRANSITION_SPEED * transitionSpeedMultiplier * Time.deltaTime;
            //membuat renderer alpha dari sprite terbaru transisi dari posisi awal ke 1 dengan kecepatan tertentu
            rendererCG.alpha = Mathf.MoveTowards(rendererCG.alpha,1, speed);

            //perulangan untuk membuat oldrenderer alpha dari posisi awal ke 0
            //memakai perulangan agar semua oldrenderer didalamnya bisa berubah menjadi 0 lalu dihapus dari canvas group
            for(int i = oldRenderers.Count-1 ; i>= 0 ; i--)
            {
                CanvasGroup oldCG = oldRenderers[i];
                oldCG.alpha = Mathf.MoveTowards(oldCG.alpha, 0 , speed);
                if(oldCG.alpha <= 0)
                {
                    oldRenderers.RemoveAt(i);
                    Object.Destroy(oldCG.gameObject);
                }
            }
            yield return null;
        }
        co_levelingAlpha = null;
    }

    //fungsi untuk setting color dari sprite
    public void SetColor(Color color)
    {
        //setting renderer current dengan image didalam oldcg
        renderer.color = color;
        foreach(CanvasGroup oldCG in oldRenderers)
        {
            oldCG.GetComponent<Image>().color = color;
        }
    }

    //fungsi untuk transisi warna dari sprite
    public Coroutine TransitionColor(Color color, float speed)
    {
        if(isChangingColor)
        {
            characterManager.StopCoroutine(co_changingColor);
        }

        //memanggil IEnumerator changing color
        co_changingColor = characterManager.StartCoroutine(ChangingColor(color,speed));
        return co_changingColor;
    }

    //Ienumerator untuk mengubah warna secara transisi
    private IEnumerator ChangingColor (Color color, float speedMultiplier)
    {
        Color oldColor = renderer.color;
        List<Image> oldImages = new List<Image>();

        foreach (var oldCG in oldRenderers)
        {
            oldImages.Add(oldCG.GetComponent<Image>());
        }

        float colorPercent = 0;
        while(colorPercent < 1)
        {
            colorPercent += DEFAULT_TRANSITION_SPEED * speedMultiplier * Time.deltaTime;
            renderer.color = Color.Lerp(oldColor, color, colorPercent);
            foreach(Image oldImage in oldImages)
            {
                oldImage.color = renderer.color;
            }

            yield return null;
        }
        co_changingColor = null;
    }

    //fungsi untuk menghentikan coroutine stop changing color
        public void StopChangingColor()
    {
        if(!isChangingColor)
            return;
        
        characterManager.StopCoroutine(co_changingColor);
        co_changingColor = null;
    }



}

}

