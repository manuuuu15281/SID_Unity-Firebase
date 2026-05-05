using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using UnityEngine;

public class LabelUsername : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private TMP_Text _label;

    private void Reset()
    {
        _label = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        SetUsername();
    }

    private void SetUsername()
    {
        FirebaseUser currentUser = FirebaseAuth.DefaultInstance.CurrentUser;

        if (currentUser == null)
        {
            _label.text = "Usuario: no autenticado";
            return;
        }

        string userId = currentUser.UserId;

        FirebaseDatabase.DefaultInstance
            .GetReference("users")
            .Child(userId)
            .Child("username")
            .GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("La consulta del username fue cancelada.");
                    _label.text = "Usuario: error";
                }
                else if (task.IsFaulted)
                {
                    Debug.LogError("Error al consultar username: " + task.Exception);
                    _label.text = "Usuario: error";
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;

                    if (snapshot.Exists && snapshot.Value != null)
                    {
                        _label.text = "Usuario: " + snapshot.Value.ToString();
                    }
                    else
                    {
                        _label.text = "Usuario: sin nombre";
                    }
                }
            });
    }
}