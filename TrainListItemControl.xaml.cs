using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace IpisCentralDisplayController
{
    public partial class TrainListItemControl : UserControl
    {
        public TrainListItemControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty TrainNoProperty =
            DependencyProperty.Register("TrainNo", typeof(int), typeof(TrainListItemControl));

        public static readonly DependencyProperty TrainNameProperty =
            DependencyProperty.Register("TrainName", typeof(string), typeof(TrainListItemControl));

        public static readonly DependencyProperty ADFlagProperty =
            DependencyProperty.Register("ADFlag", typeof(string), typeof(TrainListItemControl));

        public static readonly DependencyProperty TrainStatusProperty =
            DependencyProperty.Register("TrainStatus", typeof(string), typeof(TrainListItemControl));

        public static readonly DependencyProperty STAProperty =
            DependencyProperty.Register("STA", typeof(DateTime?), typeof(TrainListItemControl));

        public static readonly DependencyProperty STDProperty =
            DependencyProperty.Register("STD", typeof(DateTime?), typeof(TrainListItemControl));

        public static readonly DependencyProperty LateProperty =
            DependencyProperty.Register("Late", typeof(DateTime?), typeof(TrainListItemControl));

        public static readonly DependencyProperty ETAProperty =
            DependencyProperty.Register("ETA", typeof(DateTime?), typeof(TrainListItemControl));

        public static readonly DependencyProperty ETDProperty =
            DependencyProperty.Register("ETD", typeof(DateTime?), typeof(TrainListItemControl));

        public static readonly DependencyProperty PlatformNoProperty =
            DependencyProperty.Register("PlatformNo", typeof(int), typeof(TrainListItemControl));

        public static readonly DependencyProperty TADDBProperty =
            DependencyProperty.Register("TADDB", typeof(bool), typeof(TrainListItemControl));

        public static readonly DependencyProperty CGDBProperty =
            DependencyProperty.Register("CGDB", typeof(bool), typeof(TrainListItemControl));

        public static readonly DependencyProperty ANNProperty =
            DependencyProperty.Register("ANN", typeof(bool), typeof(TrainListItemControl));

        public static readonly DependencyProperty DeleteCommandProperty =
            DependencyProperty.Register("DeleteCommand", typeof(ICommand), typeof(TrainListItemControl));

        public int TrainNo
        {
            get => (int)GetValue(TrainNoProperty);
            set => SetValue(TrainNoProperty, value);
        }

        public string TrainName
        {
            get => (string)GetValue(TrainNameProperty);
            set => SetValue(TrainNameProperty, value);
        }

        public string ADFlag
        {
            get => (string)GetValue(ADFlagProperty);
            set => SetValue(ADFlagProperty, value);
        }

        public string TrainStatus
        {
            get => (string)GetValue(TrainStatusProperty);
            set => SetValue(TrainStatusProperty, value);
        }

        public DateTime? STA
        {
            get => (DateTime?)GetValue(STAProperty);
            set => SetValue(STAProperty, value);
        }

        public DateTime? STD
        {
            get => (DateTime?)GetValue(STDProperty);
            set => SetValue(STDProperty, value);
        }

        public DateTime? Late
        {
            get => (DateTime?)GetValue(LateProperty);
            set => SetValue(LateProperty, value);
        }

        public DateTime? ETA
        {
            get => (DateTime?)GetValue(ETAProperty);
            set => SetValue(ETAProperty, value);
        }

        public DateTime? ETD
        {
            get => (DateTime?)GetValue(ETDProperty);
            set => SetValue(ETDProperty, value);
        }

        public int PlatformNo
        {
            get => (int)GetValue(PlatformNoProperty);
            set => SetValue(PlatformNoProperty, value);
        }

        public bool TADDB
        {
            get => (bool)GetValue(TADDBProperty);
            set => SetValue(TADDBProperty, value);
        }

        public bool CGDB
        {
            get => (bool)GetValue(CGDBProperty);
            set => SetValue(CGDBProperty, value);
        }

        public bool ANN
        {
            get => (bool)GetValue(ANNProperty);
            set => SetValue(ANNProperty, value);
        }

        public ICommand DeleteCommand
        {
            get => (ICommand)GetValue(DeleteCommandProperty);
            set => SetValue(DeleteCommandProperty, value);
        }
    }
}
