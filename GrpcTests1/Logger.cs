using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpcTests
{
    public static class Logger
    {
        public static string Id { get; set; }
        public static void Log(string message) => Console.WriteLine($"[{DateTime.Now}][{Id}]{message}");
    }
}
