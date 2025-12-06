namespace Renoza.Domain.Options
{
    public class RabbitMqOptions
    {
        public string Host { get; set; }
        public int Port { get; set; } = 5672;
        public string VirtualHost { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
