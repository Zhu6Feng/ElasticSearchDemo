using Nest;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ElasticSearchDemo
{
    /// <summary>
    /// 连接到ElasticSearch引擎服务器
    /// </summary>
    public static class Setting
    {
        public static string strConnectionString = @"http://localhost:9200";
        public static Uri Node
        {
            get
            {
                return new Uri(strConnectionString);
            }
        }
        public static ConnectionSettings ConnectionSettings
        {
            get
            {
                return new ConnectionSettings(Node).DefaultIndex("default");
            }
        }
    }
    /// <summary>
    /// 创建数据模型
    /// </summary>
    public class WebModel
    {
        public string id { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public string content { get; set; }
        public int sortid { get; set; }
    }

    /// <summary>
    /// 更新文档
    /// </summary>
    public class ESProvider
    {
        public static ElasticClient client = new ElasticClient(Setting.ConnectionSettings);
        public static string strIndexName = @"dtcweb".ToLower();
        public static bool PopulateIndex(WebModel webmodel, string strDocType)
        {
            var index = client.Index(webmodel, i => i.Index(strIndexName).Type(strDocType.ToLower()).Id(webmodel.id));
            return index.Created;
        }

        public static bool BulkPopulateIndex(List<WebModel> posts, string strDocType)
        {
            var bulkRequest = new BulkRequest(strIndexName, strDocType.ToLower()) { Operations = new List<IBulkOperation>() };
            var idxops = posts.Select(o => new BulkIndexOperation<WebModel>(o) { Id = o.id }).Cast<IBulkOperation>().ToList();
            bulkRequest.Operations = idxops;
            var response = client.Bulk(bulkRequest);
            return response.IsValid;
        }

        public static bool DeleteIndex(string strType, string strID)
        {
            DeleteRequest deleteRequest = new DeleteRequest(strIndexName, strType, strID);
            var response = client.Delete(deleteRequest);
            return response.IsValid;
        }
    }

    /// <summary>
    /// 搜索文档
    /// </summary>
    public class ESSearch
    {
        public static ElasticClient client = new ElasticClient(Setting.ConnectionSettings);
        public static string strIndexName = @"dtcweb".ToLower();
        /// <summary>
        /// 词条查询
        /// </summary>
        /// <param name="strType">类型</param>
        /// <param name="strQuery">关键词</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns></returns>
        public static List<WebModel> GetResult_TermQuery(string strType, string strQuery, bool isAsc)
        {
            //create term query
            TermQuery tq = new TermQuery();
            tq.Field = "content";
            tq.Value = strQuery;

            //create search request
            SearchRequest sr = new SearchRequest(strIndexName, strType);
            sr.Query = tq;

            //windows
            sr.From = 0;
            sr.Size = 100;

            //sort
            ISort sort = new SortField { Field = "sortid", Order = isAsc ? Nest.SortOrder.Ascending : Nest.SortOrder.Descending };
            sr.Sort = new List<ISort>();
            sr.Sort.Add(sort);

            //source filter
            sr.Source = new SourceFilter()
            {
                Includes = new string[] { "title", "url", "sortid" },
                Excludes = new string[] { "content" }
            };

            var result = client.Search<WebModel>(sr);
            return result.Documents.ToList<WebModel>();
        }

        /// <summary>
        /// 匹配查询
        /// </summary>
        /// <param name="strType">类型</param>
        /// <param name="strQuery">关键词</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns></returns>
        public static List<WebModel> GetResult_MatchQuery(string strType, string strQuery, bool isAsc)
        {
            SearchRequest sr = new SearchRequest(strIndexName, strType);
            MatchQuery mq = new MatchQuery();
            mq.Field = new Field("content");
            mq.Query = strQuery;
            mq.MinimumShouldMatch = 2;
            mq.Operator = Operator.Or;

            sr.Query = mq;
            sr.From = 0;
            sr.Size = 100;
            sr.Sort = new List<ISort>();
            sr.Sort.Add(new SortField { Field = "sortid", Order = isAsc?Nest.SortOrder.Ascending : Nest.SortOrder.Descending });
            sr.Source = new SourceFilter()
            {
                Includes = new string[] { "title", "url", "sortid" },
                Excludes = new string[] { "content" }
            };

            ISearchResponse<WebModel> result = client.Search<WebModel>(sr);

            return result.Documents.ToList<WebModel>();
        }

        //正则表达式查询
        public static List<WebModel> GetResult_RegexpQuery(string strType, string strQuery, bool isAsc)
        {
            SearchRequest sr = new SearchRequest(strIndexName, strType);

            RegexpQuery rq = new RegexpQuery();
            rq.Field = "content";
            rq.Value = strQuery + ".*";
            rq.MaximumDeterminizedStates = 20000;

            sr.Query = rq;
            sr.From = 0;
            sr.Size = 100;
            sr.Sort = new List<ISort>();
            sr.Sort.Add(new SortField { Field = "sortid", Order = isAsc ? Nest.SortOrder.Ascending : Nest.SortOrder.Descending });
            sr.Source = new SourceFilter()
            {
                Includes = new string[] { "title", "url", "sortid" },
                Excludes = new string[] { "content" }
            };

            var result = client.Search<WebModel>(sr);
            return result.Documents.ToList<WebModel>();
        }
    }
}
