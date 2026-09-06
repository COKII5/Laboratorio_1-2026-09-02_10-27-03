using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RegistrationData
{
    public string Name;
    public string Email;
    public string Destination;
    public string Reason;

    public RegistrationData(string name, string email, string destination, string reason = null)
    {
        Name = name;
        Email = email;
        Destination = destination;
        Reason = reason;
    }
}

public class RegistrationForm : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField nameInput;
    public TMP_InputField emailInput;
    public TMP_InputField toInput;
    public Button submitButton;

    public event Action<RegistrationData> OnRegistrationSuccess;
    public event Action<RegistrationData> OnRegistrationFailed;

    private static readonly Regex EmailRegex = new Regex(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    private void Awake()
    {
        if (submitButton != null)
        {
            submitButton.onClick.AddListener(HandleSubmit);
        }
    }

    public void HandleSubmit()
    {
        string name        = nameInput  != null ? nameInput.text  : string.Empty;
        string email       = emailInput != null ? emailInput.text : string.Empty;
        string destination = toInput    != null ? toInput.text    : string.Empty;

        if (string.IsNullOrWhiteSpace(name))
        {
            Fail(name, email, destination, "El nombre está vacío.");
        }
        else if (string.IsNullOrWhiteSpace(email))
        {
            Fail(name, email, destination, "El correo está vacío.");
        }
        else if (!EmailRegex.IsMatch(email))
        {
            Fail(name, email, destination, "El correo no tiene un formato válido.");
        }
        else if (string.IsNullOrWhiteSpace(destination))
        {
            Fail(name, email, destination, "El correo de destino (To) está vacío.");
        }
        else if (!EmailRegex.IsMatch(destination))
        {
            Fail(name, email, destination, "El correo de destino (To) no tiene un formato válido.");
        }
        else
        {
            Succeed(name, email, destination);
        }
    }

    private void Succeed(string name, string email, string destination)
    {
        OnRegistrationSuccess?.Invoke(new RegistrationData(name, email, destination));
    }

    private void Fail(string name, string email, string destination, string reason)
    {
        OnRegistrationFailed?.Invoke(new RegistrationData(name, email, destination, reason));
    }
}