﻿using MongoDB.Bson;
using MongoDB.Driver;
using Lab11.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Lab11.Controllers
{
    public class MemberController : ApiController
    {
        [Route("api/member")]
        [HttpPost]
        public AddMemberResponse Post(AddMemberRequest request)
        {

            var response = new AddMemberResponse();
            MongoClient client = new MongoClient("mongodb://localhost:27017");
            MongoDatabaseBase db = client.GetDatabase("ntut") as MongoDatabaseBase;
            var colMembers = db.GetCollection<MembersDocument>("members");

            var query = Builders<MembersDocument>.Filter.Eq(e => e.uid, request.uid);
            var doc = colMembers.Find(query).ToListAsync().Result.FirstOrDefault();

            if (doc == null)
            {
                colMembers.InsertOne(new MembersDocument()
                {
                    _id = ObjectId.GenerateNewId(),
                    uid = request.uid,
                    name = request.name,
                    phone = request.phone
                });
            }
            else
            {
                response.ok = false;
                response.errMsg = "編號為" + request.uid + "的會員已存在，請重新輸入別組會員編號。";
            }
            return response;
        }

        [Route("api/member")]
        [HttpPut]
        public EditMemberResponse Put(EditMemberRequest request)
        {
            var response = new EditMemberResponse();
            MongoClient client = new MongoClient("mongodb://localhost:27017");

            MongoDatabaseBase db = client.GetDatabase("ntut") as MongoDatabaseBase;
            var colMembers = db.GetCollection<MembersDocument>("members");

            var query = Builders<MembersDocument>.Filter.Eq(e => e.uid, request.uid);
            var doc = colMembers.Find(query).ToListAsync().Result.FirstOrDefault();

            if (doc != null)
            {
                var update = Builders<MembersDocument>.Update
                .Set("name", request.name)
                .Set("phone", request.phone);
                colMembers.UpdateOne(query, update);
            }
            else
            {
                response.ok = false;
                response.errMsg = "編號為" + request.uid + "的會員不存在，請確認會員編號。";
            }
            return response;
        }

        [Route("api/member/{id}")]
        [HttpDelete]
        public DeleteMemberResponse Delete(string id)
        {
            var response = new DeleteMemberResponse();
            MongoClient client = new MongoClient("mongodb://localhost:27017");
            MongoDatabaseBase db = client.GetDatabase("ntut") as MongoDatabaseBase;
            var colMembers = db.GetCollection<MembersDocument>("members");

            var query = Builders<MembersDocument>.Filter.Eq(e => e.uid, id);
            var result = colMembers.DeleteOne(query);

            if (result.DeletedCount != 0)
            {
                return response;
            }
            else
            {
                response.ok = false;
                response.errMsg = "編號為" + id + "的會員不存在，請確認會員編號。";
                return response;
            }
        }
        
        [Route("api/member")]
        [HttpGet]
        public GetMemberListResponse Get()
        {
            var response = new GetMemberListResponse();
            MongoClient client = new MongoClient("mongodb://localhost:27017");
            MongoDatabaseBase db = client.GetDatabase("ntut") as MongoDatabaseBase;
            var colMembers = db.GetCollection<MembersDocument>("members");
 
            var query = new BsonDocument();
            var cursor = colMembers.Find(query).ToListAsync().Result;

            foreach (var doc in cursor)
            {
                response.list.Add(new MemberInfo()
                {
                    uid = doc.uid,
                    name = doc.name,
                    phone = doc.phone
                });
            }
            return response;
        }
        
        [Route("api/member/{id}")]
        [HttpGet]
        public GetMemberResponse Get(string id)
        {
            var response = new GetMemberResponse();
            MongoClient client = new MongoClient("mongodb://localhost:27017");
            MongoDatabaseBase db = client.GetDatabase("ntut") as MongoDatabaseBase;
            var colMembers = db.GetCollection<MembersDocument>("members");
            
            var query = Builders<MembersDocument>.Filter.Eq(e => e.uid, id);
            var doc = colMembers.Find(query).ToListAsync().Result.FirstOrDefault();
            
            if (doc != null)
            {
                response.data.uid = doc.uid;
                response.data.name = doc.name;
                response.data.phone = doc.phone;
            }
            else
            {
                response.ok = false;
                response.errMsg = "沒有此會員";
            }
            return response;
        }
        
        [Route("")]
        [HttpGet]
        public HttpResponseMessage GetHealth()
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.StatusCode = HttpStatusCode.OK;
            response.Content = new StringContent($"{System.Configuration.ConfigurationManager.AppSettings["EnvInfo"]} Web API is running");

            return response;
        }
    }
}