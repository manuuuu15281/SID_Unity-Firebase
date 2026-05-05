using Firebase.Auth;
using System;
using UnityEngine;

public class AuthStateHandler : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject _panelAuth;
    [SerializeField] private GameObject _panelHome;

    [Header("Game")]
    [SerializeField] private ClickGameManager _clickGameManager;

    private void Reset()
    {
        _panelAuth = GameObject.Find("PanelAuth");
        _panelHome = GameObject.Find("PanelHome");
        _clickGameManager = FindObjectOfType<ClickGameManager>();
    }

    private void Start()
    {
        FirebaseAuth.DefaultInstance.StateChanged += AuthStateChanged;
        UpdateUIImmediate();
    }

    private void OnDestroy()
    {
        if (FirebaseAuth.DefaultInstance != null)
        {
            FirebaseAuth.DefaultInstance.StateChanged -= AuthStateChanged;
        }
    }

    private void AuthStateChanged(object sender, EventArgs e)
    {
        CancelInvoke(nameof(ShowHomeDelayed));

        if (FirebaseAuth.DefaultInstance.CurrentUser != null)
        {
            Invoke(nameof(ShowHomeDelayed), 2f);
        }
        else
        {
            ShowAuth();
        }
    }

    private void UpdateUIImmediate()
    {
        if (FirebaseAuth.DefaultInstance.CurrentUser != null)
        {
            ShowHomeDelayed();
        }
        else
        {
            ShowAuth();
        }
    }

    private void ShowHomeDelayed()
    {
        if (_clickGameManager != null)
        {
            _clickGameManager.ResetScore();
        }

        if (_panelAuth != null)
        {
            _panelAuth.SetActive(false);
        }

        if (_panelHome != null)
        {
            _panelHome.SetActive(true);
        }

        if (FirebaseAuth.DefaultInstance.CurrentUser != null)
        {
            Debug.Log("Usuario autenticado: " + FirebaseAuth.DefaultInstance.CurrentUser.Email);
        }
    }

    private void ShowAuth()
    {
        if (_clickGameManager != null)
        {
            _clickGameManager.ResetScore();
        }

        if (_panelAuth != null)
        {
            _panelAuth.SetActive(true);
        }

        if (_panelHome != null)
        {
            _panelHome.SetActive(false);
        }

        Debug.Log("No hay usuario autenticado.");
    }
}