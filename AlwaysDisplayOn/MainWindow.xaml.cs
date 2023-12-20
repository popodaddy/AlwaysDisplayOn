using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;


namespace AlwaysDisplayOn
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private int text_num = 0;
        private int term_time = 10000;

        // mouse 제어 start
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);//in WPF you can use this line of code to set mouse position.

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        // end

        System.ComponentModel.BackgroundWorker Worker;

        List<ComboItem> items;

        public class ComboItem
        {
            public string Name { get; set; }
            public int Time { get; set; }
        }

        public MainWindow()
        {
            InitializeComponent();

            items = new List<ComboItem>();
            items.Add(new ComboItem() { Name = "1sec", Time = 1 });
            items.Add(new ComboItem() { Name = "5sec", Time = 5 });
            items.Add(new ComboItem() { Name = "10sec", Time = 10 });
            items.Add(new ComboItem() { Name = "30sec", Time = 30 });
            items.Add(new ComboItem() { Name = "1min", Time = 60 });
            items.Add(new ComboItem() { Name = "3min", Time = 180 });
            this.combo_box.ItemsSource = items;

            start_bt.IsEnabled = false;

            this.Worker = new System.ComponentModel.BackgroundWorker();
            this.Worker.DoWork += new System.ComponentModel.DoWorkEventHandler(Worker_DoWork);
            this.Worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(Worker_RunWorkerCompleted);

            // キャンセル処理をできるようにする
            this.Worker.WorkerSupportsCancellation = true;

        }

        private void OnRendering(object sender, EventArgs e)
        {
            //text_block.Text = Mouse.GetPosition(this).ToString();
            //var current_left = System.Windows.Application.Current.MainWindow.Left;
            //var current_top = System.Windows.Application.Current.MainWindow.Top;

            //text_block.Text = current_left.ToString() + "," + current_top.ToString();

        }

        private static void LeftMouseClick(int Xposition, int Yposition) //Main window와 관계 없이, Display의 왼쪽 위 (0,0)부터 숫자를 카운트 한다.
        {
            SetCursorPos(Xposition, Yposition);
            mouse_event(MOUSEEVENTF_LEFTDOWN, Xposition, Yposition, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, Xposition, Yposition, 0, 0);
        }

        private bool first_click = true;
        Point pointToScreen;

        private void start_bt_Click(object sender, RoutedEventArgs e)
        {
            //Point point = Mouse.GetPosition(this); //Main window 안에서의 마우스 위치(좌표)를 get하는 함수.

            //var current_left = System.Windows.Application.Current.MainWindow.Left;
            //var current_top = System.Windows.Application.Current.MainWindow.Top;
            //var current_width = System.Windows.Application.Current.MainWindow.Width;
            //var current_height = System.Windows.Application.Current.MainWindow.Height;

            //int x_x = System.Convert.ToInt32(x);
            //int y_y = System.Convert.ToInt32(y);

            Point pointToWindow = Mouse.GetPosition(this);
            pointToScreen = PointToScreen(pointToWindow);

            text_block.Text = text_num.ToString();


            if (first_click)
            {
                start_bt.Content = "Input";
                //CompositionTarget.Rendering += OnRendering;
                this.Worker.RunWorkerAsync();
                first_click = false;
            }
            else
            {
                text_num += 1;
            }

        }

        void Worker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            int x = System.Convert.ToInt32(pointToScreen.X);
            int y = System.Convert.ToInt32(pointToScreen.Y);

            LeftMouseClick(x, y);

            System.Threading.Thread.Sleep(term_time);
        }
        


        // 「時間のかかる処理」終了時の処理
        void Worker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            // バックグラウンド処理の実行
            this.Worker.RunWorkerAsync();
        }

        private void combo_box_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected_index_num = combo_box.SelectedIndex;
            if (selected_index_num == 0)
            {
                term_time = 1000;
            }
            if (selected_index_num == 1)
            {
                term_time = 5000;
            }
            if (selected_index_num == 2)
            {
                term_time = 10000;
            }
            if (selected_index_num == 3)
            {
                term_time = 30000;
            }
            if (selected_index_num == 4)
            {
                term_time = 60000;
            }
            if (selected_index_num == 5)
            {
                term_time = 180000;
            }

            start_bt.IsEnabled = true;

        }


    }
}

