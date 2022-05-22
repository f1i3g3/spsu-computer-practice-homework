using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChatDescription;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;

using System;

namespace NinthTask.Tests
{
        [TestClass]
        public class ChatTests
        {
                Client firstUser;
                Client secondUser;
                Client thirdUser;
                IPEndPoint ipF;
                IPEndPoint ipS;
                IPEndPoint ipT;

                //полностью переписать тесты из-за проблем с сокетами
                public void TestsInit()
                {
                        firstUser = new Client();
                        secondUser = new Client();
                        thirdUser = new Client();

                        ipF = IPInit(11111);
                        ipS = IPInit(22222);
                        ipT = IPInit(33333);
                }

                [TestMethod]
                public void TwoConnectionTest()
                {
                        //TestsInit();
                        firstUser = new Client();
                        secondUser = new Client();

                        /*
                        ipF = IPInit(11111);
                        ipS = IPInit(22222);
                        */
                        foreach (var iPAdress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
                        {
                                if (iPAdress.AddressFamily == AddressFamily.InterNetwork)
                                {
                                        ipF = new IPEndPoint(iPAdress, 11111);
                                        break;
                                }
                        }
                        foreach (var iPAdress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
                        {
                                if (iPAdress.AddressFamily == AddressFamily.InterNetwork)
                                {
                                        ipS = new IPEndPoint(iPAdress, 22222);
                                        break;
                                }
                        }

                        firstUser.TryGetAddress(ipF);
                        secondUser.TryGetAddress(ipS);

                        //firstUser.StartListening();
                        secondUser.StartListening();

                        Assert.AreEqual(11111, firstUser.ClientIP.Port);
                        Assert.AreEqual(22222, secondUser.ClientIP.Port);
                       
                        firstUser.Connect(secondUser.ClientIP);
                        firstUser.SendMessage(ByteOperations.MessageToBytes("0fffffffffff"));

                        Thread.Sleep(1);

                        var actualFirst = Connections(firstUser);
                        Assert.AreEqual(1, actualFirst.Count);
                        Assert.AreEqual(22222, actualFirst[0].Port);
                        Assert.AreEqual(secondUser.ClientIP.Address, actualFirst[0].Address);

                        Thread.Sleep(1);

                        var actualSecond = Connections(secondUser);
                        Assert.AreEqual(1, actualSecond.Count);
                        Assert.AreEqual(11111, actualSecond[0].Port);
                        Assert.AreEqual(firstUser.ClientIP.Address, actualSecond[0].Address);
                }

                /*
                [TestMethod]
                public void ThreeConnectionTest()
		{
                        TestsInit();

                        firstUser.TryGetAddress(ipF);
                        secondUser.TryGetAddress(ipS);
                        thirdUser.TryGetAddress(ipT);

                        //firstUser.StartListening();
                        secondUser.StartListening();
                        thirdUser.StartListening();

                        Assert.AreEqual(33333, thirdUser.ClientIP.Port);

                        //firstUser.Connect(thirdUser.ClientIP);
                        thirdUser.Connect(firstUser.ClientIP);

                        Thread.Sleep(1);

                        var actualFirst = Connections(firstUser);
                        Assert.AreEqual(2, actualFirst.Count);
                        Assert.AreEqual(22222, actualFirst[0].Port);
                        Assert.AreEqual(33333, actualFirst[1].Port);
                        Assert.AreEqual(secondUser.ClientIP.Address, actualFirst[0].Address);
                        Assert.AreEqual(thirdUser.ClientIP.Address, actualFirst[1].Address);

                        var actualThird = new List<IPEndPoint>(thirdUser.UserList); //вынеси отдельно
                        Assert.AreEqual(2, actualThird.Count);
                        Assert.AreEqual(11111, actualThird[0].Port);
                        Assert.AreEqual(22222, actualThird[1].Port);
                        Assert.AreEqual(firstUser.ClientIP.Address, actualThird[0].Address);
                        Assert.AreEqual(secondUser.ClientIP.Address, actualThird[1].Address);
                }
                */

                /*
                [TestMethod]
                public void DisconnectionTest()
		{
                        TestsInit();
                        firstUser.TryGetAddress(ipF);
                        secondUser.TryGetAddress(ipS);
                        thirdUser.TryGetAddress(ipT);

                        firstUser.StartListening();
                        secondUser.StartListening();
                        thirdUser.StartListening();

                        firstUser.Disconnect();

                        Thread.Sleep(1);

                        var actualThird = Connections(thirdUser);
                        Assert.AreEqual(1, actualThird.Count);
                        Assert.AreEqual(101, actualThird[0].Port);
                        Assert.AreEqual(secondUser.ClientIP.Address, actualThird[0].Address);
                }
                */

                private static List<IPEndPoint> Connections(Client client)
		{
                        var temp = new List<IPEndPoint>(client.UserList);
                        Console.WriteLine(temp.Count);
                        foreach (var t in temp)
                        {
                                Console.WriteLine("!!!!!!!");
                                Console.WriteLine(t);
                        }
                        return temp;
		}

                private static IPEndPoint IPInit(int port)
		{
                        foreach (var iPAdress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
                        {
                                if (iPAdress.AddressFamily == AddressFamily.InterNetwork)
                                {
                                        return new IPEndPoint(iPAdress, port);
                                }
                        }

                        return null;
                }
        }
}
