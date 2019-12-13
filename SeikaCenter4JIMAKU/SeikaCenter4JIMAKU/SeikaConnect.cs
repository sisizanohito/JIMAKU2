
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace Voice
{
    class  SeikaConnect
    {
        private static SeikaConnect _instance = new SeikaConnect();
        public static SeikaConnect Instance()
        {
            return _instance;
        }

        /// <summary>
        /// 任意のオブジェクトを JSON メッセージへシリアライズします。
        /// </summary>
        public static string Serialize(object graph,bool SimpleDictionary = false)
        {
            using (var stream = new MemoryStream())
            {
                var setting = new DataContractJsonSerializerSettings()
                {
                    UseSimpleDictionaryFormat = SimpleDictionary,
                };
                var serializer = new DataContractJsonSerializer(graph.GetType(), setting);
                serializer.WriteObject(stream, graph);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
        /// <summary>
        /// Jsonメッセージをオブジェクトへデシリアライズします。
        /// </summary>
        public static T Deserialize<T>(string message)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(message)))
            {
                var setting = new DataContractJsonSerializerSettings()
                {
                    UseSimpleDictionaryFormat = true,
                };
                var serializer = new DataContractJsonSerializer(typeof(T), setting);
                return (T)serializer.ReadObject(stream);
            }
        }

        /// <summary>
        /// 話者のリストを返す
        /// </summary>
        public static async Task<List<SeikaACTOR>> GetActor(string url)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url + "/AVATOR2")
            };

            //認証ヘッダーの追加
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
            "Basic",
            Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", "SeikaServerUser", "SeikaServerPassword"))));


            var response = await client.SendAsync(request);
            string result = await response.Content.ReadAsStringAsync();
            return Deserialize<List<SeikaACTOR>>(result);
        }

        /// <summary>
        /// 話者のパラメータを返す
        /// </summary>
        public static async Task<SeikaGETpram> Getpram(string url,int cid)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url + $"/AVATOR2/{cid}")
            };

            //認証ヘッダーの追加
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
            "Basic",
            Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", "SeikaServerUser", "SeikaServerPassword"))));


            var response = await client.SendAsync(request);
            string result = await response.Content.ReadAsStringAsync();
            return Deserialize<SeikaGETpram>(result);
        }

        /// <summary>
        /// 話す
        /// </summary>
        public static async Task<string> Talk(string url, int cid,string pram)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url + $"/PLAY2/{cid}"),
                Content = new StringContent(pram, Encoding.UTF8, "application/json")
        };

            //認証ヘッダーの追加
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
            "Basic",
            Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", "SeikaServerUser", "SeikaServerPassword"))));

            var response = await client.SendAsync(request);
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// 音声の保存
        /// </summary>
        public static async Task<Stream> SAVE(string url, int cid, string pram)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url + $"/SAVE2/{cid}"),
                Content = new StringContent(pram, Encoding.UTF8, "application/json")
            };

            //認証ヘッダーの追加
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
            "Basic",
            Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", "SeikaServerUser", "SeikaServerPassword"))));

            var response = await client.SendAsync(request);
            return await response.Content.ReadAsStreamAsync();
        }
    }
}
