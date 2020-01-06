using ConfigServer;
using LoggerHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    partial class hsServer
    { 
        public static void ShowDebug(string msg)
        {
            Logger.Info(msg);
        }
    }
    partial class hsServer
    {
        public static string stringify(object jsonObject)
        {
            using (var ms = new MemoryStream())
            {
                new DataContractJsonSerializer(jsonObject.GetType()).WriteObject(ms, jsonObject);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
        
    }

    partial class hsServer
    {
        public static async Task<string> Get_TCP_UTF8_string_Async(string ip, int port, int delay, string instruct)
        {
            if (delay > 0)
            {
                await Task.Delay(delay);
            }
            string RecvErrorMessageString = "";
            Task<string> t = Task.Run<string>(() =>
            {
                //Thread.Sleep(delay * 1000);
                Socket EquipmentConfigurationPage_Socket = null;
                try
                {
                    List<byte> data = new List<byte>();
                    byte[] buffer = new byte[hsConfig.buffSize];
                    int length = 0;
                    byte[] currentRcv = null;
                    //EquipmentConfigurationPage_Socket.Send(Encoding.UTF8.GetBytes(jsonString));
                    IPAddress ServerSocketIP;
                    ServerSocketIP = IPAddress.Parse(ip);
                    EquipmentConfigurationPage_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    EquipmentConfigurationPage_Socket.ReceiveTimeout = hsConfig.serverReceiveTimeOut;
                    EquipmentConfigurationPage_Socket.SendTimeout = hsConfig.serverSendTimeOut;
                    EquipmentConfigurationPage_Socket.Connect(new IPEndPoint(ServerSocketIP, port)); //配置服务器IP与端口
                    EquipmentConfigurationPage_Socket.Send(Encoding.UTF8.GetBytes(instruct));

                    while ((length = EquipmentConfigurationPage_Socket.Receive(buffer)) > 0)
                    {
                        currentRcv = null;
                        if (length < buffer.Length)  //最后一个小于buff.length时跳出循环 //&&EquipmentConfigurationPage_Socket.Available==0
                        {
                            byte[] lastBytes = new byte[length];
                            Buffer.BlockCopy(buffer, 0, lastBytes, 0, length);
                            data.AddRange(lastBytes);   //提高性能       
                            currentRcv = lastBytes;
                        }
                        else if (length == buffer.Length)
                        {
                            data.AddRange(buffer);
                            currentRcv = buffer;
                        }
                        if (EquipmentConfigurationPage_Socket.Available == 0)  //此只针对单个包的完成接收处理. 若本报已接收完且下一包未收到的情况下,此时Available==0.若下一包已接收的化此时Available!=0.
                        {
                            string temp = Encoding.UTF8.GetString(currentRcv);
                            //hsServer.Logger.CWshow(instruct+"--最后一个包的字符串为：" + temp);
                            if (temp.Length >= 3)
                            {
                                char c1 = temp[temp.Length - 1];  //'}'  =125
                                //char c2 = temp[temp.Length - 2];  //'\n'  =10
                                char c3 = temp[temp.Length - 3]; //'}'  =125
                                if ((c1 == '}' && c3 == '}') | (c1 == 'k' && c3 == '-'))  //或者有返回hscmd-svr-kvmunit-setswi-ok  //若极小概率为最后一个包只有“}"则可能误判
                                {
                                    break;
                                }
                            }
                        }
                        buffer = new byte[hsConfig.buffSize];

                    }
                    if (data.Count > 0)
                    {
                        RecvErrorMessageString = Encoding.UTF8.GetString(data.ToArray(), 0, data.Count);
                    }
                    EquipmentConfigurationPage_Socket.Close();
                    Logger.CWshow("TCP连接成功：" + ip + ":" + port + instruct + "返回字符串长度：" + RecvErrorMessageString.Length);
                }
                catch (Exception ex)
                {
                    if (EquipmentConfigurationPage_Socket != null)
                    {
                        EquipmentConfigurationPage_Socket.Close();
                    }
                    Logger.CWshow("TCP连接失败：" + ip + ":" + port + instruct + "\r\n" + ex.Message);
                    //throw;  //不抛出异常 但有在调试记录显示
                    //MessageBox.Show("连接失败：" + ip + ":" + port + "\n" + instruct);
                }
                return RecvErrorMessageString;  //需要有返回值
            });
            if (t == await Task.WhenAny(t, Task.Delay(hsConfig.serverConnectTimeOut)))
            {
                RecvErrorMessageString = await t;
            }
            else
            {
                Logger.CWshow("网络连接超时：" + ip + ":" + port + "--" + instruct);
            }

            return RecvErrorMessageString;  //返回异步结果
            //await t;
            //return t.Result;  //返回异步结果
        }
        public static async Task<byte[]> Get_TCP_string_UTF8_Async(string ip, int port, int delay, string instruct)
        {
            if (delay > 0)
            {
                await Task.Delay(delay);
            }
            List<byte> data1 = new List<byte>();
            Task<List<byte>> t = Task.Run<List<byte>>(() =>
            {
                //Thread.Sleep(delay * 1000);
                List<byte> data = new List<byte>();
                Socket EquipmentConfigurationPage_Socket = null;
                try
                {
                    byte[] buffer = new byte[hsConfig.buffSize];
                    int length = 0;
                    byte[] currentRcv = null;
                    //EquipmentConfigurationPage_Socket.Send(Encoding.UTF8.GetBytes(jsonString));
                    IPAddress ServerSocketIP;
                    ServerSocketIP = IPAddress.Parse(ip);
                    EquipmentConfigurationPage_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    EquipmentConfigurationPage_Socket.Connect(new IPEndPoint(ServerSocketIP, port)); //配置服务器IP与端口
                    EquipmentConfigurationPage_Socket.ReceiveTimeout = hsConfig.serverReceiveTimeOut;
                    EquipmentConfigurationPage_Socket.Send(Encoding.UTF8.GetBytes(instruct));
                    //int len = EquipmentConfigurationPage_Socket.Receive(buffer);
                    //uint ulen=ByteTouint

                    while ((length = EquipmentConfigurationPage_Socket.Receive(buffer)) > 0) //Socket.ReceiveBufferSize  //默认值为 8192。 
                    {
                        currentRcv = null;
                        if (length < buffer.Length)  //最后一个小于buff.length时跳出循环 //&&EquipmentConfigurationPage_Socket.Available==0
                        {
                            byte[] lastBytes = new byte[length];
                            Buffer.BlockCopy(buffer, 0, lastBytes, 0, length);
                            data.AddRange(lastBytes);   //提高性能
                            currentRcv = lastBytes;
                        }
                        else if (length == buffer.Length)
                        {
                            data.AddRange(buffer);
                            currentRcv = buffer;
                        }
                        if (EquipmentConfigurationPage_Socket.Available == 0)  //此只针对单个包的完成接收处理. 若本报已接收完且下一包未收到的情况下,此时Available==0.若下一包已接收的化此时Available!=0.
                        {
                            string temp = Encoding.UTF8.GetString(currentRcv);
                            //hsServer.Logger.CWshow(instruct + "--最后一个包的字符串为：" + temp);
                            if (temp.Length >= 3)
                            {
                                char c1 = temp[temp.Length - 1];  //'}'  =125
                                //char c2 = temp[temp.Length - 2];  //'\n'  =10
                                char c3 = temp[temp.Length - 3]; //'}'  =125
                                if ((c1 == '}' && c3 == '}') | (c1 == 'k' && c3 == '-'))  //或者有返回hscmd-svr-kvmunit-setswi-ok
                                {
                                    break;
                                }
                            }
                        }
                        buffer = new byte[hsConfig.buffSize];
                    }
                    //string rcvStr = "-----ok-----";
                    //if (data.Count > 0)
                    //{
                    //    rcvStr = Encoding.UTF8.GetString(data.ToArray(), 0, data.Count);
                    //    hsServer.Logger.CWshow("查询Rx.V结果："+rcvStr);
                    //}
                    EquipmentConfigurationPage_Socket.Close();
                    //Logger.CWshow("TCP连接成功：" + ip + ":" + port + instruct + "\r\n返回byte数：" + data.Count);
                    //Logger.CWshow("TCP连接成功：" + ip + ":" + port + instruct + "\r\n返回byte数：" + rcvStr);
                }
                catch (Exception ex)
                {
                    if (EquipmentConfigurationPage_Socket != null)
                    {
                        EquipmentConfigurationPage_Socket.Close();
                    }
                    Logger.CWshow("TCP连接失败：" + ip + ":" + port + instruct + "\r\n" + ex.Message);
                    //throw;  //不抛出异常 但有在调试记录显示
                    //MessageBox.Show("连接失败：" + ip + ":" + port + "\n" + instruct);
                }
                return data;   //需要有返回值
            });
            if (t == await Task.WhenAny(t, Task.Delay(hsConfig.serverConnectTimeOut)))
            {
                data1 = await t;
            }
            else
            {
                Logger.CWshow("网络连接超时：" + ip + ":" + port + "--" + instruct);
            }
            return data1.ToArray();
            //await t;
            //return t.Result;  //返回异步结果
        }



        public static async Task<byte[]> Get_TCP_string_Log_UTF8_Async(string ip, int port, int delay, string instruct)
        {
            if (delay > 0)
            {
                await Task.Delay(delay);
            }
            List<byte> data1 = new List<byte>();
            Task<List<byte>> t = Task.Run<List<byte>>(() =>
            {
                //Thread.Sleep(delay * 1000);
                List<byte> data = new List<byte>();
                Socket EquipmentConfigurationPage_Socket = null;
                try
                {
                    byte[] buffer = new byte[hsConfig.buffSize];
                    int length = 0;
                    byte[] currentRcv = null;
                    //EquipmentConfigurationPage_Socket.Send(Encoding.UTF8.GetBytes(jsonString));
                    IPAddress ServerSocketIP;
                    ServerSocketIP = IPAddress.Parse(ip);
                    EquipmentConfigurationPage_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    EquipmentConfigurationPage_Socket.Connect(new IPEndPoint(ServerSocketIP, port)); //配置服务器IP与端口
                    //EquipmentConfigurationPage_Socket.ReceiveTimeout = hsConfig.serverReceiveTimeOut;
                    EquipmentConfigurationPage_Socket.Send(Encoding.UTF8.GetBytes(instruct));
                    //int len = EquipmentConfigurationPage_Socket.Receive(buffer);
                    //uint ulen=ByteTouint

                    while ((length = EquipmentConfigurationPage_Socket.Receive(buffer)) > 0) //Socket.ReceiveBufferSize  //默认值为 8192。 
                    {
                        currentRcv = null;
                        if (length < buffer.Length)  //最后一个小于buff.length时跳出循环 //&&EquipmentConfigurationPage_Socket.Available==0
                        {
                            byte[] lastBytes = new byte[length];
                            Buffer.BlockCopy(buffer, 0, lastBytes, 0, length);
                            data.AddRange(lastBytes);   //提高性能
                            currentRcv = lastBytes;
                        }
                        else if (length == buffer.Length)
                        {
                            data.AddRange(buffer);
                            currentRcv = buffer;
                        }
                        if (EquipmentConfigurationPage_Socket.Available == 0)  //此只针对单个包的完成接收处理. 若本报已接收完且下一包未收到的情况下,此时Available==0.若下一包已接收的化此时Available!=0.
                        {
                            string temp = Encoding.UTF8.GetString(currentRcv);
                            //hsServer.Logger.CWshow(instruct + "--最后一个包的字符串为：" + temp);
                            if (temp.Length >= 3)
                            {
                                char c1 = temp[temp.Length - 1];  //'}'  =125
                                //char c2 = temp[temp.Length - 2];  //'\n'  =10
                                char c3 = temp[temp.Length - 3]; //'}'  =125
                                if ((c1 == '}' && c3 == '}') | (c1 == 'k' && c3 == '-'))  //或者有返回hscmd-svr-kvmunit-setswi-ok
                                {
                                    break;
                                }
                            }
                        }
                        buffer = new byte[hsConfig.buffSize];
                    }
                    //string rcvStr = "-----ok-----";
                    //if (data.Count > 0)
                    //{
                    //    rcvStr = Encoding.UTF8.GetString(data.ToArray(), 0, data.Count);
                    //    hsServer.Logger.CWshow("查询Rx.V结果："+rcvStr);
                    //}
                    EquipmentConfigurationPage_Socket.Close();
                    Logger.CWshow("TCP连接成功：" + ip + ":" + port + instruct + "\r\n返回byte数：" + data.Count);
                    //Logger.CWshow("TCP连接成功：" + ip + ":" + port + instruct + "\r\n返回byte数：" + rcvStr);
                }
                catch (Exception ex)
                {
                    if (EquipmentConfigurationPage_Socket != null)
                    {
                        EquipmentConfigurationPage_Socket.Close();
                    }
                    Logger.CWshow("TCP连接失败：" + ip + ":" + port + instruct + "\r\n" + ex.Message);
                    //throw;  //不抛出异常 但有在调试记录显示
                    //MessageBox.Show("连接失败：" + ip + ":" + port + "\n" + instruct);
                }

                return data;   //需要有返回值
            });
            //if (t == await Task.WhenAny(t, Task.Delay(hsConfig.serverConnectTimeOut)))
            //{
            data1 = await t;
            //}
            //else
            //{
            //    Logger.CWshow("网络连接超时：" + ip + ":" + port + "--" + instruct);
            //}
            return data1.ToArray();
            //await t;
            //return t.Result;  //返回异步结果
        }

        public static async Task<string> Get_TCP_string_Async(string ip, int port, int delay, string instruct)
        {
            if (delay != 0)
            {
                await Task.Delay(delay);
            }
            string RecvErrorMessageString = "";
            Task<string> t = Task.Run<string>(() =>
            {

                //Thread.Sleep(delay * 1000);

                Socket EquipmentConfigurationPage_Socket = null;
                try
                {
                    List<byte> data = new List<byte>();
                    byte[] buffer = new byte[1024];
                    int length = 0;
                    //EquipmentConfigurationPage_Socket.Send(Encoding.UTF8.GetBytes(jsonString));
                    IPAddress ServerSocketIP;
                    byte[] result_1 = new byte[102400];
                    ServerSocketIP = IPAddress.Parse(ip);
                    EquipmentConfigurationPage_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    EquipmentConfigurationPage_Socket.Connect(new IPEndPoint(ServerSocketIP, port)); //配置服务器IP与端口
                    EquipmentConfigurationPage_Socket.ReceiveTimeout = hsConfig.serverReceiveTimeOut;
                    EquipmentConfigurationPage_Socket.Send(Encoding.UTF8.GetBytes(instruct));

                    while ((length = EquipmentConfigurationPage_Socket.Receive(buffer)) > 0)
                    {
                        for (int i = 0; i < length; i++)
                        {
                            data.Add(buffer[i]);
                        }

                        if (EquipmentConfigurationPage_Socket.Available == 0)
                        {
                            break;
                        }
                    }
                    if (data.Count > 0)
                    {
                        RecvErrorMessageString = Encoding.UTF8.GetString(data.ToArray(), 0, data.Count);
                    }
                    EquipmentConfigurationPage_Socket.Close();
                    //int receiveLength_1 = EquipmentConfigurationPage_Socket.Receive(result_1);

                    //RecvErrorMessageString = Encoding.UTF8.GetString(result_1, 0, receiveLength_1);
                    //Logger.CWshow("查询Tx返回字节数：" + data.Count + "\r\n" + RecvErrorMessageString);
                    //EquipmentConfigurationPage_Socket.Close();
                    //MessageBox.Show(RecvErrorMessageString);
                    //Logger.CWshow("TCP发送String成功：" + ip + ":" + port + "  --" + instruct);

                    //if (RecvErrorMessageString == "")
                    //{
                    //    //Logger.CWshow("TCP接收成功：" + ip + ":" + port + "  --" + RecvErrorMessageString);
                    //    Logger.CWshow("TCP接收成功：" + ip + ":" + port + "  --");
                    //}

                    //Logger.CWshow("TCP连接成功：" + ip + ":" + port + " - " + delay + " - " + instruct);
                }
                catch (Exception ex)
                {
                    if (EquipmentConfigurationPage_Socket != null)
                    {
                        EquipmentConfigurationPage_Socket.Close();
                    }
                    //Logger.CWshow("TCP连接失败：" + ip + ":" + port + " - " + delay + " - " + instruct + "\r\n" + ex.Message);
                    //throw;  //不抛出异常 但有在调试记录显示
                    //MessageBox.Show("连接失败：" + ip + ":" + port + "\n" + instruct);
                }
                return RecvErrorMessageString;  //需要有返回值
            });
            if (t == await Task.WhenAny(t, Task.Delay(hsConfig.serverConnectTimeOut)))
            {
                RecvErrorMessageString = await t;
            }
            else
            {
                //hsServer.Logger.CWshow("网络连接超时：" + ip + ":" + port + " - " + delay + " - " + instruct);
            }

            return RecvErrorMessageString;  //返回异步结果
            //await t;
            //return t.Result;  //返回异步结果
        }
        private static async Task<bool> CallTo_TCP_byte_Async(string ip, int port, int delay, byte[] bytes)
        {
            await Task.Delay(delay);
            bool isOk = true;
            Task<bool> t = Task.Run<bool>(() =>
            {
                //Thread.Sleep(delay * 1000);
                //MessageString = "HS-KEY16-setkey-16-onkeydown";
                Socket EquipmentConfigurationPage_Socket = null;
                try
                {
                    //EquipmentConfigurationPage_Socket.Send(Encoding.UTF8.GetBytes(jsonString));
                    IPAddress ServerSocketIP;
                    ServerSocketIP = IPAddress.Parse(ip);
                    EquipmentConfigurationPage_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    EquipmentConfigurationPage_Socket.Connect(new IPEndPoint(ServerSocketIP, port)); //配置服务器IP与端口
                    EquipmentConfigurationPage_Socket.Send(bytes);
                    EquipmentConfigurationPage_Socket.Close();
                    //MessageBox.Show(RecvErrorMessageString);
                    Logger.CWshow("TCP发送成功：" + ip + ":" + port + " - " + delay + " - " + byteToHexStr(bytes));
                }
                catch
                {
                    isOk = false;
                    if (EquipmentConfigurationPage_Socket != null)
                    {
                        EquipmentConfigurationPage_Socket.Close();
                    }
                    Logger.CWshow("TCP连接失败：" + ip + ":" + port + " -" + delay + " - " + byteToHexStr(bytes));
                    //throw;  //不抛出异常 但有在调试记录显示
                    //MessageBox.Show("连接失败：" + ip + ":" + port + "\n" + instruct);
                }
                return isOk;  //需要有返回值
            });
            if (t == await Task.WhenAny(t, Task.Delay(hsConfig.serverConnectTimeOut)))
            {
                isOk = await t;
            }
            else
            {
                Logger.CWshow("网络连接超时：" + ip + ":" + port + "--" + hsServer.byteToHexStr(bytes));
            }
            return isOk;
            //await t;
            //return t.Result;  //返回异步结果
        }
        private static async Task<bool> CallTo_UDP_byte_Async(string ip, int port, int delay, byte[] bytes)
        {
            await Task.Delay(delay);
            bool isOk = true;

            Task<bool> t = Task.Run<bool>(() =>
            {
                //Thread.Sleep(delay * 1000);
                //MessageString = "HS-KEY16-setkey-16-onkeydown";
                Socket EquipmentConfigurationPage_Socket = null;
                try
                {
                    //EquipmentConfigurationPage_Socket.Send(Encoding.UTF8.GetBytes(jsonString));
                    IPAddress ServerSocketIP;
                    ServerSocketIP = IPAddress.Parse(ip);
                    EquipmentConfigurationPage_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    EquipmentConfigurationPage_Socket.Connect(new IPEndPoint(ServerSocketIP, port)); //配置服务器IP与端口
                    EquipmentConfigurationPage_Socket.Send(bytes);
                    EquipmentConfigurationPage_Socket.Close();
                    //MessageBox.Show(RecvErrorMessageString);
                    Logger.CWshow("UDP发送成功：" + ip + ":" + port + " - " + delay + " - " + byteToHexStr(bytes));
                }
                catch
                {
                    isOk = false;
                    if (EquipmentConfigurationPage_Socket != null)
                    {
                        EquipmentConfigurationPage_Socket.Close();
                    }
                    Logger.CWshow("UDP连接失败：" + ip + ":" + port + " - " + delay + " - " + byteToHexStr(bytes));
                    //throw;  //不抛出异常 但有在调试记录显示
                    //MessageBox.Show("连接失败：" + ip + ":" + port + "\n" + instruct);
                }
                return isOk;  //需要有返回值
            });
            if (t == await Task.WhenAny(t, Task.Delay(hsConfig.serverConnectTimeOut)))
            {
                isOk = await t;
            }
            else
            {
                Logger.CWshow("网络连接超时：" + ip + ":" + port + " - " + delay + " - " + byteToHexStr(bytes));
            }

            return isOk;  //返回异步结果
            //await t;
            //return t.Result;  //返回异步结果
        }
        private static async Task<string> Get_UDP_string_Async(string ip, int port, int delay, string instruct)
        {
            await Task.Delay(delay);
            string RecvErrorMessageString = "";
            Task<string> t = Task.Run<string>(() =>
            {

                Socket EquipmentConfigurationPage_Socket = null;
                //Thread.Sleep(delay * 1000);
                try
                {
                    //EquipmentConfigurationPage_Socket.Send(Encoding.UTF8.GetBytes(jsonString));
                    IPAddress ServerSocketIP;
                    byte[] result_1 = new byte[102400];
                    ServerSocketIP = IPAddress.Parse(ip);
                    EquipmentConfigurationPage_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    EquipmentConfigurationPage_Socket.Connect(new IPEndPoint(ServerSocketIP, port)); //配置服务器IP与端口
                    EquipmentConfigurationPage_Socket.Send(Encoding.UTF8.GetBytes(instruct));
                    //int receiveLength_1 = EquipmentConfigurationPage_Socket.Receive(result_1);
                    //RecvErrorMessageString = Encoding.UTF8.GetString(result_1, 0, receiveLength_1);
                    EquipmentConfigurationPage_Socket.Close();
                    //MessageBox.Show(RecvErrorMessageString);
                    //Logger.CWshow("TCP发送String成功：" + ip + ":" + port + "  --" + instruct);

                    //if (RecvErrorMessageString == "")
                    //{
                    //    //Logger.CWshow("TCP接收成功：" + ip + ":" + port + "  --" + RecvErrorMessageString);
                    //    Logger.CWshow("TCP接收成功：" + ip + ":" + port + "  --");
                    //}
                    Logger.CWshow("UDP发送成功：" + ip + ":" + port + ":" + delay + ":" + instruct);
                }
                catch
                {
                    if (EquipmentConfigurationPage_Socket != null)
                    {
                        EquipmentConfigurationPage_Socket.Close();
                    }
                    Logger.CWshow("UDP连接失败：" + ip + ":" + port + ":" + delay + ":" + instruct);
                    //throw;  //不抛出异常 但有在调试记录显示
                    //MessageBox.Show("连接失败：" + ip + ":" + port + "\n" + instruct);
                }
                return RecvErrorMessageString;  //需要有返回值
            });
            if (t == await Task.WhenAny(t, Task.Delay(hsConfig.serverConnectTimeOut)))
            {
                RecvErrorMessageString = await t;
            }
            else
            {
                Logger.CWshow("网络连接超时：" + ip + ":" + port + " - " + delay + " - " + instruct);
            }

            return RecvErrorMessageString;  //返回异步结果
            //await t;
            //return t.Result;  //返回异步结果
        }

        public static async Task<string> Get_Delay_TCP_string_ASCII_Async(string ip, int port, int delay, string instruct)
        {
            await Task.Delay(delay);
            string RecvErrorMessageString = "";
            Task<string> t = Task.Run<string>(() =>
            {
                //Thread.Sleep(delay * 1000);
                Socket EquipmentConfigurationPage_Socket = null;
                try
                {
                    //EquipmentConfigurationPage_Socket.Send(Encoding.UTF8.GetBytes(jsonString));
                    IPAddress ServerSocketIP;
                    byte[] result_1 = new byte[102400];
                    ServerSocketIP = IPAddress.Parse(ip);
                    EquipmentConfigurationPage_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    EquipmentConfigurationPage_Socket.Connect(new IPEndPoint(ServerSocketIP, port)); //配置服务器IP与端口
                    EquipmentConfigurationPage_Socket.ReceiveTimeout = hsConfig.serverReceiveTimeOut;
                    EquipmentConfigurationPage_Socket.Send(Encoding.ASCII.GetBytes(instruct));
                    int receiveLength_1 = EquipmentConfigurationPage_Socket.Receive(result_1);
                    RecvErrorMessageString = Encoding.ASCII.GetString(result_1, 0, receiveLength_1);
                    EquipmentConfigurationPage_Socket.Close();
                    //MessageBox.Show(RecvErrorMessageString);
                    //Logger.CWshow("TCP发送String成功：" + ip + ":" + port + "  --" + instruct);

                    //if (RecvErrorMessageString == "")
                    //{
                    //    //Logger.CWshow("TCP接收成功：" + ip + ":" + port + "  --" + RecvErrorMessageString);
                    //    Logger.CWshow("TCP接收成功：" + ip + ":" + port + "  --");
                    //}
                    Logger.CWshow("TCP连接成功：" + ip + ":" + port + instruct + "返回：" + RecvErrorMessageString);
                }
                catch
                {
                    if (EquipmentConfigurationPage_Socket != null)
                    {
                        EquipmentConfigurationPage_Socket.Close();
                    }
                    Logger.CWshow("TCP连接失败：" + ip + ":" + port + instruct);
                    //throw;  //不抛出异常 但有在调试记录显示
                    //MessageBox.Show("连接失败：" + ip + ":" + port + "\n" + instruct);
                }
                return RecvErrorMessageString;  //需要有返回值
            });
            if (t == await Task.WhenAny(t, Task.Delay(hsConfig.serverConnectTimeOut)))
            {
                RecvErrorMessageString = await t;
            }
            else
            {
                Logger.CWshow("网络连接超时：" + ip + ":" + port + "--" + instruct);
            }
            //await t;
            //return t.Result;  //返回异步结果
            return RecvErrorMessageString;
        }
        public static async Task<string> Get_Delay_UDP_string_ASCII_Async(string ip, int port, int delay, string instruct)
        {
            await Task.Delay(delay);
            Task<string> t = Task.Run<string>(() =>
            {
                //Thread.Sleep(delay * 1000);
                string RecvErrorMessageString = "";
                Socket EquipmentConfigurationPage_Socket = null;
                try
                {
                    IPAddress ServerSocketIP;
                    ServerSocketIP = IPAddress.Parse(ip);
                    EquipmentConfigurationPage_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    EquipmentConfigurationPage_Socket.Connect(new IPEndPoint(ServerSocketIP, port)); //配置服务器IP与端口
                    EquipmentConfigurationPage_Socket.Send(Encoding.ASCII.GetBytes(instruct));
                    EquipmentConfigurationPage_Socket.Close();
                    EquipmentConfigurationPage_Socket = null;
                    //MessageBox.Show(RecvErrorMessageString);
                    //Logger.CWshow("TCP发送String成功：" + ip + ":" + port + "  --" + instruct);

                    //if (RecvErrorMessageString == "")
                    //{
                    //    //Logger.CWshow("TCP接收成功：" + ip + ":" + port + "  --" + RecvErrorMessageString);
                    //    Logger.CWshow("TCP接收成功：" + ip + ":" + port + "  --");
                    //}
                    Logger.CWshow("UDP连接成功：" + ip + ":" + port + instruct + "返回：" + RecvErrorMessageString);
                }
                catch
                {
                    if (EquipmentConfigurationPage_Socket != null)
                    {
                        EquipmentConfigurationPage_Socket.Close();
                        EquipmentConfigurationPage_Socket = null;
                    }
                    Logger.CWshow("UDP连接失败：" + ip + ":" + port + instruct);
                    //throw;  //不抛出异常 但有在调试记录显示
                    //MessageBox.Show("连接失败：" + ip + ":" + port + "\n" + instruct);
                }
                return RecvErrorMessageString;  //需要有返回值
            });
            await t;
            return t.Result;  //返回异步结果
        }

        public static async Task<string> Call_UDP_string_Async(string ip, int port, int delay, string instruct, Encoding encoding)
        {
            if (delay > 0)
            {
                await Task.Delay(delay);
            }
            string RecvErrorMessageString = "";
            Task<string> t = Task.Run<string>(() =>
            {
                Socket EquipmentConfigurationPage_Socket = null;
                try
                {
                    List<byte> data = new List<byte>();
                    byte[] buffer = new byte[1024];
                    int length = 0;
                    byte[] currentRcv = null;
                    IPAddress ServerSocketIP;
                    byte[] result_1 = new byte[102400];
                    ServerSocketIP = IPAddress.Parse(ip);
                    EquipmentConfigurationPage_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    EquipmentConfigurationPage_Socket.Connect(new IPEndPoint(ServerSocketIP, port)); //配置服务器IP与端口
                    EquipmentConfigurationPage_Socket.ReceiveTimeout = hsConfig.ReceiveTimeOutMosaicRoam;
                    EquipmentConfigurationPage_Socket.Send(encoding.GetBytes(instruct));

                    while ((length = EquipmentConfigurationPage_Socket.Receive(buffer)) > 0)
                    {
                        currentRcv = null;
                        if (length < buffer.Length)  //最后一个小于buff.length时跳出循环 //&&EquipmentConfigurationPage_Socket.Available==0
                        {
                            byte[] lastBytes = new byte[length];
                            Buffer.BlockCopy(buffer, 0, lastBytes, 0, length);
                            data.AddRange(lastBytes);   //提高性能
                            currentRcv = lastBytes;
                        }
                        else if (length == buffer.Length)
                        {
                            data.AddRange(buffer);
                            currentRcv = buffer;
                        }
                        if (EquipmentConfigurationPage_Socket.Available == 0)
                        {
                            string temp = encoding.GetString(currentRcv);
                            if (Encoding.UTF8 == encoding)  //hs.Json格式标识
                            {
                                //hsServer.Logger.CWshow(instruct + "--最后一个包的字符串为：" + temp);
                                if (temp.Length >= 3)
                                {
                                    char c1 = temp[temp.Length - 1];  //'}'  =125
                                    //char c2 = temp[temp.Length - 2];  //'\n'  =10
                                    char c3 = temp[temp.Length - 3]; //'}'  =125
                                    if ((c1 == '}' && c3 == '}'))  //| (c1 == 'k' && c3 == '-')或者有返回hscmd-svr-kvmunit-setswi-ok 
                                    {
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                if (temp.Contains(",OK"))
                                {
                                    break;  //其他等待多包接收结尾标识
                                }
                            }
                        }
                        buffer = new byte[hsConfig.buffSize];
                    }
                    if (data.Count > 0)
                    {
                        RecvErrorMessageString = encoding.GetString(data.ToArray(), 0, data.Count);
                    }
                    EquipmentConfigurationPage_Socket.Close();
                    //int receiveLength_1 = EquipmentConfigurationPage_Socket.Receive(result_1);

                    //RecvErrorMessageString = Encoding.UTF8.GetString(result_1, 0, receiveLength_1);
                    //Logger.CWshow("查询Tx返回字节数：" + data.Count + "\r\n" + RecvErrorMessageString);
                    //EquipmentConfigurationPage_Socket.Close();
                    //MessageBox.Show(RecvErrorMessageString);
                    //Logger.CWshow("TCP发送String成功：" + ip + ":" + port + "  --" + instruct);

                    //if (RecvErrorMessageString == "")
                    //{
                    //    //Logger.CWshow("TCP接收成功：" + ip + ":" + port + "  --" + RecvErrorMessageString);
                    //    Logger.CWshow("TCP接收成功：" + ip + ":" + port + "  --");
                    //}

                    Logger.CWshow("Udp发送/接收成功：" + ip + ":" + port + " - " + delay + " - " + instruct + "\r\n返回：" + RecvErrorMessageString);
                }
                catch (Exception ex)
                {
                    if (EquipmentConfigurationPage_Socket != null)
                    {
                        EquipmentConfigurationPage_Socket.Close();
                    }
                    Logger.CWshow("Udp连接失败：" + ex.Message + "\r\n   ------------" + ip + ":" + port + " - " + delay + " - " + instruct);
                    //throw;  //不抛出异常 但有在调试记录显示
                    //MessageBox.Show("连接失败：" + ip + ":" + port + "\n" + instruct);
                }
                return RecvErrorMessageString;  //需要有返回值
            });
            if (t == await Task.WhenAny(t, Task.Delay(hsConfig.serverConnectTimeOut)))
            {
                RecvErrorMessageString = await t;
            }
            else
            {
                Logger.CWshow("网络连接超时：" + ip + ":" + port + " - " + delay + " - " + instruct);
            }

            return RecvErrorMessageString;  //返回异步结果
                                            //await t;
                                            //return t.Result;  //返回异步结果
        }

        public static async Task<string> Call_TCP_string_Async(string ip, int port, int delay, string instruct, Encoding encoding)
        {
            if (delay > 0)
            {
                await Task.Delay(delay);
            }
            string RecvErrorMessageString = "";
            Task<string> t = Task.Run<string>(() =>
            {
                Socket EquipmentConfigurationPage_Socket = null;
                try
                {
                    List<byte> data = new List<byte>();
                    byte[] buffer = new byte[1024];
                    int length = 0;
                    byte[] currentRcv = null;
                    IPAddress ServerSocketIP;
                    byte[] result_1 = new byte[102400];
                    ServerSocketIP = IPAddress.Parse(ip);
                    EquipmentConfigurationPage_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    EquipmentConfigurationPage_Socket.Connect(new IPEndPoint(ServerSocketIP, port)); //配置服务器IP与端口
                    EquipmentConfigurationPage_Socket.ReceiveTimeout = hsConfig.ReceiveTimeOutFromRx4;
                    EquipmentConfigurationPage_Socket.Send(encoding.GetBytes(instruct));

                    while ((length = EquipmentConfigurationPage_Socket.Receive(buffer)) > 0)
                    {
                        currentRcv = null;
                        if (length < buffer.Length)  //最后一个小于buff.length时跳出循环 //&&EquipmentConfigurationPage_Socket.Available==0
                        {
                            byte[] lastBytes = new byte[length];
                            Buffer.BlockCopy(buffer, 0, lastBytes, 0, length);
                            data.AddRange(lastBytes);   //提高性能
                            currentRcv = lastBytes;
                        }
                        else if (length == buffer.Length)
                        {
                            data.AddRange(buffer);
                            currentRcv = buffer;
                        }
                        if (EquipmentConfigurationPage_Socket.Available == 0)
                        {
                            //string temp = encoding.GetString(currentRcv);
                            //if (Encoding.UTF8 == encoding)
                            //{                                
                            //    //hsServer.Logger.CWshow(instruct + "--最后一个包的字符串为：" + temp);
                            //    if (temp.Length >= 3)
                            //    {
                            //        char c1 = temp[temp.Length - 1];  //'}'  =125
                            //        //char c2 = temp[temp.Length - 2];  //'\n'  =10
                            //        char c3 = temp[temp.Length - 3]; //'}'  =125
                            //        if ((c1 == '}' && c3 == '}'))  //| (c1 == 'k' && c3 == '-')或者有返回hscmd-svr-kvmunit-setswi-ok 
                            //        {
                            //            break;
                            //        }
                            //    }
                            //}
                            //else 
                            //{
                            //    break;  //其他等待多包接收结尾标识
                            //}
                            break;// 表示Rx4返回关闭即可
                        }
                        buffer = new byte[hsConfig.buffSize];
                    }
                    if (data.Count > 0)
                    {
                        RecvErrorMessageString = encoding.GetString(data.ToArray(), 0, data.Count);
                    }
                    EquipmentConfigurationPage_Socket.Close();
                    //int receiveLength_1 = EquipmentConfigurationPage_Socket.Receive(result_1);

                    //RecvErrorMessageString = Encoding.UTF8.GetString(result_1, 0, receiveLength_1);
                    //Logger.CWshow("查询Tx返回字节数：" + data.Count + "\r\n" + RecvErrorMessageString);
                    //EquipmentConfigurationPage_Socket.Close();
                    //MessageBox.Show(RecvErrorMessageString);
                    //Logger.CWshow("TCP发送String成功：" + ip + ":" + port + "  --" + instruct);

                    //if (RecvErrorMessageString == "")
                    //{
                    //    //Logger.CWshow("TCP接收成功：" + ip + ":" + port + "  --" + RecvErrorMessageString);
                    //    Logger.CWshow("TCP接收成功：" + ip + ":" + port + "  --");
                    //}

                    //Logger.CWshow("Udp发送/接收成功：" + ip + ":" + port + " - " + delay + " - " + instruct+"\r\n-----------返回："+RecvErrorMessageString);
                }
                catch (Exception ex)
                {
                    if (EquipmentConfigurationPage_Socket != null)
                    {
                        EquipmentConfigurationPage_Socket.Close();
                    }
                    Logger.CWshow("TCP连接失败：" + ex.Message + "\r\n   ------------" + ip + ":" + port + " - " + delay + " - " + instruct);
                    //throw;  //不抛出异常 但有在调试记录显示
                    //MessageBox.Show("连接失败：" + ip + ":" + port + "\n" + instruct);
                }
                return RecvErrorMessageString;  //需要有返回值
            });
            if (t == await Task.WhenAny(t, Task.Delay(hsConfig.serverConnectTimeOut)))
            {
                RecvErrorMessageString = await t;
            }
            else
            {
                Logger.CWshow("网络连接超时：" + ip + ":" + port + " - " + delay + " - " + instruct);
            }

            return RecvErrorMessageString;  //返回异步结果
                                            //await t;
                                            //return t.Result;  //返回异步结果
        }
        public static async Task<List<byte>> Call_TCP_string_Async_ReturnByte(string ip, int port, int delay, string instruct, Encoding encoding)
        {
            if (delay > 0)
            {
                await Task.Delay(delay);
            }
            string RecvErrorMessageString = "";
            List<byte> rcvBytes = new List<byte>();
            Task<List<byte>> t = Task.Run<List<byte>>(() =>
            {
                Socket EquipmentConfigurationPage_Socket = null;
                List<byte> data = new List<byte>();
                try
                {
                    byte[] buffer = new byte[1024];
                    int length = 0;
                    byte[] currentRcv = null;
                    IPAddress ServerSocketIP;
                    byte[] result_1 = new byte[102400];
                    ServerSocketIP = IPAddress.Parse(ip);
                    EquipmentConfigurationPage_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    EquipmentConfigurationPage_Socket.Connect(new IPEndPoint(ServerSocketIP, port)); //配置服务器IP与端口
                    EquipmentConfigurationPage_Socket.ReceiveTimeout = hsConfig.ReceiveTimeOutMosaicRoam;
                    EquipmentConfigurationPage_Socket.Send(encoding.GetBytes(instruct));

                    while ((length = EquipmentConfigurationPage_Socket.Receive(buffer)) > 0)
                    {
                        currentRcv = null;
                        if (length < buffer.Length)  //最后一个小于buff.length时跳出循环 //&&EquipmentConfigurationPage_Socket.Available==0
                        {
                            byte[] lastBytes = new byte[length];
                            Buffer.BlockCopy(buffer, 0, lastBytes, 0, length);
                            data.AddRange(lastBytes);   //提高性能
                            currentRcv = lastBytes;
                        }
                        else if (length == buffer.Length)
                        {
                            data.AddRange(buffer);
                            currentRcv = buffer;
                        }
                        if (EquipmentConfigurationPage_Socket.Available == 0)
                        {
                            string temp = encoding.GetString(currentRcv);
                            if (Encoding.UTF8 == encoding)
                            {
                                //hsServer.Logger.CWshow(instruct + "--最后一个包的字符串为：" + temp);
                                if (temp.Length >= 3)
                                {
                                    char c1 = temp[temp.Length - 1];  //'}'  =125
                                    //char c2 = temp[temp.Length - 2];  //'\n'  =10
                                    char c3 = temp[temp.Length - 3]; //'}'  =125
                                    if ((c1 == '}' && c3 == '}'))  //| (c1 == 'k' && c3 == '-')或者有返回hscmd-svr-kvmunit-setswi-ok 
                                    {
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                break;  //其他等待多包接收结尾标识
                            }
                        }
                        buffer = new byte[hsConfig.buffSize];
                    }

                    EquipmentConfigurationPage_Socket.Close();
                    //int receiveLength_1 = EquipmentConfigurationPage_Socket.Receive(result_1);

                    //RecvErrorMessageString = Encoding.UTF8.GetString(result_1, 0, receiveLength_1);
                    //Logger.CWshow("查询Tx返回字节数：" + data.Count + "\r\n" + RecvErrorMessageString);
                    //EquipmentConfigurationPage_Socket.Close();
                    //MessageBox.Show(RecvErrorMessageString);
                    //Logger.CWshow("TCP发送String成功：" + ip + ":" + port + "  --" + instruct);

                    //if (RecvErrorMessageString == "")
                    //{
                    //    //Logger.CWshow("TCP接收成功：" + ip + ":" + port + "  --" + RecvErrorMessageString);
                    //    Logger.CWshow("TCP接收成功：" + ip + ":" + port + "  --");
                    //}
                    if (data.Count > 0)
                    {
                        RecvErrorMessageString = encoding.GetString(data.ToArray(), 0, data.Count);
                    }
                    Logger.CWshow("Tcp发送/接收成功：" + ip + ":" + port + " - " + delay + " - " + instruct + "\r\n返回：" + RecvErrorMessageString);
                }
                catch (Exception ex)
                {
                    if (EquipmentConfigurationPage_Socket != null)
                    {
                        EquipmentConfigurationPage_Socket.Close();
                    }
                    Logger.CWshow("TCP连接异常：" + ex.Message + "\r\n   ------------" + ip + ":" + port + " - " + delay + " - " + instruct);
                    //throw;  //不抛出异常 但有在调试记录显示
                    //MessageBox.Show("连接失败：" + ip + ":" + port + "\n" + instruct);
                }
                return data;  //需要有返回值
            });
            if (t == await Task.WhenAny(t, Task.Delay(hsConfig.serverConnectTimeOut)))
            {
                rcvBytes = await t;
            }
            else
            {
                Logger.CWshow("网络连接超时：" + ip + ":" + port + " - " + delay + " - " + instruct);
            }

            return rcvBytes;  //返回异步结果
                              //await t;
                              //return t.Result;  //返回异步结果
        }

        public static async void Call_TCP_Byte_Async(string ip, int port, int delay, byte[] bytes)
        {
            if (delay > 0)
            {
                await Task.Delay(delay);
            }
            Task t = Task.Run(() =>
            {
                //Thread.Sleep(delay * 1000);
                //MessageString = "HS-KEY16-setkey-16-onkeydown";
                Socket EquipmentConfigurationPage_Socket = null;
                try
                {
                    //EquipmentConfigurationPage_Socket.Send(Encoding.UTF8.GetBytes(jsonString));
                    IPAddress ServerSocketIP;
                    ServerSocketIP = IPAddress.Parse(ip);
                    EquipmentConfigurationPage_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    EquipmentConfigurationPage_Socket.Connect(new IPEndPoint(ServerSocketIP, port)); //配置服务器IP与端口
                    EquipmentConfigurationPage_Socket.Send(bytes);
                    EquipmentConfigurationPage_Socket.Close();
                    //MessageBox.Show(RecvErrorMessageString);
                    Logger.CWshow("TCP发送成功：" + ip + ":" + port + " - " + delay + " - " + byteToHexStr(bytes));
                }
                catch
                {
                    if (EquipmentConfigurationPage_Socket != null)
                    {
                        EquipmentConfigurationPage_Socket.Close();
                    }
                    Logger.CWshow("TCP连接失败：" + ip + ":" + port + " -" + delay + " - " + byteToHexStr(bytes));
                    //throw;  //不抛出异常 但有在调试记录显示
                    //MessageBox.Show("连接失败：" + ip + ":" + port + "\n" + instruct);
                }
            });
        }
        public static async void Call_Udp_Byte_Async(string ip, int port, int delay, byte[] bytes)
        {
            if (delay > 0)
            {
                await Task.Delay(delay);
            }
            Task t = Task.Run(() =>
            {
                //Thread.Sleep(delay * 1000);
                //MessageString = "HS-KEY16-setkey-16-onkeydown";
                Socket EquipmentConfigurationPage_Socket = null;
                try
                {
                    //EquipmentConfigurationPage_Socket.Send(Encoding.UTF8.GetBytes(jsonString));
                    IPAddress ServerSocketIP;
                    ServerSocketIP = IPAddress.Parse(ip);
                    EquipmentConfigurationPage_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    EquipmentConfigurationPage_Socket.Connect(new IPEndPoint(ServerSocketIP, port)); //配置服务器IP与端口
                    EquipmentConfigurationPage_Socket.Send(bytes);
                    EquipmentConfigurationPage_Socket.Close();
                    //MessageBox.Show(RecvErrorMessageString);
                    Logger.CWshow("Udp发送成功：" + ip + ":" + port + " - " + delay + " - " + byteToHexStr(bytes));
                }
                catch
                {
                    if (EquipmentConfigurationPage_Socket != null)
                    {
                        EquipmentConfigurationPage_Socket.Close();
                    }
                    Logger.CWshow("Udp连接失败：" + ip + ":" + port + " -" + delay + " - " + byteToHexStr(bytes));
                    //throw;  //不抛出异常 但有在调试记录显示
                    //MessageBox.Show("连接失败：" + ip + ":" + port + "\n" + instruct);
                }
            });
        }
        public static async void Call_TCP_string_Async_NoRetrun(string ip, int port, int delay, string instruct, Encoding encoding)
        {
            if (delay > 0)
            {
                await Task.Delay(delay);
            }
            Task t = Task.Run(() =>
            {
                Socket EquipmentConfigurationPage_Socket = null;
                try
                {
                    IPAddress ServerSocketIP;
                    byte[] result_1 = new byte[102400];
                    ServerSocketIP = IPAddress.Parse(ip);
                    EquipmentConfigurationPage_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    EquipmentConfigurationPage_Socket.Connect(new IPEndPoint(ServerSocketIP, port)); //配置服务器IP与端口
                    EquipmentConfigurationPage_Socket.ReceiveTimeout = hsConfig.ReceiveTimeOutMosaicRoam;
                    EquipmentConfigurationPage_Socket.Send(encoding.GetBytes(instruct));

                    EquipmentConfigurationPage_Socket.Close();
                    Logger.CWshow("TCP发送/接收成功：" + ip + ":" + port + " - " + delay + " - " + instruct);
                }
                catch (Exception ex)
                {
                    if (EquipmentConfigurationPage_Socket != null)
                    {
                        EquipmentConfigurationPage_Socket.Close();
                    }
                    Logger.CWshow("TCP连接失败：" + ex.Message + "\r\n   ------------" + ip + ":" + port + " - " + delay + " - " + instruct);
                    //throw;  //不抛出异常 但有在调试记录显示
                    //MessageBox.Show("连接失败：" + ip + ":" + port + "\n" + instruct);
                }
            });
        }
        public static async void Call_Udp_string_Async_NoRetrun(string ip, int port, int delay, string instruct, Encoding encoding)
        {
            if (delay > 0)
            {
                await Task.Delay(delay);
            }
            Task t = Task.Run(() =>
            {
                Socket EquipmentConfigurationPage_Socket = null;
                try
                {
                    IPAddress ServerSocketIP;
                    byte[] result_1 = new byte[102400];
                    ServerSocketIP = IPAddress.Parse(ip);
                    EquipmentConfigurationPage_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    EquipmentConfigurationPage_Socket.Connect(new IPEndPoint(ServerSocketIP, port)); //配置服务器IP与端口
                    //EquipmentConfigurationPage_Socket.ReceiveTimeout = hsConfig.ReceiveTimeOutMosaicRoam;
                    EquipmentConfigurationPage_Socket.Send(encoding.GetBytes(instruct));

                    EquipmentConfigurationPage_Socket.Close();
                    Logger.CWshow("Udp发送/接收成功：" + ip + ":" + port + " - " + delay + " - " + instruct);
                }
                catch (Exception ex)
                {
                    if (EquipmentConfigurationPage_Socket != null)
                    {
                        EquipmentConfigurationPage_Socket.Close();
                    }
                    Logger.CWshow("Udp连接失败：" + ex.Message + "\r\n   ------------" + ip + ":" + port + " - " + delay + " - " + instruct);
                }
            });
        }







        /// <summary>
        /// 字符串的十六进制 转化为字节的十六进制
        /// </summary>
        /// <param name="hexString">BB BB</param>
        /// <returns></returns> 
        public static byte[] strToToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            try
            {
                for (int i = 0; i < returnBytes.Length; i++)
                    returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            catch (Exception ex)
            {
                Logger.CWshow(hexString + "strToToHexByte失败：" + ex.Message);
                return returnBytes = new byte[0];
                //throw;
            }
            return returnBytes;
        }
        private static string byteToHexStr(byte[] bytes)
        {
            //string returnStr = "";
            //if (bytes != null)
            //{
            //    for (int i = 0; i < bytes.Length; i++)
            //    {
            //        returnStr += bytes[i].ToString("X2");
            //    }
            //}
            //return returnStr;
            //if (bytes==null)
            //{
            //    return "";
            //}
            return bytes == null ? "" : BitConverter.ToString(bytes);
        }

        #region 动态获取本地ipV4地址（服务器相关）-----------------------
        private static string _localIP;
        public static string LocalIP
        {
            get
            {
                if (_localIP == null)
                {
                    //_localIP = getLocalIPAddress();
                    _localIP = GetLocalIpV4();
                }
                return _localIP;
            }
            //set
            //{
            //    _localIP = value;
            //}
        }
        public static string getLocalIPAddress()
        {
            string localIp = "192.168.0.119"; //默认。。。。
            try
            {
                // 获得本机局域网IP地址   对应服务器地址的前3个地址数
                //IPAddress[] AddressList = Dns.GetHostByName(Dns.GetHostName()).AddressList;
                //if (AddressList.Length < 1)
                //{
                //    return "";
                //}
                //return AddressList[0].ToString();
                string PreIp = "";
                string[] temp = hsConfig.serverIp.Split('.');
                if (temp.Length != 4)
                {
                    Logger.CWshow("服务器设置的Ip有误：" + hsConfig.serverIp);
                    return "192.168.0.119";
                }
                for (int i = 0; i < temp.Length; i++)
                {
                    if (i < temp.Length - 1)
                    {
                        PreIp += temp[i] + ".";
                    }
                }
                IPAddress[] AddressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
                //此处能获取所有本地连接的ip （包括本地连接3..）需要筛选192.168.  注意若双网卡设置同一网段的值，则会出错，不知道系统到底使用哪个网卡的ip。如192.168.0.110和192.168.0.111同时存在
                foreach (var item in AddressList)
                {
                    if (item.ToString().Contains(PreIp))
                    {
                        localIp = item.ToString();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {

                Logger.CWshow("获取本机Ip出错：" + ex.Message);
            }
            return localIp;
        }

        public static string GetLocalIpV4()
        {
            try
            {
                string HostName = Dns.GetHostName(); //得到主机名
                IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
                for (int i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    //从IP地址列表中筛选出IPv4类型的IP地址
                    //AddressFamily.InterNetwork表示此IP为IPv4,
                    //AddressFamily.InterNetworkV6表示此地址为IPv6类型
                    if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        return IpEntry.AddressList[i].ToString();
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                Logger.CWshow("GetLocalIpV4-异常：" + ex.Message);
                return "127.0.0.1";
            }
        }
        #endregion

    }
}
