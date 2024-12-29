using System.Collections.Generic; // Untuk menggunakan koleksi seperti Dictionary
using DIALOGUE; // Namespace khusus, mungkin untuk sistem dialog
using Mono.Cecil; // Library eksternal, mungkin untuk manipulasi metadata
using UnityEngine; // Untuk fungsi Unity

// Namespace untuk karakter
namespace CHARACTERS 
{
    // Kelas utama untuk mengelola karakter
    public class CharacterManager : MonoBehaviour
    {
        // Singleton instance agar hanya ada satu instance CharacterManager
        public static CharacterManager instance { get; private set; }

        // Dictionary untuk menyimpan karakter berdasarkan nama (key adalah nama karakter dalam huruf kecil)
        private Dictionary<string, Character> characters = new Dictionary<string, Character>();

        // Mengambil konfigurasi karakter dari sistem dialog
        private CharacterConfigSO config => DialogueSystem.instance.config.characterConfigurationAsset;

        // Konstanta untuk memisahkan nama karakter dan nama casting
        private const string CHARACTER_CASTING_ID = " as ";
        private const string CHARACTERNAME_ID = "<charname>";

        // Template path untuk folder karakter
        public string characterRootPath => $"Characters/{CHARACTERNAME_ID}";
        public string characterPrefabName => $"Character - [{CHARACTERNAME_ID}]";
        public string characterPrefabPath => $"{characterRootPath}/Prefab/{characterPrefabName}";

        // Panel tempat karakter ditampilkan di UI
        [SerializeField] private RectTransform _characterpanel = null;
        public RectTransform characterpanel => _characterpanel;

        // Metode yang dijalankan saat object di-`Awake`
        public void Awake()
        {
            instance = this; // Set instance singleton
        }

        // Mendapatkan konfigurasi karakter berdasarkan nama
        public CharacterConfigData GetCharacterConfig(string characterName)
        {
            return config.GetConfig(characterName);
        }

        // Mendapatkan karakter berdasarkan nama, dan dapat membuatnya jika belum ada
        public Character GetCharacter(string characterName, bool createIfDoesNotExist = false)
        {
            if (characters.ContainsKey(characterName.ToLower()))
            {
                return characters[characterName.ToLower()]; // Kembalikan karakter jika sudah ada
            }
            else if (createIfDoesNotExist)
            {
                return CreateCharacter(characterName); // Buat karakter baru jika belum ada
            }
            return null;
        }

        // Membuat karakter baru berdasarkan nama
        public Character CreateCharacter(string characterName)
        {
            if (characters.ContainsKey(characterName.ToLower()))
            {
                Debug.LogWarning($"A Character called '{characterName}' already exists");
                return null; // Jika karakter sudah ada, beri peringatan
            }

            CHARACTER_INFO info = GetCharacterInfo(characterName); // Ambil informasi karakter
            Character character = CreateCharacterFromInfo(info); // Buat karakter berdasarkan informasi

            characters.Add(characterName.ToLower(), character); // Tambahkan ke dictionary
            return character;
        }

        // Mendapatkan informasi detail karakter berdasarkan nama
        private CHARACTER_INFO GetCharacterInfo(string characterName)
        {
            CHARACTER_INFO result = new CHARACTER_INFO();

            // Pisahkan nama karakter dan nama casting (jika ada)
            string[] nameData = characterName.Split(CHARACTER_CASTING_ID, System.StringSplitOptions.RemoveEmptyEntries);
            result.name = nameData[0];
            result.castingName = nameData.Length > 1 ? nameData[1] : result.name;

            result.config = config.GetConfig(result.castingName); // Ambil konfigurasi karakter
            result.prefab = GetPrefabForCharacter(result.castingName); // Ambil prefab untuk karakter
            result.rootCharacterFolder = FormatCharacrterPath(characterRootPath, result.castingName); // Format path folder karakter
            return result;
        }

        // Mendapatkan prefab untuk karakter berdasarkan nama
        private GameObject GetPrefabForCharacter(string characterName)
        {
            string prefabPath = FormatCharacrterPath(characterPrefabPath, characterName); // Format path prefab
            return Resources.Load<GameObject>(prefabPath); // Load prefab dari Resources
        }

        // Memformat path dengan mengganti placeholder nama karakter
        public string FormatCharacrterPath(string path, string characterName) => path.Replace(CHARACTERNAME_ID, characterName);

        // Membuat karakter baru berdasarkan informasi karakter
        private Character CreateCharacterFromInfo(CHARACTER_INFO info)
        {
            CharacterConfigData config = info.config;

            // Membuat jenis karakter berdasarkan tipe dalam konfigurasi
            if (config.characterType == Character.CharacterType.Text)
            {
                return new Character_Text(info.name, config);
            }
            if (config.characterType == Character.CharacterType.Sprite || config.characterType == Character.CharacterType.SpriteSheet)
            {
                return new Character_Sprite(info.name, config, info.prefab, info.rootCharacterFolder);
            }
            if (config.characterType == Character.CharacterType.Live2D)
            {
                return new Character_Live2D(info.name, config, info.prefab, info.rootCharacterFolder);
            }
            if (config.characterType == Character.CharacterType.Model3D)
            {
                return new Character_Model3D(info.name, config, info.prefab, info.rootCharacterFolder);
            }

            return null; // Jika tipe karakter tidak dikenali
        }

        // Kelas internal untuk menyimpan informasi karakter sementara
        private class CHARACTER_INFO
        {
            public string name = ""; // Nama karakter
            public string castingName = ""; // Nama casting (jika ada)
            public string rootCharacterFolder = ""; // Folder root karakter
            public CharacterConfigData config = null; // Konfigurasi karakter
            public GameObject prefab = null; // Prefab karakter
        }
    }
}
