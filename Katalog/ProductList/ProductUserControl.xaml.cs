using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
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
using ProductList.Properties;

namespace ProductList
{
    /// <summary>
    /// Логика взаимодействия для ProductUserControl.xaml
    /// </summary>
    public partial class ProductUserControl : UserControl, INotifyPropertyChanged
    {
        public ProductUserControl()
        {
            InitializeComponent();
            Products = new ObservableCollection<Продукты>();
            LoadProducts();
        }
        private ObservableCollection<Продукты> _originalProducts;

        private ObservableCollection<Продукты> _products;
        public ObservableCollection<Продукты> Products
        {
            get { return _products; }
            set
            {
                if (_products != value)
                {
                    _products = value;
                    OnPropertyChanged(nameof(Products));
                }
            }
        }

        public Продукты SelectedProduct { get; set; }

        public void LoadProducts()
        {
            DataBase myDbContext = new DataBase();
            using (SqlConnection connection = new SqlConnection(myDbContext.connectionString))
            {
                connection.Open();
                string query = "SELECT NameProduct, Price, DescriptionProduct, ImageProduct, StockQuantity, Manufacturer FROM Продукты";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        List<Продукты> products = new List<Продукты>();

                        while (reader.Read())
                        {
                            Продукты product = new Продукты
                            {
                                NameProduct = reader.IsDBNull(0) ? string.Empty : reader.GetString(0),
                                Price = reader.IsDBNull(1) ? 0 : reader.GetDecimal(1),
                                DescriptionProduct = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                                ImageProduct = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                                StockQuantity = reader.IsDBNull(4) ? 0 : reader.GetInt32(4),
                                Manufacturer = reader.IsDBNull(5) ? string.Empty : reader.GetString(5)
                            };

                            products.Add(product);
                        }
                        Products = new ObservableCollection<Продукты>(products);
                    }
                }
            }
        }

        public void SearchTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                string searchText = textBox.Text.ToLower();
                var filteredProducts = _products.Where(p => p.NameProduct.ToLower().Contains(searchText) ||
                                                           p.DescriptionProduct.ToLower().Contains(searchText) ||
                                                           p.Manufacturer.ToLower().Contains(searchText));
                Products = new ObservableCollection<Продукты>(filteredProducts);
            }
        }

        public void SelectedSortIndex(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox != null)
            {
                ComboBoxItem selectedItem = comboBox.SelectedItem as ComboBoxItem;
                if (selectedItem != null)
                {
                    string selectedSortOption = selectedItem.Content.ToString();
                    if (selectedSortOption == "По Возрастанию")
                    {
                        SortAscending();
                    }
                    else if (selectedSortOption == "По Убыванию")
                    {
                        SortDescending();
                    }
                    else if (selectedSortOption == "Нет сортировки")
                    {
                        ResetSort();
                    }
                }
            }
        }

        private void SortAscending()
        {
            var sortedProducts = _products.OrderBy(p => p.Price);
            Products = new ObservableCollection<Продукты>(sortedProducts);
        }
        private void ResetSort()
        {
            var sortedProducts = _products.OrderBy(p => p.Price);
            Products = new ObservableCollection<Продукты>(sortedProducts);
        }
        private void SortDescending()
        {
            var sortedProducts = _products.OrderByDescending(p => p.Price);
            Products = new ObservableCollection<Продукты>(sortedProducts);
        }

        public void FilterByManufacturer(string manufacturer)
        {
            var filteredProducts = _products.Where(p => p.Manufacturer == manufacturer);
            Products = new ObservableCollection<Продукты>(filteredProducts);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
