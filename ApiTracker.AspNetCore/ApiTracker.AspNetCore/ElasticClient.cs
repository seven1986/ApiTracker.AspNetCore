using Elasticsearch.Net;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ApiTracker
{
    /// <summary>
    /// ElasticClient
    /// </summary>
    internal class ElasticClient
    {
        private ElasticLowLevelClient client { get; set; }

        private string IndexName { get; set; }

        private int Timeout { get; set; }

        public ElasticClient(string _url,string _indexName, int _timeout = 500)
        {
            var clientConnection = new ConnectionConfiguration(new Uri(_url));

            client = new ElasticLowLevelClient(clientConnection);

            if (string.IsNullOrWhiteSpace(_indexName))
            {
                IndexName = "apitracker";
            }
            else { IndexName = _indexName; }
            if (_timeout < 500)
            {
                Timeout = 500;
            }
            else { Timeout = _timeout; }

            IndexName = IndexName.ToLower() + "-" + DateTime.UtcNow.ToString("yyyyMMdd");
        }

        /// <summary>
        /// 索引文档
        /// </summary>
        public void Index(string document)
        {
            //记录日志异步操作时间
            var tokenSource = new CancellationTokenSource();

            Task.Run(new Action(() =>
            {
                if (tokenSource.Token.IsCancellationRequested) { return; }

                // 记录日志的超时时间
                var timeout = new TimeSpan(Timeout * 10000);

                var result = client.Index<object>(IndexName,
                    "apitracker",
                    new PostData<Object>(document), (pms) =>
                    {
                        pms.Timeout(timeout);
                        return pms;
                    });

            }), tokenSource.Token);

            tokenSource.CancelAfter(1000);
        }
    }
}

