using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;

namespace RabbitMQ_Sender
{
    class Program
    {
        static void Main(string[] args)
        {
            Person person = new Person()
            {
                ID = 1,
                Name = "Hakkı",
                SurName = "Demirel",
                BirthDate = new DateTime(2020, 6, 6),
                Message = "Merhaba RabbitMq"
            };

            //ConnectionFactory: RabbitMQ hostuna bağlanmak için kullanılır.
            //      Bulunulan sunucudaki host name(localhost),virtual host ve credentials(password) girilir.
            //CreateModel() methodu ile RabbitMQ üzerinde yeni bir channel yaratılır
            //     .İşte bu açılan channel yani session ile yeni bir queue oluşturulup istenen mesaj bu channel üzerinden gönderilmektedir.
            //QueueDeclare() methodu ile oluşturulacak olan queue‘nin ismi tanımlanır. “durable” ile in-memory mi yoksa fiziksel olarak mı saklanacağı belirlenir.
            //    Genel de RabbitMQ’da hız amcı ile ilgili queuelerin memory’de saklanması tercih edilse de sunucunun restart olması durumunda ilgili
            //    mesajların kaybolmasından dolayı da, hızdan ödün verilerek fiziksel olarak bir hard diskte saklanması tercih edilebilir. “exclusive” parametresi
            //    ile diğer connectionlar ile kullanılması izni belirlenir. Eğer queue deleted olarak işaretlenmiş ise ve tüm consumerlar bunu kullanmayı bitirmiş
            //    ise ya da son consumer iptal edilmiş veya channel kapanmış ise silinmez. İşte bu gibi durumlarda “autoDelete” ile delete method’u normal olarak çalıştırılır.
            //    Ve ilgili queueler silinir. “arguments” Belirlenen exchanges ile alakalı parametrelerdir.Exchangeler birazdan inceleyeceğiz.

            //İlgili doldurulan “Person” sınıfı JsonConvert ile Serialize edilir ve byte[] dizisine çevrilip “body“‘e atanır.

            //BasicPublish() methodu “exchange” aslında mesajın alınıp bir veya daha fazla queue’ya konmasını sağlar.
            //    Bu route algoritması exchange tipine ve bindinglere göre farklılık gösterir. “Direct, Fanout , Topic ve Headers” tiplerinde exchangeler mevcuttur.
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "DenemeKuyruk",
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

                string message = JsonConvert.SerializeObject(person);
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                    routingKey: "DenemeKuyruk",
                    basicProperties: null,
                    body: body);

                Console.WriteLine($"Gönderilen Kişi: {person.Name} - {person.SurName}");
            }

            Console.WriteLine("İlgili kişi gönderildi");
            Console.ReadKey();
        }
    }

    public class Person
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string SurName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Message { get; set; }
    }
}
