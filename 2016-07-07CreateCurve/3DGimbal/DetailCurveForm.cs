using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZedGraph;

namespace _3DGimbal
{
    public partial class DetailCurveForm : Form
    {
        private float[] existData;
        private int curveType;
        private int position;
        /// <summary>
        /// 绘制曲线的背景面板
        /// </summary>
        private GraphPane myPane = null;
        private LineItem myCurve = null;
        private Chart myChart = null;
        private PointPairList myCurveList = new PointPairList();
        private float xAxis = 0.0f;

        public DetailCurveForm(float[] existdata, int typeCurve, int pos)
        {
            InitializeComponent();
            myPane = DetailCurveControl.GraphPane;
            myChart = myPane.Chart;

            myChart.Fill = new Fill(Color.Black);
            myPane.Fill = new Fill(Color.DarkGray);
            existData = existdata;
            curveType = typeCurve;
            position = pos;
            initCurve(existdata,pos);
        }

        private void initCurve(float[] existdata,int pos)
        {
            if (existdata[pos + 1] != 0.0f)  //后面为空,且不为最后一个
            {
                for (int i = 0; i < pos; i++)
                {
                    myCurveList.Add(xAxis, existdata[i]);
                    xAxis++;
                }
            }
            else
            {
                for (int j = pos + 1; j < 1024;j++ )
                {
                    myCurveList.Add(xAxis, existdata[j]);
                    xAxis++;
                }
                for (int m = 0; m < pos;m++ )
                {
                    myCurveList.Add(xAxis, existdata[m]);
                    xAxis++;
                }
            }
        }

        private void zedGraphControl1ChangeAxis()
        {
            myPane.XAxis.Scale.Min = xAxis - 50;
            myPane.XAxis.Scale.Max = xAxis;
            myPane.XAxis.Scale.MinorStep = 1;//X轴小步长1,也就是小间隔
            myPane.XAxis.Scale.MajorStep = 5;//X轴大步长为5，也就是显示文字的大间隔
            this.DetailCurveControl.AxisChange();
            this.DetailCurveControl.Refresh();  //更新界面
        }
        private void initGraphPane(GraphPane myPane)
        {
            Chart myChart = myPane.Chart;
            myChart.Fill = new Fill(Color.DarkGray);
            myPane.Fill = new Fill(Color.DarkGray);
            //this.zedGraphControl1.GraphPane.Chart.Fill = new Fill(Color.Transparent, Color.Transparent, 45.0f);
            //this.zedGraphControl1.MasterPane.Fill = new Fill(Color.Transparent, Color.Transparent, 45.0f);
            //this.zedGraphControl1.GraphPane.Fill.Color = Color.Transparent;  
            myPane.Title.Text = "波形分析";
            myPane.XAxis.Title.Text = "时间";
            myPane.YAxis.Title.Text = "数值";
            myCurve = myPane.AddCurve("PWM", myCurveList, Color.Lime, SymbolType.None);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            xAxis += 1.0f;
            zedGraphControl1ChangeAxis();
        }

        public void UpdateData(float newData)
        {
            myCurveList.Add(xAxis, newData);
        }


        
    }
}
