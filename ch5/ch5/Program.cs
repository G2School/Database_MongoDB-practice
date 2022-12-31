using MongoDB.Driver;
using System;

namespace ch5
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var db = client.GetDatabase("ntut") as MongoDatabaseBase;
            var col = db.GetCollection<StudentDocument>("student");
            var doc = new StudentDocument("108368001", "Joe");

            col.InsertOne(doc);
            Console.WriteLine("新增一筆文件");
        }
    }
}
