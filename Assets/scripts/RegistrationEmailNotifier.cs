using UnityEngine;
using TMPro;

/// <summary>
/// Único script que conoce tanto RegistrationForm como SimpleEmailSender.
/// El formulario nunca llama directamente al envío de correo: este
/// notificador se suscribe a sus eventos (OnRegistrationSuccess /
/// OnRegistrationFailed) y decide qué asunto/cuerpo construir y cuándo
/// enviar. Toda suscripción se hace en OnEnable y se deshace en
/// OnDisable, para no dejar referencias colgando si este objeto se
/// desactiva o destruye mientras el formulario sigue vivo.
/// </summary>
public class RegistrationEmailNotifier : MonoBehaviour
{
    [Header("Referencias")]
    public RegistrationForm form;
    public SimpleEmailSender emailSender;

    [Header("UI (opcional)")]
    [Tooltip("TMP_Text donde se muestra el resultado del envío SMTP (éxito o error). Puede quedar vacío.")]
    public TMP_Text sendResultText;

    private void OnEnable()
    {
        if (form == null) return;
        form.OnRegistrationSuccess += HandleSuccess;
        form.OnRegistrationFailed += HandleFailed;
    }

    private void OnDisable()
    {
        if (form == null) return;
        form.OnRegistrationSuccess -= HandleSuccess;
        form.OnRegistrationFailed -= HandleFailed;
    }

    private void HandleSuccess(RegistrationData data)
    {
        string subject = "Registro completado";
        string body =
            $"Registro exitoso.\n\n" +
            $"Nombre: {data.Nombre}\n" +
            $"Correo ingresado: {data.Correo}\n";

        Send(data.Destino, subject, body);
    }

    private void HandleFailed(RegistrationData data)
    {
        string subject = "Registro rechazado: formato inválido";
        string body =
            $"El registro fue rechazado.\n\n" +
            $"Nombre ingresado: {data.Nombre}\n" +
            $"Correo ingresado: {data.Correo}\n" +
            $"Motivo de la validación que falló: {data.Motivo}\n";

        // Si el propio campo "To" fue el que fallo la validacion (vacio o
        // con formato invalido), no hay a donde enviar la notificacion:
        // se deja constancia solo en la UI/consola, sin intentar el envio.
        if (string.IsNullOrWhiteSpace(data.Destino))
        {
            if (sendResultText != null)
            {
                sendResultText.text = "No se envio: falta un correo de destino (To) valido.";
            }
            Debug.Log("[RegistrationEmailNotifier] Fallo sin destino valido, no se intenta el envio.");
            return;
        }

        Send(data.Destino, subject, body);
    }

    private void Send(string toEmail, string subject, string body)
    {
        if (emailSender == null)
        {
            Debug.Log("[RegistrationEmailNotifier] Falta asignar emailSender.");
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
