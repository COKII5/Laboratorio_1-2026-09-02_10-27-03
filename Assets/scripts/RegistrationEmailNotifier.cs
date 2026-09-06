using UnityEngine;
using TMPro;

public class RegistrationEmailNotifier : MonoBehaviour
{
    [Header("References")]
    public RegistrationForm form;
    public SimpleEmailSender emailSender;

    [Header("UI (optional)")]
    [Tooltip("TMP_Text where the SMTP send result is shown (success or error). Can be left empty.")]
    public TMP_Text sendResultText;

    private void OnEnable()
    {
        if (form == null) return;
        form.OnRegistrationSuccess += HandleSuccess;
        form.OnRegistrationFailed  += HandleFailed;
    }

    private void OnDisable()
    {
        if (form == null) return;
        form.OnRegistrationSuccess -= HandleSuccess;
        form.OnRegistrationFailed  -= HandleFailed;
    }

    private void HandleSuccess(RegistrationData data)
    {
        string subject = $"Registro completado — {data.Name}";

        string body =
            $"Registro exitoso.\n\n" +
            $"Nombre: {data.Name}\n" +
            $"Correo ingresado: {data.Email}\n" +
            $"Fecha y hora del registro: {System.DateTime.Now:yyyy-MM-dd HH:mm:ss}\n";

        Send(data.Destination, subject, body);
    }

    private void HandleFailed(RegistrationData data)
    {
        string subject = $"Registro rechazado: {data.Reason}";

        string body =
            $"El registro fue rechazado.\n\n" +
            $"Nombre ingresado: {data.Name}\n" +
            $"Correo ingresado: {data.Email}\n" +
            $"Motivo de la validación que falló: {data.Reason}\n" +
            $"Fecha y hora del intento: {System.DateTime.Now:yyyy-MM-dd HH:mm:ss}\n";

        if (string.IsNullOrWhiteSpace(data.Destination))
        {
            if (sendResultText != null)
            {
                sendResultText.text = "No se envió: falta un correo de destino (To) válido.";
            }
            Debug.Log("[RegistrationEmailNotifier] Fallo sin destino valido, no se intenta el envio.");
            return;
        }

        Send(data.Destination, subject, body);
    }

    private void Send(string toEmail, string subject, string body)
    {
        if (emailSender == null)
        {
            Debug.LogError("[RegistrationEmailNotifier] Falta asignar emailSender en el Inspector.");
            return;
        }

        bool ok = emailSender.SendEmail(toEmail, subject, body, out string resultMessage);
        if (sendResultText != null)
        {
            sendResultText.text = ok ? "Enviado" : "No enviado";
        }
        Debug.Log($"[RegistrationEmailNotifier] {(ok ? "Enviado" : "No enviado")}: {resultMessage}");
    }
}