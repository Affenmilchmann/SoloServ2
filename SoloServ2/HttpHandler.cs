using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;


namespace SoloServ2
{
    static class HttpHandler
    {
        public static async Task Get(HttpListenerResponse resp, string data)
        {
            byte[] tempData = Encoding.ASCII.GetBytes(data);

            resp.ContentType = "text/html";
            resp.ContentEncoding = Encoding.ASCII;
            resp.ContentLength64 = tempData.LongLength;

            await resp.OutputStream.WriteAsync(tempData, 0, tempData.Length);
            resp.Close();
        }

        public static void Post(HttpListenerRequest req, List<string> ToDo)
        {
            byte[] t = new byte[Convert.ToInt32(req.ContentLength64)];
            req.InputStream.Read(t, 0, Convert.ToInt32(req.ContentLength64));
            string temp, name;

            temp = Encoding.ASCII.GetString(t);

            string[] tempArr = temp.Replace("+", " ").Split('=');

            if (tempArr.Length != 1) temp = tempArr[1];

            name = tempArr[0];

            if (temp != "") ToDo.Add(temp);

            int k;
            if (int.TryParse(name, out k)) ToDo.Remove(ToDo[k]);
        }
    }
}
