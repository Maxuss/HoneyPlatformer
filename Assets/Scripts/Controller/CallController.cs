using System;
using System.Collections;
using System.Collections.Generic;
using Dialogue;
using Save;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Controller
{
    public class CallController: MonoBehaviour
    {
        [SerializeField]
        private GameObject donMenu;

        [SerializeField]
        private Transform moreHeight;
        [SerializeField]
        private Transform moreSpeed;
        [SerializeField]
        private Transform moreCamSpeed;
        [SerializeField]
        private TMP_Text currencyText;
        [SerializeField]
        private DialogueDefinition[] possibleDialogues;

        [FormerlySerializedAs("_inMenu")] public bool inMenu;
        
        public static CallController Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            donMenu.SetActive(false);

            if (SaveManager.CurrentState.DonUpgrades == null)
                SaveManager.CurrentState.DonUpgrades = new List<DonUpgrade>();
            foreach (var upg in SaveManager.CurrentState.DonUpgrades)
            {
                switch (upg)
                {
                    case DonUpgrade.HigherJumps:
                        moreHeight.gameObject.SetActive(false);
                        break;
                    case DonUpgrade.FasterSpeed:
                        moreSpeed.gameObject.SetActive(false);
                        break;
                    case DonUpgrade.FasterCamera:
                        moreCamSpeed.gameObject.SetActive(false);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(upg), upg, null);
                }
            }
        }

        private void Update()
        {
            if (inMenu && Input.GetKeyDown(KeyCode.Escape))
            {
                donMenu.SetActive(false);
                PlayerController.Instance.InCutscene = false;
                inMenu = false;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                return;
            }
            if (inMenu || PlayerController.Instance.InCutscene || PauseController.IsPaused)
                return;
            if (PlayerController.Instance.IsDisabled || !SaveManager.CurrentState.MetDon)
                return;
            if (!Input.GetKeyDown(KeyCode.B))
                return;

            inMenu = true;
            StartCoroutine(CallCoroutine());
        }

        private IEnumerator CallCoroutine()
        {
            // TODO: call sfx??
            var randomDialogue = possibleDialogues[Random.Range(0, possibleDialogues.Length)];
            yield return DialogueManager.Instance.StartDialogue(randomDialogue);
            PlayerController.Instance.InCutscene = true;
            currencyText.text = SaveManager.CurrentState.Currency.ToString();
            donMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void Purchase(int upgIdx)
        {
            var upg = upgIdx switch
            {
                0 => DonUpgrade.FasterSpeed,
                1 => DonUpgrade.HigherJumps,
                2 => DonUpgrade.FasterCamera,
                _ => throw new ArgumentOutOfRangeException(nameof(upgIdx), upgIdx, null)
            };
            var cost = upg switch
            {
                DonUpgrade.FasterSpeed => 500,
                DonUpgrade.HigherJumps => 750,
                DonUpgrade.FasterCamera => 1500,
                _ => throw new ArgumentOutOfRangeException(nameof(upg), upg, null)
            };
            Debug.Log($"PURCHASING {upgIdx} {upg} {cost}");
            if (SaveManager.CurrentState.Currency < cost)
                return;
            // TODO: purchase sound??
            SaveManager.CurrentState.Currency -= cost;
            currencyText.text = SaveManager.CurrentState.Currency.ToString();
            SaveManager.CurrentState.DonUpgrades.Add(upg);
            donMenu.SetActive(false);
            inMenu = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            PlayerController.Instance.InCutscene = false;
            switch (upg)
            {
                case DonUpgrade.HigherJumps:
                    moreHeight.gameObject.SetActive(false);
                    break;
                case DonUpgrade.FasterSpeed:
                    moreSpeed.gameObject.SetActive(false);
                    break;
                case DonUpgrade.FasterCamera:
                    moreCamSpeed.gameObject.SetActive(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(upg), upg, null);
            }
        }
    }
}