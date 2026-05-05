using System.Collections;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonRegister : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button _registerButton;
    [SerializeField] private TMP_InputField _usernameInputField;
    [SerializeField] private TMP_InputField _emailInputField;
    [SerializeField] private TMP_InputField _passwordInputField;

    private Coroutine _registrationCoroutine;

    private void Reset()
    {
        _registerButton = GetComponent<Button>();

        _usernameInputField = GameObject.Find("InputFieldUsername")?.GetComponent<TMP_InputField>();
        _emailInputField = GameObject.Find("InputFieldEmail")?.GetComponent<TMP_InputField>();
        _passwordInputField = GameObject.Find("InputFieldPassword")?.GetComponent<TMP_InputField>();
    }

    private void Start()
    {
        _registerButton.onClick.AddListener(HandleRegisterButtonClicked);
    }

    private void HandleRegisterButtonClicked()
    {
        string username = _usernameInputField.text.Trim();
        string email = _emailInputField.text.Trim();
        string password = _passwordInputField.text;

        if (string.IsNullOrEmpty(username))
        {
            Debug.LogWarning("Debes ingresar un nombre de usuario.");
            return;
        }

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

        _registrationCoroutine = StartCoroutine(RegisterUser(username, email, password));
    }

    private IEnumerator RegisterUser(string username, string email, string password)
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;

        var registerTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(() => registerTask.IsCompleted);

        if (registerTask.IsCanceled)
        {
            Debug.LogError("El registro fue cancelado.");
        }
        else if (registerTask.IsFaulted)
        {
            Debug.LogError("Error al registrar usuario: " + registerTask.Exception);
        }
        else
        {
            AuthResult result = registerTask.Result;
            FirebaseUser user = result.User;

            Debug.Log("Usuario creado correctamente: " + user.Email + " / ID: " + user.UserId);

            SaveUserData(user.UserId, username, email);
        }
    }

    private void SaveUserData(string userId, string username, string email)
    {
        DatabaseReference databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

        UserData userData = new UserData(username, email, 0);

        string json = JsonUtility.ToJson(userData);

        databaseReference
            .Child("users")
            .Child(userId)
            .SetRawJsonValueAsync(json)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("El guardado de datos fue cancelado.");
                }
                else if (task.IsFaulted)
                {
                    Debug.LogError("Error al guardar datos del usuario: " + task.Exception);
                }
                else
                {
                    Debug.Log("Datos del usuario guardados correctamente en Realtime Database.");
                }
            });
    }
}

[System.Serializable]
public class UserData
{
    public string username;
    public string email;
    public int score;
    public string createdAt;

    public UserData(string username, string email, int score)
    {
        this.username = username;
        this.email = email;
        this.score = score;
        this.createdAt = System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
    }
}