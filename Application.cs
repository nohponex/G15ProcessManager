using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading;
using GammaJul.LgLcd;
using System.Diagnostics;

namespace G15ProcessManager
{
    public class Application
    {
        private static volatile bool _mustExit = false;

        private static LcdApplet Applet;
        private static LcdDeviceMonochrome Device;

        private static uint CurrentIndex;
        private static Font TextFontCurrent = new Font("Lucida Console", 7.0f);
        private static Font TextFont = new Font("Lucida Console", 6.5f);
        public static void Main(){

            Applet = new LcdApplet("G15 Process Manager", LcdAppletCapabilities.Monochrome);

            Applet.Configure += new EventHandler(Applet_Configure);

            Applet.Connect();

            Device = (LcdDeviceMonochrome)Applet.OpenDeviceByType(LcdDeviceType.Monochrome);

            Device.SoftButtonsChanged += new EventHandler<LcdSoftButtonsEventArgs>(Device_SoftButtonsChanged);

            CurrentIndex = 0;
            CreateGDIPages();

            do
            {
                Device.DoUpdateAndDraw();
                Thread.Sleep(3000);
            }
            while (!_mustExit);

            Applet.Disconnect();

           //Console.ReadKey();

        }
        private static Process[] GetProcesses()
        {
            Process[] ProcessList = Process.GetProcesses();

            //ProcessList.OrderByDescending(process => process.StartTime).ToArray();

            //Process[] newProcessList = new Process[ProcessList.Length]
            return ProcessList;
        }

        private static void CreateGDIPages()
        {
            Process[] ProcessList = GetProcesses();
            LcdGdiPage page = new LcdGdiPage(Device)
            {
                Children = {
                    new LcdGdiLine (Pens.Black,new PointF(00.0f,32.0f) , new PointF(260.0f,32.0f)),
                    new LcdGdiText {
                        //Up 
                        Text = "Up",
                        Margin = new MarginF(5.0f,32.0f),
                    },
                    new LcdGdiText {
                        //Down
                        Text = "Down",
                         Margin = new MarginF(20.0f,32.0f),
                    },
                     new LcdGdiText {
                        //Kill
                        Text = "Kill",
                        Margin = new MarginF(100.0f,32.0f),
                    },
                       new LcdGdiText {
                        //Exit
                        Text = "Exit",
                        Margin = new MarginF(135,32.0f),
                    },
                    new LcdGdiPolygon(Pens.Black,Brushes.White,new [] { new PointF(1.0f,0.1f) , new PointF(159.0f,0.1f) , new PointF(159.0f,9.5f),new PointF(1.0f,9.5f)},false),
                    new LcdGdiText {
                        //Current Process 
                        Text = ProcessList[CurrentIndex].ProcessName.ToString(),
                        Font = TextFontCurrent,
                        Margin = new MarginF(10.0f,0.0f),
                    },
                      new LcdGdiText {
                        //Current Process Info
                        Text = ProcessList[CurrentIndex].MainWindowTitle,
                        Font = TextFontCurrent,
                        Margin = new MarginF(60.0f,0.0f),
                    },
                    new LcdGdiText {
                        //+1
                        Text = ProcessList[CurrentIndex+1].ProcessName,
                        Font = TextFont,
                        Margin = new MarginF(10.0f,9.0f),
                    },
                    new LcdGdiText {
                        //+2
                        Text = ProcessList[CurrentIndex+2].ProcessName,
                        Font = TextFont,
                        Margin = new MarginF(10.0f,16.0f),
                    },
                    new LcdGdiText {
                        //+3
                        Text = ProcessList[CurrentIndex+3].ProcessName,
                        Font = TextFont,
                        Margin = new MarginF(10.0f,23.0f),
                    }
       		}
            };
            page.Updating += Page_Updating;

            // Adds page to the device's Pages collection (not mandatory, but helps for storing pages),
            // and sets the first page as the current page
            //Device.Pages.Clear();
            //Device.Pages.Add(page);
            Device.CurrentPage = page;
        }

        private static void Page_Updating(object sender, UpdateEventArgs e)
        {
            CreateGDIPages();
            //LcdGdiPage page = (LcdGdiPage)sender;
            //Console.WriteLine("Updating...");
        }

        static void Device_SoftButtonsChanged(object sender, LcdSoftButtonsEventArgs e)
        {
            if (e.SoftButtons == LcdSoftButtons.Button0)
            {
                Console.WriteLine("Button 1 ");
                if (CurrentIndex >= 1)
                {
                    CurrentIndex -= 1;
                    Device.DoUpdateAndDraw();
                }
            }
            if (e.SoftButtons == LcdSoftButtons.Button1)
            {
                Console.WriteLine("Button 2 ");
                if (CurrentIndex + 4 <= Process.GetProcesses().Length)
                {
                    CurrentIndex += 1;
                    CreateGDIPages();
                    //Device.DoUpdateAndDraw();
                }
            }
            if (e.SoftButtons == LcdSoftButtons.Button2)
            {
                Console.WriteLine("Kill... " + GetProcesses()[CurrentIndex].ProcessName);
                GetProcesses()[CurrentIndex].Kill();
                Device.DoUpdateAndDraw();
            }
            if (e.SoftButtons == LcdSoftButtons.Button3)
            {
                _mustExit = true;
            }
        }

        static void Applet_Configure(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
