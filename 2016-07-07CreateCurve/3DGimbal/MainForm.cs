using System;
using System.Drawing;
using System.Windows.Forms;
using SerialControl;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay;
using CurveDll;
using System.Collections.Generic;
using System.Threading;
using ZedGraph;
using System.IO;
using System.Text;

namespace _3DGimbal
{
    public partial class MainForm : Form
    {
        #region 全局变量
        /// <summary>
        /// 左侧连接按钮是否被点击
        /// </summary>
        private bool isConnectClick = false;
        /// <summary>
        /// 左侧波形分析按钮是否被点击
        /// </summary>
        private bool isWaveClick = false;
        /// <summary>
        /// 左侧波形回放按钮是否被点击
        /// </summary>
        private bool isPlaybackWaveClick = false;
        /// <summary>
        /// 左侧技术支持按钮是否被点击
        /// </summary>
        private bool isTechnicalSupportClick = false;

        /// <summary>
        /// 鼠标移动位置变量
        /// </summary>
        private Point mouseOff;
        /// <summary>
        /// 标签是否为左键
        /// </summary>
        private bool leftFlag;

        /// <summary>
        /// 左侧动态分析是否被点击
        /// </summary>
        private bool isDynamicAnalysisClick = false;
        /// <summary>
        /// 左侧静态态分析是否被点击
        /// </summary>
        private bool isStaticAnalysisClick = false;
        /// 左侧数据库回放按钮是否被点击
        /// </summary>
        private bool isDatabasePlaybackClick = false;
        /// <summary>
        /// 左侧文本按钮是否被点击
        /// </summary>
        private bool isTxtPlaybackClick = false;
        /// <summary>
        /// 左侧软件支持按钮是否被点击
        /// </summary>
        private bool isSoftwaveSupportClick = false;
        /// <summary>
        /// 左侧硬件支持按钮是否被点击
        /// </summary>
        private bool isHardwareSupportClick = false;
        /// <summary>
        /// 设置PWM波的占空比
        /// </summary>
        private float PWMSettingValue = 0.0f;
        /// <summary>
        /// 串口
        /// </summary>
        private SerialCom serialCom;
        /// <summary>
        /// 从串口中读取100条数据的存储队列，用于存储重量
        /// </summary>
        private float[] portData = new float[100]; 
        /// <summary>
        /// 计数，计100次数
        /// </summary>
        private int countAve = 0;
        /// <summary>
        /// 是否进行标定，每次开机启动标定一次
        /// </summary>
        private bool isCalibration = true; 
        /// <summary>
        /// 标定的值，用于标定重物的重量
        /// </summary>
        private float calibrationValue = 0.0f;
        /// <summary>
        /// 设置的PWM值
        /// </summary>
        private float PWMCurveData = 0f;
        /// <summary>
        /// 转速的数据缓存队列
        /// </summary>
        private float[] rotateDataCache = new float[1024];
        private int rotateCachePosition = 0;
        /// <summary>
        /// 电流的数据缓存队列
        /// </summary>
        private float[] currentDataCache = new float[1024];
        private int currentCachePosition = 0;
        /// <summary>
        /// 升力的数据缓存队列
        /// </summary>
        private float[] liftDataChche = new float[1024];
        private int liftCachePosition = 0;
        /// <summary>
        /// pwm的数据缓存队列
        /// </summary>
        private float[] pwmDataCache = new float[1024];
        private int pwmCachePosition = 0;
        /// <summary>
        /// 电压的数据缓存队列
        /// </summary>
        private float[] voltageDataCache = new float[1024];
        private int voltageCachePosition = 0;
        /// <summary>
        /// 效率的数据缓存队列
        /// </summary>
        private float[] efficiencyDataCache = new float[1024];
        private int efficiencyCachePosition = 0;

        #endregion

        //private static CCurveInitialise curveInitialise = new CCurveInitialise("无人机状态曲线图", "0.1s", "度");
        //private static ObservableDataSource<System.Windows.Point> dataSource = new ObservableDataSource<System.Windows.Point>();
        //private CCurveDrow curveDrowT = new CCurveDrow(curveInitialise, dataSource, System.Windows.Media.Colors.Gold, 3, "IMU温度");

        public MainForm()
        {
            InitializeComponent();
            serialCom = new SerialCom();
            serialCom.Visible = false;
            myPane = zedGraphControl1.GraphPane;
            initGraphPane(myPane);  
            secondPerformsTabPanel.Visible = false;
            secondTechSupportPanel.Visible = false;
            secondPlaybackPanel.Visible = false;
            btnSettingPWM.Enabled = false;
            if (!File.Exists(@"C:\\Users\\Silenoff\\Desktop\\A.txt"))
            {
                fs = new FileStream("C:\\Users\\Silenoff\\Desktop\\A.txt", FileMode.Append);
                sw = new StreamWriter(fs, Encoding.UTF8);
            }
            else
            {
                File.Delete(@"C:\\Users\\Silenoff\\Desktop\\A.txt");
                fs = new FileStream("C:\\Users\\Silenoff\\Desktop\\A.txt", FileMode.Append);
                sw = new StreamWriter(fs, Encoding.UTF8);
            }
            

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
            realCurve = myPane.AddCurve("真实", realCurveList, setRealCurveColor(), SymbolType.None);
            orderCurve = myPane.AddCurve("命令", orderCurveList, setOrderCurveColor(), SymbolType.None);
        }
        private Color setRealCurveColor()
        {
            return Color.Red;
        }
        private Color setOrderCurveColor()
        {
            return Color.Blue;
        }

        #region 顶部鼠标左键拖动程序位置
        /// <summary>
        /// 鼠标在正上方区域按下的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void centerTabPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseOff = new Point(-e.X, -e.Y); //得到变量的值
                leftFlag = true;                  //点击左键按下时标注为true;
            }
        }
        /// <summary>
        /// 鼠标在正上方区域移动的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void centerTabPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (leftFlag)
            {
                Point mouseSet = Control.MousePosition;
                mouseSet.Offset(mouseOff.X - leftTabPanel.Size.Width, mouseOff.Y);  //设置移动后的位置
                Location = mouseSet;
            }
        }
        /// <summary>
        /// 鼠标在正上方区域取消按下的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void centerTabPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (leftFlag)
            {
                leftFlag = false;//释放鼠标后标注为false;
            }
        }
        #endregion

        #region 设置鼠标进入item的背景颜色逻辑
        /// <summary>
        /// 鼠标进入连接无人机控件的时候，设置字体的背景颜色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void connectLabelText_MouseEnter(object sender, EventArgs e)
        {
            connectLabelText.ForeColor = setMouseInLabelTextColor();
        }
        /// <summary>
        /// 鼠标离开连接无人机控件的时候，设置字体的背景颜色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void connectLabelText_MouseLeave(object sender, EventArgs e)
        {
            connectLabelText.ForeColor = setMouseInLabelTextColor();
        }
        /// <summary>
        /// 鼠标进入连接无人机中右边的图片区域时，设置字体的背景颜色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void connectPicBox_MouseEnter(object sender, EventArgs e)
        {
            connectLabelText.ForeColor = setMouseInLabelTextColor();
        }
        /// <summary>
        /// 鼠标离开连接无人机中右边的图片区域时，设置字体的背景颜色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void connectPicBox_MouseLeave(object sender, EventArgs e)
        {
            connectLabelText.ForeColor = setMouseInLabelTextColor();
        }
        /// <summary>
        /// 鼠标点击连接无人机右边的图片时，把监听事件传递给其父类面板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void connectPicBox_Click(object sender, EventArgs e)
        {
            connectTabPanel_Click(sender, e);
        }
        /// <summary>
        /// 鼠标点击连接无人机左边的字体时，把监听事件传递给其父类面板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void connectBtnText_Click(object sender, EventArgs e)
        {
            connectTabPanel_Click(sender, e);
        }

        private void waveLabelText_Click(object sender, EventArgs e)
        {
            waveTabPanel_Click(sender, e);
        }

        private void wavePicBox_Click(object sender, EventArgs e)
        {
            waveTabPanel_Click(sender, e);
        }

        private void connectTabPanel_MouseEnter(object sender, EventArgs e)
        {
            connectLabelText.ForeColor = setMouseInLabelTextColor();
        }

        private void waveTabPanel_MouseEnter(object sender, EventArgs e)
        {
            waveLabelText.ForeColor = setMouseInLabelTextColor();
        }

        private void connectTabPanel_MouseLeave(object sender, EventArgs e)
        {
            connectLabelText.ForeColor = setMouseDefaultLabelTextColor();
        }

        private void waveTabPanel_MouseLeave(object sender, EventArgs e)
        {
            waveLabelText.ForeColor = setMouseDefaultLabelTextColor();
        }

        private void waveLabelText_MouseEnter(object sender, EventArgs e)
        {
            waveLabelText.ForeColor = setMouseInLabelTextColor();
        }

        private void waveLabelText_MouseLeave(object sender, EventArgs e)
        {
            waveLabelText.ForeColor = setMouseInLabelTextColor();
        }

        private void wavePicBox_MouseEnter(object sender, EventArgs e)
        {
            waveLabelText.ForeColor = setMouseInLabelTextColor();
        }

        private void wavePicBox_MouseLeave(object sender, EventArgs e)
        {
            waveLabelText.ForeColor = setMouseInLabelTextColor();
        }

        private void exitLabelText_Click(object sender, EventArgs e)
        {
            exitTabPanel_Click(sender, e);
        }
        private void playbackWaveTabPanel_MouseEnter(object sender, EventArgs e)
        {
            playbackWaveLabelText.ForeColor = setMouseInLabelTextColor();
        }

        private void playbackWaveTabPanel_MouseLeave(object sender, EventArgs e)
        {
            playbackWaveLabelText.ForeColor = setMouseDefaultLabelTextColor();
        }

        private void playbackWaveLabelText_MouseEnter(object sender, EventArgs e)
        {
            playbackWaveLabelText.ForeColor = setMouseInLabelTextColor();
        }

        private void playbackWaveLabelText_MouseLeave(object sender, EventArgs e)
        {
            playbackWaveLabelText.ForeColor = setMouseDefaultLabelTextColor();
        }

        private void playbackWavePicBox_MouseEnter(object sender, EventArgs e)
        {
            playbackWaveLabelText.ForeColor = setMouseInLabelTextColor();
        }

        private void playbackWavePicBox_MouseLeave(object sender, EventArgs e)
        {
            playbackWaveLabelText.ForeColor = setMouseDefaultLabelTextColor();
        }

        private void technicalSupportTabPanel_MouseEnter(object sender, EventArgs e)
        {
            technicalSupportLabelText.ForeColor = setMouseInLabelTextColor();
        }

        private void technicalSupportTabPanel_MouseLeave(object sender, EventArgs e)
        {
            technicalSupportLabelText.ForeColor = setMouseDefaultLabelTextColor();
        }

        private void technicalSupportLabelText_MouseEnter(object sender, EventArgs e)
        {
            technicalSupportLabelText.ForeColor = setMouseInLabelTextColor();
        }

        private void technicalSupportLabelText_MouseLeave(object sender, EventArgs e)
        {
            technicalSupportLabelText.ForeColor = setMouseDefaultLabelTextColor();
        }

        private void technicalSupportPicBox_MouseEnter(object sender, EventArgs e)
        {
            technicalSupportLabelText.ForeColor = setMouseInLabelTextColor();
        }

        private void technicalSupportPicBox_MouseLeave(object sender, EventArgs e)
        {
            technicalSupportLabelText.ForeColor = setMouseDefaultLabelTextColor();
        }

        private void exitLabelText_MouseEnter(object sender, EventArgs e)
        {
            exitLabelText.ForeColor = setMouseInLabelTextColor();
        }

        private void exitLabelText_MouseLeave(object sender, EventArgs e)
        {
            exitLabelText.ForeColor = setMouseDefaultLabelTextColor();
        }

        private void exitTabPanel_MouseEnter(object sender, EventArgs e)
        {
            exitLabelText.ForeColor = setMouseInLabelTextColor();
        }

        private void exitTabPanel_MouseLeave(object sender, EventArgs e)
        {
            exitLabelText.ForeColor = setMouseDefaultLabelTextColor();
        }

        private void playbackWaveLabelText_Click(object sender, EventArgs e)
        {
            playbackWaveTabPanel_Click(sender, e);
        }

        private void playbackWavePicBox_Click(object sender, EventArgs e)
        {
            playbackWaveTabPanel_Click(sender, e);
        }

        

        private void technicalSupportLabelText_Click(object sender, EventArgs e)
        {
            technicalSupportTabPanel_Click(sender, e);
        }

        private void technicalSupportPicBox_Click(object sender, EventArgs e)
        {
            technicalSupportTabPanel_Click(sender, e);
        }

        #endregion

        #region 左侧菜单的界面逻辑
        /// <summary>
        /// 点击了连接按钮时，显示连接的控件
        /// </summary>
        private void connect_Click_Position()
        {
            if (isWaveClick)
            {
                closeWavePanel();
                isWaveClick = !isWaveClick;
            }
            if (isPlaybackWaveClick)
            {
                closePlaybackPanel();
                isPlaybackWaveClick = !isPlaybackWaveClick;
            }
            if (isTechnicalSupportClick)
            {
                closeTechnicalSupportPanel();
                isTechnicalSupportClick = !isTechnicalSupportClick;
            }
            isConnectClick = !isConnectClick;
            if (isConnectClick)
            {
                openConnectPanel();
            }
            else
            {
                closeConnectPanel();
            }
        }
        /// <summary>
        /// 连接面板打开时，对界面的布局
        /// </summary>
        private void openConnectPanel()
        {
            int connectTabPanelSizeX = connectTabPanel.Location.X;
            connectTabPanel.Size = new System.Drawing.Size(connectTabPanelSizeX, serialCom.Size.Height + connectTabPanel.Size.Height);
            serialCom.Parent = connectTabPanel;
            serialCom.Location = new Point(connectTabPanel.Margin.Left, getLeftItemPanelHeight());
            serialCom.Visible = true;
            Image img = connectPicBox.Image;
            img.RotateFlip(RotateFlipType.Rotate90FlipNone);
            connectPicBox.Image = img;
        }
        /// <summary>
        /// 关闭连接面板，界面的布局发生相应的变化
        /// </summary>
        private void closeConnectPanel()
        {
            int connectTabPanelSizeX = connectTabPanel.Location.X;
            connectTabPanel.Size = new System.Drawing.Size(connectTabPanelSizeX, getLeftItemPanelHeight());

            Image img = connectPicBox.Image;
            img.RotateFlip(RotateFlipType.Rotate270FlipNone);
            connectPicBox.Image = img;
        }
        /// <summary>
        /// 点击了波形分析按钮时，显示分析的控件
        /// </summary>
        private void wave_Click_Position()
        {
            if (isConnectClick)
            {
                closeConnectPanel();
                isConnectClick = !isConnectClick;
            }
            if (isPlaybackWaveClick)
            {
                closePlaybackPanel();
                isPlaybackWaveClick = !isPlaybackWaveClick;
            }
            if (isTechnicalSupportClick)
            {
                closeTechnicalSupportPanel();
                isTechnicalSupportClick = !isTechnicalSupportClick;
            }
            isWaveClick = !isWaveClick;
            if (isWaveClick)
            {
                openWavePanel();
            }
            else
            {
                closeWavePanel();
            }
        }
        /// <summary>
        /// 打开波形分析面板时的界面布局
        /// </summary>
        private void openWavePanel()
        {
            int waveTabPanelSizeX = performsAnalysisTabPanel.Location.X;
            performsAnalysisTabPanel.Size = new System.Drawing.Size(waveTabPanelSizeX, getLeftWavePanelClickedHeight());
            Image img = wavePicBox.Image;
            img.RotateFlip(RotateFlipType.Rotate90FlipNone);
            wavePicBox.Image = img;

            secondPerformsTabPanel.Parent = performsAnalysisTabPanel;
            secondPerformsTabPanel.Location = new System.Drawing.Point(3, getLeftItemPanelHeight());//位置
            secondPerformsTabPanel.Visible = true;
        }
        /// <summary>
        /// 关闭波形分析面板时界面布局
        /// </summary>
        private void closeWavePanel()
        {
            int waveTabPanelSizeX = performsAnalysisTabPanel.Location.X;
            performsAnalysisTabPanel.Size = new System.Drawing.Size(waveTabPanelSizeX, getLeftItemPanelHeight());
            Image img = wavePicBox.Image;
            img.RotateFlip(RotateFlipType.Rotate270FlipNone);
            wavePicBox.Image = img;
        }
        /// <summary>
        /// 点击了波形回放按钮时，显示回放的控件
        /// </summary>
        private void playbackWave_Click_Position()
        {
            if (isConnectClick)
            {
                closeConnectPanel();
                isConnectClick = !isConnectClick;
            }
            if (isWaveClick)
            {
                closeWavePanel();
                isWaveClick = !isWaveClick;
            }
            if (isTechnicalSupportClick)
            {
                closeTechnicalSupportPanel();
                isTechnicalSupportClick = !isTechnicalSupportClick;
            }
            isPlaybackWaveClick = !isPlaybackWaveClick;
            if (isPlaybackWaveClick)
            {
                openPlaybackPanel();
            }
            else
            {
                closePlaybackPanel();
            }
        }
        /// <summary>
        /// 打开波形回放面板的界面布局
        /// </summary>
        private void openPlaybackPanel()
        {
            int playbackWaveTabPanelSizeX = playbackWaveTabPanel.Location.X;
            playbackWaveTabPanel.Size = new System.Drawing.Size(playbackWaveTabPanelSizeX, getLeftPlaybackPanelClickedHeight());
            Image img = playbackWavePicBox.Image;
            img.RotateFlip(RotateFlipType.Rotate90FlipNone);
            playbackWavePicBox.Image = img;

            secondPlaybackPanel.Parent = playbackWaveTabPanel;
            secondPlaybackPanel.Location = new System.Drawing.Point(3, getLeftItemPanelHeight());//位置
            secondPlaybackPanel.Visible = true;

        }
        /// <summary>
        /// 关闭波形回放面板的界面布局
        /// </summary>
        private void closePlaybackPanel()
        {
            int playbackWaveTabPanelSizeX = playbackWaveTabPanel.Location.X;
            playbackWaveTabPanel.Size = new System.Drawing.Size(playbackWaveTabPanelSizeX, getLeftItemPanelHeight());

            Image img = playbackWavePicBox.Image;
            img.RotateFlip(RotateFlipType.Rotate270FlipNone);
            playbackWavePicBox.Image = img;
        }
        /// <summary>
        /// 点击了技术支持按钮时，显示支持的控件
        /// </summary>
        private void technicalSupport_Click_position()
        {
            if (isConnectClick)
            {
                closeConnectPanel();
                isConnectClick = !isConnectClick;
            }
            if (isPlaybackWaveClick)
            {
                closePlaybackPanel();
                isPlaybackWaveClick = !isPlaybackWaveClick;
            }
            if (isWaveClick)
            {
                closeWavePanel();
                isWaveClick = !isWaveClick;
            }
            isTechnicalSupportClick = !isTechnicalSupportClick;
            if (isTechnicalSupportClick)
            {
                openTechnicalSupportPanel();
            }
            else
            {
                closeTechnicalSupportPanel();
            }
        }
        /// <summary>
        /// 打开技术支持的面板时的界面布局
        /// </summary>
        private void openTechnicalSupportPanel()
        {
            int technicalSupportTabPanelSizeX = technicalSupportTabPanel.Location.X;
            technicalSupportTabPanel.Size = new System.Drawing.Size(technicalSupportTabPanelSizeX, getLeftTechnicalSupportPanelClickedHeight());
            Image img = technicalSupportPicBox.Image;
            img.RotateFlip(RotateFlipType.Rotate90FlipNone);
            technicalSupportPicBox.Image = img;
            secondTechSupportPanel.Parent = technicalSupportTabPanel;
            secondTechSupportPanel.Location = new System.Drawing.Point(3, getLeftItemPanelHeight());//位置
            secondTechSupportPanel.Visible = true;
        }
        /// <summary>
        /// 关闭技术支持的界面布局
        /// </summary>
        private void closeTechnicalSupportPanel()
        {
            int technicalSupportTabPanelSizeX = technicalSupportTabPanel.Location.X;
            technicalSupportTabPanel.Size = new System.Drawing.Size(technicalSupportTabPanelSizeX, getLeftItemPanelHeight());

            Image img = technicalSupportPicBox.Image;
            img.RotateFlip(RotateFlipType.Rotate270FlipNone);
            technicalSupportPicBox.Image = img;
        }
        #endregion

        #region 左侧子菜单界面颜色逻辑
        /// <summary>
        /// 动态分析的颜色逻辑
        /// </summary>
        private void DynamicAnalysisColorLogic()
        {
            isDynamicAnalysisClick = true;
            if (isStaticAnalysisClick)
            {
                isStaticAnalysisClick = !isStaticAnalysisClick;
                StaticAnalysis.BackColor = Color.FromArgb(255, 51, 63, 75);
            }
            DynamicAnalysis.BackColor = Color.White;
        }
        /// <summary>
        /// 静态分析的颜色逻辑
        /// </summary>
        private void StaticAnalysisColorLogic()
        {
            isStaticAnalysisClick = true;
            if (isDynamicAnalysisClick)
            {
                isDynamicAnalysisClick = !isDynamicAnalysisClick;
                DynamicAnalysis.BackColor = Color.FromArgb(255, 51, 63, 75);
            }
            StaticAnalysis.BackColor = Color.White;
        }
        /// <summary>
        /// 数据库回放的颜色逻辑
        /// </summary>
        private void DatabasePlaybackColorLogic()
        {
            isDatabasePlaybackClick = true;
            if (isTxtPlaybackClick)
            {
                isTxtPlaybackClick = !isTxtPlaybackClick;
                TxtPlayback.BackColor = Color.FromArgb(255, 51, 63, 75);
            }

            DatabasePlayback.BackColor = Color.White;
        }
        /// <summary>
        /// 文本回放的颜色逻辑
        /// </summary>
        private void TxtPlaybackColorLogic()
        {
            isTxtPlaybackClick = true;
            if (isDatabasePlaybackClick)
            {
                isDatabasePlaybackClick = !isDatabasePlaybackClick;
                DatabasePlayback.BackColor = Color.FromArgb(255, 51, 63, 75);
            }

            TxtPlayback.BackColor = Color.White;
        }
        /// <summary>
        /// 软件支持的颜色逻辑
        /// </summary>
        private void softwaveSupportColorLogic()
        {
            isSoftwaveSupportClick = true;
            if (isHardwareSupportClick)
            {
                isHardwareSupportClick = !isHardwareSupportClick;
                hardwareSupport.BackColor = Color.FromArgb(255, 51, 63, 75);
            }

            softwaveSupport.BackColor = Color.White;
        }
        /// <summary>
        /// 硬件支持的颜色逻辑
        /// </summary>
        private void hardwareSupportColorLogic()
        {
            isHardwareSupportClick = true;
            if (isSoftwaveSupportClick)
            {
                isSoftwaveSupportClick = !isSoftwaveSupportClick;
                softwaveSupport.BackColor = Color.FromArgb(255, 51, 63, 75);
            }

            hardwareSupport.BackColor = Color.White;
        }
        #endregion

        #region 左侧菜单属性值设置
        /// <summary>
        /// 设置鼠标进入面板时字体的颜色
        /// </summary>
        /// <returns></returns>
        private Color setMouseInLabelTextColor()
        {
            return Color.Yellow;
        }
        /// <summary>
        /// 设置左侧菜单字体面板默认的字体颜色
        /// </summary>
        /// <returns></returns>
        private Color setMouseDefaultLabelTextColor()
        {
            return SystemColors.ActiveCaption;
        }
        /// <summary>
        /// 设置鼠标点击面板时字体的颜色
        /// </summary>
        /// <returns></returns>
        private Color setMouseClickLabelTextColor()
        {
            return Color.Pink;
        }
        /// <summary>
        /// 设置左边菜单每一行面板的高度
        /// </summary>
        /// <returns></returns>
        private int getLeftItemPanelHeight()
        {
            return 41;
        }
        /// <summary>
        /// 设置左边波形分析被打开时面板的高度
        /// </summary>
        /// <returns></returns>
        private int getLeftWavePanelClickedHeight()
        {
            return 82;
        }
        /// <summary>
        /// 设置左边波形产生器被打开时面板的高度
        /// </summary>
        /// <returns></returns>
        private int getLeftCreateWavePanelClickedHeight()
        {
            return 125;
        }
        /// <summary>
        /// 设置左边回放面板被打开的高度
        /// </summary>
        /// <returns></returns>
        private int getLeftPlaybackPanelClickedHeight()
        {
            return 80;
        }
        /// <summary>
        /// 设置左边技术支持被打开时面板的高度
        /// </summary>
        /// <returns></returns>
        private int getLeftTechnicalSupportPanelClickedHeight()
        {
            return 80;
        }
        #endregion

        #region 左侧菜单点击事件
        /// <summary>
        /// 鼠标点击连接无人机,对其面板的位置和内容进行定位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void connectTabPanel_Click(object sender, EventArgs e)
        {
            connectLabelText.ForeColor = setMouseClickLabelTextColor();
            connect_Click_Position();
            tabControl.SelectedIndex = 0;
        }
        /// <summary>
        /// 波形分析面板的点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void waveTabPanel_Click(object sender, EventArgs e)
        {
            if (!serialCom.isComOpen())
            {
                MessageBox.Show("未打开串口，请先打开串口", "警告");
                return;
            }
            if (isDynamicAnalysisClick)
            {
                tabControl.SelectedIndex = 1;
            }
            if (isStaticAnalysisClick)
            {
                tabControl.SelectedIndex = 2;
            }
            waveLabelText.ForeColor = setMouseClickLabelTextColor();
            wave_Click_Position();
        }
        /// <summary>
        /// 退出面板被点击时的相应函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitTabPanel_Click(object sender, EventArgs e)
        {
            if (!textBoxPWMSetting.Text.Equals("0"))
            {
                serialCom.txFrame.fnCode = FunctionCode.FC_MOTOR_SET_VEL;
                BitConverter.GetBytes(0.0f).CopyTo(serialCom.txFrame.data, 0);
                serialCom.txFrame.isUpdated = true;
                serialCom.SendFrame();
            }
            if (sw!=null&&fs!=null)
            {
                sw.Close();
                fs.Close();
            }
            
            System.Environment.Exit(0);
        }
        /// <summary>
        /// 数据回放面板的点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void playbackWaveTabPanel_Click(object sender, EventArgs e)
        {
            playbackWaveLabelText.ForeColor = setMouseClickLabelTextColor();
            if (isDatabasePlaybackClick)
            {
                tabControl.SelectedIndex = 3;
            }
            if (isTxtPlaybackClick)
            {
                tabControl.SelectedIndex = 4;
            }
            playbackWave_Click_Position();
        }
        /// <summary>
        /// 技术支持的点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void technicalSupportTabPanel_Click(object sender, EventArgs e)
        {
            technicalSupportLabelText.ForeColor = setMouseClickLabelTextColor();
            if (isSoftwaveSupportClick)
            {
                tabControl.SelectedIndex = 5;
            }
            if (isHardwareSupportClick)
            {
                tabControl.SelectedIndex = 6;
            }
            technicalSupport_Click_position();
        }
        /// <summary>
        /// 左侧动态分析菜单点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DynamicAnalysis_Click(object sender, EventArgs e)
        {
            DynamicAnalysisColorLogic();
            tabControl.SelectedIndex = 1;
        }
        /// <summary>
        /// 左侧静态分析菜单点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StaticAnalysis_Click(object sender, EventArgs e)
        {
            StaticAnalysisColorLogic();
            tabControl.SelectedIndex = 2;
        }
        /// <summary>
        /// 数据库回放点击时的响应函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DatabasePlayback_Click(object sender, EventArgs e)
        {
            DatabasePlaybackColorLogic();
            tabControl.SelectedIndex = 3;
        }
        /// <summary>
        /// 文本回放点击时的响应函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtPlayback_Click(object sender, EventArgs e)
        {
            TxtPlaybackColorLogic();
            tabControl.SelectedIndex = 4;
        }
        /// <summary>
        /// 软件支持被点击时的响应函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void softwaveSupport_Click(object sender, EventArgs e)
        {
            softwaveSupportColorLogic();
            tabControl.SelectedIndex = 5;
        }
        /// <summary>
        /// 硬件支持被点击时的响应函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hardwareSupport_Click(object sender, EventArgs e)
        {
            hardwareSupportColorLogic();
            tabControl.SelectedIndex = 6;
        }
        #endregion


        float lift = 0.0f;       //升力（g）
        float voltage = 0.0f;    //电压（v）
        float rotate = 0.0f;     //转速(r/s)
        float current = 0.0f;    //电流(i)
        float efficiency = 0.0f; //效率(g/w)
        byte[] receiveData = new byte[16];
        /// <summary>
        /// 定时器的回调函数，周期为10ms
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            //PWMFExpression.Text = current + "x²+" + current + "x+" + current;

            if (serialCom.CheckFrame())
            {
                receiveData = serialCom.rxFrame.data;
                if (isCalibration)
                {
                    labelCalibrationStatus.Text = "正在标定";
                    CalibrationWeight();
                }
                else
                {
                    labelCalibrationStatus.Text = "标定成功";
                    btnSettingPWM.Enabled = true;
                    //  ((calibrationValue - BitConverter.ToSingle(serialCom.rxFrame.data, 0)) / 213.0605137);
                    lift = (calibrationValue - BitConverter.ToSingle(receiveData, 4)) / 213.0605137f;
                    LiftCurveControl.Value = (int)lift;
                    LiftTextLabel.Text = lift.ToString("f2");
                    if (liftCachePosition >= 1023)
                    {
                        liftCachePosition = 0;
                    }
                    liftDataChche[liftCachePosition++] = lift;
                    
                }

                rotate = BitConverter.ToSingle(receiveData, 0);
                RotateCurveControl.Value = (int)rotate;
                RotageTextLabel.Text = rotate.ToString("f2");
                rotateDataCache[rotateCachePosition++] = rotate;
                if (rotateCachePosition>=1023)
                {
                    rotateCachePosition = 0;
                }

                voltage = BitConverter.ToSingle(receiveData, 8);
                voltageLabel.Text = voltage.ToString("f2");

                current = BitConverter.ToSingle(receiveData, 12);
                CurrentCurveControl.Value = (int)current;
                CurrentTextLabel.Text = current.ToString("f2");
                if (currentCachePosition >= 1023)
                {
                    currentCachePosition = 0;
                }
                currentDataCache[currentCachePosition++] = current;
                
            }

            PWMCurveControl.Value = (int)PWMCurveData;
            PWMTextLabel.Text = PWMCurveData.ToString();
            if (pwmCachePosition >= 1023)
            {
                pwmCachePosition = 0;
            }
            pwmDataCache[pwmCachePosition++] = PWMCurveData;
            

            if (voltage * current == 0)
            {
                efficiency = 0;
            }
            else if (current < 0.1f)
            {
                efficiency = 0;
            }
            else if (lift < 1.0f)
            {
                efficiency = 0;
            }
            else
            {
                efficiency = lift / (voltage * current);
            }

            EfficiencyCurveControl.Value = (int)efficiency;
            EfficiencyTextLabel.Text = efficiency.ToString("f2");
            if (efficiencyCachePosition >= 1023)
            {
                efficiencyCachePosition = 0;
            }
            efficiencyDataCache[efficiencyCachePosition++] = efficiency;
            
            ReflushShowCurve(); //每次定时器完成，更新一次界面
        }
        /// <summary>
        /// 更新 状态显示的界面曲线控件
        /// </summary>
        private void ReflushShowCurve()
        {
            xAxis1.XAxisRefreshControl();
            RotateCurveControl.CurveRefreshControl();
            CurrentCurveControl.CurveRefreshControl();
            LiftCurveControl.CurveRefreshControl();
            PWMCurveControl.CurveRefreshControl();
            EfficiencyCurveControl.CurveRefreshControl();
        }


        /// <summary>
        /// 标定重量，每次运行程序都会用100组数据标定重量
        /// </summary>
        private void CalibrationWeight()
        {
            if (countAve < 100)
            {
                portData[countAve] = BitConverter.ToSingle(receiveData, 4);
                countAve++;
            }
            else
            {
                countAve = 0;
                isCalibration = false; //此处只进来一次，用于标定秤
                float a = 0;
                for (int j = 0; j < 100; j++)
                {
                    a += portData[j];
                }
                calibrationValue = a / 100f;
            }
        }

        /// <summary>
        /// 当滑动框的值变化时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trackBarPWMSetting_ValueChanged(object sender, EventArgs e)
        {
            PWMSettingValue = (float)trackBarPWMSetting.Value / 100.0f;
            textBoxPWMSetting.Text = PWMSettingValue.ToString();
        }
        /// <summary>
        /// 设置PWM占空比的按钮被点击执行的回调函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSettingPWM_Click(object sender, EventArgs e)
        {
            if (!serialCom.isComOpen())
            {
                MessageBox.Show("未打开串口，请先打开串口", "警告");
                return;
            }
            PWMCurveData = float.Parse(textBoxPWMSetting.Text);
            serialCom.txFrame.fnCode = FunctionCode.FC_MOTOR_SET_VEL;
            BitConverter.GetBytes((float)trackBarPWMSetting.Value / 100.0f).CopyTo(serialCom.txFrame.data, 0);
            serialCom.txFrame.isUpdated = true;
            serialCom.SendFrame();
        }

        #region 双击曲线控件逻辑
        /// <summary>
        /// 双击转速的曲线，显示具体的转速信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RotateCurveControl_DoubleClick(object sender, EventArgs e)
        {
            DetailCurveForm dcf = new DetailCurveForm(rotateDataCache, 0, rotateCachePosition); 
            dcf.Show();
        }
        /// <summary>
        /// 双击电流的曲线，显示具体电流信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CurrentCurveControl_DoubleClick(object sender, EventArgs e)
        {
            DetailCurveForm dcf = new DetailCurveForm(currentDataCache, 1, currentCachePosition);
            dcf.Show();
        }
        /// <summary>
        /// 双击升力的曲线，显示具体的升力信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LiftCurveControl_DoubleClick(object sender, EventArgs e)
        {
            DetailCurveForm dcf = new DetailCurveForm(liftDataChche, 2, liftCachePosition);
            dcf.Show();
        }
        /// <summary>
        /// 双击PWM波的曲线，显示具体的升力信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PWMCurveControl_DoubleClick(object sender, EventArgs e)
        {
            DetailCurveForm dcf = new DetailCurveForm(pwmDataCache, 3, pwmCachePosition);

            dcf.Show();
        }
        /// <summary>
        /// 双击效率的曲线，显示效率的具体信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EfficiencyCurveControl_DoubleClick(object sender, EventArgs e)
        {
            DetailCurveForm dcf = new DetailCurveForm(efficiencyDataCache, 4, efficiencyCachePosition);
            dcf.Show();
        }
        #endregion

        private Boolean isClickDynamicAnalysisBtn = false;
        private void DynamicAnalysisBtn_Click(object sender, EventArgs e)
        {
            if (!isClickDynamicAnalysisBtn)
            {
                AnalysisTimer.Enabled = true;
                isClickDynamicAnalysisBtn = true;
                DynamicAnalysisBtn.Text = "取消";
            }
            else
            {
                AnalysisTimer.Enabled = false;
                isClickDynamicAnalysisBtn = false;
                DynamicAnalysisBtn.Text = "确定";
            }
            
        }

        private int XAxisAnalysisWave = 0;  //分析波形的X坐标

        private float curLiftSendPWM = 0;  //当前发送的升力PWM波的值
        private float curLineSendPWM = 0;  //当前发送的线性波的PWM值
        private bool  incOrdec = true;  //true表示增加（0-100），false表示减少（100-0）
        FileStream fs;
        StreamWriter sw;

        private void LiftText()   //PWM从0-100，测试升力
        {
            if (!serialCom.isComOpen())
            {
                MessageBox.Show("未打开串口，请先打开串口", "警告");
                return;
            }
            XAxisAnalysisWave += 1;
            serialCom.txFrame.fnCode = FunctionCode.FC_MOTOR_SET_VEL;
            BitConverter.GetBytes((float)curLiftSendPWM).CopyTo(serialCom.txFrame.data, 0);
            serialCom.txFrame.isUpdated = true;
            serialCom.SendFrame();

            orderCurveList.Add(XAxisAnalysisWave, CountLift(curLiftSendPWM));

            //接收数据，显示在示波器上
            realCurveList.Add(XAxisAnalysisWave, (double)liftDataChche[liftCachePosition - 1]);


            zedGraphControl1ChangeAxis();

            if(incOrdec)
            {
                curLiftSendPWM += 0.01f;
                if (curLiftSendPWM >= 100.0f-0.05f)
                {
                    incOrdec = false;
                }
            }
            else
            {
                curLiftSendPWM -= 0.01f;
                if (curLiftSendPWM < 0.0f)
                {
                    incOrdec = true;
                    curLiftSendPWM = 0.0f;
                }
            }

            sw.Write(curLiftSendPWM + "\t" + liftDataChche[liftCachePosition - 1] + "\t" + CountLift(curLiftSendPWM) + "\r\n");
        }
        
        /// <summary>
        /// 根据PWM波得出升力
        /// </summary>
        /// <param name="pwm"></param>
        /// <returns></returns>
        private double CountLift(float pwm)
        {
          //  double data = 0.05917 * pwm * pwm + 0.8007 * pwm - 9.074;
          //  double data = 0.06309 * pwm * pwm + 1.491 * pwm - 30.16;
            double data = 0.04953 * pwm * pwm + 2.011 * pwm - 12.82;
            if (data<0)
            {
                data = 0;
            }
            return data;
        }
        /// <summary>
        /// 根据升力得到PWM波
        /// </summary>
        /// <param name="lift"></param>
        /// <returns></returns>
        private float CountPWM(double lift)
        {
            return (float)(-2.011 + Math.Sqrt(2.011 * 2.011 - 4 * 0.04953 * (-12.82 - lift)) / (2 * 0.04953));
        }


        private void LineWave() //产生线性波
        {
            if (!serialCom.isComOpen())
            {
                MessageBox.Show("未打开串口，请先打开串口", "警告");
                return;
            }
            XAxisAnalysisWave += 1;
            serialCom.txFrame.fnCode = FunctionCode.FC_MOTOR_SET_VEL;
            BitConverter.GetBytes((float)curLineSendPWM).CopyTo(serialCom.txFrame.data, 0);
            serialCom.txFrame.isUpdated = true;
            serialCom.SendFrame();

            orderCurveList.Add(XAxisAnalysisWave, CountLift(curLineSendPWM));

            //接收数据，显示在示波器上
            realCurveList.Add(XAxisAnalysisWave, (double)liftDataChche[liftCachePosition - 1]);

            zedGraphControl1ChangeAxis();

            if (incOrdec)
            {
                curLineSendPWM += 0.1f;
                if (curLineSendPWM >= 100.0f - 0.05f)
                {
                    incOrdec = false;
                }
            }
            else
            {
                curLineSendPWM -= 0.1f;
                if (curLineSendPWM < 0.0f)
                {
                    incOrdec = true;
                    curLineSendPWM = 0.0f;
                }
            }

            sw.Write(curLineSendPWM + "\t" + liftDataChche[liftCachePosition - 1] + "\t" + CountLift(curLineSendPWM) + "\r\n");
        }
        private float curTiSendPWM = 0.0f;
        private int tiboW = 500;//梯波的长度
        private int tiboH = 10;//梯波的高度
        private int tiboTimes = 0;//相同的PWM的次数
        private void TerracedWave() //梯波
        {
            if (!serialCom.isComOpen())
            {
                MessageBox.Show("未打开串口，请先打开串口", "警告");
                return;
            }
            XAxisAnalysisWave += 1;
            serialCom.txFrame.fnCode = FunctionCode.FC_MOTOR_SET_VEL;
            BitConverter.GetBytes(curTiSendPWM).CopyTo(serialCom.txFrame.data, 0);
            serialCom.txFrame.isUpdated = true;
            serialCom.SendFrame();

            if (tiboTimes >= tiboW)
            {
                tiboTimes = 0;
                curTiSendPWM += tiboH;
                if (curTiSendPWM>100)
                {
                    curTiSendPWM = 0;
                }
            }
            else
            {
                tiboTimes++;
                
            }
            orderCurveList.Add(XAxisAnalysisWave, CountLift(curTiSendPWM));

            //接收数据，显示在示波器上
            realCurveList.Add(XAxisAnalysisWave, (double)liftDataChche[liftCachePosition - 1]);

            zedGraphControl1ChangeAxis();
            sw.Write(curTiSendPWM + "\t" + XAxisAnalysisWave + "\t" + liftDataChche[liftCachePosition - 1] + "\t" + CountLift(curTiSendPWM) + "\r\n");
        }

        private float curSinSendPWM  = 0.0f;
        private float sinLift = 0.0f;
        private void SinWave()   //正弦波
        {
            if (!serialCom.isComOpen())
            {
                MessageBox.Show("未打开串口，请先打开串口", "警告");
                return;
            }
            XAxisAnalysisWave += 1;
            
            serialCom.txFrame.fnCode = FunctionCode.FC_MOTOR_SET_VEL;
            BitConverter.GetBytes(curSinSendPWM).CopyTo(serialCom.txFrame.data, 0);
            serialCom.txFrame.isUpdated = true;
            serialCom.SendFrame();

           // orderCurveList.Add(XAxisAnalysisWave, CountPWM(100) * Math.Sin(((2 * Math.PI) / 200) * curLiftSendPWM));
           // orderCurveList.Add(XAxisAnalysisWave, CountLift(50 + (50 * (float)Math.Sin(((2 * Math.PI) / 100) * curLiftSendPWM))));
            sinLift = (float)((CountLift(100)) / 2 + (CountLift(100) / 2) * (float)Math.Sin(((2 * Math.PI) / 200) * XAxisAnalysisWave));
            curSinSendPWM = CountPWM(sinLift);
            orderCurveList.Add(XAxisAnalysisWave, sinLift);

            //接收数据，显示在示波器上
            realCurveList.Add(XAxisAnalysisWave, (double)liftDataChche[liftCachePosition - 1]);
            
            zedGraphControl1ChangeAxis();
            
            sw.Write(curSinSendPWM + "\t" + XAxisAnalysisWave + "\t" + liftDataChche[liftCachePosition - 1] + "\t" + CountLift(curSinSendPWM) + "\r\n");
        }
        private float curCosSendPWM = 0.0f;
        private float cosLift = 0.0f;
        private void CosWave()   //余弦波
        {
            if (!serialCom.isComOpen())
            {
                MessageBox.Show("未打开串口，请先打开串口", "警告");
                return;
            }
            XAxisAnalysisWave += 1;

            serialCom.txFrame.fnCode = FunctionCode.FC_MOTOR_SET_VEL;
            BitConverter.GetBytes(curCosSendPWM).CopyTo(serialCom.txFrame.data, 0);
            serialCom.txFrame.isUpdated = true;
            serialCom.SendFrame();

            cosLift = (float)((CountLift(100)) / 2 + (CountLift(100) / 2) * (float)Math.Cos(((2 * Math.PI) / 200) * XAxisAnalysisWave));
            curCosSendPWM = CountPWM(cosLift);
            orderCurveList.Add(XAxisAnalysisWave, cosLift);

            //接收数据，显示在示波器上
            realCurveList.Add(XAxisAnalysisWave, (double)liftDataChche[liftCachePosition - 1]);
            
            zedGraphControl1ChangeAxis();

            sw.Write(curLiftSendPWM + "\t" + liftDataChche[liftCachePosition - 1] + "\t" + CountLift(curLiftSendPWM) + "\r\n");
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            IList<string> listOptions = new List<string>();
            listOptions.Add("能力");
            listOptions.Add("性能");
            DynamicOptionCombox.DataSource = listOptions;
        }

        private void DynamicOptionCombox_SelectedIndexChanged(object sender, EventArgs e)
        {
            IList<string> listAnalysis = new List<string>();
            switch (DynamicOptionCombox.SelectedIndex)
            {
                case 0:
                    listAnalysis.Add("升力");
                    break;
                case 1:
                    listAnalysis.Add("线性波");
                    listAnalysis.Add("梯波");
                    listAnalysis.Add("正弦波");
                    listAnalysis.Add("余弦波");
                    break;
                default:
                    break;
            }
            DynamicAnalysisCombox.DataSource = listAnalysis;
        }

        /// <summary>
        /// 绘制曲线的背景面板
        /// </summary>
        private GraphPane myPane = null;
        /// <summary>
        /// 真实数据的曲线
        /// </summary> 
        private LineItem realCurve = null;
        /// <summary>
        /// 绘制真实的曲线点集合
        /// </summary>
        private PointPairList realCurveList = new PointPairList();
        /// <summary>
        /// 命令数据的曲线
        /// </summary> 
        private LineItem orderCurve = null;
        /// <summary>
        /// 绘制命令的曲线点集合
        /// </summary>
        private PointPairList orderCurveList = new PointPairList(); 

        private void AnalysisTimer_Tick(object sender, EventArgs e)
        {

            if (DynamicOptionCombox.SelectedIndex == 0)   //制造各种波形，需要显示在示波器上
            {
                switch (DynamicAnalysisCombox.SelectedIndex)
                {
                    case 0:
                        LiftText();
                        break;
                    default:
                        break;
                }
            }
            else if (DynamicOptionCombox.SelectedIndex == 1)
            {
                switch (DynamicAnalysisCombox.SelectedIndex)
                {
                    case 0:
                        LineWave();
                        break;
                    case 1:
                        TerracedWave();
                        break;
                    case 2:
                        SinWave();
                        break;
                    case 3:
                        CosWave();
                        break;
                    default:
                        break;
                }
            }
            else
            { }
        }
        private void zedGraphControl1ChangeAxis()
        {
            myPane.XAxis.Scale.Min = XAxisAnalysisWave - 500;
            myPane.XAxis.Scale.Max = XAxisAnalysisWave;
            myPane.XAxis.Scale.MinorStep = 10;//X轴小步长1,也就是小间隔
            myPane.XAxis.Scale.MajorStep = 50;//X轴大步长为5，也就是显示文字的大间隔
            this.zedGraphControl1.AxisChange();
            this.zedGraphControl1.Refresh();  //更新界面
        }



    }
}
