using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;

namespace DIALOGUE
{

//class untuk speaker
public class DL_SPEAKER_DATA 
{
    //deklarasi variable
    public string name, castName;
    public string displayname => isCastingName ? castName : name;

    public bool isCastingName => castName != string.Empty;
    public bool isCastingPosition = false;
    public bool isCastingExpressions => CastExpressions.Count>0;
    public Vector2 castPosition;
    public bool makeCharacterEnter = false;
    
    public List<(int layer, string expression)> CastExpressions {get;set;}
    private const string NAMECAST_ID = " as ";
    private const string POSITIONCAST_ID = " at ";
    private const string EXPRESSIONCAST_ID = " [";
    private const char AXISDELIMITER = ':';
    private const char EXPRESSIONLAYER_JOINER = ',';
    private const char EXPRESSIONLAYER_DELIMITER = ':';

    private const string ENTRER_KEYWORD = "enter ";

    //construcutor untuk class ini untuk mengetahui nama dari speaker

    private string ProcessKeywords(string rawSpeaker)
    {
        if(rawSpeaker.StartsWith(ENTRER_KEYWORD))
        {
            rawSpeaker = rawSpeaker.Substring(ENTRER_KEYWORD.Length);
            makeCharacterEnter = true;
        }

        return rawSpeaker;
    }
    public DL_SPEAKER_DATA(string rawSpeaker)
    {
        rawSpeaker = ProcessKeywords(rawSpeaker);
        //pattern dari text 
        string pattern = @$"{NAMECAST_ID}|{POSITIONCAST_ID}|{EXPRESSIONCAST_ID.Insert(EXPRESSIONCAST_ID.Length-1, @"\")}";
        //mencari kecocokan antara pattern dengan rawspeaker menggunakan regex
        MatchCollection matches = Regex.Matches(rawSpeaker,pattern);
        
        //format default untuk setiap fungsinya
        castName = "";
        castPosition = Vector2.zero;
        CastExpressions = new List<(int layer, string expression)>();

        //jika tidak ada yang cocok maka
        if(matches.Count == 0)
        {
            name = rawSpeaker;
            return;
        }

        //membuat inisialisasi index 
        int index = matches[0].Index;

        //memasukan substring dari awal hingga nilai index sebagai nama 
        name = rawSpeaker.Substring(0, index);

        //perulangan untuk mendapatkan castname,castposition,dan cast expression
        for (int i = 0 ; i< matches.Count ; i++)
        {
            //memisahkan setiap match dalam matches (seperti foreach)
            Match match = matches[i];
            int startIndex = 0, endIndex = 0;

            //jika terdapat value seperti namecast id
            if(match.Value == NAMECAST_ID)
            {
                //mengetahui posisi dari castname berdasarkan index matchnya
                startIndex = match.Index + NAMECAST_ID.Length;
                endIndex = (i<matches.Count - 1) ? matches[i+1].Index : rawSpeaker.Length;
                castName = rawSpeaker.Substring(startIndex, endIndex - startIndex);
            }
            //jika terdapat value potition cast
            else if( match.Value == POSITIONCAST_ID)
            {
                isCastingPosition = true;
                //mengetahui posisi dari castposition
                startIndex = match.Index + POSITIONCAST_ID.Length;
                endIndex = (i<matches.Count - 1) ? matches[i+1].Index : rawSpeaker.Length;
                string castPos = rawSpeaker.Substring(startIndex, endIndex - startIndex);

                //membagi dua string dengan kunci AXISDELIMITER sebagai kuncinya, plus menghapus text yang kosong
                string[] axis = castPos.Split(AXISDELIMITER, System.StringSplitOptions.RemoveEmptyEntries);

                //menyimpan nilai axis0 pada castposisition.x
                float.TryParse(axis[0], out castPosition.x);

                //jika axis memiliki 2 data 
                if(axis.Length>1)
                {
                    //menyimpan nilai axis1 pada castposistion y
                    float.TryParse(axis[1], out castPosition.y);
                }
            }

            //jika terdapat value expressioncast
            else if( match.Value == EXPRESSIONCAST_ID)
            {
                //mencari posisi dari nilai expresinya
                startIndex = match.Index + EXPRESSIONCAST_ID.Length;
                endIndex = (i<matches.Count - 1) ? matches[i+1].Index : rawSpeaker.Length;
                string castExp = rawSpeaker.Substring(startIndex, endIndex - (startIndex + 1));

                //isi dari cast expression harus int untuk layer dan enama ekspresinya
                //memisahkan data dari castexp menjadi dua berdasarkan EXPRESSIONLAYER_JOINER
                CastExpressions = castExp.Split(EXPRESSIONLAYER_JOINER).Select(
                    //setelah dibagi dua didapat nilai x sebagai contoh "1:layer1,2:layer2"
                    x => {
                        //x dipecah menjadi dua dengan kunci EXPRESSIONLAYER_DELIMITER dan hasilnya menjadi "1,layer 1,2,layer 2 didalam parts"
                        var parts = x.Trim().Split(EXPRESSIONLAYER_DELIMITER);

                        if(parts.Length ==2)
                            return(int.Parse(parts[0]), parts[1]);
                        else    
                            return (0, parts[0]);
                    }).ToList();
            }
        }


    }
}
}
