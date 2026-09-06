using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using UnityEngine;

public class SimpleEmailSender : MonoBehaviour
{
    private const string FromEmail = "ingmultimediausbbog@gmail.com";

    [Serializable]
    private class EmailConfig
    {
        public string appPassword;
    }

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

        string json   = File.ReadAllText(path);
        EmailConfig config = JsonUtility.FromJson<EmailConfig>(json);
        if (config == null || string.IsNullOrEmpty(config.appPassword))
        {
            throw new InvalidOperationException(
                "email_config.json existe pero no tiene un campo 'appPassword' valido.");
        }
        return config.appPassword;
    }

    public bool SendEmail(string toEmail, string subject, string body, out string resultMessage)
    {
        try
        {
            string password = LoadAppPassword();

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(FromEmail);
            mail.To.Add(toEmail);
            mail.Subject = subject;
            mail.Body    = body;

            SmtpClient smtp = new SmtpClient("smtp.gmail.com")
            {
                Port        = 587,
                Credentials = new NetworkCredential(FromEmail, password),
                EnableSsl   = true
            };

            smtp.Send(mail);
            resultMessage = "Correo enviado exitosamente.";
            Debug.Log("[SimpleEmailSender] " + resultMessage);
            return true;
        }
        catch (Exception ex)
        {
            resultMessage = "Error al enviar el correo: " + ex.Message;
            Debug.LogError("[SimpleEmailSender] " + resultMessage);
            return false;
        }
    }
}