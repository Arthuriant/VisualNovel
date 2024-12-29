using System;

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DIALOGUE
{
//class untuk tyoe dialoge
public class DL_DIALOGUE_DATA 
{
    //pembuatan list dengan isi dalam bentuk dialogue segment
    public List<DIALOGUE_SEGMENT> segments;

    //format untuk mengidentifikasi dialognya
    private const string segmentIdentifierPattern = @"\{[ca]\}|\{w[ca]\s\d*\.?\d*\}";

    //memasukan data pada variable segments dan ini adalah constructir
    public DL_DIALOGUE_DATA(string rawDialogue)
    {
        segments = RipSegments(rawDialogue);
    }



    public List<DIALOGUE_SEGMENT> RipSegments(string rawDialogue)
    {
        List<DIALOGUE_SEGMENT> segments =  new List<DIALOGUE_SEGMENT>();
        //match collection adalah tipe data untuk objek yang match yang ditemukan oleh regex
        //regex adalah fungsi untuk menemukan kecocokan dalam string
        //rawdialogue di compare dengan polanya yaitu segmentidentifier segment dan dicocokan, hasil dari pencocokan disimpan di matches
        MatchCollection matches = Regex.Matches(rawDialogue, segmentIdentifierPattern);

        int LastIndex = 0;
        //pembuatan structur baru
        DIALOGUE_SEGMENT segment = new DIALOGUE_SEGMENT();
        //mengisi dialogue dalam struct ketika tidak ada yg cocok maka = raw dialogue
        //jika ada yg cocok maka substringnya raw dialogue pada matches di index 0;
        segment.dialogue = (matches.Count == 0? rawDialogue : rawDialogue.Substring(0,matches[0].Index));
        //mengisi default signal sebagai none
        segment.startSignal = DIALOGUE_SEGMENT.StartSignal.NONE;
        //mengatur signal delay sebagai 0
        segment.signalDelay = 0;
        //memasukan segment yang telah diisi sebelumnya ke segment
        segments.Add(segment);

        //jika tidak ada yang cocok maka langsung mengembalikan nilai segment
        if(matches.Count == 0)
        {
            return segments;
        }
        //jika ada yang cocok maka last indexnya adalah indext pertama dari matches
        else
        {
            LastIndex = matches[0].Index;
        }

        for(int i = 0;i<matches.Count ; i++)
        {
            //memisahkan setiap matches menjadi match
            Match match = matches[i];
            //membuat list segment baru
            segment = new DIALOGUE_SEGMENT();

            //memisahkan antara signal dengan dialogue
            string signalMatch = match.Value;
            signalMatch = signalMatch.Substring(1, match.Length -2);
            string[] singalSplit = signalMatch.Split(' ');
            segment.startSignal = (DIALOGUE_SEGMENT.StartSignal) Enum.Parse(typeof(DIALOGUE_SEGMENT.StartSignal),singalSplit[0].ToUpper());

            //jika signalnya ada maka
            if(singalSplit.Length>1)
            {
                float.TryParse(singalSplit[1], out segment.signalDelay);
            }

            //memasukan ??? pada segments setelah memisahkan dialogue pada substring
            int nextIndex = i + 1 < matches.Count ? matches[i+1].Index : rawDialogue.Length;
            segment.dialogue = rawDialogue.Substring(LastIndex + match.Length, nextIndex -(LastIndex+ match.Length));
            LastIndex = nextIndex;

            segments.Add(segment);
        }
        return segments;
        
    }

    //struktur untuk dialouge segment
    public struct DIALOGUE_SEGMENT
    {
        public string dialogue;
        public StartSignal startSignal;
        public float signalDelay;
        public enum StartSignal { NONE, C, A, WA, WC}

        public bool appendText => (startSignal == StartSignal.A || startSignal == StartSignal.WA);
    }
}
}
