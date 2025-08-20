
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace ShadowHunters
{
    public class MainMenuUI : MonoBehaviour
    {
        [Header("Audio")]
        public AudioSource musicSource;
        public AudioClip menuMusic;

        [Header("UI")]
        public TMP_InputField ipInput;
        public TMP_Text versionText;

        [Header("Layout")]
        public CanvasScaler scaler;
        public Vector2 referenceResolution = new Vector2(720, 1080);

        void Start()
        {
            if (scaler)
            {
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = referenceResolution;
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                scaler.matchWidthOrHeight = 0.5f;
            }

            if (musicSource && menuMusic)
            {
                musicSource.loop = true;
                musicSource.clip = menuMusic;
                musicSource.Play();
            }

            if (versionText)
                versionText.text = $"Build {Application.version}";
        }

        // --- Button Hooks ---
        public void PlayLocal()
        {
            SceneManager.LoadScene("Game"); // replace with your scene
        }

        public void OpenInventory()
        {
            SceneManager.LoadScene("Inventory");
        }

        public void OpenShop()
        {
            SceneManager.LoadScene("Shop");
        }

        public void JoinByIP()
        {
            var ip = ipInput ? ipInput.text : "";
            Debug.Log($"Join by IP: {ip} (hook up your Netcode here)");
            // TODO: Implement Netcode for GameObjects or NGO Relay join with the given IP/Code
        }

        public void Quit()
        {
            Application.Quit();
        }
    }
}
