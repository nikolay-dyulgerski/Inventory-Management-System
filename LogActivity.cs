using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senior_Project
{
    public static class LogActivity
    {
        
            private static readonly string FilePath = "activity.log";
            public static void WriteLog( string acvivity){
                try
                {
                    using(StreamWriter sw = new StreamWriter(FilePath, true))
                    {
                        sw.WriteLine($"{DateTime.Now:G}: User '{SessionManager.CurrentUser}': {acvivity}");
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Error writing log: {ex.Message}");
                }
                return;
} 
        }
    }

