using Microsoft.Win32;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KenoGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        ObservableCollection<Szelveny> szelvenyek;
        int[] highlighted;
        ObservableCollection<int> randomSzamok;

        public ObservableCollection<Szelveny> Szelvenyek { get => szelvenyek; set { szelvenyek = value; OnPropertyChanged(); } }
        public int[] Highlighted { get => highlighted; set { highlighted = value; OnPropertyChanged(); } }
        public ObservableCollection<int> RandomSzamok { get => randomSzamok; set { randomSzamok = value; OnPropertyChanged(); } }

        public MainWindow()
        {
            szelvenyek = new ObservableCollection<Szelveny>();
            highlighted = Array.Empty<int>();
            randomSzamok = new ObservableCollection<int>();

            InitializeComponent();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

            if (ofd.ShowDialog() == false) return;

            Szelvenyek.Clear();
            string[] lines = File.ReadAllLines(ofd.FileName);
            foreach (string line in lines)
            {
                Szelvenyek.Add(new Szelveny(line));
            }
        }

        private void lbSzelvenyek_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox lbSender && lbSender.SelectedItem is Szelveny selected)
            {
                Highlighted = selected.Tippek.ToArray();
            }
        }

        private void btnDaily_Click(object sender, RoutedEventArgs e)
        {
            Random rnd = new Random();
            RandomSzamok = new ObservableCollection<int>(Enumerable.Range(1, 80).OrderBy(x => rnd.Next()).Take(20).Order());
        }
    }


    public class HighlightedToCells : IValueConverter
    {
        struct Cell
        {
            public int Index { get; set; }
            public bool IsHighlighted { get; set; }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int[] selected && selected.Length < 80)
            {
                Cell[] cells = new Cell[80];
                for (int i= 0; i < 80; i++)
                {
                    cells[i] = new Cell { Index = i+1, IsHighlighted = selected.Contains(i+1) };
                }
                return cells;
            }
            return new List<string>();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RewardsConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is IEnumerable items && values[1] is Szelveny selected)
            {
                return 200 * selected.Szorzo * Szorzo(items.Cast<int>().ToList(), selected.Tippek);
            }
            return 0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private int Szorzo(List<int> kenoSzamai, List<int> tippek)
        {
            Dictionary<String, int> nyeroParok = new Dictionary<string, int>(){
                {"10-10",1000000}, {"10-9",8000}, {"10-8",350}, {"10-7",30}, {"10-6",3}, {"10-5",1}, {"10-0",2},
                {"9-9",100000}, {"9-8",1200}, {"9-7",100}, {"9-6",12}, {"9-5",3}, {"9-0",1},
                {"8-8",20000}, {"8-7",350}, {"8-6",25}, {"8-5",5}, {"8-0",1},
                {"7-7",5000}, {"7-6",60}, {"7-5",6}, {"7-4",1}, {"7-0",1},
                {"6-6",500}, {"6-5",20}, {"6-4",3}, {"6-0",1},
                {"5-5",200}, {"5-4",10}, {"5-3",2},
                {"4-4",100}, {"4-3",2},
                {"3-3",15}, {"3-2",1},
                {"2-2",6},
                {"1-1",2}
            };
            int jatekTipus = tippek.Count;
            int talalatokSzama = 0;
            foreach (int tipp in tippek)
            {
                if (kenoSzamai.Contains(tipp))
                {
                    talalatokSzama++;
                }
            }
            string kulcs = jatekTipus + "-" + talalatokSzama;

            if (nyeroParok.Keys.Contains(kulcs))
                return nyeroParok[kulcs];
            else
                return 0;
        }
    }
}