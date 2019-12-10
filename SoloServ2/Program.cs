using System;
using System.IO;
using System.Linq;
using System.Text;

using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SoloServ2
{
    class Program
    {
        static string link = "http://localhost:8000/", 
            pageName = "mainpage.html", 
            dataFileName = "data.txt",
            itemFileName = "item.html";
        static Page mainPage = new Page();
        static List<string> ToDo = new List<string>();
        static StreamReader dataInput;
        static StreamWriter dataOutput;
        static void Main(string[] args)
        {
            //init
            mainPage.LoadFromFile(pageName);
            if (File.Exists(dataFileName))
            {
                dataInput = new StreamReader(dataFileName);
                while (!dataInput.EndOfStream) ToDo.Add(dataInput.ReadLine());
                dataInput.Close();
            }

            //main loop
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(link);
            listener.Start();
            Console.WriteLine("Listeting to {0}", link);

            Task listenTask = HandleRequests(listener);
            listenTask.GetAwaiter().GetResult();

            listener.Close();
        }

        public static async Task HandleRequests(HttpListener listener)
        {
            bool isRunning = true;

            while (isRunning)
            {
                HttpListenerContext ctx = await listener.GetContextAsync();

                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;

                Console.WriteLine("Request ");
                Console.WriteLine(req.Url.ToString());
                Console.WriteLine(req.HttpMethod);
                Console.WriteLine(req.UserHostName);
                Console.WriteLine(req.UserAgent);
                Console.WriteLine();

                if (req.HttpMethod == "POST")
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

                    SaveToDo();
                }
                if (req.HttpMethod == "PUT")
                {
                    byte[] t = new byte[Convert.ToInt32(req.ContentLength64)];
                    req.InputStream.Read(t, 0, Convert.ToInt32(req.ContentLength64));
                    string temp, name;

                    temp = Encoding.ASCII.GetString(t);

                    string[] tempArr = temp.Replace("+", " ").Split('=');
                }


                byte[] data = Encoding.ASCII.GetBytes(String.Format(mainPage.data, MakeHtml(ToDo)));
                resp.ContentType = "text/html";
                resp.ContentEncoding = Encoding.ASCII;
                resp.ContentLength64 = data.LongLength;

                await resp.OutputStream.WriteAsync(data, 0, data.Length);
                resp.Close();
            }
        }

        static public void SaveToDo()
        {
            dataOutput = new StreamWriter(dataFileName);
            dataOutput.Flush();
            foreach (string str in ToDo) dataOutput.WriteLine(str);
            dataOutput.Close();
        }
        static public string MakeHtml(List<string> Input)
        {
            string inputHtml, outputHtml = "";
            if (!File.Exists(itemFileName)) return "DATA_FILE_DOES_NOT_EXISTS";

            inputHtml = File.ReadAllText(itemFileName);
            for (int i = 0; i < ToDo.Count(); i++)
                outputHtml += String.Format(inputHtml, ToDo[i], Convert.ToString(i));
           
            return outputHtml;
        }
    }
}
