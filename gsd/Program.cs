using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Net.Http;
using System.Net;
using System.IO;
using Quartz;

namespace gsd
{
    class Program
    {
        public static string captchaImageName = "captcha.png";
        public static string processResultImageName = "processTemp.bmp";
        public static string capchaCodeResultName = "ocr.txt";

        static void Main(string[] args)
        {
            //clear temp files
            clearTempFiles();

            //setup png watcher
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = AppDomain.CurrentDomain.BaseDirectory;
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName;
            watcher.Filter = "*.png";
            watcher.Created += Watcher_Created; ;
            watcher.EnableRaisingEvents = true;

            //setup quartz
            var schedulerFactory = new Quartz.Impl.StdSchedulerFactory();
            var schedular = schedulerFactory.GetScheduler();

            var job = JobBuilder.Create<CrawlerJob>()
                            .WithIdentity("Crawler")
                            .Build();

            var trigger = TriggerBuilder.Create()
                                .WithCronSchedule("0 0/1 * * * ?")
                                .WithIdentity("CrawlerTrigger")
                                .Build();

            schedular.ScheduleJob(job, trigger);
            schedular.Start();

            Console.WriteLine("Crawler will be raised at every minute");
        }

        //looking for captcha png
        private static void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Created && e.Name == captchaImageName)
            {
                using (var img = CaptchaHelper.process(captchaImageName))
                {
                    img.Save(processResultImageName);
                }
                var text = OCRHelper.getText(processResultImageName);
                var dateString = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss_fffff");
                System.IO.File.Copy(processResultImageName, text + "_" + dateString + ".bmp");
                System.IO.File.Copy(captchaImageName, text + "_" + dateString + "_src.png");
                System.IO.File.WriteAllText(capchaCodeResultName, text);
            }
        }

        public static void clearTempFiles()
        {
            System.IO.File.Delete(captchaImageName);
            System.IO.File.Delete(processResultImageName);
            System.IO.File.Delete(capchaCodeResultName);
        }
    }
}
