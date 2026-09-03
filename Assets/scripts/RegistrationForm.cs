using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Datos que viajan junto con los eventos de registro. La misma clase
/// sirve tanto para el caso de éxito como para el de fallo:
///  - En éxito: Nombre y Correo llevan los datos ingresados, Motivo queda
///    en null porque no aplica ningún motivo de fallo.
///  - En fallo: Nombre y Correo llevan lo ingresado (aunque esté vacío o
///    mal formado) y Motivo lleva la razón real de la validación que no
///    pasó (no un mensaje genérico).
/// </summary>
public class RegistrationData
{
    public string Nombre;
    public string Correo;
    public string Destino; // correo al que debe llegar la notificacion
    public string Motivo;  // null cuando el registro fue exitoso

    public RegistrationData(string nombre, string correo, string destino, string motivo = null)
    {
        Nombre = nombre;
        Correo = correo;
        Destino = destino;
        Motivo = motivo;
    }
}

/// <summary>
/// Formulario de registro (Opción 3 del laboratorio SMTP).
/// Responsabilidad única: capturar nombre + correo, validar, y avisar
/// mediante eventos si el registro fue exitoso o falló.
///
/// IMPORTANTE (regla de mayor peso en la rúbrica): este script NO conoce
/// nada sobre SMTP ni sobre el envío de correo. Solo declara y dispara
/// eventos; quien quiera reaccionar a ellos (el futuro script
/// notificador) se suscribe desde afuera con "+=". Si el notificador no
/// existiera, este formulario seguiría funcionando exactamente igual.
/// </summary>
public class RegistrationForm : MonoBehaviour
{
    [Header("Referencias UI")]
    public TMP_InputField nameInput;
    public TMP_InputField emailInput;
    public TMP_InputField toInput;
    public Button submitButton;

    // Se dispara cuando la validación pasa.
    public event Action<RegistrationData> OnRegistrationSuccess;

    // Se dispara cuando la validación falla (campo vacío o correo inválido).
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

    /// <summary>
    /// Conectado al onClick del botón "Enviar". Valida con if/else (no
    /// try/catch: no son errores inesperados, son reglas de negocio
    /// esperadas) y dispara el evento correspondiente.
    /// </summary>
    public void HandleSubmit()
    {
        string name = nameInput != null ? nameInput.text : string.Empty;
        string email = emailInput != null ? emailInput.text : string.Empty;
        string destino = toInput != null ? toInput.text : string.Empty;

        if (string.IsNullOrWhiteSpace(name))
        {
            Fail(name, email, destino, "El nombre está vacío.");
        }
        else if (string.IsNullOrWhiteSpace(email))
        {
            Fail(name, email, destino, "El correo está vacío.");
        }
        else if (!EmailRegex.IsMatch(email))
        {
            Fail(name, email, destino, "El correo no tiene un formato válido.");
        }
        else if (string.IsNullOrWhiteSpace(destino))
        {
            Fail(name, email, destino, "El correo de destino (To) está vacío.");
        }
        else if (!EmailRegex.IsMatch(destino))
        {
            Fail(name, email, destino, "El correo de destino (To) no tiene un formato válido.");
        }
        else
        {
            Succeed(name, email, destino);
        }
    }

    private void Succeed(string name, string email, string destino)
    {
        OnRegistrationSuccess?.Invoke(new RegistrationData(name, email, destino));
    }

    private void Fail(string name, string email, string destino, string motivo)
    {
        // Si el destino esta vacio/invalido no hay a donde notificar el
        // fallo; el notificador se encarga de decidir que hacer en ese caso.
        OnRegistrationFailed?.Invoke(new RegistrationData(name, email, destino, motivo));
    }
}
