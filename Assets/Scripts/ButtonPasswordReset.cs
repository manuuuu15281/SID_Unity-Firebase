using Firebase.Auth;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonPasswordReset : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button _passwordResetButton;
    [SerializeField] private TMP_InputField _emailInputField;

    private void Reset()
    {
        _passwordResetButton = GetComponent<Button>();
        _emailInputField = GameObject.Find("InputFieldEmail")?.GetComponent<TMP_InputField>();
    }

    private void Start()
    {
        _passwordResetButton.onClick.AddListener(HandlePasswordResetButtonClicked);
    }

    private void HandlePasswordResetButtonClicked()
    {
        string email = _emailInputField.text.Trim();

        if (string.IsNullOrEmpty(email))
        {
            Debug.LogWarning("Debes ingresar un correo electrónico para recuperar la contraseña.");
            return;
        }

        SendPasswordResetEmail(email);
    }

    private void SendPasswordResetEmail(string email)
    {
        FirebaseAuth.DefaultInstance
            .SendPasswordResetEmailAsync(email)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("El envío del correo de recuperación fue cancelado.");
                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.LogError("Error al enviar correo de recuperación: " + task.Exception);
                    return;
                }

                Debug.Log("Correo de recuperación enviado correctamente a: " + email);
            });
    }
}