using NReco.PhantomJS;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gsd
{
    class CrawlerJob : IJob
    {
        protected string _login = "";
        protected string _password = "";
        public void Execute(IJobExecutionContext context)
        {
            if (string.IsNullOrWhiteSpace(this._login)
                || string.IsNullOrWhiteSpace(this._password))
                throw new Exception("must setup login and password!! (CrawlerJob.cs)");

            Console.WriteLine("["+ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "]CrawlerJob Triggered");

            Program.clearTempFiles();

            var phantomJS = new PhantomJS();
            phantomJS.OutputReceived += (sender, e) => {
                Console.WriteLine("PhantomJS output: {0}", e.Data);
            };
            phantomJS.ErrorReceived += (sender, e) => {
                Console.WriteLine("PhantomJS error: {0}", e.Data);
            };
            try
            {
                var login_username = "var login_username = '" + this._login + "';\r\n";
                var login_password = "var login_password = '" + this._password + "';\r\n";
                var script = login_username 
                    + login_password 
                    + System.IO.File.ReadAllText("crawler.js");
                phantomJS.RunScript(script, null);
            }
            finally
            {
                phantomJS.Abort(); // ensure that phantomjs.exe is stopped
            }

            Console.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "]CrawlerJob Done!!!");
        }
    }
}
