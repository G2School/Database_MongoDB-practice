using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;

namespace ch6
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var db = client.GetDatabase("ntut") as MongoDatabaseBase;
            var colLibrary = db.GetCollection<LibraryDocument>("library");
            var builderLibraryFilter = Builders<LibraryDocument>.Filter;
            var builderLibraryProjection = Builders<LibraryDocument>.Projection;
            var builderLibrarySort = Builders<LibraryDocument>.Sort;

            controlPanel();
            #region 控制介面
            void controlPanel()
            {
                Console.WriteLine("---------------------------------");
                Console.WriteLine("1.查詢特定作者的所有書籍");
                Console.WriteLine("2.查詢王小明在特定日期借閱的書籍");
                Console.WriteLine("3.查詢未被借閱的書籍");
                Console.WriteLine("4.查詢特定價格以上的書籍");
                Console.WriteLine("5.查詢書名包含特定關鍵字的書籍，並以價格低至高排序");
                Console.WriteLine("\n請輸入編號1~5，選擇要執行的功能");
                try
                {
                    var num = int.Parse(Console.ReadLine());
                    Console.Clear();

                    switch(num)
                    {
                        case 1:
                            findAuthor();
                            break;
                        case 2:
                            findBorrow();
                            break;
                        case 3:
                            findNoBorrow();
                            break;
                        case 4:
                            findPrice();
                            break;
                        case 5:
                            findKeyword();
                            break;
                        default:
                            Console.WriteLine("\n請輸入正確內容");
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("\n請輸入正確內容");
                }
                finally
                {
                    controlPanel();
                }
            }
            #endregion

            #region findAuthor
            void findAuthor()
            {
                Console.WriteLine("1.查詢特定作者的所有書籍\n");
                Console.WriteLine("請輸入作者名稱");

                var author = Console.ReadLine();
                var filter = builderLibraryFilter.AnyIn( e => e.authors, new string[] { author } );
                var result = colLibrary.Find(filter).ToListAsync().Result;

                if( result.Count == 0 )
                {
                    Console.WriteLine("\n查無資料");
                    Console.WriteLine("\n查尋結果");
                    foreach (LibraryDocument doc in result)
                    {
                        Console.WriteLine(doc.book);
                    }
                }
                else
                {
                    /*Console.WriteLine("\n查尋結果");
                    foreach (LibraryDocument doc in result)
                    {
                        Console.WriteLine(doc.book);
                    }*/
                }
            }
            #endregion

            #region findBorrow
            void findBorrow()
            {
                Console.WriteLine("2.查詢王小明在特定日期借閱的書籍\n");

                Console.WriteLine("請輸入月份");
                var month = int.Parse( Console.ReadLine() );

                Console.WriteLine("請輸入日期:");
                var day = int.Parse(Console.ReadLine());

                var nameFilter = builderLibraryFilter.Eq( e => e.borrower.name, "王小明" );
                var timeUpperFilter = builderLibraryFilter.Gte(e => e.borrower.timestamp, new DateTime( 2015, month, day, 0, 0, 0 ));
                var timeLowerFilter = builderLibraryFilter.Gte(e => e.borrower.timestamp, new DateTime(2015, month, day, 23, 59, 59));
                var filter = builderLibraryFilter.And(nameFilter, timeUpperFilter, timeLowerFilter);
                var result = colLibrary.Find( filter ).ToListAsync().Result;

                if (result.Count == 0)
                {
                    Console.WriteLine("\n查無資料");
                }
                else
                {
                    Console.WriteLine("\n查詢結果");
                    foreach (LibraryDocument doc in result)
                    {
                        Console.WriteLine( $"王小明借了{ doc.book }");
                    }
                }
            }
            #endregion

            #region findNoBorrow
            void findNoBorrow()
            {
                Console.WriteLine("3.查詢未被借閱的書籍\n");

                var filter = builderLibraryFilter.Exists( e => e.borrower, false );
                var result = colLibrary.Find(filter).ToListAsync().Result;

                if (result.Count == 0)
                {
                    Console.WriteLine("\n查無資料");
                }
                else
                {
                    Console.WriteLine("\n查尋結果");
                    foreach (LibraryDocument doc in result)
                    {
                        Console.WriteLine(doc.book);
                    }
                }
            }
            #endregion

            #region findPrice
            void findPrice()
            {
                Console.WriteLine("4.查詢特定價格以上的書籍\n");

                Console.WriteLine("請輸入價格");
                var price = int.Parse(Console.ReadLine());

                var filter = builderLibraryFilter.Where( e => e.price >= price );
                var result = colLibrary.Find(filter).ToListAsync().Result;

                if (result.Count == 0)
                {
                    Console.WriteLine("\n查無資料");
                }
                else
                {
                    Console.WriteLine("\n查詢結果");
                    foreach (LibraryDocument doc in result)
                    {
                        Console.WriteLine( doc.book );
                    }
                }
            }
            #endregion

            #region findKeyword
            void findKeyword()
            {
                Console.WriteLine("5.查詢書名包含特定關鍵字的書籍，並以價格低至高排序\n");

                Console.WriteLine("請輸入關鍵字");
                var keyword = Console.ReadLine();

                Console.WriteLine("請輸入日期:");
                var pattern = new BsonRegularExpression(keyword, "i");

                var filter = builderLibraryFilter.Regex( e => e.book, pattern );
                var projection = builderLibraryProjection.Include(e => e.book).Include(e => e.price);
                var sort = builderLibrarySort.Ascending(e => e.price);
                var result = colLibrary.Find(filter).Project(projection).Sort(sort).ToListAsync().Result;
                if (result.Count == 0)
                {
                    Console.WriteLine("\n查無資料");
                }
                else
                {
                    Console.WriteLine("\n查詢結果");
                    foreach (BsonDocument bsonDoc in result)
                    {
                        var doc = BsonSerializer.Deserialize<LibraryDocument>(bsonDoc);
                        Console.WriteLine($"書名: { doc.book }, 價格: {doc.price}");
                    }
                }
            }
            #endregion
        }
    }
}
