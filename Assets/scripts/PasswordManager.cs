using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class PasswordManager : MonoBehaviour
{
    private const string PASSWORD_KEY = "UserPassword";
    private const string EXPIRY_DATE_KEY = "LicenseExpiryDate";
    private const int REQUIRED_PASSWORD_LENGTH = 4;

    [Header("Security Settings")]
    [SerializeField] private string correctPassword = "1701";
    [SerializeField] private int maxAttempts = 3;

    [Header("License Expiry (Fixed Date)")]
    [SerializeField] private int expiryYear = 2026;
    [SerializeField] private int expiryMonth = 3;
    [SerializeField] private int expiryDay = 30;

    [Header("UI References")]
    [SerializeField] private TMP_Text displayText;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private GameObject keyboardPanel;
    [SerializeField] private GameObject successPanel;

    [Header("Scene Settings")]
    [SerializeField] private string nextSceneName = "MainScene";

    private string currentInput = "";
    private int attemptCount = 0;

    private void Start()
    {
        if (correctPassword.Length != REQUIRED_PASSWORD_LENGTH)
        {
            Debug.LogError($"Correct password must be exactly {REQUIRED_PASSWORD_LENGTH} characters! Current length: {correctPassword.Length}");
        }

        //  Check expiry first
        if (IsLicenseExpired())
        {
            ResetPassword();
            statusText.text = "LICENSE EXPIRED! Contact support.";
            statusText.color = Color.red;
            DisableKeyboard();
            return;
        }

        if (IsPasswordVerified())
        {
            LoadNextScene();
        }
        else
        {
            ShowKeyboard();
        }
    }

    public void AddCharacter(string character)
    {
        if (currentInput.Length < REQUIRED_PASSWORD_LENGTH)
        {
            currentInput += character;
            UpdateDisplay();
        }
    }

    public void DeleteCharacter()
    {
        if (currentInput.Length > 0)
        {
            currentInput = currentInput.Substring(0, currentInput.Length - 1);
            UpdateDisplay();
        }
    }

    public void ClearInput()
    {
        currentInput = "";
        UpdateDisplay();
    }

    public void SubmitPassword()
    {
        if (currentInput.Length == REQUIRED_PASSWORD_LENGTH)
        {
            if (ValidatePassword(currentInput))
            {
                SavePasswordVerification();
                ShowSuccess();
                Invoke(nameof(LoadNextScene), 2f);
            }
            else
            {
                attemptCount++;
                int remaining = maxAttempts - attemptCount;

                if (remaining > 0)
                {
                    statusText.text = $"INCORRECT PASSWORD! {remaining} attempt(s) remaining";
                    statusText.color = Color.red;
                    ClearInput();
                }
                else
                {
                    statusText.text = "ACCESS DENIED! Too many failed attempts.";
                    statusText.color = Color.red;
                    DisableKeyboard();
                }
            }
        }
        else
        {
            statusText.text = $"Password must be exactly {REQUIRED_PASSWORD_LENGTH} characters!";
            statusText.color = Color.red;
        }
    }

    private bool ValidatePassword(string input)
    {
        return input == correctPassword;
    }

    private void DisableKeyboard()
    {
        if (keyboardPanel != null)
            keyboardPanel.SetActive(false);

        Invoke(nameof(ReloadScene), 3f);
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void UpdateDisplay()
    {
        string maskedPassword = new string('*', currentInput.Length);
        displayText.text = maskedPassword;

        int remaining = REQUIRED_PASSWORD_LENGTH - currentInput.Length;
        if (remaining > 0)
        {
            statusText.text = $"Enter {remaining} more character(s)";
            statusText.color = Color.white;
        }
        else
        {
            statusText.text = "Press SUBMIT to continue";
            statusText.color = Color.green;
        }
    }

    private void SavePasswordVerification()
    {
        PlayerPrefs.SetInt(PASSWORD_KEY, 1);

        // Save fixed expiry date (optional, for persistence)
        DateTime expiryDate = new DateTime(expiryYear, expiryMonth, expiryDay, 23, 59, 59);
        PlayerPrefs.SetString(EXPIRY_DATE_KEY, expiryDate.ToString("o"));

        PlayerPrefs.Save();
    }

    private bool IsPasswordVerified()
    {
        return PlayerPrefs.GetInt(PASSWORD_KEY, 0) == 1;
    }

    //  NEW: Expiry check using FIXED DATE
    private bool IsLicenseExpired()
    {
        DateTime expiryDate = new DateTime(expiryYear, expiryMonth, expiryDay, 23, 59, 59);
        return DateTime.Now > expiryDate;
    }

    private void ShowKeyboard()
    {
        if (keyboardPanel != null)
            keyboardPanel.SetActive(true);
        if (successPanel != null)
            successPanel.SetActive(false);
    }

    private void ShowSuccess()
    {
        if (keyboardPanel != null)
            keyboardPanel.SetActive(false);
        if (successPanel != null)
            successPanel.SetActive(true);
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }

    public static void ResetPassword()
    {
        PlayerPrefs.DeleteKey(PASSWORD_KEY);
        PlayerPrefs.DeleteKey(EXPIRY_DATE_KEY);
        PlayerPrefs.Save();
    }
}