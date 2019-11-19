using SmartHealth.Shared.ChatConnectivity;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ChatClient
{
    class Program
    {
        static ChatConnectivity chatConnectivity;
        static string room;
        static string user;
        static string url;
        static async Task Main(string[] args)
        {
            user = "Console " + new Random().Next(0, 100);
            chatConnectivity = new ChatConnectivity();
            chatConnectivity.OnReceivedMessage += ChatConnectivity_OnReceivedMessage;
            chatConnectivity.OnEnteredOrExited += ChatConnectivity_OnEnteredOrExited;
            
            Console.WriteLine("Enter chat server IP:");
            url = Console.ReadLine();
            
            chatConnectivity.Init(url);
            await chatConnectivity.ConnectAsync();
            
            Console.WriteLine("You're connected...");
            
            await JoinRoom();
            
            while (true)
            {
                var text = Console.ReadLine();
                if (text == "exit") break;
                else if (text == "leave")
                {
                    await chatConnectivity.LeaveChannelAsync(room, user);
                    await JoinRoom();
                }
                else
                {
                    await chatConnectivity.SendMessageAsync(room, user, text);
                }
            }
            Environment.Exit(0);
        }

        private static void ChatConnectivity_OnEnteredOrExited(SmartHealth.Shared.ChatConnectivity.EventArgs.MessageEventArgs obj)
        {
            Console.WriteLine(obj.Message);
        }

        static async Task JoinRoom()
        {
            Console.WriteLine($"Hi {user}, now type the room name to join: AWS Lex Room");
            room = Console.ReadLine();

            if (room == "exit") Environment.Exit(0);
            await chatConnectivity.JoinChannelAsync(room, user);
            Console.WriteLine($"Entered room {room}. {chatConnectivity.ActiveUsers.Count} users online.");
        }

        private static async void ChatConnectivity_OnReceivedMessage(SmartHealth.Shared.ChatConnectivity.EventArgs.MessageEventArgs obj)
        {
            if (obj.User == user)
            {
                Console.WriteLine($"Answered: {obj.Message}");
                return;
            }

            Console.WriteLine(obj.Message);

            // process message

            // send BOT response
            await SendMessage($"Great, got your message: {obj.Message}");
        }

        static async Task SendMessage(string message)
        {
            await chatConnectivity.SendMessageAsync(room, user, message);
        }
    }
}
