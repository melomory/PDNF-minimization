using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;


namespace Minimization
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDataErrorInfo, INotifyPropertyChanged
    {
        #region Validation
        HashSet<string> errors = new HashSet<string>();

        public string this[string columnName]
        {
            get
            {
                string result = String.Empty;
                if (columnName == "Variables")
                {
                    if (Variables < 1 || Variables > 5)
                    {
                        result = "Значение должно быть от 1 до 5";
                    } else
                    {
                        errors.Remove("Значение должно быть от 1 до 5");
                        CheckVector();
                    }
                }
                if (columnName == "FunctionVector")
                {
                    Regex regx = new Regex(@"^[01]+$");
                    if (!regx.IsMatch(FunctionVector))
                    {
                        result = "Вектор должен состоять из 0 и 1";
                    }
                    else if (FunctionVector.Length != (int)Math.Pow(2, Variables))
                    {
                        result = "Длина вектора должна соответствовать количеству переменных";
                    } else
                    {
                        errors.Remove("Вектор должен состоять из 0 и 1");
                        errors.Remove("Длина вектора должна соответствовать количеству переменных");
                        ChekVariables();
                    }
                }

                errors.Add(result);

                if (errors.Count > 1 || !string.IsNullOrEmpty(errors.First())) 
                {
                    Error = "Error";
                } else
                {
                    Error = null;
                }

                return result;
            }
        }

        private void CheckVector()
        {
            Regex regx = new Regex(@"^[01]+$");
            if (!regx.IsMatch(FunctionVector))
            {
                errors.Add("Вектор должен состоять из 0 и 1");
            }
            else if (FunctionVector.Length != (int)Math.Pow(2, Variables))
            {
                errors.Add("Длина вектора должна соответствовать количеству переменных");
            }
            else
            {
                errors.Remove("Вектор должен состоять из 0 и 1");
                errors.Remove("Длина вектора должна соответствовать количеству переменных");
            }
        }

        private void ChekVariables()
        {
            if (Variables < 1 || Variables > 5)
            {
                errors.Add("Значение должно быть от 1 до 5");
            }
            else
            {
                errors.Remove("Значение должно быть от 1 до 5");
            }
        }


        private string _error;
        public string Error
        {
            get => _error;
            set
            {
                if (_error != value)
                {
                    _error = value;
                    OnPropertyChanged("Error");
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion Validation

        #region Variables
        private const string VARS = "xyzwt";

        private int _variables = 2;
        public int Variables
        {
            get => _variables;
            set
            {
                if (_variables != value)
                {
                    _variables = value;
                    OnPropertyChanged("Variables");
                }

            }
        }

        private string _functionVector = String.Empty;
        public string FunctionVector
        {
            get => _functionVector;
            set
            {
                if (_functionVector != value)
                {
                    _functionVector = value;
                    OnPropertyChanged("FunctionVector");
                }
                
            }
        }
        #endregion Variables

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
            Alphabet.Content = VARS;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            List<string> sets = new List<string>();
            List<string> implicants = new List<string>();
            List<string> coef = new List<string>();
            string f = FV.Text;
            Variables = int.Parse(Vars.Text);

            View.Items.Clear();

            // get sets as strings
            for (int i = 0; i < f.Length; ++i)
            {
                    sets.Add(Convert.ToString(i, 2).PadLeft(Variables, '0'));
            }

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < sets.Count; ++i)
            {
                for (int j = 0; j < sets[i].Length; ++j)
                {
                    sb.Append((sets[i][j] == '0') ? VARS[j] : char.ToUpper(VARS[j]));
                }
                implicants.Add(sb.ToString());
                sb.Clear();
            }

            // get equation system with coefficients
            List<List<string>> eq_system = new List<List<string>>();

            
            for (int j = 0; j < implicants.Count; ++j)
            {
                for (int i = 1; i <= Variables; ++i)
                {
                    coef = coef.Concat(Combination.GenerateCombinations(implicants[j], i)).ToList();
                }
                eq_system.Add(new List<string>(coef));
                coef.Clear();
            }

            // getting PDNF
            int len = implicants.Count - 1;
            for (int i = 0; i < len; ++i)
            {
                sb.Append(implicants[i]);
                sb.Append(" V ");
            }
            sb.Append(implicants[len]);
            PDNF.Content = sb.ToString();
            sb.Clear();

            // reducing coefficients
            HashSet<string> coef_to_remove = new HashSet<string>();
            for (int i = 0; i < f.Length; ++i)
            {
                if (f[i] == '0')
                {
                    for (int j = 0; j < eq_system[i].Count; ++j)
                    {
                        coef_to_remove.Add(eq_system[i][j]); 
                    }
                }
            }

            for (int i = f.Length - 1; i >= 0; --i)
            {
                if (f[i] == '0')
                {
                    eq_system.RemoveAt(i);
                } else
                {
                    foreach (string item in coef_to_remove)
                    {
                        eq_system[i].Remove(item);
                    }
                }
            }

            // Output
            if (f.Contains('1') && f.Contains('0'))
            {
                List<HashSet<string>> minimized_functions = Combination.GetMiniminizedFunctions(eq_system);
                int count = 0;
                foreach (var item in minimized_functions)
                {
                    sb.Append(String.Join(" V ", item.OrderBy(s => s)));
                    View.Items.Add(sb.ToString());
                    sb.Clear();
                    count++;
                }
                FormsNumber.Content = count;
            } else if (f.Contains('0'))
            {
                sb.Append('0');
                View.Items.Add(sb.ToString());
                sb.Clear();
                FormsNumber.Content = '0';
            } else if (f.Contains('1'))
            {
                sb.Append('1');
                View.Items.Add(sb.ToString());
                sb.Clear();
                FormsNumber.Content = '1';
            }
        }  
    }
}
