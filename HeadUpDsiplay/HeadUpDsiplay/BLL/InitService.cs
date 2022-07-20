using HeadUpDsiplay.BLL;
using HeadUpDsiplay.Model;
using Newtonsoft.Json;
using NLog;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeadUpDsiplay
{
    class InitService
    {
        //日志文件记录
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 调用API
        /// </summary>
        /// <param name="url">API地址</param>
        /// <returns>API返回结果</returns>
        public static object UseAPI<T> (string url,Method method)
        {
            object data = null;
            IRestResponse response = null;
            try
            {
                if (!string.IsNullOrEmpty(url))
                {
                    var client = new RestClient(url.Trim());
                    client.Timeout = -1;
                    var request = new RestRequest(method);
                    request.AddHeader("Content-Type", "application/json");
                    request.AddParameter("application/json", "", ParameterType.RequestBody);
                    response = client.Execute(request);
                    if (response.StatusCode != 0)
                        GlobalData.ServerStatus = true;
                    else
                        GlobalData.ServerStatus = false;
                    data = JsonConvert.DeserializeObject<T>(response.Content);
                }
            }
            catch (Exception ex)
            {
                logger.Error("调用API异常：" + ex.Message);
            }
            return data;
        }
    }
}
