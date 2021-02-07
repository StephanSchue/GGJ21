using UnityEngine;
using UnityEngine.Events;
using TMPro;
#if UNITY_EDITOR
using UnityEditor.Events;
#endif

namespace GGJ21.Localisation
{
    [System.Serializable] public class LocaStringEvent : UnityEvent<string> { }

    public class LocalizeStringComponent : MonoBehaviour
    {
        public string variable = "";
        public LocaStringEvent OnRefresh;

        private LocalisationManager localisationManager;
        private bool foundLocalisationManager;

        #if UNITY_EDITOR

        private void Reset()
        {
            TextMeshProUGUI textMeshProUGUI = GetComponent<TextMeshProUGUI>();
            var textProp = textMeshProUGUI.text;

            var targetInfo = UnityEvent.GetValidMethodInfo(textMeshProUGUI, "SendMessage", new System.Type[] { textProp.GetType() });
            UnityAction<string> methodDelegate = System.Delegate.CreateDelegate(typeof(UnityAction<string>), textMeshProUGUI, targetInfo) as UnityAction<string>;
            UnityEventTools.AddPersistentListener<string>(OnRefresh, methodDelegate);
        }

        #endif

        private void Start()
        {
            localisationManager = GameObject.FindGameObjectWithTag("GameController")?.GetComponent<LocalisationManager>();
            foundLocalisationManager = localisationManager != null;

            if(foundLocalisationManager)
                localisationManager.RegisterLocalizeStringComponent(this);
        }

        private void OnDestroy()
        {
            if(foundLocalisationManager)
                localisationManager.UnRegisterLocalizeStringComponent(this);
        }
    }
}