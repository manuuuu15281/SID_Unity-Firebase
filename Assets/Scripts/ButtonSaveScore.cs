using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSaveScore : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button _saveScoreButton;

    [Header("Game Reference")]
    [SerializeField] private ClickGameManager _clickGameManager;

    private void Reset()
    {
        _saveScoreButton = GetComponent<Button>();
        _clickGameManager = FindObjectOfType<ClickGameManager>();
    }

    private void Start()
    {
        _saveScoreButton.onClick.AddListener(HandleSaveScoreButtonClicked);
    }

    private void HandleSaveScoreButtonClicked()
    {
        FirebaseUser currentUser = FirebaseAuth.DefaultInstance.CurrentUser;

        if (currentUser == null)
        {
            Debug.LogWarning("No hay usuario autenticado. No se puede guardar el puntaje.");
            return;
        }

        if (_clickGameManager == null)
        {
            Debug.LogError("No se encontró ClickGameManager.");
            return;
        }

        int currentScore = _clickGameManager.CurrentScore;
        string userId = currentUser.UserId;

        SaveBestScore(userId, currentScore);
    }

    private void SaveBestScore(string userId, int currentScore)
    {
        DatabaseReference scoreReference = FirebaseDatabase.DefaultInstance
            .GetReference("users")
            .Child(userId)
            .Child("score");

        scoreReference.GetValueAsync().ContinueWithOnMainThread(getTask =>
        {
            if (getTask.IsCanceled)
            {
                Debug.LogError("La consulta del puntaje anterior fue cancelada.");
                return;
            }

            if (getTask.IsFaulted)
            {
                Debug.LogError("Error al consultar puntaje anterior: " + getTask.Exception);
                return;
            }

            int previousScore = 0;

            DataSnapshot snapshot = getTask.Result;

            if (snapshot.Exists && snapshot.Value != null)
            {
                int.TryParse(snapshot.Value.ToString(), out previousScore);
            }

            if (currentScore <= previousScore)
            {
                Debug.Log("El puntaje actual no supera el mejor puntaje guardado. Mejor puntaje: " + previousScore);
                return;
            }

            scoreReference.SetValueAsync(currentScore).ContinueWithOnMainThread(saveTask =>
            {
                if (saveTask.IsCanceled)
                {
                    Debug.LogError("El guardado del puntaje fue cancelado.");
                    return;
                }

                if (saveTask.IsFaulted)
                {
                    Debug.LogError("Error al guardar puntaje: " + saveTask.Exception);
                    return;
                }

                Debug.Log("Nuevo mejor puntaje guardado correctamente: " + currentScore);
            });
        });
    }
}