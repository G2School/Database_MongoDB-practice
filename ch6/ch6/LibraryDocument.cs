using MongoDB.Bson;

namespace ch6
{
    class LibraryDocument
    {
        public string _id { get; set; }
        public string book { get; set; }
        public float price { get; set; }
        public string[] authors { get; set; }
        public Borrower borrower { get; set; }
        public class Borrower
        {
            public string name { get; set; }
            public BsonDateTime timestamp { get; set; }
        }
    }
}
