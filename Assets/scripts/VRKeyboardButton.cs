using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class VRKeyboardButton : MonoBehaviour
{
    [SerializeField] private string buttonValue;
    [SerializeField] private TMP_Text buttonText;
    
    private Button button;
    private PasswordManager passwordManager;
    
    private void Start()
    {
        button = GetComponent<Button>();
        passwordManager = FindFirstObjectByType<PasswordManager>();
        
        if (buttonText != null && !string.IsNullOrEmpty(buttonValue))
        {
            buttonText.text = buttonValue;
        }
        
        button.onClick.AddListener(OnButtonClick);
    }
    
    private void OnButtonClick()
    {
        if (passwordManager != null && !string.IsNullOrEmpty(buttonValue))
        {
            passwordManager.AddCharacter(buttonValue);
        }
    }
    
    public void SetButtonValue(string value)
    {
        buttonValue = value;
        if (buttonText != null)
        {
            buttonText.text = value;
        }
    }
}
