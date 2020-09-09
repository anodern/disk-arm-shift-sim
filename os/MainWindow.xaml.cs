using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace anodern.os {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow:Window {
        public MainWindow() {
            InitializeComponent();
        }

        private int maxIndex = 200;
        private int startIndex = 143;

        private int lastIndex;
        private int totalMove = 0;
        private List<int> list = null;
        private DispatcherTimer timer;
        int k;

        //先来先服务
        public void fcfs(object sender,EventArgs e) {
            //取出下一个地址
            int thisIndex = list[0];
            slider.Value=thisIndex;
            list.RemoveAt(0);
            //计算移动量
            int thisMove = Math.Abs(thisIndex-lastIndex);
            totalMove+=thisMove;
            //绘制
            drawCanvas(thisIndex);

            lastIndex = thisIndex;
            k++;
            if(list.Count<=0) timer.Stop();
        }

        //最短寻找时间优先
        public void sstf(object sender,EventArgs e) {
            //判断距离最短的地址
            int minIndex=0,minDistance=maxIndex;
            for(int j = 0;j < list.Count;j++) {
                if(Math.Abs(list[j]-lastIndex)<=minDistance) {
                    minDistance=Math.Abs(list[j]-lastIndex);
                    minIndex=j;
                }
            }
            int thisIndex = list[minIndex];
            slider.Value=thisIndex;
            list.RemoveAt(minIndex);
            
            //计算移动量
            int thisMove = Math.Abs(thisIndex-lastIndex);
            totalMove+=thisMove;
            //绘制
            drawCanvas(thisIndex);
            
            lastIndex = thisIndex;
            k++;
            if(list.Count<=0) timer.Stop();
        }

        //双向扫描
        int plus,lastx, lasty;
        public void scan(object sender,EventArgs e) {
            //判断是否要返回
            if(plus==1 && lastIndex==maxIndex-1) plus=-1;
            else if(plus==-1 && lastIndex==0) plus=1;

            //判断当前地址是否需要访问
            int thisIndex = lastIndex+plus;
            slider.Value=thisIndex;
            bool matched = false;
            if(list.Contains(thisIndex)) {
                matched=true;    //标记需要访问
                list.Remove(thisIndex);
            }

            //计算移动量
            int thisMove = Math.Abs(thisIndex-lastIndex);
            totalMove+=thisMove;

            //需要访问、移动到头尾时 画线
            if(matched || thisIndex==0 || thisIndex==maxIndex-1) {
                drawCanvas2(thisIndex);
                k++;
            }
            lastIndex = thisIndex;
            if(list.Count<=0) timer.Stop();
        }

        //电梯
        public void elev(object sender,EventArgs e) {
            bool stay=false;    //标记是否继续移动
            //判断是否需要换向
            for(int i = 0;i<list.Count;i++) {
                if(plus==1) if(list[i] >= lastIndex) stay=true;
                if(plus==-1) if(list[i] <= lastIndex) stay=true;
            }
            if(!stay) {
                if(plus==1) plus=-1;
                else if(plus==-1) plus=1;
            }

            //判断当前地址是否需要访问
            int thisIndex = lastIndex+plus;
            slider.Value=thisIndex;
            bool matched = false;
            if(list.Contains(thisIndex)) {
                matched=true;    //标记需要访问
                list.Remove(thisIndex);
            }

            //计算移动量
            int thisMove = Math.Abs(thisIndex-lastIndex);
            totalMove+=thisMove;

            //访问该数据 画线
            if(matched) {
                drawCanvas2(thisIndex);
                k++;
            }
            lastIndex = thisIndex;
            if(list.Count<=0) timer.Stop();
        }


        private Line draw(int thisIndex) {
            text_now.Content="当前位置："+thisIndex;
            text_total.Content="移动量："+totalMove;
            text_avg.Content="平均寻道长度："+(totalMove/(float)(k+1)).ToString("F2");
            //画线
            Line mydrawline = new Line();
            mydrawline.Stroke= Brushes.DarkGray;
            mydrawline.StrokeThickness = 2;//线宽
            mydrawline.Width = 600;
            mydrawline.Height = 300;
            return mydrawline;
        }

        private void drawCanvas(int thisIndex) {
            Line mydrawline = draw(thisIndex);
            mydrawline.X1 = (lastIndex/(float)maxIndex)*600;
            mydrawline.Y1 = k*15;
            mydrawline.X2 = (thisIndex/(float)maxIndex)*600;
            mydrawline.Y2 = (k+1)*15;
            //将直线添加到canvas
            canvas.Children.Add(mydrawline);
            Label label = new Label();
            label.Content=thisIndex;
            label.SetValue(Canvas.LeftProperty,(thisIndex/(float)maxIndex)*600.0-10);
            label.SetValue(Canvas.TopProperty,(k+1)*15.0-5);
            canvas.Children.Add(label);
        }

        private void drawCanvas2(int thisIndex) {
            Line mydrawline = draw(thisIndex);
            mydrawline.X1 = lastx;
            mydrawline.Y1 = lasty;
            bool noDraw = false;
            if(lastx==0) noDraw=true;
            lastx=(int)((thisIndex/(float)maxIndex)*600);
            lasty=k*15;
            mydrawline.X2 = lastx;
            mydrawline.Y2 = lasty;
            //将直线添加到canvas
            if(!noDraw) canvas.Children.Add(mydrawline);
            Label label = new Label();
            label.Content=thisIndex;
            label.SetValue(Canvas.LeftProperty,lastx-10.0);
            label.SetValue(Canvas.TopProperty,lasty-5.0);
            canvas.Children.Add(label);
        }

        private void btn_sim_Click(object sender,RoutedEventArgs e) {
            //开始模拟
            maxIndex=int.Parse(text_count.Text);
            int last = int.Parse(text_last.Text);
            startIndex=int.Parse(text_start.Text);
            if(startIndex-last>=0) plus=1;
            else plus=-1;

            if(maxIndex<10) maxIndex=10;
            startIndex = int.Parse(text_start.Text);
            if(startIndex>=maxIndex) startIndex=maxIndex-1;
            if(startIndex<0) startIndex=0;
            lastIndex=startIndex;
            lastx=lasty=0;

            string[] indexs = text_query.Text.Split(",");
            list = new List<int>();
            for(int i = 0;i<indexs.Length;i++) {
                int a;
                try {
                    a=int.Parse(indexs[i]);
                } catch(Exception ex) {
                    MessageBox.Show(ex.Message);
                    return;
                }
                //添加到访问序列list
                list.Add(a);
            }
            canvas.Children.Clear();
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0,0,0,0,500);

            if((bool)radio_fcfs.IsChecked) {
                //先来先服务
                totalMove=0;
                k=0;
                timer.Tick += new EventHandler(fcfs);
            } else if((bool)radio_sstf.IsChecked) {
                //最短寻道时间优先
                totalMove=0;
                k=0;
                timer.Tick += new EventHandler(sstf);
            } else if((bool)radio_scan.IsChecked) {
                //双向扫描
                totalMove=0;
                k=0;
                timer.Interval = new TimeSpan(0,0,0,0,30);
                timer.Tick += new EventHandler(scan);
            } else if((bool)radio_elev.IsChecked) {
                //电梯
                totalMove=0;
                k=0;
                timer.Interval = new TimeSpan(0,0,0,0,30);
                timer.Tick += new EventHandler(elev);
            }
            timer.Start();
        }

        private void btn_clear_Click(object sender,RoutedEventArgs e) {
            //清除画布
            canvas.Children.Clear();
        }

        private void text_count_TextChanged(object sender,TextChangedEventArgs e) {
            try {
                maxIndex = int.Parse(text_start.Text);
                slider.Maximum = maxIndex;
            } catch(Exception) {
                return;
            }
        }

        private void text_start_TextChanged(object sender,TextChangedEventArgs e) {
            try {
                startIndex = int.Parse(text_start.Text);
                slider.Value = startIndex;
            }catch(Exception) {
                return;
            }
        }
    }
}