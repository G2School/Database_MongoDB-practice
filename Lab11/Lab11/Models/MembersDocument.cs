using MongoDB.Bson;

namespace Lab11.Models
{
    public class MembersDocument
    {
        public ObjectId _id { get; set; }
        public string uid { get; set; }

        public string name { get; set; }

        public string phone { get; set; }
    }
}