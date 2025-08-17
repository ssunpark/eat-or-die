using TMPro;
using UnityEngine;

public class UI_Authentication : MonoBehaviour
{
    [SerializeField] private AnimatePopup _loginPanel;
    [SerializeField] private AnimatePopup _registerPanel;
    
    [SerializeField] private TMP_InputField _loginEmailInputField;
    [SerializeField] private TMP_InputField _loginPasswordInputField;
    [SerializeField] private TextMeshProUGUI _loginFeedbackText;
    
    [SerializeField] private TMP_InputField _registerEmailInputField;
    [SerializeField] private TMP_InputField _registerPasswordInputField;
    [SerializeField] private TextMeshProUGUI _registerFeedbackText;
    
    private TMP_InputField _emailInputField;
    private TMP_InputField _passwordInputField;
    
    [SerializeField] private TMP_InputField _passwordConfirmInputField;
    
    private TMP_Text _feedbackText;

    private void Start()
    {
        AuthenticationManager.Instance.OnAuthenticated += HandleAuthenticationResult;
        AuthenticationManager.Instance.OnLogin += () => _loginPanel.Close();
    }
    
    public void ShowLoginPanel()
    {
        _emailInputField = _loginEmailInputField;
        _passwordInputField = _loginPasswordInputField;
        _feedbackText = _loginFeedbackText;

        _registerPanel.Close();
        _loginPanel.Open();
    }
    
    public void ShowRegisterPanel()
    {
        _emailInputField = _registerEmailInputField;
        _passwordInputField = _registerPasswordInputField;
        _feedbackText = _registerFeedbackText;
        
        _loginPanel.Close();
        _registerPanel.Open();
    }
    
    public void CreateAccount()
    {
        string email = _emailInputField.text;
        string password = _passwordInputField.text;
        string passwordConfirm = _passwordConfirmInputField.text;

        AuthenticationManager.Instance.CreateAccountAsync(email, password, passwordConfirm);
    }

    public void SignIn()
    {
        string email = _emailInputField.text;
        string password = _passwordInputField.text;

        AuthenticationManager.Instance.SignInAsync(email, password);
    }

    private void HandleAuthenticationResult(string message)
    {
        _feedbackText.text = message;
    }
}