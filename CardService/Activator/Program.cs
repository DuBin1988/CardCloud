using Card;
using Com.Aote.Logs;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Card
{
    class Program
    {
        private static ILog Log = LogManager.GetLogger(typeof(Program));
       
        //调明华动态库
        [DllImport("Mwic_32.dll", EntryPoint = "srdCard_ver", CallingConvention = CallingConvention.StdCall)]
        public static extern int srdCard_ver(Int16 len, byte[] data_buffer);


        [DllImport("Mwic_32.dll", EntryPoint = "swrCard_ver", CallingConvention = CallingConvention.StdCall)]
        public static extern int swrCard_ver(Int16 offset, Int16 len, byte[] data_buffer);


        [DllImport("Mwic_32.dll", EntryPoint = "swr_4442", CallingConvention = CallingConvention.StdCall)]
        public static extern int swr_4442(Int16 icdev, Int16 offset, Int16 len, byte[] data_buffer);

        [DllImport("Mwic_32.dll", EntryPoint = "csc_4442", CallingConvention = CallingConvention.StdCall)]
        public static extern int csc_4442(Int16 icdev, Int16 len, byte[] data_buffer);

        [DllImport("Mwic_32.dll", EntryPoint = "get_kmm", CallingConvention = CallingConvention.StdCall)]
        public static extern int get_kmm(byte[] data_buffer);


        [DllImport("Mwic_32.dll", EntryPoint = "get_Date", CallingConvention = CallingConvention.StdCall)]
        public static extern int get_Date(byte[] data_buffer);

        //把Hex转换成byte[]                 
        public static byte[] HexToBytes(string hex)
        {
            hex = hex.Trim();
            byte[] bytes = new byte[hex.Length / 2];
            int index = 0;

                for (int i = 0; i < hex.Length; i = i + 2)
                {
                    string test = hex.Substring(i, 2);
                   // bytes[index] = Encoding.ASCII.GetBytes(test);
                   // bytes[index] = (byte)Convert.ToByte(test);
                    bytes[index] = byte.Parse(test, NumberStyles.HexNumber);
                    index++;
                }
            return bytes;
        }

        //转换数据类型把byte转换为HexString
        public static string ToHexString(byte[] bytes)
        {
            string byteStr = "";
            foreach (var item in bytes)
            {
                byteStr += string.Format("{0:X2}", item);
            }
            return byteStr;
        }
   
        static void Main(string[] args)
        {
           // Log.Debug("in");
          //  writeFile(args[0] +"*******"+ args[1]);
            CardInfos ci = new CardInfos();
            try
            {
                StreamReader sr = new StreamReader("card.config");
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] attr = line.Split(',');
                    String[] ccPair = attr[0].Split('=');
                    String[] icard = attr[1].Split('=');
                    CardConfig cc = new CardConfig() { Name = ccPair[1].Trim() };
                    ICard card = (ICard)Assembly.GetExecutingAssembly().CreateInstance(icard[1].Trim());
                    cc.Card = card;
                    ci.Add(cc);
                }
                sr.Close();
            }
            catch(Exception e)
            {
                String config = JsonConvert.SerializeObject(new Ret() {  Err="卡配置文件错误。"});
                Console.Write(config);
                Log.Debug(config);
                return;                
            }

            int BaudRate = int.Parse(Config.GetConfig("Baud"));
            short Port = short.Parse(Config.GetConfig("Port"));
            Log.Debug(String.Join(" ", args));
            Log.Debug("args[0]: " + args[0] + "--" + args[1]);
            GenericService service = new GenericService(ci, Port, BaudRate);
            Object obj = null;
            Object obj1 = null;

            string result = null;
            string result1 = null;
            string result2 = null;
            string result3 = null;
         
            switch(args[0])
            {
                case "ReadCard":
                   byte [] b =  HexToBytes(args[1]);
                   Log.Debug("args[1]: " + args[1]);
                   srdCard_ver(256,b);
                    obj = service.ReadCard();
                    break;
                case "WriteGasCard":
//===================================================================
                    byte[] bbbb = HexToBytes(args[19]);
                    byte[] bbb = new byte[3];
                    byte[] bb = new byte[256];
                    Log.Debug("card password!!!!!!!!!!!!!!!!!!");

                    srdCard_ver(256,bbbb);
                    Log.Debug("start read card!!!!!!!!!!!!!!!!!!!!");

                    Log.Debug("写卡开始:" + args[1]+"result"+args[19]);
                    obj1 = service.WriteGasCard(args[1], args[2], args[3], args[4], 
                        int.Parse(args[5]), int.Parse(args[6]), int.Parse(args[7]), short.Parse(args[8]),
                        int.Parse(args[9]), int.Parse(args[10]), int.Parse(args[11]), int.Parse(args[12]),
                        args[13], args[14], int.Parse(args[15]), int.Parse(args[16]), args[17], args[18],args[19]
                        );

                    get_kmm(bbb);
                    result1 = ToHexString(bbb);
                    Log.Debug("result1" + result1);
                    get_Date(bb);
                    result2 = ToHexString(bb);
                    Log.Debug("result2" + result2);
                    WriteRet ret = new WriteRet();
                    ret.Kmy = result1;
                    ret.Kdata = result2;
                    break;
                case "WriteNewCard":
                    byte[] data = HexToBytes(args[24]);
                    Log.Debug("data======" + args[24]);
                    byte[] password = new byte[3];
                    string str = "WriteNewCard";
                    srdCard_ver(256, data);
                    Log.Debug("WriteNewCard:"+str);
                    obj = service.WriteNewCard(args[1], args[2], short.Parse(args[3]), args[4], args[5], args[6],args[7],
                        int.Parse(args[8]), int.Parse(args[9]), int.Parse(args[10]), short.Parse(args[11]),
                        int.Parse(args[12]), short.Parse(args[13]), int.Parse(args[14]), int.Parse(args[15]),
                        int.Parse(args[16]), int.Parse(args[17]), args[18], args[19], int.Parse(args[20]), int.Parse(args[21]), args[22], args[23],args[24]
                        );
                    get_kmm(password);
                    result1 = Convert.ToString(password);
                    result2 = JsonConvert.SerializeObject(obj);
                    break;
                case "FormatGasCard":
                    obj = service.FormatGasCard(args[1], args[2], args[3], args[4]);
                    break;
                case "OpenCard":
                    obj = service.OpenCard(args[1], args[2], args[3], args[4]);
                    break;
                default:
                    return;
            }
            result = JsonConvert.SerializeObject(obj);

            result = result1 + result2 + result3;
            Console.Write(result);
            Log.Debug("*******************"+result+"*******************");
            return ;
        }
    }
}
