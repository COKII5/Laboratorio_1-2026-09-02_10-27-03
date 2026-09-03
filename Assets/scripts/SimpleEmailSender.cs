using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using UnityEngine;

/// <summary>
/// Código SMTP entregado por el profesor (uso obligatorio). Adaptado
/// únicamente para:
///  1. Recibir destinatario/asunto/cuerpo como PARÁMETROS en vez de
///     tenerlos fijos en el código, y devolver un bool + mensaje de
///     resultado (para poder mostrarlo en la UI).
///  2. Leer la clave de aplicación desde un archivo LOCAL que NO se sube
///     al repositorio, en vez de dejarla escrita en el código fuente
///     (requisito obligatorio del laboratorio).
/// La lógica de conexión SMTP en sí (host, puerto, SSL, MailMessage,
/// SmtpClient, try/catch) es la misma que la entregada.
/// </summary>
public class SimpleEmailSender : MonoBehaviour
{
    // Cuenta remitente entregada por el profesor para el laboratorio.
    private const string FromEmail = "ingmultimediausbbog@gmail.com";

    [Serializable]
    private class EmailConfig
    {
        public string appPassword;
    }

    // Archivo fuera de Assets (raíz del proyecto), para que Unity ni
    // siquiera lo importe como asset, y para poder excluirlo de git.
    private static string ConfigPath =>
        Path.Combine(Application.dataPath, "..", "email_config.json");

    private string LoadAppPassword()
    {
        string path = ConfigPath;
        if (!File.Exists(path))
        {
            throw new FileNotFoundException(
                "No se encontro email_config.json en la raiz del proyecto. " +
                "Crea el archivo con el contenido: " +
                "{\"appPassword\": \"xxxx xxxx xxxx xxxx\"} " +
                "usando la clave de aplicacion de Gmail (NO la contraseña real).",
                path);
        }

        string json = File.ReadAllText(path);
        EmailConfig config = JsonUtility.FromJson<EmailConfig>(json);
        if (config == null || string.IsNullOrEmpty(config.appPassword))
        {
            throw new InvalidOperationException(
                "email_config.json existe pero no tiene un campo 'appPassword' valido.");
        }
        return config.appPassword;
    }

    /// <summary>
    /// Envia un correo real. Devuelve true/false segun el resultado en
    /// vez de dejar que la excepcion se propague, para que quien llama
    /// pueda mostrar el resultado en la UI sin necesitar su propio
    /// try/catch. resultMessage siempre queda con un texto describiendo
    /// que paso (exito o el motivo del error).
    /// </summary>
    public bool SendEmail(string toEmail, string subject, string body, out string resultMessage)
    {
        try
        {
            string password = LoadAppPassword();

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(FromEmail);
            mail.To.Add(toEmail);
            mail.Subject = subject;
            mail.Body = body;

            SmtpClient smtp = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(FromEmail, password),
                EnableSsl = true
            };

            smtp.Send(mail);
            resultMessage = "Email sended succesfuly";
            Debug.Log("[SimpleEmailSender] " + resultMessage);
            return true;
        }
        catch (Exception ex)
        {
            resultMessage = "Error: " + ex.Message;
            Debug.Log("[SimpleEmailSender] " + resultMessage);
            return false;
        }
    }
}
