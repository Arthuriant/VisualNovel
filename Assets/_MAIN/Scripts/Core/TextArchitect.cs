using UnityEngine;
using TMPro;
using UnityEngine.Purchasing;
using System.Collections;

//MALAS BANGET JADI PAKE AI UNTUK YG INI

// Class ini digunakan untuk mengelola dan membangun teks pada objek TextMeshPro (baik UI maupun World)
public class TextArchitect 
{
    // Variabel untuk menyimpan referensi ke komponen TextMeshPro pada UI dan dunia
    private TextMeshProUGUI tmpro_ui; // UI TextMeshPro
    private TextMeshPro tmpro_world; // World TextMeshPro

    // Properti untuk memilih antara UI dan World TextMeshPro, yang digunakan tergantung pada objek yang diberikan
    public TMP_Text tmpro => tmpro_ui != null ? tmpro_ui : tmpro_world;

    // Properti untuk mendapatkan teks saat ini
    public string currentText => tmpro.text;

    // Properti untuk teks target yang akan ditampilkan
    public string targetText { get; private set;} = "";

    // Properti untuk teks yang sudah diproses
    public string proText { get; private set;} = "";

    // Variabel untuk menyimpan panjang teks yang telah diproses
    private int proTextLength = 0;

    // Properti untuk menggabungkan proText dengan targetText menjadi full text
    public string fullTargetText => proText + targetText;

    // Enum untuk menentukan metode pembuatan teks (instant, typewriter, fade)
    public enum BuildMethod {instant, typewriter, fade}

    // Metode pembuatan teks yang dipilih (default: typewriter)
    public BuildMethod buildMethod = BuildMethod.typewriter;

    // Properti untuk mengatur warna teks
    public Color textColor { get { return tmpro.color; } set { tmpro.color = value; } }

    // Properti untuk mengatur kecepatan pembuatan teks
    public float speed { get { return baseSpeed * speedMultiplier; } set { speedMultiplier = value; } }
    private const float baseSpeed = 1; // Kecepatan dasar
    private float speedMultiplier = 1; // Faktor pengali kecepatan

    // Properti untuk menghitung jumlah karakter yang akan ditampilkan per siklus
    public int charactersPerCycle { get { return speed <= 2f ? characterMultiplier : speed <= 2.5f ? characterMultiplier * 2 : characterMultiplier * 3; } }

    // Faktor pengali jumlah karakter per siklus
    public int characterMultiplier = 1;

    // Properti untuk mempercepat proses pembuatan teks
    public bool hurryUp = false;

    // Konstruktor untuk objek TextMeshProUGUI (UI)
    public TextArchitect(TextMeshProUGUI tmpro_ui)
    {
        this.tmpro_ui = tmpro_ui;
    }

    // Konstruktor untuk objek TextMeshPro (World)
    public TextArchitect(TextMeshPro tmpro_world)
    {
        this.tmpro_world = tmpro_world;
    }

    // Fungsi untuk membangun teks baru
    public Coroutine Build(string text)
    {
        proText = ""; // Reset teks yang sudah diproses
        targetText = text; // Set teks target
        Stop(); // Hentikan proses sebelumnya jika ada
        buildProcess = tmpro.StartCoroutine(Building()); // Mulai proses pembuatan teks
        return buildProcess;
    }

    // Fungsi untuk menambahkan teks baru ke teks yang sudah ada
    public Coroutine Append(string text)
    {
        proText = tmpro.text; // Simpan teks yang sudah ada
        targetText = text; // Set teks baru yang akan ditambahkan
        Stop(); // Hentikan proses sebelumnya jika ada
        buildProcess = tmpro.StartCoroutine(Building()); // Mulai proses pembuatan teks
        return buildProcess;
    }

    private Coroutine buildProcess = null;

    // Properti untuk mengecek apakah pembuatan teks sedang berlangsung
    public bool isBuilding => buildProcess != null;

    // Fungsi untuk menghentikan proses pembuatan teks
    public void Stop()
    {
        if (!isBuilding) return; // Jika tidak ada proses yang berjalan, tidak perlu dihentikan
        
        tmpro.StopCoroutine(buildProcess); // Hentikan coroutine
        buildProcess = null;
    }

    // Fungsi utama yang mengatur proses pembuatan teks
    IEnumerator Building()
    {
        Prepare(); // Persiapkan sebelum mulai membangun teks
        switch (buildMethod)
        {
            case BuildMethod.typewriter:
                yield return Build_Typewrite(); // Pembuatan teks dengan efek mesin ketik
                break;
            case BuildMethod.fade:
                yield return Build_Fade(); // Pembuatan teks dengan efek pudar
                break;
        }
        OnComplete(); // Selesai, lakukan finalisasi
    }

    // Fungsi yang dipanggil setelah pembuatan teks selesai
    private void OnComplete()
    {
        buildProcess = null;
        hurryUp = false;
    }

    // Fungsi untuk memaksa teks selesai seketika
    public void ForceComplete()
    {
        switch (buildMethod)
        {
            case BuildMethod.typewriter:
                tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount; // Menampilkan seluruh teks
                break;
            case BuildMethod.fade:
                tmpro.ForceMeshUpdate(); // Update mesh teks
                break;
        }
    }

    // Fungsi untuk mempersiapkan teks berdasarkan metode pembuatan yang dipilih
    private void Prepare()
    {
        switch (buildMethod)
        {
            case BuildMethod.instant:
                prepare_Instan(); // Persiapkan untuk instant
                break;
            case BuildMethod.typewriter:
                prepare_Typeweriter(); // Persiapkan untuk efek mesin ketik
                break;
            case BuildMethod.fade:
                prepare_Fade(); // Persiapkan untuk efek pudar
                break;
        }
    }

    // Persiapan untuk metode instant (langsung tampil)
    private void prepare_Instan()
    {
        tmpro.color = tmpro.color; // Tidak merubah warna
        tmpro.text = fullTargetText; // Set seluruh teks
        tmpro.ForceMeshUpdate(); // Update mesh
        tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount; // Tampilkan seluruh karakter
    }

    // Persiapan untuk metode typewriter (teks muncul seperti mesin ketik)
    private void prepare_Typeweriter()
    {
        tmpro.color = tmpro.color; // Tidak merubah warna
        tmpro.maxVisibleCharacters = 0; // Mulai dengan tidak ada karakter yang terlihat
        tmpro.text = proText; // Set teks yang sudah diproses

        if (proText != "")
        {
            tmpro.ForceMeshUpdate(); // Update mesh
            tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount; // Update jumlah karakter terlihat
        }

        tmpro.text += targetText; // Tambahkan teks target ke teks yang sudah ada
        tmpro.ForceMeshUpdate(); // Update mesh
    }

    // Persiapan untuk metode fade (teks muncul dengan efek pudar)
    private void prepare_Fade()
    {
        tmpro.text = proText; // Set teks yang sudah diproses
        if (proText != "")
        {
            tmpro.ForceMeshUpdate(); // Update mesh
            proTextLength = tmpro.textInfo.characterCount; // Hitung panjang teks yang sudah diproses
        }
        else
        {
            proTextLength = 0;
        }

        tmpro.text += targetText; // Tambahkan teks target ke teks yang sudah ada
        tmpro.maxVisibleCharacters = int.MaxValue; // Menampilkan seluruh karakter
        tmpro.ForceMeshUpdate(); // Update mesh

        TMP_TextInfo textInfo = tmpro.textInfo; // Ambil informasi teks
        Color colorVisible = new Color(textColor.r, textColor.g, textColor.b, 1); // Warna teks yang terlihat
        Color colorHidden = new Color(textColor.r, textColor.g, textColor.b, 0); // Warna teks yang tersembunyi
        Color32[] vertexColors = textInfo.meshInfo[textInfo.characterInfo[0].materialReferenceIndex].colors32; // Ambil warna vertex teks

        // Loop untuk mengatur warna setiap karakter sesuai dengan visibilitasnya
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue; // Lewatkan karakter yang tidak terlihat

            // Set warna karakter berdasarkan apakah sudah diproses atau belum
            if (i < proTextLength)
            {
                for (int v = 0; v < 4; v++)
                {
                    vertexColors[charInfo.vertexIndex + v] = colorVisible; // Karakter terlihat
                }
            }
            else
            {
                for (int v = 0; v < 4; v++)
                {
                    vertexColors[charInfo.vertexIndex + v] = colorHidden; // Karakter tersembunyi
                }
            }
        }
        tmpro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32); // Update data vertex warna
    }

    // Coroutine untuk efek mesin ketik (Typewriter)
    private IEnumerator Build_Typewrite()
    {
        while (tmpro.maxVisibleCharacters < tmpro.textInfo.characterCount)
        {
            tmpro.maxVisibleCharacters += hurryUp ? charactersPerCycle * 5 : charactersPerCycle; // Tambahkan karakter yang terlihat
            yield return new WaitForSeconds(0.015f / speed); // Tunggu berdasarkan kecepatan
        }
    }

    // Coroutine untuk efek pudar (Fade)
    private IEnumerator Build_Fade()
    {
        int minRange = proTextLength;
        int maxRange = minRange + 1;
        byte alphaThreshold = 15; // Ambang batas alpha
        TMP_TextInfo textInfo = tmpro.textInfo; // Ambil informasi teks
        Color32[] vertexColors = textInfo.meshInfo[textInfo.characterInfo[0].materialReferenceIndex].colors32; // Ambil warna vertex
        float[] alphas = new float[textInfo.characterCount]; // Array untuk menyimpan alpha setiap karakter

        // Loop untuk memudar karakter satu per satu
        while (true)
        {
            float fadeSpeed = ((hurryUp ? charactersPerCycle * 5 : charactersPerCycle) * speed) * 10f; // Kecepatan pudar

            // Fade karakter satu per satu
            for (int i = minRange; i < maxRange; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible) continue;

                int vertexIndex = charInfo.vertexIndex; 
                alphas[i] = Mathf.MoveTowards(alphas[i], 255, fadeSpeed); // Ubah alpha
                for (int v = 0; v < 4; v++)
                {
                    vertexColors[vertexIndex + v].a = (byte)alphas[i]; // Atur alpha vertex
                }

                if (alphas[i] >= 255)
                {
                    minRange++; // Jika sudah sepenuhnya terlihat, lanjutkan ke karakter berikutnya
                }
            }
            tmpro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32); // Update data vertex

            // Jika karakter terakhir sudah terlihat atau alpha-nya sudah cukup tinggi, lanjutkan ke karakter berikutnya
            bool lastCharacterisVisible = !textInfo.characterInfo[maxRange - 1].isVisible;
            if (alphas[maxRange - 1] > alphaThreshold || lastCharacterisVisible)
            {
                if (maxRange < textInfo.characterCount)
                {
                    maxRange++; // Lanjutkan ke karakter berikutnya
                }
                else if (alphas[maxRange - 1] >= 255 || lastCharacterisVisible)
                {
                    break; // Selesai jika semua karakter sudah terlihat
                }
            }
            yield return new WaitForEndOfFrame(); // Tunggu frame berikutnya
        }
    }
}
