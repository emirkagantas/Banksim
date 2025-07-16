using System;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using BankSim.NotificationService;
using BankSim.Application.DTOs;

namespace BankSim.NotificationService
{
    
    public class Program
    {
        public static void Main()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "transaction.mail",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            Console.WriteLine("[🔔] NotificationService dinlemede...");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                var message = JsonSerializer.Deserialize<EmailMessageDto>(json);

                Console.WriteLine($"[📩] Mail gönderiliyor: {message?.To}");

              
                using var smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new System.Net.NetworkCredential("e.kagantas06@gmail.com", "qhgonkgojinpmtcp"),
                    EnableSsl = true
                };

                var mail = new MailMessage("e.kagantas06@gmail.com", message!.To)
                {
                    Subject = message.Subject,
                    Body = message.Body
                };

                smtp.Send(mail);
                Console.WriteLine("[✅] Mail gönderildi.");
            };

            channel.BasicConsume(queue: "transaction.mail",
                                 autoAck: true,
                                 consumer: consumer);

            Console.WriteLine("Çıkmak için Enter'a basın...");
            Console.ReadLine();
        }
    }
}
