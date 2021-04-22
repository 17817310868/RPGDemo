/*
 * ===================================================================
 * 
 *          projectName:
 *              项目名称:拱猪
 *                  
 *          title:
 *              标题:数据库系统
 *          
 *          description:
 *              功能描述:管理数据库中的数据，对数据库的增删查改进行封装
 *              
 *          author:
 *              作者:
 * 
 * ===================================================================
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Server
{
    public class DBSys
    {
        private static DBSys instance;  //数据库单例
        private MongoClient mongoClient;  //数据库客户端
        private IMongoDatabase database;  //游戏数据库

        /// <summary>
        /// 数据库单例
        /// 用于封装对数据库的操作
        /// </summary>
        public static DBSys Instance {
            get
            {
                if (instance == null)
                {
                    instance = new DBSys();
                }
                return instance;
            }
        }

        private DBSys()  //初始化游戏数据库
        {
            mongoClient = new MongoClient();
            database = mongoClient.GetDatabase("Game");
            
        }

        #region 获取数据对应的集合
        /// <summary>
        /// 获取数据对应的所属集合
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="data">数据</param>
        /// <returns></returns>
        //IMongoCollection<T> GetCollection<T>(T data) where T : Data
        //{
        //    IMongoCollection<T> collection = null;
        //    //若集合为空，判断是否等于传进来的数据类型，若等于，则赋值为该数据类型对应的表
        //    collection = collection ?? (typeof(T) == typeof(PlayerData) ? database.GetCollection<T>("PlayerData") : null);
        //    collection = collection ?? (typeof(T) == typeof(MainRoleData) ? database.GetCollection<T>("MainRoleData") : null);
        //    collection = collection ?? (typeof(T) == typeof(ItemInfoData) ? database.GetCollection<T>("ItemInfoData") : null);
        //    collection = collection ?? (typeof(T) == typeof(EquipInfoData) ? database.GetCollection<T>("EqiupInfoData") : null);
        //    collection = collection ?? (typeof(T) == typeof(SkillData) ? database.GetCollection<T>("SkillData") : null);
        //    collection = collection ?? (typeof(T) == typeof(TaskInfoData) ? database.GetCollection<T>("TaskInfoData") : null);
        //    collection = collection ?? (typeof(T) == typeof(MailData) ? database.GetCollection<T>("MailData") : null);
        //    return collection;

        //}
        #endregion
        #region 获取对应的数据

        /// <summary>
        /// 获取满足对应条件的第一条数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="firstFilterKey">第一个条件的键</param>
        /// <param name="firstFilterValue">第一个条件的值</param>
        /// <returns></returns>
        public T GetData<T>(string firstFilterKey,string firstFilterValue)where T:Data
        {
            IMongoCollection<T> collection = database.GetCollection<T>(typeof(T).Name);
            var builder = Builders<T>.Filter;
            var filter = builder.Eq(firstFilterKey, firstFilterValue);
            if (collection.Find(filter).FirstOrDefault() == null)
                return default(T);
            T result = collection.Find(filter).FirstOrDefault();
            return result;
        }

        /// <summary>
        /// 获取满足对应条件的第一条数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="firstFilterKey">第一个条件的键</param>
        /// <param name="firstFilterValue">第一个条件的值</param>
        /// <param name="secondFilterKey">第二个条件的键</param>
        /// <param name="secondFilterValue">第二个条件的值</param>
        /// <returns></returns>
        public T GetData<T>(string firstFilterKey, string firstFilterValue,string secondFilterKey, string secondFilterValue) where T : Data
        {
            IMongoCollection<T> collection = database.GetCollection<T>(typeof(T).Name);

            var builder = Builders<T>.Filter;
            var filter = builder.Eq(firstFilterKey, firstFilterValue) & builder.Eq(secondFilterKey, secondFilterValue);
            if (collection.Find(filter).FirstOrDefault() == null)
                return default(T);
            T result = collection.Find(filter).FirstOrDefault();
            return result;
        }

        /// <summary>
        /// 获取满足对应条件的第一条数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="firstFilterKey">第一个条件的键</param>
        /// <param name="firstFilterValue">第一个条件的值</param>
        /// <param name="secondFilterKey">第二个条件的键</param>
        /// <param name="secondFilterValue">第二个条件的值</param>
        /// <param name="thirdFilterKey">第三个条件的键</param>
        /// <param name="thirdFilterValue">第三个条件的值</param>
        /// <returns></returns>
        public T GetData<T>(string firstFilterKey, string firstFilterValue, string secondFilterKey, string secondFilterValue, string thirdFilterKey, string thirdFilterValue) where T : Data
        {
            IMongoCollection<T> collection = database.GetCollection<T>(typeof(T).Name);
            var builder = Builders<T>.Filter;
            var filter = builder.Eq(firstFilterKey, firstFilterValue) & builder.Eq(secondFilterKey, secondFilterValue) & builder.Eq(thirdFilterKey, thirdFilterValue);
            if (collection.Find(filter).FirstOrDefault() == null)
                return default(T);
            T result = collection.Find(filter).FirstOrDefault();
            return result;
        }

        /// <summary>
        /// 获取该类型的所有数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> GetAllDatas<T>() where T : Data
        {
            IMongoCollection<T> collection = database.GetCollection<T>(typeof(T).Name);
            if (collection.Find(new BsonDocument()).ToList<T>() == null)
                return null;
            List<T> result = collection.Find(new BsonDocument()).ToList<T>();
            return result;
        }

        /// <summary>
        /// 获取满足对应条件的所有数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="firstFilterKey">第一个条件的键</param>
        /// <param name="firstFilterValue">第一个条件的值</param>
        /// <returns></returns>
        public List<T> GetAllDatas<T>(string firstFilterKey, string firstFilterValue) where T : Data
        {
            IMongoCollection<T> collection = database.GetCollection<T>(typeof(T).Name);
            var builder = Builders<T>.Filter;
            var filter = builder.Eq(firstFilterKey, firstFilterValue);
            if (collection.Find(filter).ToList() == null)
                return null;
            List<T> result = collection.Find(filter).ToList();
            return result;
        }

        /// <summary>
        /// 获取满足对应条件的所有数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="firstFilterKey">第一个条件的键</param>
        /// <param name="firstFilterValue">第一个条件的值</param>
        /// <param name="secondFilterKey">第二个条件的键</param>
        /// <param name="secondFilterValue">第二个条件的值</param>
        /// <returns></returns>
        public List<T> GetAllDatas<T>(string firstFilterKey, string firstFilterValue, string secondFilterKey, string secondFilterValue) where T : Data
        {
            IMongoCollection<T> collection = database.GetCollection<T>(typeof(T).Name);
            var builder = Builders<T>.Filter;
            var filter = builder.Eq(firstFilterKey, firstFilterValue) & builder.Eq(secondFilterKey, secondFilterValue);
            if (collection.Find(filter).ToList() == null)
                return null;
            List<T> result = collection.Find(filter).ToList();
            return result;
        }

        /// <summary>
        /// 获取满足对应条件的所有数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="firstFilterKey">第一个条件的键</param>
        /// <param name="firstFilterValue">第一个条件的值</param>
        /// <param name="secondFilterKey">第二个条件的键</param>
        /// <param name="secondFilterValue">第二个条件的值</param>
        /// <param name="thirdFilterKey">第三个条件的键</param>
        /// <param name="thirdFilterValue">第三个条件的值</param>
        /// <returns></returns>
        public List<T> GetAllDatas<T>(string firstFilterKey, string firstFilterValue, string secondFilterKey, string secondFilterValue, string thirdFilterKey, string thirdFilterValue) where T : Data
        {
            IMongoCollection<T> collection = database.GetCollection<T>(typeof(T).Name);
            var builder = Builders<T>.Filter;
            var filter = builder.Eq(firstFilterKey, firstFilterValue) & builder.Eq(secondFilterKey, secondFilterValue) & builder.Eq(thirdFilterKey, thirdFilterValue);
            if (collection.Find(filter).ToList() == null)
                return null;
            List<T> result = collection.Find(filter).ToList();
            return result;
        }
        #endregion  获取对应数据的数量
        #region 删除对应的数据

        /// <summary>
        /// 删除满足条件的第一条数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="filterKey">对应的键</param>
        /// <param name="filterValue">对应的值</param>
        public void DeleteData<T>(string filterKey,string filterValue)where T:Data
        {
            IMongoCollection<T> collection = database.GetCollection<T>(typeof(T).Name);
            var builder = Builders<T>.Filter;
            var filter = builder.Eq(filterKey, filterValue);
            collection.DeleteOne(filter);
        }

        /// <summary>
        /// 删除满足条件的第一条数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">对应的键</param>
        /// <param name="value">对应的值</param>
        public void DeleteData<T>(string filterKey, int filterValue) where T : Data
        {
            IMongoCollection<T> collection = database.GetCollection<T>(typeof(T).Name);
            var builder = Builders<T>.Filter;
            var filter = builder.Eq(filterKey, filterValue);
            collection.DeleteOne(filter);
        }

        /// <summary>
        /// 删除满足条件的所有数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="filterKey">对应的键</param>
        /// <param name="filterValue">对应的值</param>
        public void DeleteAllDatas<T>(string filterKey, string filterValue) where T : Data
        {
            IMongoCollection<T> collection = database.GetCollection<T>(typeof(T).Name);
            var builder = Builders<T>.Filter;
            var filter = builder.Eq(filterKey, filterValue);
            collection.DeleteMany(filter);
        }

        /// <summary>
        /// 删除满足条件的所有数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="filterKey">对应的键</param>
        /// <param name="filterValue">对应的值</param>
        public void DeleteAllDatas<T>(string filterKey, int filterValue) where T : Data
        {
            IMongoCollection<T> collection = database.GetCollection<T>(typeof(T).Name);
            var builder = Builders<T>.Filter;
            var filter = builder.Eq(filterKey, filterValue);
            collection.DeleteMany(filter);
        }

        /// <summary>
        /// 删除所有数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void DeleteAllDatas<T>() where T: Data
        {
            IMongoCollection<T> collection = database.GetCollection<T>(typeof(T).Name);
            collection.DeleteMany(new BsonDocument());
        }

        #endregion
        #region  增加数据

        /// <summary>
        /// 增加数据进入对应的集合中
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="data">数据</param>
        public void InsertData<T>(T data)where T : Data
        {
            IMongoCollection<T> collection = database.GetCollection<T>(typeof(T).Name);
            collection.InsertOne(data);
        }

        /// <summary>
        /// 增加多个数据进入对应的集合中
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="data">数据数组</param>
        public void InsertData<T>(List<T> data) where T : Data
        {
            IMongoCollection<T> collection = database.GetCollection<T>(typeof(T).Name);
            collection.InsertMany(data);
        }
        #endregion
        #region  修改对应数据的信息

        /// <summary>
        /// 修改对应的第一条数据的对应信息
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="filterKey">条件的键</param>
        /// <param name="filterValue">条件的值</param>
        /// <param name="updataKey">要修改的键</param>
        /// <param name="updataValue">修改后的值</param>
        public void UpdateData<T>(string filterKey,string filterValue,string updataKey,string updataValue)where T : Data
        {
            IMongoCollection<T> collection = database.GetCollection<T>(typeof(T).Name);
            var builder = Builders<T>.Filter;
            var filter = builder.Eq(filterKey, filterValue);
            var updata = Builders<T>.Update.Set(updataKey, updataValue);
            collection.UpdateOne(filter, updata);
        }

        /// <summary>
        /// 修改对应的第一条数据的对应信息
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="filterKey">条件的键</param>
        /// <param name="filterValue">条件的值</param>
        /// <param name="updataKey">要修改的键</param>
        /// <param name="updataValue">修改后的值</param>
        public void UpdateData<T>(string filterKey, string filterValue, string updataKey, int updataValue) where T : Data
        {
            IMongoCollection<T> collection = database.GetCollection<T>(typeof(T).Name);
            var builder = Builders<T>.Filter;
            var filter = builder.Eq(filterKey, filterValue);
            var updata = Builders<T>.Update.Set(updataKey, updataValue);
            collection.UpdateOne(filter, updata);
        }

        /// <summary>
        /// 修改对应的所有数据的对应信息
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="filterKey">条件的键</param>
        /// <param name="filterValue">条件的值</param>
        /// <param name="updataKey">要修改的键</param>
        /// <param name="updataValue">修改后的值</param>
        public void UpdateAllDatas<T>(string filterKey, string filterValue, string updataKey, int updataValue) where T : Data
        {
            IMongoCollection<T> collection = database.GetCollection<T>(typeof(T).Name);
            var builder = Builders<T>.Filter;
            var filter = builder.Eq(filterKey, filterValue);
            var updata = Builders<T>.Update.Set(updataKey, updataValue);
            collection.UpdateMany(filter, updata);
        }

        /// <summary>
        /// 修改对应的所有数据的对应信息
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="filterKey">条件的键</param>
        /// <param name="filterValue">条件的值</param>
        /// <param name="updataKey">要修改的键</param>
        /// <param name="updataValue">修改后的值</param>
        public void UpdateAllDatas<T>(string filterKey, string filterValue, string updataKey, string updataValue) where T : Data
        {
            IMongoCollection<T> collection = database.GetCollection<T>(typeof(T).Name);
            var builder = Builders<T>.Filter;
            var filter = builder.Eq(filterKey, filterValue);
            var updata = Builders<T>.Update.Set(updataKey, updataValue);
            collection.UpdateMany(filter, updata);
        }
        #endregion
    }
}
