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

namespace Cache_imitation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        class CacheInstance
        {
            public int Address { get; }

            public int Value { get; set; }

            public int Searches { get; set; }

            public DateTime LastUsed { get; set; }

            public bool Changed { get; set; }

            public CacheInstance(int address)
            {
                Address = address;
            }
        }

        private List<int> mainMemory;
        private int _memSize;
        private List<CacheInstance> cache;
        private int _cacheSize;
        private int _currentCache;
        private Random _rnd;

        public MainWindow()
        {
            InitializeComponent();

            _rnd = new Random();
            FillMainMem(text_memSize.Text);
            text_CacheSize.Text = CreateCache(_memSize).ToString();
            
        }

        private void button_memSizeChange_Click(object sender, RoutedEventArgs e)
        {
            listView_Log.Items.Clear();
            FillMainMem(text_memSize.Text);
            text_CacheSize.Text = CreateCache(_memSize).ToString();
        }

        private void FillMainMem(string text)
        {
            int size = 0;
            if (!ConvertStoI(text, out size, "Invalid memory size"))
            {
                return;
            }
            
            _memSize = size;
            
            mainMemory = new List<int>();
            for (int i = 0; i < size; i++)
            {
                mainMemory.Add(_rnd.Next());
            }

            WriteLog("Пам'ять ініціалізовано");
        }

        private int CreateCache(int memSize)
        {
            _cacheSize = memSize / 10 + 1;
            cache = new List<CacheInstance>();
            WriteLog("Кеш ініціалізовано");

            return _cacheSize;
        }

        private void WriteLog(string log)
        {
            listView_Log.Items.Add(log);
        }

        private bool ConvertStoI(string s, out int i, string log)
        {
            if (!int.TryParse(s, out i))
            {
                WriteLog(log);
                return false;
            }
            return true;
        }

        private void button_findCell_Click(object sender, RoutedEventArgs e)
        {
            WriteLog("-----------------------------------");

            int cell = 0;
            if (!ConvertStoI(text_chooseCell.Text, out cell, "Не адреса"))
            {
                return;
            }

            if (cell < 0 || cell > _memSize)
            {
                WriteLog("Не дозволена адреса");
                return;
            }

            int cacheNum = FindInCache(cell);

            if (cacheNum >= 0)
            {
                // Found in cache
                WriteLog("Знайдено у кеш");
                FoundInCache(cacheNum);
                return;
            }

            // Not found in cache
            WriteLog("Не знайдено у кеш");
            NotFoundInCache(cell);
        }

        private int FindInCache(int cell)
        {
            for (int i = 0; i < cache.Count; i++)
            {
                if (cache[i].Address == cell)
                    return i;
            }
            return -1;
        }

        private void FoundInCache(int num)
        {
            _currentCache = num;
            var cInst = cache[num];
            cInst.LastUsed = DateTime.Now;
            cInst.Searches++;
            text_cellValue.Text = cInst.Value.ToString();
        }

        private void NotFoundInCache(int address)
        {
            if (cache.Count < _cacheSize)
            {
                // Cache not full
                WriteLog("Додано дані до кеш");
                cache.Add(new CacheInstance(address) { Value = mainMemory[address]});
                FoundInCache(cache.Count - 1);
                return;
            }

            // Cache full
            var cacheN = FindLeastPopular();
            if (cache[cacheN].Changed)
            {
                mainMemory[cache[cacheN].Address] = cache[cacheN].Value;
                WriteLog("Дані записанo в пам'ять");
            }
            
            WriteLog("Заміна даних в кеш");
            cache[cacheN] = new CacheInstance(address) { Value = mainMemory[address] };
            FoundInCache(cacheN);
            return;
        }

        private int FindLeastPopular()
        {
            var now = DateTime.Now;
            int lpItem = 0;
            for (int i = 1; i < cache.Count; i++)
            {
                if (cache[i].LastUsed.CompareTo(now) > cache[lpItem].LastUsed.CompareTo(now))
                    lpItem = i;
            }

            return lpItem;
        }

        private void button_CellValueWrite_Click(object sender, RoutedEventArgs e)
        {
            WriteLog("-----------------------------------");
            int newVal = 0;
            if (!ConvertStoI(text_cellValue.Text, out newVal, "Invalid new value"))
            {
                return;
            }

            cache[_currentCache].Value = newVal;
            cache[_currentCache].Changed = true;
            WriteLog("Нове значення було занесено в кеш");
        }
    }
}
