
using MongoDB.Bson;
using MongoDB.Driver;
using System;
namespace CH9
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var dbNtut = client.GetDatabase("ntut") as MongoDatabaseBase;
            var dbTaiwan = client.GetDatabase("taiwan") as MongoDatabaseBase;
            var colCustomers = dbNtut.GetCollection<CustomersDocument>("customers");
            var colPeople = dbTaiwan.GetCollection<PeopleDocument>("people");

            controlPanel();
            #region 控制介面
            void controlPanel()
            {
                Console.WriteLine("--------------------------------");
                Console.WriteLine("1.計算來自台北市各個分區之消費者的總人數與平均年齡");
                Console.WriteLine("2.計算107年全台灣的出生與死亡人數、結婚與離婚人數");
                Console.WriteLine("請輸入編號1~2，選擇要執行的範例");
                try
                {
                    var num = int.Parse(Console.ReadLine()); 
                    Console.Clear(); 
                                     
                    switch (num)
                    {
                        case 1:
                            countTaipeiPeopleAndAvgAge();
                            break;
                        case 2:
                            countTaiwanBirthDeathMarryDivorce();
                            break;
                        default:
                            Console.WriteLine("請輸入正確編號"); 
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("請輸入正確編號"); 
                }
                finally
                {
                    controlPanel(); 
                }
            }
            #endregion
            #region 1.計算來自台北市各個分區之消費者的總人數與平均年齡
            void countTaipeiPeopleAndAvgAge()
            {
                Console.WriteLine("1.計算來自台北市各個分區之消費者的總人數與平均年齡\n");
                
                string map = @"
                    function() {
                        emit(this.district, { count: 1, age: this.age });
                    }
                ";
                
                string reduce = @"
                    function(key, values) {
                        var reduced = {count:0, age:0};
                        for(var idx=0 ; idx<values.length ; idx++)
                        {
                            var val = values[idx];
                            reduced.age += val.age;
                            reduced.count+= val.count;
                        }
                        return reduced;
                    }
                ";
                
                string finalize = @"
                    function(key, reduced) {
                        reduced.avgAge = reduced.age / reduced.count;
                        return reduced;
                    }
                ";
                
                var builderCustomersFilter = Builders<CustomersDocument>.Filter;
                var filter = builderCustomersFilter.Eq(e => e.city, "台北市");
                
                var options = new MapReduceOptions<CustomersDocument, BsonDocument>
                {
                    Filter = filter,
                    Finalize = finalize,
                    OutputOptions = MapReduceOutputOptions.Inline
                };
                
                var result = colCustomers.MapReduce(map, reduce, options).ToListAsync().Result;
                
                foreach (var data in result)
                {
                    Console.WriteLine(data.ToJson());
                }
            }
            #endregion
            #region 2.計算107年全台灣的出生與死亡人數、結婚與離婚人數
            void countTaiwanBirthDeathMarryDivorce()
            {
                Console.WriteLine("2.計算107年全台灣的出生與死亡人數、結婚與離婚人數\n");
                
                var pipeline = new BsonDocument[]
                {
                    new BsonDocument
                    {
                        {
                            "$group", new BsonDocument
                            {
                                
                                {"_id", "result-9-2"},
                                {
                                    
                                    "birth", new BsonDocument
                                    {
                                        {"$sum", "$birth_total"}
                                    }
                                },
                                {
                                    
                                    "death", new BsonDocument
                                    {
                                        {"$sum", "$death_total"}
                                    }
                                },
                                {
                                    
                                    "marry", new BsonDocument
                                    {
                                        {"$sum", "$marry_pair"}
                                    }
                                },
                                {
                                    
                                    "divorce", new BsonDocument
                                    {
                                        {"$sum", "$divorce_pair"}
                                    }
                                }
                            }
                        }
                    }
                };
                
                var result = colPeople.Aggregate<BsonDocument>(pipeline).ToListAsync().Result[0];
                
                Console.WriteLine($"出生人數:{result["birth"]}");
                Console.WriteLine($"死亡人數:{result["death"]}");
                Console.WriteLine($"結婚對數:{result["marry"]}");
                Console.WriteLine($"離婚對數:{result["divorce"]}");
            }
            #endregion
        }
    }
}