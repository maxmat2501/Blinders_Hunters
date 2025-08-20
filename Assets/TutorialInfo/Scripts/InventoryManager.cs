
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ShadowHunters
{
    public class InventoryManager : MonoBehaviour
    {
        public static InventoryManager Instance { get; private set; }

        [Header("Bootstrap")]
        public bool persistAcrossScenes = true;

        [Header("Debug")]
        [SerializeField] private bool clearOnStart = false;

        public SaveData Data { get; private set; } = new SaveData();

        public event Action OnDataChanged;

        string SaveDir => Path.Combine(Application.persistentDataPath, "ShadowHunters");
        string SavePath => Path.Combine(SaveDir, "save.json");

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            if (persistAcrossScenes) DontDestroyOnLoad(gameObject);
            Load();
            if (clearOnStart) { Data = new SaveData(); Save(); }
        }

        public void AddCoins(int amount)
        {
            Data.coins = Mathf.Max(0, Data.coins + amount);
            SaveAndNotify();
        }

        public bool TrySpend(int amount)
        {
            if (Data.coins < amount) return false;
            Data.coins -= amount;
            SaveAndNotify();
            return true;
        }

        public bool Owns(string cardId) => Data.owned.Contains(cardId);

        public void AddCard(string cardId)
        {
            if (!Data.owned.Contains(cardId))
            {
                Data.owned.Add(cardId);
                SaveAndNotify();
            }
        }

        public void ToggleFavorite(string cardId)
        {
            if (Data.favorites.Contains(cardId)) Data.favorites.Remove(cardId);
            else Data.favorites.Add(cardId);
            SaveAndNotify();
        }

        public bool IsFavorite(string cardId) => Data.favorites.Contains(cardId);

        public void Save()
        {
            try
            {
                if (!Directory.Exists(SaveDir)) Directory.CreateDirectory(SaveDir);
                var json = JsonUtility.ToJson(Data, true);
                File.WriteAllText(SavePath, json);
            }
            catch (Exception e)
            {
                Debug.LogError($"Save failed: {e}");
            }
        }

        public void Load()
        {
            try
            {
                if (File.Exists(SavePath))
                {
                    var json = File.ReadAllText(SavePath);
                    Data = JsonUtility.FromJson<SaveData>(json) ?? new SaveData();
                }
                else
                {
                    Data = new SaveData();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Load failed: {e}");
                Data = new SaveData();
            }
        }

        void SaveAndNotify()
        {
            Save();
            OnDataChanged?.Invoke();
        }
    }
}
