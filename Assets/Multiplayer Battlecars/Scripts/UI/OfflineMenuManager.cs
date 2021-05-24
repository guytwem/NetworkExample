using UnityEngine;
using System.Collections.Generic;

namespace Battlecars.UI
{
    public class OfflineMenuManager : MonoBehaviour
    {
        [System.Serializable]
        public struct Menu
        {
            public GameObject obj;
            public string id;
            public bool isDefault;
        }

        [System.Serializable]
        public struct Dialogue
        {
            public GameObject obj;
            public string id;
        }


        [SerializeField] private List<Menu> menus = new List<Menu>();
        private Dictionary<string, GameObject> menuDict = new Dictionary<string, GameObject>();
        private string currentMenuID = "";

        [SerializeField] private List<Dialogue> dialogues = new List<Dialogue>();
        private Dictionary<string, GameObject> dialogueDict = new Dictionary<string, GameObject>();
        private string currentDialogueID = "";

        // Start is called before the first frame update
        void Start()
        {
            menus.ForEach(menu =>
            {
                if(!menuDict.ContainsKey(menu.id))
                {
                    menuDict.Add(menu.id, menu.obj);
                }

                menu.obj.SetActive(false);

                if(menu.isDefault && string.IsNullOrEmpty(currentMenuID))
                {
                    menu.obj.SetActive(true);
                    currentMenuID = menu.id;
                }
            });

            dialogues.ForEach(dialogue =>
            {
                if(!dialogueDict.ContainsKey(dialogue.id))
                {
                    dialogueDict.Add(dialogue.id, dialogue.obj);
                }

                dialogue.obj.SetActive(false);
            });
        }

        public void OpenDialogue(string _id)
        {
            CloseDialogue();

            dialogueDict[_id].SetActive(true);
            currentDialogueID = _id;
        }

        public void CloseDialogue()
        {
            if(string.IsNullOrEmpty(currentDialogueID))
            {
                return;
            }

            dialogueDict[currentDialogueID].SetActive(false);
        }

        public void OpenMenu(string _id)
        {
            if(currentMenuID.Equals(_id))
            {
                Debug.LogWarning($"Menu {_id} is already active!");
                return;
            }

            menuDict[currentMenuID].SetActive(false);
            menuDict[_id].SetActive(true);

            currentMenuID = _id;
        }

        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}