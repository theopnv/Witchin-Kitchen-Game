using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace con2.game
{

    public class PlayerHUD : MonoBehaviour
    {
        private DetectController _DetectController;
        public int OwnerId = -1;
        public Text Message;
        public GameObject RecipeIconsParent;
        public Text Score;
        [HideInInspector]
        public PlayerManager Manager;

        [SerializeField] private GameObject RegularHUD;
        [SerializeField] private GameObject ReadyPanel;
        [SerializeField] private GameObject ReadyPanelRecipe;
        [SerializeField] private Text ReadyPanelMessage;

        void Start()
        {
            _DetectController = FindObjectOfType<DetectController>();
            _DetectController.OnConnected += OnConnected;
            _DetectController.OnDisconnected += OnDisconnected;
        }

        void OnDisable()
        {
            _DetectController.OnConnected -= OnConnected;
            _DetectController.OnDisconnected -= OnDisconnected;
        }

        void OnConnected(int i)
        {
            if (i == OwnerId)
            {
                Manager?.SendMessageToPlayerInHUD("", Color.red);
            }
        }

        void OnDisconnected(int i)
        {
            if (i == OwnerId)
            {
                Manager?.SendMessageToPlayerInHUD("Your controller was disconnected.", Color.red, true);
            }
        }

        public void SetReadyActive()
        {
            RegularHUD.SetActive(false);
            ReadyPanel.SetActive(true);
            ReadyPanelMessage.color = ColorsManager.Get().PlayerMeshColors[Manager.ID];

            var rect = ReadyPanelRecipe.transform.GetComponent<Image>();
            rect.color = ColorsManager.Get().PlayerMeshColors[Manager.ID];
        }

        public void CollectIngredient(Ingredient type)
        {
            for (int i = 0; i < RecipeIconsParent.transform.childCount; i++)
            {
                var child = RecipeIconsParent.transform.GetChild(i);
                if (child.GetComponent<IngredientType>().m_type == type)
                {
                    var icon = child.GetComponent<Image>();
                    var color = icon.color;
                    if (color.a > 0.95f)
                    {
                        icon.color = new Color(color.r, color.g, color.b, 0.9f);
                        icon.sprite = GlobalRecipeList.completedIngredient;
                        break;
                    }
                }
            }

        }

        public void SetNewRecipeIcons(List<Image> icons)
        {
            //Clear old recipe
            while (RecipeIconsParent.transform.childCount > 0)
            {
                var child = RecipeIconsParent.transform.GetChild(0);
                child.parent = null;
                GameObject.Destroy(child.gameObject);
            }

            foreach (var icon in icons)
            {
                Instantiate(icon, RecipeIconsParent.transform);
            }
        }
    }

}
