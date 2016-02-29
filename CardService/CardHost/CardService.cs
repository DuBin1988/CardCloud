using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Card;
using System.ServiceModel;
using Com.Aote.Logs;
using System.IO;
using System.Windows;
using System.Xaml;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Runtime.InteropServices;

namespace CardHost
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single)]
    class CardService : ICardService
    {
        
        private static Log Log = Log.GetInstance(typeof(CardService));
        private static String EXE_IN_FULL_PATH = @"Activator.exe";
        private Dictionary<String, String> map = new Dictionary<string, string>();
        public bool Spawn(String args, out string result)
        {
            StringBuilder outputBuilder;
            ProcessStartInfo processStartInfo;    //启动另外一个程序
            Process process;                      //对进程进行管理

            outputBuilder = new StringBuilder();
            processStartInfo = new ProcessStartInfo(EXE_IN_FULL_PATH, args);
            processStartInfo.CreateNoWindow = true;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardInput = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.WindowStyle = ProcessWindowStyle.Normal;

            using (process = new Process())
            {
                process.StartInfo = processStartInfo;
                process.EnableRaisingEvents = true;
                process.OutputDataReceived += new DataReceivedEventHandler
                (
                    delegate(object sender, DataReceivedEventArgs e)
                    {
                        outputBuilder.Append(e.Data);
                    }
                );
                process.Start();
                process.BeginOutputReadLine();
                bool terminated = process.WaitForExit(10*1000);
                process.CancelOutputRead();
                if (!terminated)
                {
                    process.Kill();
                    result = null;
                    return false;
                }
                else
                {
                    result = outputBuilder.ToString();
                    return true;
                }
            }
        }

        public string Test(string name)
        {
            return "Hi, Buddy.";
        }

        public CardInfo ReadCard(Stream number)
        {
            StreamReader reader = new StreamReader(number);//读取文件
            String str = reader.ReadToEnd();

            String args = "ReadCard " + str;
            Log.Debug(args + " started.");
            String result = null;
            CardInfo ci = new CardInfo();
            try
            {
                if (this.Spawn(args, out result))
                {
                    ci = JsonConvert.DeserializeObject<CardInfo>(result);
                    Log.Debug(args + "=" + result);
                }
                else
                {
                    ci.Err = "调用未返回。";
                    Log.Debug(args + "=" + ci.Err);
                }
            }
            catch (Exception e)
            {
                ci.Exception = e.Message;
                ci.Err = "调用错误。";
            }
            return ci;
        }
        public WriteRet WriteNewCard(string factory, string kmm, short kzt, string kh, string dqdm, string yhh, string tm, 
            int ql, int csql, int ccsql, short cs, int ljgql, short bkcs, int ljyql, int bjql, int czsx, int tzed, 
            string sqrq, string cssqrq, int oldprice, int newprice, string sxrq, string sxbj,string result1)
        {
            
            String args = "WriteNewCard";
            Log.Debug(args + " started.");

            args += " " + (String.IsNullOrWhiteSpace(factory) ? "0" : factory);
            args += " " + (String.IsNullOrWhiteSpace(kmm) ? "\"0\"" : "\"" + kmm + "\"");
            args += " " + kzt;
            args += " " + (String.IsNullOrWhiteSpace(kh) ? "0" : kh);
            args += " " + (String.IsNullOrWhiteSpace(dqdm) ? "0" : dqdm);
            args += " " + (String.IsNullOrWhiteSpace(yhh) ? "0" : yhh);
            args += " " + (String.IsNullOrWhiteSpace(tm) ? "0" : tm);
            args += " " + ql;
            args += " " + csql;
            args += " " + ccsql;
            args += " " + cs;
            args += " " + ljgql;
            args += " " + bkcs;
            args += " " + ljyql;
            args += " " + bjql;
            args += " " + czsx;
            args += " " + tzed;
            args += " " + (String.IsNullOrWhiteSpace(sqrq) ? "0" : sqrq);
            args += " " + (String.IsNullOrWhiteSpace(cssqrq) ? "0" : cssqrq);
            args += " " + oldprice;
            args += " " + newprice;
            args += " " + (String.IsNullOrWhiteSpace(sxrq) ? "0" : sxrq);
            args += " " + (String.IsNullOrWhiteSpace(sxbj) ? "0" : sxbj);
            args += " " + (String.IsNullOrWhiteSpace(result1) ? "0" : result1);
            String result = null;
            WriteRet ret = new WriteRet();
            try
            {
                if (this.Spawn(args, out result))
                {
                    ret = JsonConvert.DeserializeObject<WriteRet>(result);
                    Log.Debug(args + "=" + result);
                }
                else
                {
                    ret.Err = "调用未返回。";
                    Log.Debug(args + "=" + ret.Err);
                }
            }
            catch (Exception e)
            {
                ret.Exception = e.Message;
                ret.Err = "调用错误。";
            }
            return ret;
        }

        public WriteRet WriteGasCard(string factory, string kmm, string kh, string dqdm, 
            int ql, int csql, int ccsql, short cs, int ljgql, int bjql, int czsx, int tzed, 
            string sqrq, string cssqrq, int oldprice, int newprice, string sxrq, string sxbj,string result)
        {
            String args = "WriteGasCard";
            Log.Debug(args + " started.");

            args += " " + (String.IsNullOrWhiteSpace(factory) ? "0" : factory);
            args += " " + (String.IsNullOrWhiteSpace(kmm) ? "\"0\"" : "\"" + kmm + "\"");
            args += " " + (String.IsNullOrWhiteSpace(kh) ? "0" : kh);
            args += " " + (String.IsNullOrWhiteSpace(dqdm) ? "0" : dqdm);

            args += " " + ql;
            args += " " + csql;
            args += " " + ccsql;
            args += " " + cs;
            args += " " + ljgql;
            args += " " + bjql;
            args += " " + czsx;
            args += " " + tzed;

            args += " " + (String.IsNullOrWhiteSpace(sqrq) ? "0" : sqrq);
            args += " " + (String.IsNullOrWhiteSpace(cssqrq) ? "0" : cssqrq);
            args += " " + oldprice;
            args += " " + newprice;
            args += " " + (String.IsNullOrWhiteSpace(sxrq) ? "0" : sxrq);
            args += " " + (String.IsNullOrWhiteSpace(sxbj) ? "0" : sxbj);
          
            args += " " + (String.IsNullOrWhiteSpace(result) ? "0" : result);


            string results = null;
            WriteRet ret = new WriteRet();

            try
            {
                if (this.Spawn(args, out results))
                {
                    ret = JsonConvert.DeserializeObject<WriteRet>(results);
                    Log.Debug(args + "=" + results);
                }
                else
                {
                    ret.Err = "调用未返回。";
                    Log.Debug(args + "=" + ret.Err);
                }
            }
            catch (Exception e)
            {
                ret.Exception = e.Message;
                ret.Err = "调用错误。";
            }
            return ret;
        }

        public Ret FormatGasCard(string factory, string kmm, string kh, string dqdm)
        {
            String args = "FormatGasCard";
            Log.Debug(args + " started.");

            args += " " + (String.IsNullOrWhiteSpace(factory) ? "0" : factory);
            args += " " + (String.IsNullOrWhiteSpace(kmm) ? "\"0\"" : "\"" + kmm + "\"");
            args += " " + (String.IsNullOrWhiteSpace(kh) ? "0" : kh);
            args += " " + (String.IsNullOrWhiteSpace(dqdm) ? "0" : dqdm);

            String result = null;
            WriteRet ret = new WriteRet();
            try
            {
                if (this.Spawn(args, out result))
                {
                    ret = JsonConvert.DeserializeObject<WriteRet>(result);
                    Log.Debug(args + "=" + result);
                }
                else
                {
                    ret.Err = "调用未返回。";
                    Log.Debug(args + "=" + ret.Err);
                }
            }
            catch (Exception e)
            {
                ret.Exception = e.Message;
                ret.Err = "调用错误。";
            }
            return ret;
        }

        public Ret OpenCard(string factory, string kmm, string kh, string dqdm)
        {
            String args = "OpenCard";
            Log.Debug(args + " started.");

            args += " " + (String.IsNullOrWhiteSpace(factory) ? "0" : factory);
            args += " " + (String.IsNullOrWhiteSpace(kmm) ? "\"0\"" : "\"" + kmm + "\"");
            args += " " + (String.IsNullOrWhiteSpace(kh) ? "0" : kh);
            args += " " + (String.IsNullOrWhiteSpace(dqdm) ? "0" : dqdm);

            String result = null;
            WriteRet ret = new WriteRet();
            try
            {
                if (this.Spawn(args, out result))
                {
                    ret = JsonConvert.DeserializeObject<WriteRet>(result);
                    Log.Debug(args + "=" + result);
                }
                else
                {
                    ret.Err = "调用未返回。";
                    Log.Debug(args + "=" + ret.Err);
                }
            }
            catch (Exception e)
            {
                ret.Exception = e.Message;
                ret.Err = "调用错误。";
            }
            return ret;
        }
        //接受数据并保存到 Dictionary 集合中去
 //       public void Jieshounumber(String cardInformation)
 //       {
  //          StreamReader reader = new StreamReader(cardInformation);
//            String str = reader.ReadToEnd();
 //           ReadCard(str);
           
//        }
        //.........
        public  void fanhuinumber(){
            StreamReader sr = new StreamReader("c;\\Sanmple.txt");
            String    shuju = sr.ReadLine();
             
             

        }
    }
}
