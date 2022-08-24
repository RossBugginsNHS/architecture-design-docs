namespace dhc;

public class RabbitMqSettings
{
    public static readonly string Location = "RabbitMq";
    public string RabbitHost{get;set;}
    public string RabbitUserName {get;set;}
    public string RabbitPassword{get;set;}
    public int RabbitPort{get;set;}
}
