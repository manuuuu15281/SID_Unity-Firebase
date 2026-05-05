using System.Collections.Generic;
using Firebase.Database;
using TMPro;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    [Header("Leaderboard Rows")]
    [SerializeField] private TMP_Text[] _leaderboardRows = new TMP_Text[5];

    [Header("Settings")]
    [SerializeField] private int _maxResults = 5;

    private Query _leaderboardQuery;

    private void Reset()
    {
        _leaderboardRows = new TMP_Text[5];

        for (int i = 0; i < _leaderboardRows.Length; i++)
        {
            Transform rowTransform = transform.Find("LeaderboardRow" + (i + 1));

            if (rowTransform != null)
            {
                _leaderboardRows[i] = rowTransform.GetComponent<TMP_Text>();
            }
        }
    }

    private void OnEnable()
    {
        ClearRows();
        StartListeningLeaderboard();
    }

    private void OnDisable()
    {
        StopListeningLeaderboard();
    }

    private void StartListeningLeaderboard()
    {
        _leaderboardQuery = FirebaseDatabase.DefaultInstance
            .GetReference("users")
            .OrderByChild("score")
            .LimitToLast(_maxResults);

        _leaderboardQuery.ValueChanged += HandleLeaderboardChanged;

        Debug.Log("Escuchando cambios en la tabla de puntajes.");
    }

    private void StopListeningLeaderboard()
    {
        if (_leaderboardQuery != null)
        {
            _leaderboardQuery.ValueChanged -= HandleLeaderboardChanged;
            _leaderboardQuery = null;
        }
    }

    private void HandleLeaderboardChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError("Error al leer leaderboard: " + args.DatabaseError.Message);
            return;
        }

        List<LeaderboardEntry> entries = new List<LeaderboardEntry>();

        foreach (DataSnapshot userSnapshot in args.Snapshot.Children)
        {
            string username = "Usuario";
            int score = 0;

            DataSnapshot usernameSnapshot = userSnapshot.Child("username");
            DataSnapshot scoreSnapshot = userSnapshot.Child("score");

            if (usernameSnapshot.Exists && usernameSnapshot.Value != null)
            {
                username = usernameSnapshot.Value.ToString();
            }

            if (scoreSnapshot.Exists && scoreSnapshot.Value != null)
            {
                int.TryParse(scoreSnapshot.Value.ToString(), out score);
            }

            entries.Add(new LeaderboardEntry(username, score));
        }

        entries.Sort((a, b) => b.score.CompareTo(a.score));

        UpdateLeaderboardUI(entries);
    }

    private void UpdateLeaderboardUI(List<LeaderboardEntry> entries)
    {
        ClearRows();

        for (int i = 0; i < _leaderboardRows.Length; i++)
        {
            if (_leaderboardRows[i] == null)
            {
                continue;
            }

            if (i < entries.Count)
            {
                int position = i + 1;
                _leaderboardRows[i].text = position + ". " + entries[i].username + " - " + entries[i].score + " pts";
            }
            else
            {
                _leaderboardRows[i].text = (i + 1) + ". ---";
            }
        }
    }

    private void ClearRows()
    {
        for (int i = 0; i < _leaderboardRows.Length; i++)
        {
            if (_leaderboardRows[i] != null)
            {
                _leaderboardRows[i].text = (i + 1) + ". ---";
            }
        }
    }
}

public class LeaderboardEntry
{
    public string username;
    public int score;

    public LeaderboardEntry(string username, int score)
    {
        this.username = username;
        this.score = score;
    }
}