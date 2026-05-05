using Firebase.Auth;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonLogin : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button _loginButton;
    [SerializeField] private TMP_InputField _emailInputField;
    [SerializeField] private TMP_InputField _passwordInputField;

    private void Reset()
    {
        _loginButton = GetComponent<Button>();

        _emailInputField = GameObject.Find("InputFieldEmail")?.GetComponent<TMP_InputField>();
        _passwordInputField = GameObject.Find("InputFieldPassword")?.GetComponent<TMP_InputField>();
    }

    private void Start()
    {
        _loginButton.onClick.AddListener(HandleLoginButtonClicked);
    }

    private void HandleLoginButtonClicked()
    {
        string email = _emailInputField.text.Trim();
        string password = _passwordInputField.text;

        if (string.IsNullOrEmpty(email))
        {
            Debug.LogWarning("Debes ingresar un correo electrónico.");
            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            Debug.LogWarning("Debes ingresar una contraseña.");
            return;
        }

        LoginUser(email, password);
    }

    private void LoginUser(string email, string password)
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("El login fue cancelado.");
                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError("Error al iniciar sesión: " + task.Exception);
                return;
            }

            AuthResult result = task.Result;

            Debug.Log("Usuario inició sesión correctamente: " + result.User.Email + " / ID: " + result.User.UserId);
        });
    }
}